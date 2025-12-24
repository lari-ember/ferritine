using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Controlador de efeito Tilt-Shift para fazer a cidade parecer uma maquete/miniatura.
/// Utiliza Depth of Field do URP Post-Processing para criar o efeito característico.
/// 
/// Como funciona:
/// - Aplica desfoque nas bordas superior e inferior da imagem
/// - Mantém o centro em foco
/// - Simula o efeito de lentes tilt-shift usadas em fotografia de miniaturas
/// 
/// Configuração:
/// 1. Adicione um Volume com perfil de Post-Processing na cena
/// 2. Adicione este script à câmera
/// 3. Atribua o Volume no Inspector ou deixe auto-detect
/// </summary>
[RequireComponent(typeof(Camera))]
public class TiltShiftEffect : MonoBehaviour
{
    [Header("Tilt-Shift Settings")]
    [Tooltip("Enable or disable the tilt-shift effect")]
    public bool enableTiltShift = true;
    
    [Tooltip("Focus distance from camera (auto-calculated if 0)")]
    [Range(0f, 500f)]
    public float focusDistance = 0f;
    
    [Tooltip("Size of the focused area (smaller = more blur)")]
    [Range(0.1f, 20f)]
    public float focalLength = 50f;
    
    [Tooltip("Aperture - controls blur intensity (smaller = more blur)")]
    [Range(1f, 32f)]
    public float aperture = 5.6f;
    
    [Tooltip("Blade count for bokeh shape")]
    [Range(3, 9)]
    public int bladeCount = 5;
    
    [Tooltip("Blade curvature for softer bokeh")]
    [Range(0f, 1f)]
    public float bladeCurvature = 1f;
    
    [Tooltip("Blade rotation angle")]
    [Range(-180f, 180f)]
    public float bladeRotation = 0f;
    
    [Header("Auto-Focus Settings")]
    [Tooltip("Auto-focus on terrain below camera center")]
    public bool autoFocus = true;
    
    [Tooltip("Layer mask for auto-focus raycast")]
    public LayerMask focusLayerMask;
    
    [Tooltip("Update focus distance every frame (performance impact)")]
    public bool continuousAutoFocus = true;
    
    [Tooltip("Smooth focus distance changes")]
    [Range(0.01f, 1f)]
    public float focusSmoothTime = 0.3f;
    
    [Header("References")]
    [Tooltip("Post-processing volume (auto-detected if null)")]
    public Volume postProcessVolume;
    
