using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages toast notifications with pooling, queue, and animations.
/// Displays temporary messages with icons and sounds.
/// </summary>
public class ToastNotificationManager : MonoBehaviour
{
    public static ToastNotificationManager Instance { get; private set; }
    
    public enum ToastType
    {
        Success,
        Warning,
        Error,
        Info
    }
    
    [System.Serializable]
    public class ToastStyle
    {
        public ToastType type;
        public Color backgroundColor;
        public Sprite icon;
        public string soundName;
    }
    
    [Header("Prefab")]
    public GameObject toastPrefab;
    
    [Header("Settings")]
    public int poolSize = 10;
    public int prewarmCount = 5;
    public float displayDuration = 3f;
    public float slideSpeed = 500f;
    public float spacing = 10f;
    
    [Header("Styles")]
    public List<ToastStyle> toastStyles = new List<ToastStyle>();
    
    [Header("Container")]
    public RectTransform toastContainer;
    
    // Pools
    private Queue<GameObject> availableToasts = new Queue<GameObject>();
    private List<GameObject> activeToasts = new List<GameObject>();
    
    // Message queue
    private Queue<ToastMessage> messageQueue = new Queue<ToastMessage>();
    private bool isDisplaying = false;
    
    // Style lookup
    private Dictionary<ToastType, ToastStyle> styleDict = new Dictionary<ToastType, ToastStyle>();
    
    private class ToastMessage
    {
        public string message;
        public ToastType type;
        public float duration;
    }
    
    void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Build style dictionary
        foreach (var style in toastStyles)
        {
            styleDict[style.type] = style;
        }
        
