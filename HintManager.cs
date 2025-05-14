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
    
    [Header("Audio")]
    [SerializeField] private AudioClip hintSound;
    [SerializeField] private AudioSource audioSource;
    
    [Header("Hint Settings")]
    [SerializeField] private float hintDisplayDuration = 60f; // 1 dakika
    [SerializeField] private float hintLockDuration = 300f; // 5 dakika
    
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
    
    // Görev ipuçları
    private Dictionary<int, string[]> questHints = new Dictionary<int, string[]>()
    {
        { 0, new string[] {
            "1. Işığa ihtiyacın olacak...",
            "2. Yolun parçalarını birleştirmelisin...",
            "3. Çantaları ve dolapları kontrol etmeyi unutma."
        }},
            
        { 1, new string[] {
            "1. Anahtar genellikle kapının yakınında bir yerde olur.",
            "2. Gece dışarıda kalmak istediğine emin misin?",
            "3. Pencere ve arka kapıları da kontrol et."
        }},
            
        { 2, new string[] {
            "1. Eskiye önem ver!",
            "2. Kilisenin içindeki sembollere dikkat et.",
            "3. Bodruma girmek istediğine emin misin?"
        }},
            
        { 3, new string[] {
            "1. Yolu birleştirdin mi?",
            "2. Frekansları birleştirmek zorundasın...",
            "3. Radyo sinyallerini dinle ve not al."
        }},
            
        { 4, new string[] {
            "1. Hasta kayıtları...gizliliği ihlal ediyorlar...",
            "2. Günlüğünü okumak zorundasın..",
            "3. Tedavi odası mı yoksa koridor mu?"
        }},
            
        { 5, new string[] {
            "1. Önemli sayı kaçtır ?",
            "2. Ritüel kitabı karanlık bir yerde saklanmış olabilir.",
            "3. Ritüel sırasında dikkatli ol, her adımı doğru takip et."
        }},
            
        { 6, new string[] {
            "1. Kilisenin tarihi...",
            "2. Murpheus'un geçmişi...",
            "3. Kilisenin alt katmanlarında neler vardır?"
        }},
            
        { 7, new string[] {
            "1. Ritüel sırasında dikkatli ol, her adımı doğru takip et.",
            "2. Her obje için özel bir yer olmalı."
        }},
            
        { 8, new string[] {
            "1. İzlerini takip et.",
            "2. Murpheus.. canavarı yeterinde tanıdın mı?",
            "3. Canavarın kendisini görmek ister misin?"
        }},
            
        { 9, new string[] {
            "1. Ritüeli tamamlamak için tüm objeleri doğru sırayla kullan.",
            "2. Murpheus'un gerçek kimliğini doğru buldun mu, görelim..",
            "3. Son kararını verirken dikkatli ol, her seçimin bir sonucu var."
        }}
    };

    private Dictionary<int, int> currentHintIndex = new Dictionary<int, int>();
    
    private void Start()
    {
        if (hintPanel == null)
        {
            Debug.LogError("Hint Panel reference is missing!");
            return;
        }
        
        panelRectTransform = hintPanel.GetComponent<RectTransform>();
        visibleScale = Vector3.one;
        hiddenScale = Vector3.zero;
        
        // Initialize overlay colors
        overlayStartColor = new Color(0, 0, 0, 0);
        overlayEndColor = new Color(0, 0, 0, 0.7f);
        
        // Hide panel initially
        panelRectTransform.localScale = hiddenScale;
        hintPanel.SetActive(false);
    }
    
    private void Update()
    {
        // Check for H key press to toggle hint panel
        if (Input.GetKeyDown(KeyCode.H) && !isAnimating)
        {
            ToggleHintPanel();
        }
        
        // Handle animation
        if (isAnimating)
        {
            animationTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(animationTime / slideDuration);
            float curveValue = slideCurve.Evaluate(normalizedTime);
            
            if (isHintPanelVisible)
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
            if (audioSource != null && hintSound != null)
            {
                audioSource.PlayOneShot(hintSound);
            }
            // Pause the game
            Time.timeScale = 0f;
        }
        else
        {
            // Resume the game
            Time.timeScale = 1f;
        }
        
        // Start animation
        animationTime = 0f;
        isAnimating = true;
    }
    
    public void ShowHint(int questIndex)
    {
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
        }
        
        if (questHints.ContainsKey(questIndex))
        {
            // İlk ipucu için index oluştur
            if (!currentHintIndex.ContainsKey(questIndex))
            {
                currentHintIndex[questIndex] = 0;
            }
            
            // Mevcut ipucunu al
            string[] hints = questHints[questIndex];
            int currentIndex = currentHintIndex[questIndex];
            
            if (currentIndex < hints.Length)
            {
                string hint = hints[currentIndex];
                hintCoroutine = StartCoroutine(ShowHintCoroutine(hint));
                
                // Bir sonraki ipucuna geç
                currentHintIndex[questIndex]++;
            }
        }
    }
    
    private IEnumerator ShowHintCoroutine(string hint)
    {
        // İpucunu göster
        hintTitleText.text = "Görev İpucu";
        hintDescriptionText.text = hint;
        
        // Panel'i göster
        isHintPanelVisible = true;
        hintPanel.SetActive(true);
        backgroundOverlay.gameObject.SetActive(true);
        
        if (audioSource != null && hintSound != null)
        {
            audioSource.PlayOneShot(hintSound);
        }
        
        // Geri sayım başlat
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(UpdateTimer());
        
        // 1 dakika bekle
        yield return new WaitForSeconds(hintDisplayDuration);
        
        // Panel'i gizle
        isHintPanelVisible = false;
        hintPanel.SetActive(false);
        backgroundOverlay.gameObject.SetActive(false);
        
        // 5 dakika bekle (ipucu kilitli)
        yield return new WaitForSeconds(hintLockDuration);
        
        // İpucu tekrar kullanılabilir
        hintCoroutine = null;
    }
    
    private IEnumerator UpdateTimer()
    {
        float remainingTime = hintDisplayDuration;
        
        while (remainingTime > 0)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("Kalan Süre: {0:00}:{1:00}", minutes, seconds);
            
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }
        
        timerText.text = "Süre Doldu!";
    }
    
    public bool IsHintAvailable()
    {
        return hintCoroutine == null;
    }
} 
