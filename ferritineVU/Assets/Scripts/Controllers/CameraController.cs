using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controls camera interactions including entity selection via raycasting,
/// follow mode, and preview mode for teleport destinations.
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Selection Settings")]
    [Tooltip("Layer mask for selectable entities")]
    public LayerMask selectableLayer;
    
    [Header("Follow Settings")]
    public float followSmoothTime = 0.5f;
    public Vector3 followOffset = new Vector3(0, 10, -10);
    public float followRotationSpeed = 2f;
    
    [Header("Preview Settings")]
    public float previewSmoothTime = 0.8f;
    public float previewHeight = 15f;
    
    [Header("Events")]
    public UnityEvent<SelectableEntity> OnEntitySelected = new UnityEvent<SelectableEntity>();
    
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
    
    void Awake()
    {
        mainCamera = GetComponent<Camera>();
        
        // Set default selectable layer if not set
        if (selectableLayer.value == 0)
        {
            selectableLayer = LayerMask.GetMask("Selectable");
        }
    }
    
    void Update()
    {
        HandleMouseInput();
        HandleKeyboardInput();
        UpdateFollowMode();
        UpdatePreviewMode();
    }
    
    /// <summary>
    /// Handles mouse click for entity selection.
    /// </summary>
    void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectEntity();
            StopFollowing();
            StopPreview();
        }
    }
    
    /// <summary>
    /// Performs raycast from mouse position to detect selectable entities.
    /// </summary>
    void PerformRaycast()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
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
        // Deselect previous entity
        if (currentSelectedEntity != null)
        {
            currentSelectedEntity.Unhighlight();
            RemoveSelectionPin();
        }
        
        // Select new entity
        currentSelectedEntity = entity;
        currentSelectedEntity.Highlight();
        
        // Spawn selection pin
        SpawnSelectionPin(entity.transform);
        
        // Play selection sound
        AudioManager.PlayUISound("entity_select");
        
        // Notify listeners (UI)
        OnEntitySelected?.Invoke(entity);
        
        Debug.Log($"[CameraController] Selected {entity.entityType}: {entity.GetDisplayName()}");
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
        currentSelectionPin = SelectionPinPool.Instance?.Get();
        if (currentSelectionPin != null)
        {
            currentSelectionPin.transform.position = target.position + Vector3.up * 2f;
            currentSelectionPin.transform.SetParent(target);
        }
    }
    
    /// <summary>
    /// Removes and returns selection pin to pool.
    /// </summary>
    void RemoveSelectionPin()
    {
        if (currentSelectionPin != null)
        {
            SelectionPinPool.Instance?.Return(currentSelectionPin);
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

