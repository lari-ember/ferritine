using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gerenciador centralizado de UI que instancia painéis dinamicamente.
/// Singleton pattern para acesso global.
/// Gerencia TODOS os painéis: Inspector, Teleport, Notifications, etc.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI Prefabs")]
    [SerializeField] private GameObject entityInspectorPrefab;
    [SerializeField] private GameObject teleportSelectorPrefab;
    
    [Header("UI Containers")]
    [SerializeField] private Transform uiCanvasTransform;
    
    // Entity Inspector
    private GameObject currentInspectorPanel;
    private EntityInspectorPanel currentInspector;
    
    // Teleport Selector
    private GameObject currentTeleportPanel;
    private TeleportSelectorUI currentTeleportSelector;
    
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
        
        if (teleportSelectorPrefab == null)
        {
            Debug.LogWarning("[UIManager] ⚠️ TeleportSelectorUI prefab not assigned. Teleport feature will not work.");
        }
        else
        {
            Debug.Log($"[UIManager] ✓ teleportSelectorPrefab assigned: {teleportSelectorPrefab.name}");
            
            // Verify the prefab has the TeleportSelectorUI component
            TeleportSelectorUI teleport = teleportSelectorPrefab.GetComponent<TeleportSelectorUI>();
            if (teleport == null)
            {
                Debug.LogError($"[UIManager] ❌ O prefab '{teleportSelectorPrefab.name}' NÃO tem o componente TeleportSelectorUI!");
            }
            else
            {
                Debug.Log($"[UIManager] ✓ Prefab tem componente TeleportSelectorUI");
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
    
    // ==================== TELEPORT SELECTOR ====================
    
    /// <summary>
    /// Abre o seletor de teleporte para um agente.
    /// </summary>
    public void ShowTeleportSelector(AgentData agent)
    {
        if (agent == null)
        {
            Debug.LogWarning("[UIManager] ShowTeleportSelector called with null agent!");
            return;
        }
        
        if (teleportSelectorPrefab == null)
        {
            Debug.LogError("[UIManager] Cannot show teleport selector: prefab not assigned!");
            return;
        }
        
        // Destroy previous panel if exists
        if (currentTeleportPanel != null)
        {
            Debug.Log($"[UIManager] Fechando teleport selector anterior");
            Destroy(currentTeleportPanel);
            currentTeleportPanel = null;
            currentTeleportSelector = null;
        }
        
        // Instantiate new panel
        Debug.Log($"[UIManager] Abrindo Teleport Selector para agente: {agent.name}");
        currentTeleportPanel = Instantiate(teleportSelectorPrefab, uiCanvasTransform);
        currentTeleportSelector = currentTeleportPanel.GetComponent<TeleportSelectorUI>();
        
        if (currentTeleportSelector == null)
        {
            Debug.LogError($"[UIManager] ❌ GameObject '{currentTeleportPanel.name}' NÃO tem TeleportSelectorUI component!");
            Destroy(currentTeleportPanel);
            return;
        }
        
        // Show selector for agent
        currentTeleportSelector.ShowForAgent(agent);
        
        Debug.Log($"[UIManager] ✅ Teleport Selector exibido para {agent.name}");
    }
    
    /// <summary>
    /// Fecha o seletor de teleporte.
    /// </summary>
    public void HideTeleportSelector()
    {
        if (currentTeleportPanel != null)
        {
            Destroy(currentTeleportPanel);
            currentTeleportPanel = null;
            currentTeleportSelector = null;
            
            Debug.Log("[UIManager] Teleport Selector fechado");
        }
    }
    
    /// <summary>
    /// Verifica se o seletor de teleporte está visível.
    /// </summary>
    public bool IsTeleportSelectorVisible()
    {
        return currentTeleportPanel != null && currentTeleportPanel.activeSelf;
    }
    
    /// <summary>
    /// Verifica se QUALQUER painel modal está aberto.
    /// Usado pelo CameraController para bloquear seleção de entidades
    /// enquanto painéis estão abertos (evita selecionar entidades "através" da UI).
    /// </summary>
    public bool IsAnyPanelOpen()
    {
        return currentInspectorPanel != null || currentTeleportPanel != null;
    }
    
    /// <summary>
    /// Fecha o painel do topo da pilha hierárquica (prioridade: Teleport > Inspector).
    /// Retorna true se fechou algum painel, false se nenhum estava aberto.
    /// Usado pelo CameraController quando ESC é pressionado.
    /// </summary>
    public bool CloseTopPanel()
    {
        // Prioridade: Teleport Selector é mais "em cima" que Inspector
        if (currentTeleportPanel != null)
        {
            HideTeleportSelector();
            AudioManager.PlayUISound("panel_close");
            Debug.Log("[UIManager] ESC: Teleport Selector fechado (topo da pilha)");
            return true;
        }
        
        if (currentInspectorPanel != null)
        {
            HideInspector();
            AudioManager.PlayUISound("panel_close");
            Debug.Log("[UIManager] ESC: Inspector fechado");
            return true;
        }
        
        Debug.Log("[UIManager] ESC: Nenhum painel para fechar");
        return false;
    }
    
    /// <summary>
    /// Fecha TODOS os painéis abertos (útil para ESC ou reset).
    /// </summary>
    public void CloseAllPanels()
    {
        HideInspector();
        HideTeleportSelector();
        
        Debug.Log("[UIManager] Todos os painéis fechados");
    }
    
    void OnDestroy()
    {
        // Cleanup all panels
        if (currentInspectorPanel != null)
        {
            Destroy(currentInspectorPanel);
        }
        
        if (currentTeleportPanel != null)
        {
            Destroy(currentTeleportPanel);
        }
        
        if (_instance == this)
        {
            _instance = null;
        }
    }
}

