using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Selection Settings")]
    [Tooltip("Layer mask for selectable entities")]
    public LayerMask selectableLayer;
    
    [Tooltip("Prefab for selection pin indicator")]
    public GameObject selectionPinPrefab;
    
    [Header("Camera Movement Settings")]
    [Tooltip("Speed of camera movement (WASD/Arrow keys)")]
    public float movementSpeed = 10f;
    
    [Tooltip("Speed of camera zoom (Mouse wheel)")]
    public float zoomSpeed = 5f;
    
    [Header("Follow Settings")]
    public float followSmoothTime = 0.5f;
    public Vector3 followOffset = new Vector3(0, 10, -10);
    public float followRotationSpeed = 2f;
    
    [Header("Preview Settings")]
    public float previewSmoothTime = 0.8f;
    public float previewHeight = 15f;
    
    [Header("Events")]
    public UnityEvent<SelectableEntity> OnEntitySelected = new UnityEvent<SelectableEntity>();
    
    [Header("Input Actions")]
    public InputAction clickAction;
    public InputAction moveAction;
    public InputAction zoomAction;
    public InputAction cancelAction;
    
    // Current state
    private SelectableEntity currentSelectedEntity;
    private GameObject currentSelectionPin;
    private Camera mainCamera;
    
    // Follow mode state
    private bool isFollowing = false;
    private Transform followTarget;
    private Vector3 followVelocity = Vector3.zero;
    
    // Preview mode state
    private bool isPreviewing = false;
    private Vector3 previewTarget;
    private Vector3 previewVelocity = Vector3.zero;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    
    // Debug
    private int frameCounter = 0;
    private float lastLogTime = 0f;
    
    void Awake()
    {
        Debug.Log("[CameraController] ===== AWAKE INICIADO =====");
        
        mainCamera = GetComponent<Camera>();
        if (mainCamera == null)
        {
            Debug.LogError("[CameraController] ERRO: Camera component não encontrada!");
        }
        else
        {
            Debug.Log("[CameraController] ✓ Camera encontrada");
        }
        
        // Set default selectable layer if not set
        if (selectableLayer.value == 0)
        {
            selectableLayer = LayerMask.GetMask("Selectable");
            
            // Validate if layer exists
            if (selectableLayer.value == 0)
            {
                Debug.LogError("[CameraController] ⚠️ CRÍTICO: Layer 'Selectable' não existe! " +
                              "Crie a layer em Edit → Project Settings → Tags and Layers. " +
                              "Seleção de entidades NÃO funcionará até que a layer seja criada!");
            }
            else
            {
                Debug.Log($"[CameraController] ✓ Layer 'Selectable' configurada: mask = {selectableLayer.value}");
            }
        }
        else
        {
            Debug.Log($"[CameraController] ✓ Layer 'Selectable' já estava configurada: mask = {selectableLayer.value}");
        }
        
        // Setup Input Actions if not assigned in inspector
        Debug.Log("[CameraController] Configurando Input Actions...");
        
        if (clickAction == null)
        {
            clickAction = new InputAction("Click", InputActionType.Button, "<Mouse>/leftButton");
            Debug.Log("[CameraController] ✓ clickAction criada via código");
        }
        else
        {
            Debug.Log("[CameraController] ✓ clickAction já estava atribuída no Inspector");
        }
        
        if (moveAction == null)
        {
            moveAction = new InputAction("Move", InputActionType.Value, "<Gamepad>/leftStick");
            moveAction.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w")
                .With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/s")
                .With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/a")
                .With("Left", "<Keyboard>/leftArrow")
                .With("Right", "<Keyboard>/d")
                .With("Right", "<Keyboard>/rightArrow");
            Debug.Log("[CameraController] ✓ moveAction criada via código");
        }
        
        if (zoomAction == null)
        {
            zoomAction = new InputAction("Zoom", InputActionType.Value, "<Mouse>/scroll/y");
            Debug.Log("[CameraController] ✓ zoomAction criada via código");
        }
        
        if (cancelAction == null)
        {
            cancelAction = new InputAction("Cancel", InputActionType.Button, "<Keyboard>/escape");
            Debug.Log("[CameraController] ✓ cancelAction criada via código");
        }
        
        Debug.Log("[CameraController] ===== AWAKE COMPLETO =====");
    }
    
    void OnEnable()
    {
        Debug.Log("[CameraController] ===== OnEnable CHAMADO =====");
        
        clickAction?.Enable();
        moveAction?.Enable();
        zoomAction?.Enable();
        cancelAction?.Enable();
        
        Debug.Log($"[CameraController] Input Actions habilitadas - clickAction enabled: {clickAction?.enabled}");
    }
    
    void OnDisable()
    {
        clickAction?.Disable();
        moveAction?.Disable();
        zoomAction?.Disable();
        cancelAction?.Disable();
    }
    
    void Update()
    {
        frameCounter++;
        
        // Log de debug a cada segundo para verificar se Update está rodando
        if (Time.time - lastLogTime >= 1f)
        {
            bool mouseCurrentAvailable = Mouse.current != null;
            Debug.Log($"[CameraController] Update rodando - Frame: {frameCounter}, clickAction != null: {clickAction != null}, clickAction.enabled: {clickAction?.enabled}, Mouse.current disponível: {mouseCurrentAvailable}");
            lastLogTime = Time.time;
        }
        
        // FALLBACK: Teste com Input System antigo também
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("[CameraController] ===== CLIQUE DETECTADO VIA Input.GetMouseButtonDown(0) (OLD SYSTEM) =====");
            
            if (mainCamera == null)
            {
                Debug.LogError("[CameraController] ERRO: mainCamera é NULL!");
            }
            else
            {
                Vector3 mousePos = Input.mousePosition;
                Debug.Log($"[CameraController] Posição do mouse (old): {mousePos}");
                
                Ray ray = mainCamera.ScreenPointToRay(mousePos);
                Debug.DrawRay(ray.origin, ray.direction * 100f, Color.yellow, 2f);
                Debug.Log($"[CameraController] Ray criado - Origin: {ray.origin}, Direction: {ray.direction}");

                RaycastHit hit;
                bool hitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer);
                Debug.Log($"[CameraController] Raycast executado - Hit algo? {hitSomething}");
                
                if (hitSomething)
                {
                    Debug.Log($"[CameraController] ✓ HIT DETECTADO em: {hit.collider.gameObject.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)})");
                    
                    // ✅ CONECTAR AO SISTEMA DE SELEÇÃO
                    SelectableEntity entity = hit.collider.GetComponent<SelectableEntity>();
                    if (entity != null)
                    {
                        Debug.Log($"[CameraController] SelectableEntity encontrado! Chamando SelectEntity()...");
                        SelectEntity(entity);
                    }
                    else
                    {
                        Debug.LogWarning($"[CameraController] ⚠️ GameObject '{hit.collider.gameObject.name}' não tem componente SelectableEntity!");
                    }
                }
                else
                {
                    Debug.Log($"[CameraController] ✗ Nenhum objeto Selectable foi atingido pelo Raycast (LayerMask: {selectableLayer.value})");
                }
            }
        }
        
        // Código de debug usando o novo Input System
        if (clickAction != null && clickAction.WasPressedThisFrame())
        {
            Debug.Log("[CameraController] ===== CLIQUE DETECTADO VIA clickAction.WasPressedThisFrame() (NEW SYSTEM) =====");
            
            if (Mouse.current == null)
            {
                Debug.LogError("[CameraController] ERRO: Mouse.current é NULL!");
                return;
            }
            
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Debug.Log($"[CameraController] Posição do mouse (new): {mousePos}");
            
            if (mainCamera == null)
            {
                Debug.LogError("[CameraController] ERRO: mainCamera é NULL!");
                return;
            }
            
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
            Debug.Log($"[CameraController] Ray criado - Origin: {ray.origin}, Direction: {ray.direction}");

            RaycastHit hit;
            bool hitSomething = Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer);
            Debug.Log($"[CameraController] Raycast executado - Hit algo? {hitSomething}");
            
            if (hitSomething)
            {
                Debug.Log($"[CameraController] ✓ HIT DETECTADO em: {hit.collider.gameObject.name} (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)})");
                
                // ✅ CONECTAR AO SISTEMA DE SELEÇÃO
                SelectableEntity entity = hit.collider.GetComponent<SelectableEntity>();
                if (entity != null)
                {
                    Debug.Log($"[CameraController] SelectableEntity encontrado! Chamando SelectEntity()...");
                    SelectEntity(entity);
                }
                else
                {
                    Debug.LogWarning($"[CameraController] ⚠️ GameObject '{hit.collider.gameObject.name}' não tem componente SelectableEntity!");
                }
            }
            else
            {
                Debug.Log($"[CameraController] ✗ Nenhum objeto Selectable foi atingido pelo Raycast (LayerMask: {selectableLayer.value})");
            }
        }

        HandleMouseInput();
        HandleKeyboardInput();
        HandleCameraMovement();
        HandleCameraZoom();
        UpdateFollowMode();
        UpdatePreviewMode();
    }
    
    /// <summary>
    /// Handles mouse click for entity selection.
    /// </summary>
    void HandleMouseInput()
    {
        if (clickAction.WasPressedThisFrame())
        {
            PerformRaycast();
        }
    }
    
    /// <summary>
    /// Handles keyboard shortcuts.
    /// </summary>
    void HandleKeyboardInput()
    {
        // ESC to deselect
        if (cancelAction.WasPressedThisFrame())
        {
            DeselectEntity();
            StopFollowing();
            StopPreview();
        }
    }
    
    /// <summary>
    /// Handles WASD/Arrow key camera movement.
    /// Disabled when following or previewing.
    /// </summary>
    void HandleCameraMovement()
    {
        // Don't move camera manually when following or previewing
        if (isFollowing || isPreviewing) return;
        
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        
        if (moveInput.magnitude > 0.1f)
        {
            // Move relative to camera's forward direction (ignoring Y axis)
            Vector3 forward = transform.forward;
            forward.y = 0;
            forward.Normalize();
            
            Vector3 right = transform.right;
            
            Vector3 movement = (forward * moveInput.y + right * moveInput.x) * movementSpeed * Time.deltaTime;
            transform.position += movement;
        }
    }
    
    /// <summary>
    /// Handles mouse wheel zoom.
    /// Disabled when following or previewing.
    /// </summary>
    void HandleCameraZoom()
    {
        // Don't zoom manually when following or previewing
        if (isFollowing || isPreviewing) return;
        
        float scroll = zoomAction.ReadValue<float>();
        
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Zoom in/out along camera's forward direction
            Vector3 zoomDirection = transform.forward * scroll * zoomSpeed;
            transform.position += zoomDirection;
        }
    }
    
    /// <summary>
    /// Performs raycast from mouse position to detect selectable entities.
    /// </summary>
    void PerformRaycast()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, selectableLayer))
        {
            SelectableEntity entity = hit.collider.GetComponent<SelectableEntity>();
            if (entity != null)
            {
                SelectEntity(entity);
            }
        }
    }
    
    /// <summary>
    /// Selects an entity and triggers highlight + pin spawn.
    /// </summary>
    public void SelectEntity(SelectableEntity entity)
    {
        Debug.Log($"[CameraController] ===== SelectEntity CHAMADO ===== Entity: {entity.gameObject.name}");
        
        // Deselect previous entity
        if (currentSelectedEntity != null)
        {
            Debug.Log($"[CameraController] Deselecionando entidade anterior: {currentSelectedEntity.gameObject.name}");
            currentSelectedEntity.Unhighlight();
            RemoveSelectionPin();
        }
        
        // Select new entity
        currentSelectedEntity = entity;
        Debug.Log($"[CameraController] Chamando Highlight() na entidade...");
        currentSelectedEntity.Highlight();
        
        Debug.Log($"[CameraController] Chamando SpawnSelectionPin()...");
        SpawnSelectionPin(currentSelectedEntity.transform);
        
        // Play selection sound
        Debug.Log($"[CameraController] Tentando tocar som de seleção...");
        AudioManager.PlayUISound("entity_select");
        
        // Notify listeners (UI)
        Debug.Log($"[CameraController] Invocando OnEntitySelected event (listeners: {OnEntitySelected.GetPersistentEventCount()})...");
        OnEntitySelected?.Invoke(entity);
        
        Debug.Log($"[CameraController] ✅ Selected {entity.entityType}: {entity.GetDisplayName()}");
    }
    
    /// <summary>
    /// Deselects current entity.
    /// </summary>
    public void DeselectEntity()
    {
        if (currentSelectedEntity != null)
        {
            currentSelectedEntity.Unhighlight();
            RemoveSelectionPin();
            currentSelectedEntity = null;
            
            Debug.Log("[CameraController] Entity deselected");
        }
    }
    
    /// <summary>
    /// Spawns a selection pin indicator above the entity.
    /// </summary>
    void SpawnSelectionPin(Transform target)
    {
        Debug.Log($"[CameraController] SpawnSelectionPin iniciado para: {target.name}");
        
        // Get entity ID for pool tracking
        string entityId = currentSelectedEntity != null ? currentSelectedEntity.GetEntityId() : System.Guid.NewGuid().ToString();
        Debug.Log($"[CameraController] Entity ID: {entityId}");
        
        // Try to get from pool first
        if (SelectionPinPool.Instance != null)
        {
            Debug.Log($"[CameraController] SelectionPinPool.Instance encontrado, tentando obter pin do pool...");
            Vector3 pinPosition = target.position + Vector3.up * 2f;
            currentSelectionPin = SelectionPinPool.Instance.GetPin(pinPosition, entityId);
            if (currentSelectionPin != null)
            {
                Debug.Log($"[CameraController] ✅ Pin obtido do pool: {currentSelectionPin.name}");
            }
            else
            {
                Debug.LogWarning($"[CameraController] ⚠️ Pool retornou null");
            }
        }
        else
        {
            Debug.LogWarning($"[CameraController] ⚠️ SelectionPinPool.Instance é NULL");
        }
        
        // If pool is not available, instantiate from prefab
        if (currentSelectionPin == null && selectionPinPrefab != null)
        {
            Debug.Log($"[CameraController] Criando pin a partir do prefab: {selectionPinPrefab.name}");
            currentSelectionPin = Instantiate(selectionPinPrefab);
            currentSelectionPin.transform.position = target.position + Vector3.up * 2f;
            Debug.Log($"[CameraController] ✅ Pin criado do prefab na posição: {currentSelectionPin.transform.position}");
        }
        else if (currentSelectionPin == null && selectionPinPrefab == null)
        {
            Debug.LogError($"[CameraController] ❌ selectionPinPrefab é NULL! Atribua o prefab no Inspector!");
        }
        
        if (currentSelectionPin != null)
        {
            currentSelectionPin.transform.SetParent(target);
            Debug.Log($"[CameraController] ✅ Pin anexado como filho de: {target.name}");
        }
        else
        {
            Debug.LogError("[CameraController] ❌ currentSelectionPin é NULL após todas as tentativas!");
        }
    }
    
    /// <summary>
    /// Removes and returns selection pin to pool or destroys it.
    /// </summary>
    void RemoveSelectionPin()
    {
        if (currentSelectionPin != null)
        {
            // Try to return to pool first
            if (SelectionPinPool.Instance != null && currentSelectedEntity != null)
            {
                string entityId = currentSelectedEntity.GetEntityId();
                SelectionPinPool.Instance.ReturnPin(entityId);
            }
            else
            {
                // If pool doesn't exist or no entity, destroy the pin
                Destroy(currentSelectionPin);
            }
            
            currentSelectionPin = null;
        }
    }
    
    /// <summary>
    /// Starts following a target entity.
    /// </summary>
    public void StartFollowing(Transform target)
    {
        followTarget = target;
        isFollowing = true;
        isPreviewing = false;
        
        Debug.Log($"[CameraController] Started following {target.name}");
    }
    
    /// <summary>
    /// Starts following the currently selected entity.
    /// </summary>
    public void FollowSelectedEntity()
    {
        if (currentSelectedEntity != null)
        {
            StartFollowing(currentSelectedEntity.transform);
        }
    }
    
    /// <summary>
    /// Stops following current target.
    /// </summary>
    public void StopFollowing()
    {
        if (isFollowing)
        {
            isFollowing = false;
            followTarget = null;
            Debug.Log("[CameraController] Stopped following");
        }
    }
    
    /// <summary>
    /// Updates camera position when in follow mode.
    /// </summary>
    void UpdateFollowMode()
    {
        if (!isFollowing || followTarget == null) return;
        
        Vector3 targetPosition = followTarget.position + followOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref followVelocity, followSmoothTime);
        
        // Look at target
        Vector3 lookDirection = followTarget.position - transform.position;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * followRotationSpeed);
        }
    }
    
    /// <summary>
    /// Previews a location (used for teleport destination preview).
    /// </summary>
    public void PreviewLocation(Vector3 worldPosition)
    {
        if (!isPreviewing)
        {
            originalPosition = transform.position;
            originalRotation = transform.rotation;
        }
        
        isPreviewing = true;
        isFollowing = false;
        previewTarget = worldPosition + new Vector3(0, previewHeight, -previewHeight);
        
        Debug.Log($"[CameraController] Previewing location: {worldPosition}");
    }
    
    /// <summary>
    /// Stops preview mode and returns to original position.
    /// </summary>
    public void StopPreview()
    {
        if (isPreviewing)
        {
            isPreviewing = false;
            
            // Smoothly return to original position
            StartCoroutine(ReturnToOriginalPosition());
            
            Debug.Log("[CameraController] Stopped preview");
        }
    }
    
    /// <summary>
    /// Updates camera position when in preview mode.
    /// </summary>
    void UpdatePreviewMode()
    {
        if (!isPreviewing) return;
        
        transform.position = Vector3.SmoothDamp(transform.position, previewTarget, ref previewVelocity, previewSmoothTime);
        
        // Look at preview location
        Vector3 lookTarget = previewTarget - new Vector3(0, previewHeight, -previewHeight);
        Vector3 lookDirection = lookTarget - transform.position;
        if (lookDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 2f);
        }
    }
    
    /// <summary>
    /// Coroutine to smoothly return camera to original position.
    /// </summary>
    System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        float elapsed = 0f;
        float duration = 1f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            transform.position = Vector3.Lerp(startPos, originalPosition, t);
            transform.rotation = Quaternion.Slerp(startRot, originalRotation, t);
            
            yield return null;
        }
        
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }
    
    void OnDestroy()
    {
        OnEntitySelected.RemoveAllListeners();
    }
}