        // Initialize pool
        InitializePool();
    }
    
    /// <summary>
    /// Initializes the toast pool.
    /// </summary>
    void InitializePool()
    {
        if (toastPrefab == null)
        {
            Debug.LogError("[ToastNotificationManager] Toast prefab not assigned!");
            return;
        }
        
        if (toastContainer == null)
        {
            Debug.LogError("[ToastNotificationManager] Toast container not assigned!");
            return;
        }
        
        for (int i = 0; i < prewarmCount; i++)
        {
            CreateToast();
        }
        
        Debug.Log($"[ToastNotificationManager] Initialized with {prewarmCount} toasts");
    }
    
    /// <summary>
    /// Creates a new toast GameObject.
    /// </summary>
    GameObject CreateToast()
    {
        GameObject toast = Instantiate(toastPrefab, toastContainer);
        toast.SetActive(false);
        availableToasts.Enqueue(toast);
        return toast;
    }
    
    /// <summary>
    /// Gets a toast from the pool.
    /// </summary>
    GameObject GetToast()
    {
        GameObject toast;
        
        if (availableToasts.Count > 0)
        {
            toast = availableToasts.Dequeue();
        }
        else if (activeToasts.Count < poolSize)
        {
            toast = CreateToast();
        }
        else
        {
            Debug.LogWarning("[ToastNotificationManager] Pool exhausted! Oldest toast will be reused.");
            toast = activeToasts[0];
            ReturnToast(toast);
        }
        
        return toast;
    }
    
    /// <summary>
    /// Returns a toast to the pool.
    /// </summary>
    void ReturnToast(GameObject toast)
    {
        if (toast == null) return;
        
        toast.SetActive(false);
        activeToasts.Remove(toast);
        availableToasts.Enqueue(toast);
    }
    
    /// <summary>
    /// Shows a toast notification (static method).
    /// </summary>
    public static void Show(string message, ToastType type = ToastType.Info, float duration = -1f)
    {
        if (Instance == null)
        {
            Debug.LogWarning("[ToastNotificationManager] Instance not found!");
            return;
        }
        
        if (duration < 0) duration = Instance.displayDuration;
        
        Instance.QueueMessage(message, type, duration);
    }
    
    /// <summary>
    /// Convenience methods for different toast types.
    /// </summary>
    public static void ShowSuccess(string message) => Show(message, ToastType.Success);
    public static void ShowWarning(string message) => Show(message, ToastType.Warning);
    public static void ShowError(string message) => Show(message, ToastType.Error);
    public static void ShowInfo(string message) => Show(message, ToastType.Info);
    
    /// <summary>
    /// Queues a message for display.
    /// </summary>
    void QueueMessage(string message, ToastType type, float duration)
    {
        ToastMessage toastMsg = new ToastMessage
        {
            message = message,
            type = type,
            duration = duration
        };
        
        messageQueue.Enqueue(toastMsg);
        
        if (!isDisplaying)
        {
            StartCoroutine(ProcessQueue());
        }
    }
    
    /// <summary>
    /// Processes the message queue.
    /// </summary>
    IEnumerator ProcessQueue()
    {
        isDisplaying = true;
        
        while (messageQueue.Count > 0)
        {
            ToastMessage msg = messageQueue.Dequeue();
            yield return StartCoroutine(DisplayToast(msg));
            
            // Small delay between toasts
            yield return new WaitForSeconds(0.2f);
        }
        
        isDisplaying = false;
    }
    
    /// <summary>
    /// Displays a single toast notification.
    /// </summary>
    IEnumerator DisplayToast(ToastMessage msg)
    {
        GameObject toast = GetToast();
        activeToasts.Add(toast);
        
        // Setup toast content
        SetupToastContent(toast, msg);
        
        // Position toast
        RectTransform toastRect = toast.GetComponent<RectTransform>();
        float startY = -toastRect.rect.height;
        float targetY = CalculateToastPosition();
        toastRect.anchoredPosition = new Vector2(0, startY);
        
        // Activate
        toast.SetActive(true);
        
        // Play sound based on toast type
        if (AudioManager.Instance != null)
        {
            switch (msg.type)
            {
                case ToastType.Success:
                    AudioManager.Instance.Play(AudioManager.Instance.toastSuccess);
                    break;
                case ToastType.Warning:
                case ToastType.Error:
                    AudioManager.Instance.Play(AudioManager.Instance.toastError);
                    break;
                case ToastType.Info:
                default:
                    AudioManager.Instance.Play(AudioManager.Instance.toastInfo);
                    break;
            }
        }
        
        // Get CanvasGroup for fade
        CanvasGroup canvasGroup = toast.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = toast.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0f;
        
        // Slide up and fade in (usando unscaledDeltaTime para funcionar quando pausado)
        float elapsed = 0f;
        float slideInDuration = 0.3f;
        
        while (elapsed < slideInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / slideInDuration;
            
            toastRect.anchoredPosition = new Vector2(0, Mathf.Lerp(startY, targetY, t));
            canvasGroup.alpha = t;
            
            yield return null;
        }
        
        toastRect.anchoredPosition = new Vector2(0, targetY);
        canvasGroup.alpha = 1f;
        
        // Wait for display duration (usando Realtime para funcionar quando pausado)
        yield return new WaitForSecondsRealtime(msg.duration);
        
        // Fade out
        elapsed = 0f;
        float fadeOutDuration = 0.3f;
        
        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            canvasGroup.alpha = 1f - (elapsed / fadeOutDuration);
            yield return null;
        }
        
        // Return to pool
        ReturnToast(toast);
        
        // Reposition remaining toasts
        RepositionActiveToasts();
    }
    
    /// <summary>
    /// Sets up toast content (text, icon, colors).
    /// </summary>
    void SetupToastContent(GameObject toast, ToastMessage msg)
    {
        // Find components
        Image background = toast.GetComponent<Image>();
        Image icon = toast.transform.Find("Icon")?.GetComponent<Image>();
        TextMeshProUGUI messageText = toast.transform.Find("MessageText")?.GetComponent<TextMeshProUGUI>();
        
        // Apply style
        if (styleDict.ContainsKey(msg.type))
        {
            ToastStyle style = styleDict[msg.type];
            
            if (background != null)
                background.color = style.backgroundColor;
            
            if (icon != null && style.icon != null)
                icon.sprite = style.icon;
        }
        
        // Set message text
        if (messageText != null)
            messageText.text = msg.message;
    }
    
    /// <summary>
    /// Calculates the Y position for the next toast.
    /// </summary>
    float CalculateToastPosition()
    {
        float baseY = spacing;
        
        foreach (GameObject activeToast in activeToasts)
        {
            if (activeToast.activeSelf)
            {
                RectTransform rect = activeToast.GetComponent<RectTransform>();
                baseY += rect.rect.height + spacing;
            }
        }
        
        return baseY;
    }
    
    /// <summary>
    /// Repositions all active toasts after one is removed.
    /// </summary>
    void RepositionActiveToasts()
    {
        float currentY = spacing;
        
        foreach (GameObject activeToast in activeToasts)
        {
            if (activeToast.activeSelf)
            {
                RectTransform rect = activeToast.GetComponent<RectTransform>();
                StartCoroutine(SmoothRepositionToast(rect, currentY));
                currentY += rect.rect.height + spacing;
            }
        }
    }
    
    /// <summary>
    /// Smoothly repositions a toast.
    /// </summary>
    IEnumerator SmoothRepositionToast(RectTransform toastRect, float targetY)
    {
        float startY = toastRect.anchoredPosition.y;
        float elapsed = 0f;
        float duration = 0.2f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            
            toastRect.anchoredPosition = new Vector2(0, Mathf.Lerp(startY, targetY, t));
            yield return null;
        }
        
        toastRect.anchoredPosition = new Vector2(0, targetY);
    }
}

