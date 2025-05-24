

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HintManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private TextMeshProUGUI hintTitleText;
    [SerializeField] private TextMeshProUGUI hintDescriptionText;
    [SerializeField] private Image backgroundOverlay;
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Animation Settings")]
    [SerializeField] private float slideDuration = 0.3f;
    [SerializeField] private AnimationCurve slideCurve;

    [Header("Hint Settings")]
    [SerializeField] private float hintDisplayDuration = 60f;
    [SerializeField] private float hintLockDuration = 300f;

    private bool isHintPanelVisible = false;
    private Vector3 hiddenScale;
    private Vector3 visibleScale;
    private RectTransform panelRectTransform;
    private float animationTime = 0f;
    private bool isAnimating = false;
    private Color overlayStartColor;
    private Color overlayEndColor;
    private Coroutine hintCoroutine;
    private Coroutine timerCoroutine;

    private Dictionary<int, string[]> questHints = new Dictionary<int, string[]>()
    {
        { 0, new[] {"1. Işığa ihtiyacın olacak...", "2. Yolun parçalarını birleştirmelisin...", "3. Çantaları ve dolapları kontrol etmeyi unutma."} },
        { 1, new[] {"1. Anahtar genellikle kapının yakınında bir yerde olur.", "2. Gece dışarıda kalmak istediğine emin misin?", "3. Pencere ve arka kapıları da kontrol et."} },
        { 2, new[] {"1. Eskiye önem ver!", "2. Kilisenin içindeki sembollere dikkat et.", "3. Bodruma girmek istediğine emin misin?"} },
        { 3, new[] {"1. Yolu birleştirdin mi?", "2. Frekansları birleştirmek zorundasın...", "3. Radyo sinyallerini dinle ve not al."} },
        { 4, new[] {"1. Hasta kayıtları...gizliliği ihlal ediyorlar...", "2. Günlüğünü okumak zorundasın..", "3. Tedavi odası mı yoksa koridor mu?"} },
        { 5, new[] {"1. Önemli sayı kaçtır ?", "2. Ritüel kitabı karanlık bir yerde saklanmış olabilir.", "3. Ritüel sırasında dikkatli ol, her adımı doğru takip et."} },
        { 6, new[] {"1. Kilisenin tarihi...", "2. Murpheus'un geçmişi...", "3. Kilisenin alt katmanlarında neler vardır?"} },
        { 7, new[] {"1. Ritüel sırasında dikkatli ol, her adımı doğru takip et.", "2. Her obje için özel bir yer olmalı."} },
        { 8, new[] {"1. İzlerini takip et.", "2. Murpheus.. canavarı yeterinde tanıdın mı?", "3. Canavarın kendisini görmek ister misin?"} },
        { 9, new[] {"1. Ritüeli tamamlamak için tüm objeleri doğru sırayla kullan.", "2. Murpheus'un gerçek kimliğini doğru buldun mu, görelim..", "3. Son kararını verirken dikkatli ol, her seçimin bir sonucu var."} }
    };

    private Dictionary<int, int> currentHintIndex = new Dictionary<int, int>();

    private void Start()
    {
        if (hintPanel == null || hintTitleText == null || hintDescriptionText == null || timerText == null || backgroundOverlay == null)
        {
            Debug.LogError("UI referansları eksik! HintManager çalışamaz.");
            return;
        }

        panelRectTransform = hintPanel.GetComponent<RectTransform>();
        visibleScale = Vector3.one;
        hiddenScale = Vector3.zero;

        overlayStartColor = new Color(0, 0, 0, 0);
        overlayEndColor = new Color(0, 0, 0, 0.7f);

        panelRectTransform.localScale = hiddenScale;
        hintPanel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && !isAnimating)
        {
            ToggleHintPanel();
        }

        if (isAnimating)
        {
            animationTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(animationTime / slideDuration);
            float value = slideCurve.Evaluate(t);

            panelRectTransform.localScale = Vector3.Lerp(isHintPanelVisible ? hiddenScale : visibleScale, isHintPanelVisible ? visibleScale : hiddenScale, value);
            backgroundOverlay.color = Color.Lerp(isHintPanelVisible ? overlayStartColor : overlayEndColor, isHintPanelVisible ? overlayEndColor : overlayStartColor, value);

            if (t >= 1f)
            {
                isAnimating = false;
                if (!isHintPanelVisible)
                {
                    hintPanel.SetActive(false);
                    backgroundOverlay.gameObject.SetActive(false);
                }
            }
        }
    }

    private void ToggleHintPanel()
    {
        isHintPanelVisible = !isHintPanelVisible;

        if (isHintPanelVisible)
        {
            hintPanel.SetActive(true);
            backgroundOverlay.gameObject.SetActive(true);
        }
        else
        {
            backgroundOverlay.gameObject.SetActive(false);
        }

        animationTime = 0f;
        isAnimating = true;
    }

    public void ShowHint(int questIndex)
    {
        if (hintCoroutine != null)
            StopCoroutine(hintCoroutine);

        if (questHints.TryGetValue(questIndex, out var hints))
        {
            if (!currentHintIndex.ContainsKey(questIndex))
                currentHintIndex[questIndex] = 0;

            int index = currentHintIndex[questIndex];
            if (index < hints.Length)
            {
                hintCoroutine = StartCoroutine(ShowHintCoroutine(hints[index]));
                currentHintIndex[questIndex]++;
            }
        }
    }

    private IEnumerator ShowHintCoroutine(string hint)
    {
        hintTitleText.text = "Görev İpucu";
        hintDescriptionText.text = hint;
        isHintPanelVisible = true;
        hintPanel.SetActive(true);
        backgroundOverlay.gameObject.SetActive(true);

        if (timerCoroutine != null)
            StopCoroutine(timerCoroutine);
        timerCoroutine = StartCoroutine(UpdateTimer());

        yield return new WaitForSecondsRealtime(hintDisplayDuration);

        isHintPanelVisible = false;
        hintPanel.SetActive(false);
        backgroundOverlay.gameObject.SetActive(false);

        yield return new WaitForSecondsRealtime(hintLockDuration);
        hintCoroutine = null;
    }

    private IEnumerator UpdateTimer()
    {
        float remaining = hintDisplayDuration;
        while (remaining > 0)
        {
            timerText.text = string.Format("Kalan Süre: {0:00}:{1:00}", Mathf.FloorToInt(remaining / 60), Mathf.FloorToInt(remaining % 60));
            yield return new WaitForSecondsRealtime(1);
            remaining--;
        }
        timerText.text = "Süre Doldu!";
    }

    public bool IsHintAvailable() => hintCoroutine == null;
}


