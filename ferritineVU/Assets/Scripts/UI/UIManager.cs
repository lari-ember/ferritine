using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gerenciador centralizado de UI que instancia painéis dinamicamente.
/// Singleton pattern para acesso global.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Prefabs")]
    [SerializeField] private GameObject entityInspectorPrefab;
    
    [Header("UI Containers")]
    [SerializeField] private Transform uiCanvasTransform;
    
    private GameObject currentInspectorPanel;
    private EntityInspectorPanel currentInspector;
    
    // Singleton
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<UIManager>();
                
                if (_instance == null)
                {
                    Debug.LogError("[UIManager] No UIManager found in scene! Add UIManager component to a Canvas.");
                }
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        // Singleton pattern
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Debug.LogWarning("[UIManager] Duplicate UIManager found, destroying this one.");
            Destroy(gameObject);
            return;
        }
        
        // Auto-find canvas if not assigned
        if (uiCanvasTransform == null)
        {
            Canvas canvas = GetComponent<Canvas>();
            if (canvas != null)
            {
                uiCanvasTransform = canvas.transform;
            }
            else
            {
                Debug.LogError("[UIManager] No Canvas Transform assigned and no Canvas component found!");
            }
        }
        
        ValidatePrefabs();
    }
    
    void Start()
    {
        // Connect to CameraController events
        ConnectToCameraController();
    }
    
    void ValidatePrefabs()
    {
        if (entityInspectorPrefab == null)
        {
            Debug.LogError("[UIManager] ❌ EntityInspectorPanel prefab not assigned! " +
                          "Assign it in the Inspector or UI will not work.");
        }
        else
        {
            Debug.Log($"[UIManager] ✓ entityInspectorPrefab assigned: {entityInspectorPrefab.name}");
            
            // Verify the prefab has the EntityInspectorPanel component
            EntityInspectorPanel inspector = entityInspectorPrefab.GetComponent<EntityInspectorPanel>();
            if (inspector == null)
            {
                Debug.LogError($"[UIManager] ❌ CRÍTICO: O prefab '{entityInspectorPrefab.name}' NÃO tem o componente EntityInspectorPanel! " +
                              $"Adicione o componente EntityInspectorPanel ao prefab ou atribua o prefab correto.");
            }
            else
            {
                Debug.Log($"[UIManager] ✓ Prefab tem componente EntityInspectorPanel");
            }
        }
    }
    
    void ConnectToCameraController()
    {
        CameraController cameraController = FindFirstObjectByType<CameraController>();
        
        if (cameraController != null)
        {
            // Subscribe to entity selection events
            cameraController.OnEntitySelected.AddListener(ShowInspector);
            Debug.Log("[UIManager] Connected to CameraController.OnEntitySelected");
        }
        else
        {
            Debug.LogWarning("[UIManager] CameraController not found in scene. " +
                           "Entity inspector will not open automatically.");
        }
    }
    
    /// <summary>
    /// Shows the entity inspector panel for the selected entity.
    /// Destroys previous panel if it exists and creates a new one.
    /// </summary>
    public void ShowInspector(SelectableEntity entity)
    {
        if (entity == null)
        {
            Debug.LogWarning("[UIManager] ShowInspector called with null entity!");
            return;
        }
        
        if (entityInspectorPrefab == null)
        {
            Debug.LogError("[UIManager] Cannot show inspector: prefab not assigned!");
            return;
        }
        
        // Destroy previous panel if exists
        if (currentInspectorPanel != null)
        {
            Debug.Log($"[UIManager] Fechando painel anterior: {currentInspectorPanel.name}");
            Destroy(currentInspectorPanel);
            currentInspectorPanel = null;
            currentInspector = null;
        }
        
        // Instantiate new panel
        Debug.Log($"[UIManager] Instanciando prefab '{entityInspectorPrefab.name}' em '{uiCanvasTransform.name}'");
        currentInspectorPanel = Instantiate(entityInspectorPrefab, uiCanvasTransform);
        Debug.Log($"[UIManager] ✓ GameObject instanciado: {currentInspectorPanel.name}");
        
        Debug.Log($"[UIManager] Procurando componente EntityInspectorPanel no GameObject...");
        currentInspector = currentInspectorPanel.GetComponent<EntityInspectorPanel>();
        
        if (currentInspector == null)
        {
            Debug.LogError($"[UIManager] ❌ GameObject '{currentInspectorPanel.name}' NÃO tem EntityInspectorPanel component!");
            Debug.LogError($"[UIManager] ❌ SOLUÇÃO: Abra Assets/Prefabs/UI/{entityInspectorPrefab.name} e adicione o componente 'EntityInspectorPanel'!");
            
            // List all components for debugging
            Component[] components = currentInspectorPanel.GetComponents<Component>();
            Debug.LogWarning($"[UIManager] Componentes presentes no GameObject ({components.Length}):");
            foreach (Component comp in components)
            {
                Debug.LogWarning($"  - {comp.GetType().Name}");
            }
            
            Destroy(currentInspectorPanel);
            return;
        }
        
        Debug.Log($"[UIManager] ✓ Componente EntityInspectorPanel encontrado!");
        
        // Display entity data
        Debug.Log($"[UIManager] Chamando ShowEntity({entity.entityType})...");
        currentInspector.ShowEntity(entity);
        
        Debug.Log($"[UIManager] ✅ Inspector exibido com sucesso para {entity.entityType}: {entity.GetDisplayName()}");
    }
    
    /// <summary>
    /// Hides and destroys the current inspector panel.
    /// </summary>
    public void HideInspector()
    {
        if (currentInspectorPanel != null)
        {
            Destroy(currentInspectorPanel);
            currentInspectorPanel = null;
            currentInspector = null;
            
            Debug.Log("[UIManager] Inspector panel hidden");
        }
    }
    
    /// <summary>
    /// Checks if inspector is currently visible.
    /// </summary>
    public bool IsInspectorVisible()
    {
        return currentInspectorPanel != null && currentInspectorPanel.activeSelf;
    }
    
    void OnDestroy()
    {
        // Cleanup
        if (currentInspectorPanel != null)
        {
            Destroy(currentInspectorPanel);
        }
        
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

