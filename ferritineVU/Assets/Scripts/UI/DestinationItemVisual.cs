using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Componente visual para itens de destino no TeleportSelectorUI.
/// Gerencia hover, seleção e feedback visual.
/// </summary>
public class DestinationItemVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Visual References")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image borderImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI coordsText;
    
    [Header("Colors - Normal State")]
    [SerializeField] private Color normalBackgroundColor = new Color32(45, 45, 48, 255);
    [SerializeField] private Color normalBorderColor = new Color32(60, 60, 65, 255);
    [SerializeField] private Color normalTextColor = new Color32(200, 200, 200, 255);
    [SerializeField] private Color normalCoordsColor = new Color32(140, 140, 140, 255);
    
    [Header("Colors - Hover State")]
    [SerializeField] private Color hoverBackgroundColor = new Color32(60, 60, 65, 255);
    [SerializeField] private Color hoverBorderColor = new Color32(100, 149, 237, 255); // Cornflower blue
    [SerializeField] private Color hoverTextColor = Color.white;
    
    [Header("Colors - Selected State")]
    [SerializeField] private Color selectedBackgroundColor = new Color32(40, 80, 120, 255); // Dark blue
    [SerializeField] private Color selectedBorderColor = new Color32(65, 165, 255, 255);   // Bright blue
    [SerializeField] private Color selectedTextColor = Color.white;
    
    [Header("Animation")]
    [SerializeField] private float transitionSpeed = 10f;
    [SerializeField] private float borderWidth = 3f;
    
    // State
    private bool isSelected = false;
    private bool isHovered = false;
    private Color targetBackgroundColor;
    private Color targetBorderColor;
    private Color currentBorderColor;
    
    // Callback
    private System.Action onClick;
    
    void Awake()
    {
        // Auto-find components if not assigned
        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();
        
        // Try to find border (should be a child named "Border" or similar)
        if (borderImage == null)
        {
            Transform borderTransform = transform.Find("Border");
            if (borderTransform != null)
                borderImage = borderTransform.GetComponent<Image>();
        }
        
        // Find text components
        if (nameText == null)
        {
            Transform textGroup = transform.Find("TextGroup");
            if (textGroup != null)
            {
                Transform nameTransform = textGroup.Find("NameText");
                if (nameTransform != null)
                    nameText = nameTransform.GetComponent<TextMeshProUGUI>();
                
                Transform coordsTransform = textGroup.Find("CoordsText");
                if (coordsTransform != null)
                    coordsText = coordsTransform.GetComponent<TextMeshProUGUI>();
            }
        }
        
        // Initialize colors
        targetBackgroundColor = normalBackgroundColor;
        targetBorderColor = normalBorderColor;
        currentBorderColor = normalBorderColor;
        
        ApplyColorsImmediate();
    }
    
    void Update()
    {
        // Smooth color transitions
        if (backgroundImage != null)
        {
            backgroundImage.color = Color.Lerp(backgroundImage.color, targetBackgroundColor, Time.deltaTime * transitionSpeed);
        }
        
        if (borderImage != null)
        {
            currentBorderColor = Color.Lerp(currentBorderColor, targetBorderColor, Time.deltaTime * transitionSpeed);
            borderImage.color = currentBorderColor;
        }
    }
    
    /// <summary>
    /// Configura o item com dados e callback.
    /// </summary>
    public void Setup(string name, string coords, System.Action onClickCallback)
    {
        if (nameText != null)
            nameText.text = name;
        
        if (coordsText != null)
            coordsText.text = coords;
        
        onClick = onClickCallback;
        
        // Reset state
        isSelected = false;
        isHovered = false;
        UpdateVisualState();
    }
    
    /// <summary>
    /// Define se este item está selecionado.
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        UpdateVisualState();
        
        // Scale up slightly
        transform.localScale = Vector3.one * 1.02f;
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        UpdateVisualState();
        
        // Scale back
        transform.localScale = Vector3.one;
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
        
        // Pulse effect
        StartCoroutine(PulseEffect());
    }
    
    private void UpdateVisualState()
    {
        if (isSelected)
        {
            targetBackgroundColor = selectedBackgroundColor;
            targetBorderColor = selectedBorderColor;
            
            if (nameText != null)
                nameText.color = selectedTextColor;
            if (coordsText != null)
                coordsText.color = selectedTextColor;
        }
        else if (isHovered)
        {
            targetBackgroundColor = hoverBackgroundColor;
            targetBorderColor = hoverBorderColor;
            
            if (nameText != null)
                nameText.color = hoverTextColor;
            if (coordsText != null)
                coordsText.color = hoverTextColor;
        }
        else
        {
            targetBackgroundColor = normalBackgroundColor;
            targetBorderColor = normalBorderColor;
            
            if (nameText != null)
                nameText.color = normalTextColor;
            if (coordsText != null)
                coordsText.color = normalCoordsColor;
        }
        
        // Show/hide border based on state
        if (borderImage != null)
        {
            borderImage.gameObject.SetActive(isSelected || isHovered);
        }
    }
    
    private void ApplyColorsImmediate()
    {
        if (backgroundImage != null)
            backgroundImage.color = normalBackgroundColor;
        
        if (borderImage != null)
        {
            borderImage.color = normalBorderColor;
            borderImage.gameObject.SetActive(false);
        }
        
        if (nameText != null)
            nameText.color = normalTextColor;
        
        if (coordsText != null)
            coordsText.color = normalCoordsColor;
    }
    
    private System.Collections.IEnumerator PulseEffect()
    {
        // Quick scale pulse on click
        float duration = 0.1f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = 1f + Mathf.Sin(elapsed / duration * Mathf.PI) * 0.05f;
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        
        transform.localScale = isHovered ? Vector3.one * 1.02f : Vector3.one;
    }
    
    /// <summary>
    /// Cria a estrutura de borda programaticamente se não existir.
    /// Chamar após adicionar o componente ao prefab.
    /// </summary>
    public void CreateBorderIfNeeded()
    {
        if (borderImage != null) return;
        
        // Create border GameObject
        GameObject borderGO = new GameObject("Border");
        borderGO.transform.SetParent(transform);
        borderGO.transform.SetAsFirstSibling(); // Behind content
        
        // Setup RectTransform to stretch
        RectTransform borderRect = borderGO.AddComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = new Vector2(-borderWidth, -borderWidth);
        borderRect.offsetMax = new Vector2(borderWidth, borderWidth);
        borderRect.localScale = Vector3.one;
        
        // Add Image
        borderImage = borderGO.AddComponent<Image>();
        borderImage.color = normalBorderColor;
        borderGO.SetActive(false);
        
        Debug.Log("[DestinationItemVisual] Border created programmatically");
    }
}