    // Private state
    private Camera mainCamera;
    private DepthOfField depthOfField;
    private float targetFocusDistance;
    private float currentFocusDistance;
    private float focusVelocity;
    private bool isInitialized = false;
    
    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        
        // Set default focus layer mask
        if (focusLayerMask.value == 0)
        {
            focusLayerMask = LayerMask.GetMask("Default", "Terrain", "Ground");
        }
    }
    
    void Start()
    {
        Initialize();
    }
    
    void Initialize()
    {
        // Auto-find volume if not assigned
        if (postProcessVolume == null)
        {
            postProcessVolume = FindFirstObjectByType<Volume>();
            
            if (postProcessVolume == null)
            {
                Debug.LogWarning("[TiltShiftEffect] No Post-Processing Volume found in scene. " +
                               "Create one with a URP Volume Profile to enable tilt-shift effect.");
                return;
            }
        }
        
        // Get or add DepthOfField override
        if (!postProcessVolume.profile.TryGet(out depthOfField))
        {
            // Try to add it
            depthOfField = postProcessVolume.profile.Add<DepthOfField>(true);
            Debug.Log("[TiltShiftEffect] Added DepthOfField to volume profile");
        }
        
        if (depthOfField == null)
        {
            Debug.LogError("[TiltShiftEffect] Failed to get/create DepthOfField override!");
            return;
        }
        
        // Initialize focus distance
        currentFocusDistance = focusDistance > 0 ? focusDistance : 50f;
        targetFocusDistance = currentFocusDistance;
        
        isInitialized = true;
        Debug.Log("[TiltShiftEffect] Initialized successfully");
        
        // Apply initial settings
        ApplyTiltShiftSettings();
    }
    
    void Update()
    {
        if (!isInitialized || depthOfField == null) return;
        
        // Auto-focus update
        if (autoFocus && (continuousAutoFocus || Mathf.Abs(targetFocusDistance - currentFocusDistance) < 0.1f))
        {
            UpdateAutoFocus();
        }
        
        // Smooth focus distance
        if (Mathf.Abs(targetFocusDistance - currentFocusDistance) > 0.01f)
        {
            currentFocusDistance = Mathf.SmoothDamp(
                currentFocusDistance, 
                targetFocusDistance, 
                ref focusVelocity, 
                focusSmoothTime
            );
            
            depthOfField.focusDistance.value = currentFocusDistance;
        }
    }
    
    void UpdateAutoFocus()
    {
        if (mainCamera == null) return;
        
        // Raycast from camera center downward
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, 500f, focusLayerMask))
        {
            targetFocusDistance = hit.distance;
        }
        else
        {
            // Fallback: estimate based on camera height
            targetFocusDistance = transform.position.y * 1.5f;
        }
    }
    
    /// <summary>
    /// Applies current tilt-shift settings to the DoF effect.
    /// </summary>
    public void ApplyTiltShiftSettings()
    {
        if (depthOfField == null) return;
        
        // Enable/disable effect
        depthOfField.active = enableTiltShift;
        
        if (!enableTiltShift) return;
        
        // Set mode to Bokeh for best quality tilt-shift
        depthOfField.mode.value = DepthOfFieldMode.Bokeh;
        depthOfField.mode.overrideState = true;
        
        // Focus settings
        depthOfField.focusDistance.value = currentFocusDistance;
        depthOfField.focusDistance.overrideState = true;
        
        depthOfField.focalLength.value = focalLength;
        depthOfField.focalLength.overrideState = true;
        
        depthOfField.aperture.value = aperture;
        depthOfField.aperture.overrideState = true;
        
        // Bokeh settings
        depthOfField.bladeCount.value = bladeCount;
        depthOfField.bladeCount.overrideState = true;
        
        depthOfField.bladeCurvature.value = bladeCurvature;
        depthOfField.bladeCurvature.overrideState = true;
        
        depthOfField.bladeRotation.value = bladeRotation;
        depthOfField.bladeRotation.overrideState = true;
        
        Debug.Log($"[TiltShiftEffect] Settings applied - Focus: {currentFocusDistance:F1}m, Aperture: f/{aperture}");
    }
    
    /// <summary>
    /// Enables the tilt-shift effect.
    /// </summary>
    public void EnableEffect()
    {
        enableTiltShift = true;
        ApplyTiltShiftSettings();
    }
    
    /// <summary>
    /// Disables the tilt-shift effect.
    /// </summary>
    public void DisableEffect()
    {
        enableTiltShift = false;
        if (depthOfField != null)
        {
            depthOfField.active = false;
        }
    }
    
    /// <summary>
    /// Toggles the tilt-shift effect.
    /// </summary>
    public void ToggleEffect()
    {
        if (enableTiltShift)
        {
            DisableEffect();
        }
        else
        {
            EnableEffect();
        }
    }
    
    /// <summary>
    /// Sets the focus distance manually.
    /// </summary>
    public void SetFocusDistance(float distance)
    {
        focusDistance = distance;
        targetFocusDistance = distance;
    }
    
    /// <summary>
    /// Sets the aperture (blur intensity).
    /// Lower values = more blur, higher values = less blur.
    /// </summary>
    public void SetAperture(float newAperture)
    {
        aperture = Mathf.Clamp(newAperture, 1f, 32f);
        ApplyTiltShiftSettings();
    }
    
    /// <summary>
    /// Sets the focal length (focused area size).
    /// Lower values = smaller focused area.
    /// </summary>
    public void SetFocalLength(float newFocalLength)
    {
        focalLength = Mathf.Clamp(newFocalLength, 0.1f, 20f);
        ApplyTiltShiftSettings();
    }
    
    /// <summary>
    /// Sets preset for different tilt-shift intensities.
    /// </summary>
    public void SetPreset(TiltShiftPreset preset)
    {
        switch (preset)
        {
            case TiltShiftPreset.Subtle:
                aperture = 8f;
                focalLength = 85f;
                break;
                
            case TiltShiftPreset.Normal:
                aperture = 5.6f;
                focalLength = 50f;
                break;
                
            case TiltShiftPreset.Strong:
                aperture = 2.8f;
                focalLength = 35f;
                break;
                
            case TiltShiftPreset.Extreme:
                aperture = 1.4f;
                focalLength = 24f;
                break;
        }
        
        ApplyTiltShiftSettings();
        Debug.Log($"[TiltShiftEffect] Preset set to: {preset}");
    }
    
    void OnValidate()
    {
        // Apply settings when changed in inspector
        if (isInitialized && depthOfField != null)
        {
            ApplyTiltShiftSettings();
        }
    }
    
    void OnDestroy()
    {
        // Optionally restore default DoF settings
        if (depthOfField != null)
        {
            depthOfField.active = false;
        }
    }
}

/// <summary>
/// Presets for tilt-shift effect intensity.
/// </summary>
public enum TiltShiftPreset
{
    Subtle,     // Minimal blur, more realistic
    Normal,     // Balanced miniature effect
    Strong,     // Pronounced miniature look
    Extreme     // Maximum miniature effect
}
