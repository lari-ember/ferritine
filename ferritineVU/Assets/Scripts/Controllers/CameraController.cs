using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Utils;

/// <summary>
/// Modos de câmera disponíveis no sistema.
/// </summary>
public enum CameraMode
{
    Free,
    Follow,
    Orbit,
    Preview,
    FirstPerson
}

/// <summary>
/// Controlador de câmera estilo City Builder (Cities: Skylines, Urbek).
/// Combina controles clássicos com novas funcionalidades.
/// </summary>
public class CameraController : MonoBehaviour
{
    #region Selection Settings
    [Header("Selection Settings")]
    [Tooltip("Layer mask for selectable entities")]
    public LayerMask selectableLayer;
    
    [Tooltip("Prefab for selection pin indicator")]
    public GameObject selectionPinPrefab;
    #endregion

    #region Movement Settings
    [Header("Movement Settings")]
    [Tooltip("Speed of camera movement (WASD/Arrow keys)")]
    public float movementSpeed = 20f;
    
    [Tooltip("Sprint speed multiplier (Shift)")]
    public float sprintMultiplier = 2.5f;
    
    [Tooltip("Smoothing for movement")]
    [Range(0f, 0.3f)]
    public float movementSmoothing = 0.1f;
    #endregion

    #region Rotation Settings
    [Header("Rotation Settings")]
    [Tooltip("Speed of camera rotation (Q/E keys)")]
    public float rotationSpeed = 90f;
    
    [Tooltip("Speed of camera pitch (R/F keys)")]
    public float pitchSpeed = 45f;
    
    [Tooltip("Minimum pitch angle")]
    public float minPitch = 10f;
    
    [Tooltip("Maximum pitch angle")]
    public float maxPitch = 80f;
    #endregion

    #region Zoom Settings
    [Header("Zoom Settings")]
    [Tooltip("Speed of camera zoom (Mouse wheel)")]
    public float zoomSpeed = 5f;
    
    [Tooltip("Minimum camera height")]
    public float minHeight = 5f;
    
    [Tooltip("Maximum camera height")]
    public float maxHeight = 200f;
    
    [Tooltip("Height adjust speed (+/- keys)")]
    public float heightAdjustSpeed = 15f;
    
    [Tooltip("Suavização do zoom (0 = instantâneo, maior = mais suave)")]
    [Range(0.01f, 0.5f)]
    public float zoomSmoothing = 0.15f;
    
    [Tooltip("Multiplicador de sensibilidade do scroll")]
    [Range(0.1f, 2f)]
    public float zoomSensitivity = 0.5f;
    #endregion

    #region Edge Scrolling
    [Header("Edge Scrolling (Disabled by default)")]
    public bool enableEdgeScrolling = false;
    public float edgeScrollBorder = 20f;
    public float edgeScrollSpeed = 1f;
    #endregion

    #region Pan Settings
    [Header("Mouse Pan (Middle Button)")]
    public bool enableMousePan = true;
    public float panSpeed = 0.5f;
    #endregion

    #region Orbit Settings
    [Header("Orbit Mode (Shift + RMB)")]
    public float orbitSpeed = 180f;
    public float defaultOrbitDistance = 30f;
    #endregion

    #region Collision Settings
    [Header("Collision")]
    public LayerMask collisionLayer;
    public float collisionBuffer = 2f;
    public bool enableTerrainCollision = true;
    
    [Tooltip("Usa raycast para detectar altura do terreno voxel")]
    public bool useVoxelTerrainCollision = true;
    
    [Tooltip("Layer do terreno voxel para colisão")]
    public LayerMask voxelTerrainLayer;
    #endregion

    #region First Person Settings
    [Header("First Person Mode (V key)")]
    [Tooltip("Altura dos olhos em primeira pessoa")]
    public float fpsEyeHeight = 1.7f;
    
    [Tooltip("Velocidade de movimento em FPS")]
    public float fpsMovementSpeed = 5f;
    
    [Tooltip("Sensibilidade do mouse em FPS")]
    public float fpsSensitivity = 2f;
    
    [Tooltip("Limite de pitch em FPS (olhar para cima/baixo)")]
    public float fpsMaxPitch = 85f;
    
    [Tooltip("Colisão com paredes em FPS")]
    public bool fpsEnableWallCollision = true;
    
    [Tooltip("Raio do collider em FPS")]
    public float fpsCollisionRadius = 0.3f;
    #endregion

    #region Follow Settings
    [Header("Follow Mode")]
    public float followSmoothTime = 0.5f;
    public Vector3 followOffset = new Vector3(0, 10, -10);
    public float followRotationSpeed = 2f;
    #endregion

    #region Preview Settings
    [Header("Preview Mode")]
    public float previewSmoothTime = 0.8f;
    public float previewHeight = 15f;
    #endregion

    #region Events
    [Header("Events")]
    public UnityEvent<SelectableEntity> OnEntitySelected = new UnityEvent<SelectableEntity>();
    public UnityEvent<CameraMode> OnCameraModeChanged = new UnityEvent<CameraMode>();
    #endregion

