using UnityEngine;

/// <summary>
/// ScriptableObject para armazenar configurações persistentes de câmera.
/// Permite salvar preferências do jogador e presets de configuração.
/// </summary>
[CreateAssetMenu(fileName = "CameraSettings", menuName = "Ferritine/Camera Settings", order = 1)]
public class CameraSettings : ScriptableObject
{
    [Header("Movement")]
    [Tooltip("Base speed of camera movement")]
    [Range(5f, 50f)]
    public float movementSpeed = 20f;
    
    [Tooltip("Sprint speed multiplier")]
    [Range(1.5f, 5f)]
    public float sprintMultiplier = 2.5f;
    
    [Tooltip("Movement smoothing")]
    [Range(0f, 0.3f)]
    public float movementSmoothing = 0.1f;
    
    [Header("Rotation")]
    [Tooltip("Rotation speed (Q/E keys)")]
    [Range(30f, 180f)]
    public float rotationSpeed = 90f;
    
    [Tooltip("Pitch speed (R/F keys)")]
    [Range(15f, 90f)]
    public float pitchSpeed = 45f;
    
    [Tooltip("Minimum pitch angle")]
    [Range(0f, 45f)]
    public float minPitch = 10f;
    
    [Tooltip("Maximum pitch angle")]
    [Range(45f, 90f)]
    public float maxPitch = 80f;
    
    [Header("Zoom")]
    [Tooltip("Zoom speed (mouse wheel)")]
    [Range(5f, 30f)]
    public float zoomSpeed = 10f;
    
    [Tooltip("Minimum camera height")]
    [Range(1f, 20f)]
    public float minHeight = 5f;
    
    [Tooltip("Maximum camera height")]
    [Range(50f, 500f)]
    public float maxHeight = 200f;
    
    [Tooltip("Height adjust speed (+/- keys)")]
    [Range(5f, 30f)]
    public float heightAdjustSpeed = 15f;
    
    [Header("Edge Scrolling")]
    [Tooltip("Enable edge scrolling")]
    public bool enableEdgeScrolling = false;
    
    [Tooltip("Edge scroll border size (pixels)")]
    [Range(5f, 50f)]
    public float edgeScrollBorder = 20f;
    
    [Tooltip("Edge scroll speed multiplier")]
    [Range(0.5f, 3f)]
    public float edgeScrollSpeed = 1f;
    
    [Header("Mouse Pan")]
    [Tooltip("Enable middle mouse pan")]
    public bool enableMousePan = true;
    
    [Tooltip("Pan speed multiplier")]
    [Range(0.1f, 2f)]
    public float panSpeed = 0.5f;
    
    [Header("Orbit")]
    [Tooltip("Orbit rotation speed")]
    [Range(60f, 360f)]
    public float orbitSpeed = 180f;
    
    [Tooltip("Default orbit distance")]
    [Range(10f, 100f)]
    public float defaultOrbitDistance = 30f;
    
    [Header("Collision")]
    [Tooltip("Enable terrain collision")]
    public bool enableTerrainCollision = true;
    
    [Tooltip("Collision buffer distance")]
    [Range(0.5f, 10f)]
    public float collisionBuffer = 2f;
    
    [Header("Follow Mode")]
    [Tooltip("Follow smooth time")]
    [Range(0.1f, 1f)]
    public float followSmoothTime = 0.5f;
    
    [Tooltip("Follow offset")]
    public Vector3 followOffset = new Vector3(0, 10, -10);
    
    [Tooltip("Follow rotation speed")]
    [Range(0.5f, 5f)]
    public float followRotationSpeed = 2f;
    
    [Header("Preview Mode")]
    [Tooltip("Preview smooth time")]
    [Range(0.3f, 2f)]
    public float previewSmoothTime = 0.8f;
    
    [Tooltip("Preview height")]
    [Range(5f, 50f)]
    public float previewHeight = 15f;
    
    [Header("Tilt-Shift Effect")]
    [Tooltip("Enable tilt-shift effect")]
    public bool enableTiltShift = true;
    
    [Tooltip("Tilt-shift preset")]
    public TiltShiftPreset tiltShiftPreset = TiltShiftPreset.Normal;
    
    [Tooltip("Custom aperture (if not using preset)")]
    [Range(1f, 32f)]
    public float tiltShiftAperture = 5.6f;
    
    [Tooltip("Custom focal length (if not using preset)")]
    [Range(0.1f, 20f)]
    public float tiltShiftFocalLength = 50f;
    
