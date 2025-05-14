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
    
    [Header("Audio")]
    [SerializeField] private AudioClip openSound;
    [SerializeField] private AudioClip closeSound;
    [SerializeField] private AudioClip questCompleteSound;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Quest Data")]
    [SerializeField] private List<Quest> quests = new List<Quest>();
    
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
        [TextArea(3, 5)]
        public string description;
        public int totalSteps = 1;
        public int currentStep = 0;
        public bool isCompleted = false;
        public bool isUnlocked = false;
    }
    
    private void Start()
    {
        if (questPanel == null)
        {
            Debug.LogError("Quest Panel reference is missing!");
            return;
        }
        
        panelRectTransform = questPanel.GetComponent<RectTransform>();
        visibleScale = Vector3.one;
        hiddenScale = Vector3.zero;
        
        // Initialize overlay colors
        overlayStartColor = new Color(0, 0, 0, 0);
        overlayEndColor = new Color(0, 0, 0, 0.7f);
        
        // Hide panel initially
        panelRectTransform.localScale = hiddenScale;
        questPanel.SetActive(false);
        
        // Initialize quests
        InitializeQuests();
    }
    
    private void InitializeQuests()
    {
        if (quests.Count > 0)
        {
            // İlk görevi aktif et
            quests[0].isUnlocked = true;
            UpdateQuestUI();
        }
    }
    
    private void Update()
    {
        // Check for Q key press to toggle quest panel
        if (Input.GetKeyDown(KeyCode.Q) && !isAnimating)
        {
            ToggleQuestPanel();
        }
        
        // Handle animation
        if (isAnimating)
        {
            animationTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(animationTime / slideDuration);
            float curveValue = slideCurve.Evaluate(normalizedTime);
            
            if (isQuestPanelVisible)
            {
                panelRectTransform.localScale = Vector3.Lerp(hiddenScale, visibleScale, curveValue);
                backgroundOverlay.color = Color.Lerp(overlayStartColor, overlayEndColor, curveValue);
            }
            else
            {
                panelRectTransform.localScale = Vector3.Lerp(visibleScale, hiddenScale, curveValue);
                backgroundOverlay.color = Color.Lerp(overlayEndColor, overlayStartColor, curveValue);
            }
            
            if (normalizedTime >= 1f)
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
            if (audioSource != null && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
            // Pause the game
            Time.timeScale = 0f;
        }
        else
        {
            if (audioSource != null && closeSound != null)
            {
                audioSource.PlayOneShot(closeSound);
            }
            // Resume the game
            Time.timeScale = 1f;
        }
        
        // Start animation
        animationTime = 0f;
        isAnimating = true;
    }
    
    private void UpdateQuestUI()
    {
        if (quests.Count == 0 || currentQuestIndex >= quests.Count)
        {
            questTitleText.text = "Tüm Görevler Tamamlandı!";
            questDescriptionText.text = "Tebrikler! Tüm görevleri başarıyla tamamladınız.";
            progressBar.value = progressBar.maxValue;
            progressText.text = "100%";
            return;
        }
        
        Quest currentQuest = quests[currentQuestIndex];
        
        if (!currentQuest.isUnlocked)
        {
            questTitleText.text = "Görev Kilitli";
            questDescriptionText.text = "Bu görevi görmek için önceki görevi tamamlamalısınız.";
            progressBar.value = 0;
            progressText.text = "0%";
            return;
        }
        
        questTitleText.text = currentQuest.title;
        questDescriptionText.text = currentQuest.description;
        
        float progress = (float)currentQuest.currentStep / currentQuest.totalSteps;
        progressBar.value = progress * progressBar.maxValue;
        progressText.text = $"{Mathf.RoundToInt(progress * 100)}%";
    }
    
    public void AdvanceQuestStep()
    {
        if (quests.Count == 0 || currentQuestIndex >= quests.Count)
            return;
            
        Quest currentQuest = quests[currentQuestIndex];
        
        if (!currentQuest.isUnlocked)
            return;
            
        currentQuest.currentStep++;
        
        if (currentQuest.currentStep >= currentQuest.totalSteps)
        {
            currentQuest.isCompleted = true;
            currentQuest.currentStep = currentQuest.totalSteps;
            
            // Görev tamamlama sesi
            if (audioSource != null && questCompleteSound != null)
            {
                audioSource.PlayOneShot(questCompleteSound);
            }
            
            // Bir sonraki görevi aç
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
        Quest newQuest = new Quest
        {
            title = title,
            description = description,
            totalSteps = totalSteps,
            currentStep = 0,
            isCompleted = false,
            isUnlocked = false
        };
        
        quests.Add(newQuest);
        
        // İlk görev ise otomatik olarak aç
        if (quests.Count == 1)
        {
            newQuest.isUnlocked = true;
            UpdateQuestUI();
        }
    }
    
    public bool IsCurrentQuestCompleted()
    {
        if (quests.Count == 0 || currentQuestIndex >= quests.Count)
            return true;
            
        return quests[currentQuestIndex].isCompleted;
    }
    
    public int GetCurrentQuestIndex()
    {
        return currentQuestIndex;
    }
}