    #region Input Actions
    [Header("Input Actions")]
    public InputAction clickAction;
    public InputAction moveAction;
    public InputAction zoomAction;
    public InputAction cancelAction;
    public InputAction rotateAction;
    public InputAction pitchAction;
    public InputAction heightAction;
    public InputAction sprintAction;
    public InputAction panAction;
    public InputAction orbitAction;
    public InputAction mouseDeltaAction;
    #endregion

    #region Private State
    private CameraMode currentMode = CameraMode.Free;
    private SelectableEntity currentSelectedEntity;
    private GameObject currentSelectionPin;
    private Camera mainCamera;
    
    // Movement
    private Vector3 velocity = Vector3.zero;
    private float currentYaw;
    private float currentPitch;
    
    // Zoom suave
    private float targetZoomDistance;
    private float currentZoomVelocity;
    
    // Follow mode
    private bool isFollowing = false;
    private Transform followTarget;
    private Vector3 followVelocity = Vector3.zero;
    
    // Preview mode
    private bool isPreviewing = false;
    private Vector3 previewTarget;
    private Vector3 previewVelocity = Vector3.zero;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    
    // Orbit mode
    private bool isOrbiting = false;
    private Vector3 orbitFocalPoint;
    private float orbitDistance;
    
    // Pan
    private bool isPanning = false;
    private Vector2 lastMousePosition;
    
    // First Person mode
    private bool isFirstPerson = false;
    private Vector3 fpsReturnPosition;
    private Quaternion fpsReturnRotation;
    private float fpsPitch;
    private float fpsYaw;
    
    // Bookmarks
    private Dictionary<int, CameraBookmark> bookmarks = new Dictionary<int, CameraBookmark>();
    
    // Debug
    private float lastLogTime = 0f;
    #endregion

    #region Bookmark Structure
    [System.Serializable]
    public struct CameraBookmark
    {
        public Vector3 position;
        public float yaw;
        public float pitch;
        public bool isSet;
        
        public CameraBookmark(Vector3 pos, float y, float p)
        {
            position = pos;
            yaw = y;
            pitch = p;
            isSet = true;
        }
    }
    #endregion

    #region Unity Lifecycle
    void Awake()
    {
        Debug.Log("[CameraController] ===== AWAKE INICIADO =====");
        
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("[CameraController] ERRO: Camera component não encontrada!");
        }
        
        // Set default selectable layer if not set
        if (selectableLayer.value == 0)
        {
            selectableLayer = LayerMask.GetMask("Selectable");
            if (selectableLayer.value == 0)
            {
                Debug.LogError("[CameraController] ⚠️ Layer 'Selectable' não existe!");
            }
        }
        
        // Initialize rotation from current transform
        currentYaw = transform.eulerAngles.y;
        currentPitch = transform.eulerAngles.x;
        