    /// <summary>
    /// Applies these settings to a CameraController.
    /// </summary>
    public void ApplyTo(CameraController controller)
    {
        if (controller == null) return;
        
        // Movement
        controller.movementSpeed = movementSpeed;
        controller.sprintMultiplier = sprintMultiplier;
        controller.movementSmoothing = movementSmoothing;
        
        // Rotation
        controller.rotationSpeed = rotationSpeed;
        controller.pitchSpeed = pitchSpeed;
        controller.minPitch = minPitch;
        controller.maxPitch = maxPitch;
        
        // Zoom
        controller.zoomSpeed = zoomSpeed;
        controller.minHeight = minHeight;
        controller.maxHeight = maxHeight;
        controller.heightAdjustSpeed = heightAdjustSpeed;
        
        // Edge scrolling
        controller.enableEdgeScrolling = enableEdgeScrolling;
        controller.edgeScrollBorder = edgeScrollBorder;
        controller.edgeScrollSpeed = edgeScrollSpeed;
        
        // Pan
        controller.enableMousePan = enableMousePan;
        controller.panSpeed = panSpeed;
        
        // Orbit
        controller.orbitSpeed = orbitSpeed;
        controller.defaultOrbitDistance = defaultOrbitDistance;
        
        // Collision
        controller.enableTerrainCollision = enableTerrainCollision;
        controller.collisionBuffer = collisionBuffer;
        
        // Follow
        controller.followSmoothTime = followSmoothTime;
        controller.followOffset = followOffset;
        controller.followRotationSpeed = followRotationSpeed;
        
        // Preview
        controller.previewSmoothTime = previewSmoothTime;
        controller.previewHeight = previewHeight;
        
        Debug.Log("[CameraSettings] Applied settings to CameraController");
    }
    
    /// <summary>
    /// Applies tilt-shift settings to a TiltShiftEffect.
    /// </summary>
    public void ApplyTiltShiftTo(TiltShiftEffect effect)
    {
        if (effect == null) return;
        
        effect.enableTiltShift = enableTiltShift;
        effect.SetPreset(tiltShiftPreset);
        
        if (tiltShiftAperture != effect.aperture || tiltShiftFocalLength != effect.focalLength)
        {
            effect.aperture = tiltShiftAperture;
            effect.focalLength = tiltShiftFocalLength;
            effect.ApplyTiltShiftSettings();
        }
        
        Debug.Log("[CameraSettings] Applied tilt-shift settings");
    }
    
    /// <summary>
    /// Copies current settings from a CameraController.
    /// </summary>
    public void CopyFrom(CameraController controller)
    {
        if (controller == null) return;
        
        // Movement
        movementSpeed = controller.movementSpeed;
        sprintMultiplier = controller.sprintMultiplier;
        movementSmoothing = controller.movementSmoothing;
        
        // Rotation
        rotationSpeed = controller.rotationSpeed;
        pitchSpeed = controller.pitchSpeed;
        minPitch = controller.minPitch;
        maxPitch = controller.maxPitch;
        
        // Zoom
        zoomSpeed = controller.zoomSpeed;
        minHeight = controller.minHeight;
        maxHeight = controller.maxHeight;
        heightAdjustSpeed = controller.heightAdjustSpeed;
        
        // Edge scrolling
        enableEdgeScrolling = controller.enableEdgeScrolling;
        edgeScrollBorder = controller.edgeScrollBorder;
        edgeScrollSpeed = controller.edgeScrollSpeed;
        
        // Pan
        enableMousePan = controller.enableMousePan;
        panSpeed = controller.panSpeed;
        
        // Orbit
        orbitSpeed = controller.orbitSpeed;
        defaultOrbitDistance = controller.defaultOrbitDistance;
        
        // Collision
        enableTerrainCollision = controller.enableTerrainCollision;
        collisionBuffer = controller.collisionBuffer;
        
        // Follow
        followSmoothTime = controller.followSmoothTime;
        followOffset = controller.followOffset;
        followRotationSpeed = controller.followRotationSpeed;
        
        // Preview
        previewSmoothTime = controller.previewSmoothTime;
        previewHeight = controller.previewHeight;
        
        Debug.Log("[CameraSettings] Copied settings from CameraController");
    }
    
    /// <summary>
    /// Resets to default values.
    /// </summary>
    public void ResetToDefaults()
    {
        movementSpeed = 20f;
        sprintMultiplier = 2.5f;
        movementSmoothing = 0.1f;
        
        rotationSpeed = 90f;
        pitchSpeed = 45f;
        minPitch = 10f;
        maxPitch = 80f;
        
        zoomSpeed = 10f;
        minHeight = 5f;
        maxHeight = 200f;
        heightAdjustSpeed = 15f;
        
        enableEdgeScrolling = false;
        edgeScrollBorder = 20f;
        edgeScrollSpeed = 1f;
        
        enableMousePan = true;
        panSpeed = 0.5f;
        
        orbitSpeed = 180f;
        defaultOrbitDistance = 30f;
        
        enableTerrainCollision = true;
        collisionBuffer = 2f;
        
        followSmoothTime = 0.5f;
        followOffset = new Vector3(0, 10, -10);
        followRotationSpeed = 2f;
        
        previewSmoothTime = 0.8f;
        previewHeight = 15f;
        
        enableTiltShift = true;
        tiltShiftPreset = TiltShiftPreset.Normal;
        tiltShiftAperture = 5.6f;
        tiltShiftFocalLength = 50f;
        
        Debug.Log("[CameraSettings] Reset to defaults");
    }
}
