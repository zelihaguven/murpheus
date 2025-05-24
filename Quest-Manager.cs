

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class QuestManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject questPanel;
    [SerializeField] private TextMeshProUGUI questTitleText;
    [SerializeField] private TextMeshProUGUI questDescriptionText;
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image backgroundOverlay;

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private AnimationCurve slideCurve;

    [Header("Quest Data")]
    [SerializeField] private List<Quest> quests = new List<Quest>();

    [Header("References")]
    [SerializeField] private HintManager hintManager;

    private int currentQuestIndex = 0;
    private bool isQuestPanelVisible = false;
    private Vector3 hiddenScale;
    private Vector3 visibleScale;
    private RectTransform panelRectTransform;
    private float animationTime = 0f;
    private bool isAnimating = false;
    private Color overlayStartColor;
    private Color overlayEndColor;

    [System.Serializable]
    public class Quest
    {
        public string title;
        [TextArea(3, 5)] public string description;
        public int totalSteps = 1;
        public int currentStep = 0;
        public bool isCompleted = false;
        public bool isUnlocked = false;
    }

    private void Start()
    {
        if (questPanel == null || questTitleText == null || questDescriptionText == null || progressBar == null || progressText == null || backgroundOverlay == null || hintManager == null)
        {
            Debug.LogError("UI veya referans alanları eksik. QuestManager çalışamaz.");
            return;
        }

        panelRectTransform = questPanel.GetComponent<RectTransform>();
        visibleScale = Vector3.one;
        hiddenScale = Vector3.zero;
        overlayStartColor = new Color(0, 0, 0, 0);
        overlayEndColor = new Color(0, 0, 0, 0.7f);

        panelRectTransform.localScale = hiddenScale;
        questPanel.SetActive(false);
        InitializeQuests();
    }

    private void InitializeQuests()
    {
        AddNewQuest("Eşyaların Bulunması", "Fenerin bataryası ve harita parçası gibi eşyaları bulmalısın.", 3);
        AddNewQuest("Güvenli Bir Yer", "Kilitli bir eve sığınmalı ve anahtarını bulmalısın.", 2);
        AddNewQuest("Lanetin Bulunması", "Kiliseye gidip bodrum için şifre bulmalısın.", 2);
        AddNewQuest("Radyo Kulesi", "Radyo şifresini çözerek iletişim kur.", 2);
        AddNewQuest("Doktorun Evi", "Notları ve günlükleri incele.", 3);
        AddNewQuest("Lanetin Çözümü", "Üç kutsal objeyi bul.", 3);
        AddNewQuest("Eski Kilise", "Murpheus'un geçmişini öğren.", 2);
        AddNewQuest("Ritüel Hazırlığı", "Bulmacaları çöz, objeleri sırala.", 3);
        AddNewQuest("Murpheus'u Bul", "Yerini keşfet ve izle.", 2);
        AddNewQuest("Son Karar", "Ritüel ile laneti kır.", 2);

        if (quests.Count > 0)
        {
            quests[0].isUnlocked = true;
            UpdateQuestUI();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) && !isAnimating)
        {
            ToggleQuestPanel();
        }

        if (isAnimating)
        {
            animationTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(animationTime / slideDuration);
            float v = slideCurve.Evaluate(t);
            panelRectTransform.localScale = Vector3.Lerp(isQuestPanelVisible ? hiddenScale : visibleScale, isQuestPanelVisible ? visibleScale : hiddenScale, v);
            backgroundOverlay.color = Color.Lerp(isQuestPanelVisible ? overlayStartColor : overlayEndColor, isQuestPanelVisible ? overlayEndColor : overlayStartColor, v);

            if (t >= 1f)
            {
                isAnimating = false;
                if (!isQuestPanelVisible)
                {
                    questPanel.SetActive(false);
                    backgroundOverlay.gameObject.SetActive(false);
                }
            }
        }
    }

    private void ToggleQuestPanel()
    {
        isQuestPanelVisible = !isQuestPanelVisible;
        if (isQuestPanelVisible)
        {
            questPanel.SetActive(true);
            backgroundOverlay.gameObject.SetActive(true);
            UpdateQuestUI();
        }
        else
        {
            backgroundOverlay.gameObject.SetActive(false);
        }

        animationTime = 0f;
        isAnimating = true;
    }

    private void UpdateQuestUI()
    {
        if (quests.Count == 0 || currentQuestIndex >= quests.Count)
        {
            questTitleText.text = "Tüm Görevler Tamamlandı!";
            questDescriptionText.text = "Tebrikler, tüm görevler başarıyla tamamlandı.";
            progressBar.value = progressBar.maxValue;
            progressText.text = "100%";
            return;
        }

        Quest q = quests[currentQuestIndex];
        if (!q.isUnlocked)
        {
            questTitleText.text = "Görev Kilitli";
            questDescriptionText.text = "Bu görev için önceki görevi tamamlamalısınız.";
            progressBar.value = 0;
            progressText.text = "0%";
            return;
        }

        questTitleText.text = q.title;
        questDescriptionText.text = q.description;
        float p = (float)q.currentStep / q.totalSteps;
        progressBar.value = p * progressBar.maxValue;
        progressText.text = $"{Mathf.RoundToInt(p * 100)}%";

        if (hintManager.IsHintAvailable())
            hintManager.ShowHint(currentQuestIndex);
    }

    public void AdvanceQuestStep()
    {
        if (quests.Count == 0 || currentQuestIndex >= quests.Count)
            return;

        Quest q = quests[currentQuestIndex];
        if (!q.isUnlocked)
            return;

        q.currentStep++;

        if (q.currentStep >= q.totalSteps)
        {
            q.currentStep = q.totalSteps;
            q.isCompleted = true;

            if (currentQuestIndex < quests.Count - 1)
            {
                currentQuestIndex++;
                quests[currentQuestIndex].isUnlocked = true;
            }
        }

        UpdateQuestUI();
    }

    public void AddNewQuest(string title, string description, int totalSteps = 1)
    {
        Quest q = new Quest { title = title, description = description, totalSteps = totalSteps };
        quests.Add(q);
        if (quests.Count == 1)
        {
            q.isUnlocked = true;
            UpdateQuestUI();
        }
    }

    public bool IsCurrentQuestCompleted() => quests.Count == 0 || currentQuestIndex >= quests.Count || quests[currentQuestIndex].isCompleted;

    public int GetCurrentQuestIndex() => currentQuestIndex;
}