        SetupInputActions();
        Debug.Log("[CameraController] ===== AWAKE COMPLETO =====");
    }
    
    void SetupInputActions()
    {
        // Click action
        if (clickAction == null)
        {
            clickAction = new InputAction("Click", InputActionType.Button, "<Mouse>/leftButton");
        }
        
        // Move action (WASD + Arrows)
        if (moveAction == null)
        {
            moveAction = new InputAction("Move", InputActionType.Value);
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");
        }
        
        // Zoom action
        if (zoomAction == null)
        {
            zoomAction = new InputAction("Zoom", InputActionType.Value, "<Mouse>/scroll/y");
        }
        
        // Cancel action
        if (cancelAction == null)
        {
            cancelAction = new InputAction("Cancel", InputActionType.Button, "<Keyboard>/escape");
        }
        
        // Rotate action (Q/E)
        if (rotateAction == null)
        {
            rotateAction = new InputAction("Rotate", InputActionType.Value);
            rotateAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/q")
                .With("Positive", "<Keyboard>/e");
        }
        
        // Pitch action (R/F)
        if (pitchAction == null)
        {
            pitchAction = new InputAction("Pitch", InputActionType.Value);
            pitchAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/r")
                .With("Positive", "<Keyboard>/f");
        }
        
        // Height action (+/-/PageUp/PageDown)
        if (heightAction == null)
        {
            heightAction = new InputAction("Height", InputActionType.Value);
            heightAction.AddCompositeBinding("1DAxis")
                .With("Negative", "<Keyboard>/minus")
                .With("Negative", "<Keyboard>/pageDown")
                .With("Positive", "<Keyboard>/equals")
                .With("Positive", "<Keyboard>/pageUp");
        }
        
        // Sprint action (Shift)
        if (sprintAction == null)
        {
            sprintAction = new InputAction("Sprint", InputActionType.Button, "<Keyboard>/leftShift");
        }
        
        // Pan action (Middle mouse)
        if (panAction == null)
        {
            panAction = new InputAction("Pan", InputActionType.Button, "<Mouse>/middleButton");
        }
        
        // Orbit action (Right mouse)
        if (orbitAction == null)
        {
            orbitAction = new InputAction("Orbit", InputActionType.Button, "<Mouse>/rightButton");
        }
        
        // Mouse delta
        if (mouseDeltaAction == null)
        {
            mouseDeltaAction = new InputAction("MouseDelta", InputActionType.Value, "<Mouse>/delta");
        }
        
        Debug.Log("[CameraController] ✓ Input Actions configuradas");
    }
    
    void OnEnable()
    {
        clickAction?.Enable();
        moveAction?.Enable();
        zoomAction?.Enable();
        cancelAction?.Enable();
        rotateAction?.Enable();
        pitchAction?.Enable();
        heightAction?.Enable();
        sprintAction?.Enable();
        panAction?.Enable();
        orbitAction?.Enable();
        mouseDeltaAction?.Enable();
        
        Debug.Log("[CameraController] Input Actions habilitadas");
    }
    
    void OnDisable()
    {
        clickAction?.Disable();
        moveAction?.Disable();
        zoomAction?.Disable();
        cancelAction?.Disable();
        rotateAction?.Disable();
        pitchAction?.Disable();
        heightAction?.Disable();
        sprintAction?.Disable();
        panAction?.Disable();
        orbitAction?.Disable();
        mouseDeltaAction?.Disable();
    }
    
    void Update()
    {
        // Handle ESC - both input systems
        if ((cancelAction != null && cancelAction.WasPressedThisFrame()) || Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
        }
        
        // Check if UI is blocking selection
        bool uiBlocking = UIManager.Instance != null && UIManager.Instance.IsAnyPanelOpen();
        
        // Handle mouse click for selection (when UI not blocking) - BOTH input systems
        bool clicked = (clickAction != null && clickAction.WasPressedThisFrame()) || Input.GetMouseButtonDown(0);
        if (!uiBlocking && clicked)
        {
            HandleClick();
        }
        
        // Camera movement (always works)
        if (!isFollowing && !isPreviewing && !isFirstPerson)
        {
            HandleMovement();
            HandleRotation();
            HandleZoom();
            HandleHeight();
            HandlePan();
            HandleOrbit();
            
            if (enableEdgeScrolling)
            {
                HandleEdgeScrolling();
            }
        }
        
        // First Person mode
        if (isFirstPerson)
        {
            UpdateFirstPersonMode();
        }
        
        // Toggle First Person with V
        if (Input.GetKeyDown(KeyCode.V))
        {
            ToggleFirstPerson();
        }
        
        // Bookmarks
        HandleBookmarks();
        
        // Update special modes
        UpdateFollowMode();
        UpdatePreviewMode();
        
        // Enforce height limits
        EnforceHeightLimits();
        
        // Debug log a cada 10 segundos
        if (Time.time - lastLogTime >= 10f)
        {
            Debug.Log($"[CameraController] Pos: {transform.position:F1}, Mode: {currentMode}");
            lastLogTime = Time.time;
        }
    }
    #endregion

    #region Input Handlers
    void HandleEscape()
    {
        // Try to close UI panels first
        if (UIManager.Instance != null && UIManager.Instance.CloseTopPanel())
        {
            return;
        }
        
        // Deselect entity
        if (currentSelectedEntity != null)
        {
            DeselectEntity();
        }
        
        // Stop special modes
        StopFollowing();
        StopPreview();
        StopOrbiting();
    }
    
    void HandleClick()
    {
        if (mainCamera == null) return;
        
        // Get mouse position - try new input, fallback to old
        Vector2 mousePos;
        if (Mouse.current != null)
        {
            mousePos = Mouse.current.position.ReadValue();
        }
        else
        {
            mousePos = Input.mousePosition;
        }
        
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        
        Debug.Log($"[CameraController] Click at {mousePos}, Layer: {selectableLayer.value}");
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, selectableLayer))
        {
            SelectableEntity entity = hit.collider.GetComponent<SelectableEntity>();
            if (entity != null)
            {
                Debug.Log($"[CameraController] Hit entity: {entity.gameObject.name}");
                SelectEntity(entity);
            }
            else
            {
                Debug.Log($"[CameraController] Hit object without SelectableEntity: {hit.collider.gameObject.name}");
            }
        }
        else
        {
            Debug.Log($"[CameraController] No hit on Selectable layer");
        }
    }
    
    void HandleMovement()
    {
        // Try new input system first, fallback to old
        Vector2 input = Vector2.zero;
        
        if (moveAction != null && moveAction.enabled)
        {
            input = moveAction.ReadValue<Vector2>();
        }
        
        // Fallback: Old input system
        if (input.magnitude < 0.1f)
        {
            float h = 0f, v = 0f;
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) v = 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) v = -1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) h = -1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) h = 1f;
            input = new Vector2(h, v);
        }
        
        if (input.magnitude < 0.1f) return;
        
        // Sprint check - both systems
        bool sprinting = (sprintAction != null && sprintAction.IsPressed()) || Input.GetKey(KeyCode.LeftShift);
        float speed = movementSpeed * (sprinting ? sprintMultiplier : 1f);
        
        // Get forward/right based on camera yaw
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();
        
        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();
        
        Vector3 movement = (forward * input.y + right * input.x) * speed * Time.unscaledDeltaTime;
        
        if (movementSmoothing > 0)
        {
            transform.position = Vector3.SmoothDamp(transform.position, transform.position + movement, ref velocity, movementSmoothing, Mathf.Infinity, Time.unscaledDeltaTime);
        }
        else
        {
            transform.position += movement;
        }
    }
    
    void HandleRotation()
    {
        // Sprint check - both systems
        bool sprinting = (sprintAction != null && sprintAction.IsPressed()) || Input.GetKey(KeyCode.LeftShift);
        
        // Q/E rotation - try new input, fallback to old
        float rotateInput = 0f;
        if (rotateAction != null && rotateAction.enabled)
        {
            rotateInput = rotateAction.ReadValue<float>();
        }
        if (Mathf.Abs(rotateInput) < 0.01f)
        {
            if (Input.GetKey(KeyCode.Q)) rotateInput = -1f;
            if (Input.GetKey(KeyCode.E)) rotateInput = 1f;
        }
        
        if (Mathf.Abs(rotateInput) > 0.01f)
        {
            float speed = rotationSpeed * (sprinting ? sprintMultiplier : 1f);
            currentYaw += rotateInput * speed * Time.unscaledDeltaTime;
        }
        
        // R/F pitch - try new input, fallback to old
        float pitchInput = 0f;
        if (pitchAction != null && pitchAction.enabled)
        {
            pitchInput = pitchAction.ReadValue<float>();
        }
        if (Mathf.Abs(pitchInput) < 0.01f)
        {
            if (Input.GetKey(KeyCode.R)) pitchInput = -1f;
            if (Input.GetKey(KeyCode.F)) pitchInput = 1f;
        }
        
        if (Mathf.Abs(pitchInput) > 0.01f)
        {
            float speed = pitchSpeed * (sprinting ? sprintMultiplier : 1f);
            currentPitch += pitchInput * speed * Time.unscaledDeltaTime;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
        }
        
        // Apply rotation
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
    }
    
    void HandleZoom()
    {
        // Try new input, fallback to old
        float scroll = 0f;
        if (zoomAction != null && zoomAction.enabled)
        {
            scroll = zoomAction.ReadValue<float>();
        }
        if (Mathf.Abs(scroll) < 0.01f)
        {
            scroll = Input.mouseScrollDelta.y * 120f; // Old system returns smaller values
        }
        
        // Acumular input de zoom no target
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Aplicar sensibilidade reduzida para movimento mais sutil
            float zoomDelta = scroll * zoomSpeed * zoomSensitivity * 0.01f;
            targetZoomDistance += zoomDelta;
        }
        
        // Interpolar suavemente para o target
        if (Mathf.Abs(targetZoomDistance) > 0.001f)
        {
            // Calcular quanto mover este frame
            float smoothedZoom = Mathf.SmoothDamp(0f, targetZoomDistance, ref currentZoomVelocity, zoomSmoothing, Mathf.Infinity, Time.unscaledDeltaTime);
            
            // Aplicar movimento na direção forward
            Vector3 zoomMove = transform.forward * smoothedZoom;
            transform.position += zoomMove;
            
            // Reduzir o target pelo que já foi aplicado
            targetZoomDistance -= smoothedZoom;
            
            // Reset quando muito pequeno para evitar drift
            if (Mathf.Abs(targetZoomDistance) < 0.001f)
            {
                targetZoomDistance = 0f;
                currentZoomVelocity = 0f;
            }
        }
    }
    
    void HandleHeight()
    {
        // Try new input, fallback to old
        float heightInput = 0f;
        if (heightAction != null && heightAction.enabled)
        {
            heightInput = heightAction.ReadValue<float>();
        }
        if (Mathf.Abs(heightInput) < 0.01f)
        {
            if (Input.GetKey(KeyCode.PageUp) || Input.GetKey(KeyCode.Equals) || Input.GetKey(KeyCode.Plus)) heightInput = 1f;
            if (Input.GetKey(KeyCode.PageDown) || Input.GetKey(KeyCode.Minus)) heightInput = -1f;
        }
        
        if (Mathf.Abs(heightInput) < 0.01f) return;
        
        bool sprinting = (sprintAction != null && sprintAction.IsPressed()) || Input.GetKey(KeyCode.LeftShift);
        float speed = heightAdjustSpeed * (sprinting ? sprintMultiplier : 1f);
        Vector3 pos = transform.position;
        pos.y += heightInput * speed * Time.unscaledDeltaTime;
        transform.position = pos;
    }
    
    void HandlePan()
    {
        if (!enableMousePan) return;
        
        // Check pan button - both systems
        bool panPressed = (panAction != null && panAction.WasPressedThisFrame()) || Input.GetMouseButtonDown(2);
        bool panReleased = (panAction != null && panAction.WasReleasedThisFrame()) || Input.GetMouseButtonUp(2);
        bool panHeld = (panAction != null && panAction.IsPressed()) || Input.GetMouseButton(2);
        
        if (panPressed)
        {
            isPanning = true;
            lastMousePosition = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
        }
        else if (panReleased)
        {
            isPanning = false;
        }
        
        if (isPanning && panHeld)
        {
            Vector2 currentPos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
            Vector2 delta = currentPos - lastMousePosition;
            lastMousePosition = currentPos;
            
            Vector3 right = transform.right;
            right.y = 0;
            right.Normalize();
            
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            
            Vector3 panMove = (-right * delta.x - forward * delta.y) * panSpeed * Time.unscaledDeltaTime;
            transform.position += panMove;
        }
    }
    
    void HandleOrbit()
    {
        bool shiftHeld = (sprintAction != null && sprintAction.IsPressed()) || Input.GetKey(KeyCode.LeftShift);
        bool orbitPressed = (orbitAction != null && orbitAction.WasPressedThisFrame()) || Input.GetMouseButtonDown(1);
        bool orbitReleased = (orbitAction != null && orbitAction.WasReleasedThisFrame()) || Input.GetMouseButtonUp(1);
        
        if (shiftHeld && orbitPressed)
        {
            StartOrbiting();
        }
        
        if (orbitReleased)
        {
            StopOrbiting();
        }
        
        if (isOrbiting)
        {
            // Get mouse delta - both systems
            Vector2 delta = Vector2.zero;
            if (mouseDeltaAction != null && mouseDeltaAction.enabled)
            {
                delta = mouseDeltaAction.ReadValue<Vector2>();
            }
            if (delta.magnitude < 0.1f)
            {
                // Fallback: calculate delta manually
                Vector2 currentPos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
                delta = currentPos - lastMousePosition;
                lastMousePosition = currentPos;
            }
            
            if (delta.magnitude > 0.1f)
            {
                currentYaw += delta.x * orbitSpeed * Time.unscaledDeltaTime;
                currentPitch -= delta.y * orbitSpeed * 0.5f * Time.unscaledDeltaTime;
                currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);
                
                // Calculate orbit position
                float yawRad = currentYaw * Mathf.Deg2Rad;
                float pitchRad = currentPitch * Mathf.Deg2Rad;
                
                Vector3 offset = new Vector3(
                    Mathf.Sin(yawRad) * Mathf.Cos(pitchRad),
                    Mathf.Sin(pitchRad),
                    Mathf.Cos(yawRad) * Mathf.Cos(pitchRad)
                ) * orbitDistance;
                
                transform.position = orbitFocalPoint + offset;
                transform.LookAt(orbitFocalPoint);
            }
        }
    }
    
    void HandleEdgeScrolling()
    {
        Vector2 mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;
        Vector2 move = Vector2.zero;
        
        if (mousePos.x < edgeScrollBorder) move.x = -1f;
        else if (mousePos.x > Screen.width - edgeScrollBorder) move.x = 1f;
        
        if (mousePos.y < edgeScrollBorder) move.y = -1f;
        else if (mousePos.y > Screen.height - edgeScrollBorder) move.y = 1f;
        
        if (move.magnitude > 0.1f)
        {
            float speed = movementSpeed * edgeScrollSpeed;
            
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            
            Vector3 right = transform.right;
            right.y = 0;
            right.Normalize();
            
            transform.position += (forward * move.y + right * move.x) * speed * Time.unscaledDeltaTime;
        }
    }
    
    void HandleBookmarks()
    {
        for (int i = 1; i <= 9; i++)
        {
            KeyCode key = (KeyCode)((int)KeyCode.Alpha1 + i - 1);
            
            if (Input.GetKeyDown(key))
            {
                bool ctrl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
                
                if (ctrl)
                {
                    SaveBookmark(i);
                }
                else
                {
                    RestoreBookmark(i);
                }
            }
        }
    }
    
    void EnforceHeightLimits()
    {
        // Em modo FPS, a altura é controlada pelo modo FPS
        if (isFirstPerson) return;
        
        Vector3 pos = transform.position;
        
        // Get terrain height
        float terrainHeight = 0f;
        
        // Método 1: Unity Terrain (se existir)
        if (enableTerrainCollision && Terrain.activeTerrain != null)
        {
            terrainHeight = Terrain.activeTerrain.SampleHeight(pos) + Terrain.activeTerrain.transform.position.y;
        }
        
        // Método 2: Voxel Terrain via Raycast (prioridade)
        if (useVoxelTerrainCollision)
        {
            float voxelHeight = GetVoxelTerrainHeight(pos);
            if (voxelHeight > terrainHeight)
            {
                terrainHeight = voxelHeight;
            }
        }
        
        float minAllowed = Mathf.Max(minHeight, terrainHeight + collisionBuffer);
        
        if (pos.y < minAllowed)
        {
            pos.y = minAllowed;
            transform.position = pos;
        }
        else if (pos.y > maxHeight)
        {
            pos.y = maxHeight;
            transform.position = pos;
        }
    }
    
    /// <summary>
    /// Obtém a altura do terreno de voxels usando raycast para baixo.
    /// </summary>
    float GetVoxelTerrainHeight(Vector3 position)
    {
        // Raycast de cima para baixo para encontrar o terreno
        Vector3 rayOrigin = new Vector3(position.x, position.y + 100f, position.z);
        
        // Usa voxelTerrainLayer se configurada, senão usa collisionLayer
        LayerMask layerToUse = voxelTerrainLayer.value != 0 ? voxelTerrainLayer : collisionLayer;
        
        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 500f, layerToUse))
        {
            return hit.point.y;
        }
        
        return 0f;
    }
    #endregion

    #region Orbit Mode
    void StartOrbiting()
    {
        if (currentSelectedEntity != null)
        {
            orbitFocalPoint = currentSelectedEntity.transform.position;
        }
        else if (Mouse.current != null && mainCamera != null)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, 500f))
            {
                orbitFocalPoint = hit.point;
            }
            else
            {
                orbitFocalPoint = transform.position + transform.forward * defaultOrbitDistance;
            }
        }
        else
        {
            orbitFocalPoint = transform.position + transform.forward * defaultOrbitDistance;
        }
        
        orbitDistance = Vector3.Distance(transform.position, orbitFocalPoint);
        isOrbiting = true;
        currentMode = CameraMode.Orbit;
        OnCameraModeChanged?.Invoke(currentMode);
        
        Debug.Log($"[CameraController] Orbit started at {orbitFocalPoint}");
    }
    
    void StopOrbiting()
    {
        if (isOrbiting)
        {
            isOrbiting = false;
            currentMode = CameraMode.Free;
            OnCameraModeChanged?.Invoke(currentMode);
        }
    }
    #endregion

    #region Bookmarks
    public void SaveBookmark(int slot)
    {
        bookmarks[slot] = new CameraBookmark(transform.position, currentYaw, currentPitch);
        AudioManager.Instance?.Play(AudioManager.Instance.buttonClick);
        Debug.Log($"[CameraController] Bookmark {slot} saved");
    }
    
    public void RestoreBookmark(int slot)
    {
        if (bookmarks.TryGetValue(slot, out CameraBookmark bm) && bm.isSet)
        {
            StartCoroutine(TransitionToBookmark(bm));
            AudioManager.Instance?.Play(AudioManager.Instance.buttonClick);
        }
    }
    
    System.Collections.IEnumerator TransitionToBookmark(CameraBookmark bm)
    {
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion endRot = Quaternion.Euler(bm.pitch, bm.yaw, 0);
        
        float duration = 0.5f;
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            
            transform.position = Vector3.Lerp(startPos, bm.position, t);
            transform.rotation = Quaternion.Slerp(startRot, endRot, t);
            
            yield return null;
        }
        
        transform.position = bm.position;
        transform.rotation = endRot;
        currentYaw = bm.yaw;
        currentPitch = bm.pitch;
    }
    #endregion

    #region Entity Selection
    public void SelectEntity(SelectableEntity entity)
    {
        if (currentSelectedEntity != null)
        {
            currentSelectedEntity.Unhighlight();
            RemoveSelectionPin();
        }
        
        currentSelectedEntity = entity;
        currentSelectedEntity.Highlight();
        SpawnSelectionPin(currentSelectedEntity.transform);
        
        AudioManager.Instance?.Play(AudioManager.Instance.entitySelect);
        OnEntitySelected?.Invoke(entity);
        
        Debug.Log($"[CameraController] Selected: {entity.GetDisplayName()}");
    }
    
    public void DeselectEntity()
    {
        if (currentSelectedEntity != null)
        {
            currentSelectedEntity.Unhighlight();
            RemoveSelectionPin();
            currentSelectedEntity = null;
            Debug.Log("[CameraController] Deselected");
        }
    }
    
    public SelectableEntity GetSelectedEntity() => currentSelectedEntity;
    
    void SpawnSelectionPin(Transform target)
    {
        string entityId = currentSelectedEntity?.GetEntityId() ?? System.Guid.NewGuid().ToString();
        
        if (SelectionPinPool.Instance != null)
        {
            currentSelectionPin = SelectionPinPool.Instance.GetPin(target.position + Vector3.up * 2f, entityId);
        }
        
        if (currentSelectionPin == null && selectionPinPrefab != null)
        {
            currentSelectionPin = Instantiate(selectionPinPrefab);
            currentSelectionPin.transform.position = target.position + Vector3.up * 2f;
        }
        
        if (currentSelectionPin != null)
        {
            currentSelectionPin.transform.SetParent(target);
        }
    }
    
    void RemoveSelectionPin()
    {
        if (currentSelectionPin != null)
        {
            if (SelectionPinPool.Instance != null && currentSelectedEntity != null)
            {
                SelectionPinPool.Instance.ReturnPin(currentSelectedEntity.GetEntityId());
            }
            else
            {
                Destroy(currentSelectionPin);
            }
            currentSelectionPin = null;
        }
    }
    #endregion

    #region Follow Mode
    public void StartFollowing(Transform target)
    {
        followTarget = target;
        isFollowing = true;
        isPreviewing = false;
        StopOrbiting();
        currentMode = CameraMode.Follow;
        OnCameraModeChanged?.Invoke(currentMode);
        Debug.Log($"[CameraController] Following {target.name}");
    }
    
    public void FollowSelectedEntity()
    {
        if (currentSelectedEntity != null)
        {
            StartFollowing(currentSelectedEntity.transform);
        }
    }
    
    public void StopFollowing()
    {
        if (isFollowing)
        {
            isFollowing = false;
            followTarget = null;
            currentMode = CameraMode.Free;
            OnCameraModeChanged?.Invoke(currentMode);
        }
    }
    
    void UpdateFollowMode()
    {
        if (!isFollowing || followTarget == null) return;
        
        Vector3 targetPos = followTarget.position + followOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref followVelocity, followSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        
        Vector3 lookDir = followTarget.position - transform.position;
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.unscaledDeltaTime * followRotationSpeed);
        }
    }
    #endregion

    #region Preview Mode
    public void PreviewLocation(Vector3 worldPosition)
    {
        if (!isPreviewing)
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }
        
        isPreviewing = true;
        isFollowing = false;
        StopOrbiting();
        previewTarget = worldPosition + new Vector3(0, previewHeight, -previewHeight);
        currentMode = CameraMode.Preview;
        OnCameraModeChanged?.Invoke(currentMode);
    }
    
    public void StopPreview()
    {
        if (isPreviewing)
        {
            isPreviewing = false;
            StartCoroutine(ReturnFromPreview());
            currentMode = CameraMode.Free;
            OnCameraModeChanged?.Invoke(currentMode);
        }
    }
    
    void UpdatePreviewMode()
    {
        if (!isPreviewing) return;
        
        transform.position = Vector3.SmoothDamp(transform.position, previewTarget, ref previewVelocity, previewSmoothTime, Mathf.Infinity, Time.unscaledDeltaTime);
        
        Vector3 lookTarget = previewTarget - new Vector3(0, previewHeight, -previewHeight);
        Vector3 lookDir = lookTarget - transform.position;
        if (lookDir != Vector3.zero)
        {
            Quaternion targetRot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.unscaledDeltaTime * 2f);
        }
    }
    
    System.Collections.IEnumerator ReturnFromPreview()
    {
        float duration = 1f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, originalRotation, t);
            yield return null;
        }
        
        transform.position = originalPosition;
        transform.rotation = originalRotation;
        currentYaw = originalRotation.eulerAngles.y;
        currentPitch = originalRotation.eulerAngles.x;
    }
    #endregion

    #region Public API
    public CameraMode GetCurrentMode() => currentMode;
    
    public void TeleportTo(Vector3 position, float? yaw = null, float? pitch = null)
    {
        transform.position = position;
        if (yaw.HasValue) currentYaw = yaw.Value;
        if (pitch.HasValue) currentPitch = pitch.Value;
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
    }
    
    public void SetEdgeScrollingEnabled(bool value)
    {
        enableEdgeScrolling = value;
    }
    #endregion

    #region First Person Mode
    /// <summary>
    /// Alterna entre modo normal e primeira pessoa.
    /// </summary>
    public void ToggleFirstPerson()
    {
        if (isFirstPerson)
        {
            ExitFirstPerson();
        }
        else
        {
            EnterFirstPerson();
        }
    }
    
    /// <summary>
    /// Entra no modo primeira pessoa na posição atual.
    /// </summary>
    public void EnterFirstPerson()
    {
        if (isFirstPerson) return;
        
        // Salvar posição para retornar
        fpsReturnPosition = transform.position;
        fpsReturnRotation = transform.rotation;
        
        // Parar outros modos
        StopFollowing();
        StopPreview();
        StopOrbiting();
        
        // Calcular posição no chão
        Vector3 groundPos = transform.position;
        float terrainHeight = GetVoxelTerrainHeight(groundPos);
        groundPos.y = terrainHeight + fpsEyeHeight;
        
        transform.position = groundPos;
        
        // Configurar rotação
        fpsYaw = transform.eulerAngles.y;
        fpsPitch = 0f; // Olhando para frente
        transform.rotation = Quaternion.Euler(fpsPitch, fpsYaw, 0f);
        
        // Travar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        isFirstPerson = true;
        currentMode = CameraMode.FirstPerson;
        OnCameraModeChanged?.Invoke(currentMode);
        
        Debug.Log("[CameraController] Entered First Person mode (V to exit)");
    }
    
    /// <summary>
    /// Entra no modo primeira pessoa em uma posição específica.
    /// </summary>
    public void EnterFirstPersonAt(Vector3 worldPosition)
    {
        fpsReturnPosition = transform.position;
        fpsReturnRotation = transform.rotation;
        
        StopFollowing();
        StopPreview();
        StopOrbiting();
        
        float terrainHeight = GetVoxelTerrainHeight(worldPosition);
        Vector3 fpsPosition = new Vector3(worldPosition.x, terrainHeight + fpsEyeHeight, worldPosition.z);
        
        transform.position = fpsPosition;
        fpsYaw = transform.eulerAngles.y;
        fpsPitch = 0f;
        transform.rotation = Quaternion.Euler(fpsPitch, fpsYaw, 0f);
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        isFirstPerson = true;
        currentMode = CameraMode.FirstPerson;
        OnCameraModeChanged?.Invoke(currentMode);
        
        Debug.Log($"[CameraController] Entered First Person at {fpsPosition:F1}");
    }
    
    /// <summary>
    /// Sai do modo primeira pessoa e retorna à posição anterior.
    /// </summary>
    public void ExitFirstPerson()
    {
        if (!isFirstPerson) return;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        isFirstPerson = false;
        currentMode = CameraMode.Free;
        OnCameraModeChanged?.Invoke(currentMode);
        
        // Retornar suavemente
        StartCoroutine(TransitionFromFirstPerson());
        
        Debug.Log("[CameraController] Exited First Person mode");
    }
    
    System.Collections.IEnumerator TransitionFromFirstPerson()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            
            transform.position = Vector3.Lerp(startPos, fpsReturnPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, fpsReturnRotation, t);
            
            yield return null;
        }
        
        transform.position = fpsReturnPosition;
        transform.rotation = fpsReturnRotation;
        currentYaw = fpsReturnRotation.eulerAngles.y;
        currentPitch = fpsReturnRotation.eulerAngles.x;
    }
    
    /// <summary>
    /// Atualiza o modo primeira pessoa (chamado no Update).
    /// </summary>
    void UpdateFirstPersonMode()
    {
        // ESC para sair
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitFirstPerson();
            return;
        }
        
        // Rotação com mouse
        float mouseX = Input.GetAxis("Mouse X") * fpsSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * fpsSensitivity;
        
        fpsYaw += mouseX;
        fpsPitch -= mouseY;
        fpsPitch = Mathf.Clamp(fpsPitch, -fpsMaxPitch, fpsMaxPitch);
        
        transform.rotation = Quaternion.Euler(fpsPitch, fpsYaw, 0f);
        
        // Movimento WASD
        float horizontal = 0f, vertical = 0f;
        if (Input.GetKey(KeyCode.W)) vertical = 1f;
        if (Input.GetKey(KeyCode.S)) vertical = -1f;
        if (Input.GetKey(KeyCode.A)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D)) horizontal = 1f;
        
        if (Mathf.Abs(horizontal) > 0.01f || Mathf.Abs(vertical) > 0.01f)
        {
            // Sprint
            bool sprinting = Input.GetKey(KeyCode.LeftShift);
            float speed = fpsMovementSpeed * (sprinting ? sprintMultiplier : 1f);
            
            // Direção baseada na rotação (apenas yaw, não pitch)
            Vector3 forward = Quaternion.Euler(0, fpsYaw, 0) * Vector3.forward;
            Vector3 right = Quaternion.Euler(0, fpsYaw, 0) * Vector3.right;
            
            Vector3 moveDir = (forward * vertical + right * horizontal).normalized;
            Vector3 targetPos = transform.position + moveDir * speed * Time.deltaTime;
            
            // Colisão com paredes
            if (fpsEnableWallCollision)
            {
                targetPos = CheckFPSWallCollision(transform.position, targetPos);
            }
            
            // Manter altura do terreno
            float terrainHeight = GetVoxelTerrainHeight(targetPos);
            targetPos.y = terrainHeight + fpsEyeHeight;
            
            transform.position = targetPos;
        }
        else
        {
            // Mesmo parado, ajustar altura ao terreno
            Vector3 pos = transform.position;
            float terrainHeight = GetVoxelTerrainHeight(pos);
            pos.y = Mathf.Lerp(pos.y, terrainHeight + fpsEyeHeight, Time.deltaTime * 10f);
            transform.position = pos;
        }
    }
    
    /// <summary>
    /// Verifica colisão com paredes em modo FPS.
    /// </summary>
    Vector3 CheckFPSWallCollision(Vector3 from, Vector3 to)
    {
        Vector3 direction = to - from;
        float distance = direction.magnitude;
        
        if (distance < 0.001f) return to;
        
        // SphereCast na direção do movimento
        if (Physics.SphereCast(from, fpsCollisionRadius, direction.normalized, out RaycastHit hit, distance, collisionLayer))
        {
            // Parar antes da parede
            float safeDistance = hit.distance - 0.1f;
            if (safeDistance < 0) safeDistance = 0;
            
            return from + direction.normalized * safeDistance;
        }
        
        return to;
    }
    
    /// <summary>
    /// Retorna se está em modo primeira pessoa.
    /// </summary>
    public bool IsFirstPerson => isFirstPerson;
    #endregion

    #region Cleanup
    void OnDestroy()
    {
        OnEntitySelected.RemoveAllListeners();
        OnCameraModeChanged.RemoveAllListeners();
    }
    #endregion
}
