using UnityEngine;
using System.Collections;

/// <summary>
/// Reusable UI animation component for smooth open/close transitions.
/// Provides fade + scale animations for panels.
/// Polish visual - subtle, fast, non-intrusive.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Duration of fade/scale animation")]
    public float fadeDuration = 0.15f;
    
    [Tooltip("Starting scale for open animation (1.0 = no scale effect)")]
    [Range(0.8f, 1.0f)]
    public float scaleFrom = 0.95f;
    
    [Tooltip("Use unscaled time (works when game is paused)")]
    public bool useUnscaledTime = true;
    
    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Coroutine currentAnimation;
    
    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }
    
    /// <summary>
    /// Plays the open animation (fade in + scale up).
    /// </summary>
    public void PlayOpen()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(OpenRoutine());
    }
    
    /// <summary>
    /// Plays the close animation (fade out).
    /// Calls onComplete when finished.
    /// </summary>
    public void PlayClose(System.Action onComplete = null)
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
        }
        currentAnimation = StartCoroutine(CloseRoutine(onComplete));
    }
    
    /// <summary>
    /// Immediately sets the panel to fully visible state.
    /// Use when you need to skip animation.
    /// </summary>
    public void SetVisible()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        
        if (rect != null)
        {
            rect.localScale = Vector3.one;
        }
    }
    
    /// <summary>
    /// Immediately sets the panel to hidden state.
    /// Use when you need to skip animation.
    /// </summary>
    public void SetHidden()
    {
        if (currentAnimation != null)
        {
            StopCoroutine(currentAnimation);
            currentAnimation = null;
        }
        
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }
    
    IEnumerator OpenRoutine()
    {
        float t = 0f;
        
        // Start hidden and scaled down
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        rect.localScale = Vector3.one * scaleFrom;
        
        while (t < fadeDuration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float progress = Mathf.Clamp01(t / fadeDuration);
            
            // Smooth easing (ease out)
            float easedProgress = 1f - Mathf.Pow(1f - progress, 2f);
            
            canvasGroup.alpha = easedProgress;
            rect.localScale = Vector3.Lerp(
                Vector3.one * scaleFrom,
                Vector3.one,
                easedProgress
            );
            
            yield return null;
        }
        
        // Ensure final state
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        rect.localScale = Vector3.one;
        
        currentAnimation = null;
    }
    
    IEnumerator CloseRoutine(System.Action onComplete)
    {
        float t = 0f;
        float startAlpha = canvasGroup.alpha;
        
        // Disable interaction immediately
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        while (t < fadeDuration)
        {
            t += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            float progress = Mathf.Clamp01(t / fadeDuration);
            
            canvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, progress);
            
            yield return null;
        }
        
        // Ensure final state
        canvasGroup.alpha = 0f;
        
        currentAnimation = null;
        
        // Invoke callback
        onComplete?.Invoke();
    }
    
    /// <summary>
    /// Check if an animation is currently playing.
    /// </summary>
    public bool IsAnimating => currentAnimation != null;
}

