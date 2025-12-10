using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

/// <summary>
/// Entity inspector panel - displays detailed information about selected entities.
/// Draggable, tabbed interface with interactive controls.
/// Implements Observer pattern to auto-update when entity data changes.
/// </summary>
public class EntityInspectorPanel : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("References")]
    public TextMeshProUGUI titleText;
    public Image iconImage;
    public Button closeButton;
    public Button expandButton;
    
    [Header("Tab System")]
    public ToggleGroup tabGroup;
    public Toggle basicTab;
    public Toggle statisticsTab;
    public Toggle historyTab;
    
    [Header("Content Panels")]
    public GameObject stationPanel;
    public GameObject vehiclePanel;
    public GameObject agentPanel;
    public GameObject buildingPanel;
    
    [Header("Station Fields")]
    public TextMeshProUGUI stationType;
    public TextMeshProUGUI stationQueue;
    public TextMeshProUGUI stationCoords;
    public TextMeshProUGUI stationOperational;
    public InputField stationQueueInput;
    public Button modifyQueueButton;
    
    [Header("Vehicle Fields")]
    public TextMeshProUGUI vehicleType;
    public TextMeshProUGUI vehiclePassengers;
    public Image vehiclePassengersBar;
    public TextMeshProUGUI vehicleFuel;
    public Image vehicleFuelBar;
    public TextMeshProUGUI vehicleStation;
    public Button pauseResumeButton;
    public TextMeshProUGUI pauseResumeButtonText;
    
    [Header("Agent Fields")]
    public TextMeshProUGUI agentStatus;
    public TextMeshProUGUI agentEnergy;
    public Image agentEnergyBar;
    public TextMeshProUGUI agentWallet;
    public TextMeshProUGUI agentLocation;
    public Button teleportButton;
    
    [Header("Action Buttons")]
    public Button followButton;
    
    [Header("Animation")]
    public float expandedHeight = 600f;
    public float collapsedHeight = 400f;
    public float animationDuration = 0.3f;
    
    // State
    private SelectableEntity currentEntity;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 dragOffset;
    private bool isExpanded = false;
    private bool isPaused = false;
    
    // API base URL
    private string apiBaseUrl = "http://localhost:8000";
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        
        // NOTE: NÃO conectar ao CameraController aqui!
        // O UIManager gerencia a criação/destruição deste painel
        // e chama ShowEntity() quando necessário.
        
        // Setup button listeners
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
        
        if (expandButton != null)
            expandButton.onClick.AddListener(ToggleExpand);
        
        if (followButton != null)
            followButton.onClick.AddListener(OnFollowClicked);
        
        if (pauseResumeButton != null)
            pauseResumeButton.onClick.AddListener(OnPauseResumeClicked);
        
        if (teleportButton != null)
            teleportButton.onClick.AddListener(OnTeleportClicked);
        
        if (modifyQueueButton != null)
            modifyQueueButton.onClick.AddListener(OnModifyQueueClicked);
        
        // Setup tab listeners
        if (basicTab != null)
            basicTab.onValueChanged.AddListener((isOn) => { if (isOn) OnTabChanged("basic"); });
        
        if (statisticsTab != null)
            statisticsTab.onValueChanged.AddListener((isOn) => { if (isOn) OnTabChanged("statistics"); });
        
        if (historyTab != null)
            historyTab.onValueChanged.AddListener((isOn) => { if (isOn) OnTabChanged("history"); });
        
        // Load saved position
        LoadPosition();
        
        // NOTE: Panel inicia ATIVO - UIManager vai instanciar quando necessário
        // Não precisa desativar aqui pois é instanciado dinamicamente
    }
    
    /// <summary>
    /// Shows the panel for a selected entity.
    /// </summary>
    public void ShowEntity(SelectableEntity entity)
    {
        if (entity == null) return;
        
        // Unregister from previous entity
        if (currentEntity != null)
        {
            currentEntity.OnDataUpdated.RemoveListener(OnEntityDataUpdated);
        }
        
        currentEntity = entity;
        
        // Register as observer
        currentEntity.OnDataUpdated.AddListener(OnEntityDataUpdated);
        
        // Update display
        UpdateDisplay();
        
        // Show panel with animation
        gameObject.SetActive(true);
        StartCoroutine(AnimateShow());
        
        // Play sound
        AudioManager.PlayUISound("panel_open");
    }
    
    /// <summary>
    /// Updates the display with current entity data.
    /// </summary>
    void UpdateDisplay()
    {
        if (currentEntity == null) return;
        
        // Update title
        if (titleText != null)
        {
            titleText.text = currentEntity.GetDisplayName();
        }
        
        // Hide all content panels
        if (stationPanel != null) stationPanel.SetActive(false);
        if (vehiclePanel != null) vehiclePanel.SetActive(false);
        if (agentPanel != null) agentPanel.SetActive(false);
        if (buildingPanel != null) buildingPanel.SetActive(false);
        
        // Show relevant panel based on entity type
        switch (currentEntity.entityType)
        {
            case SelectableEntity.EntityType.Station:
                UpdateStationDisplay();
                break;
            case SelectableEntity.EntityType.Vehicle:
                UpdateVehicleDisplay();
                break;
            case SelectableEntity.EntityType.Agent:
                UpdateAgentDisplay();
                break;
            case SelectableEntity.EntityType.Building:
                UpdateBuildingDisplay();
                break;
        }
    }
    
    /// <summary>
    /// Updates station-specific fields.
    /// </summary>
    void UpdateStationDisplay()
    {
        if (stationPanel == null || currentEntity.stationData == null) return;
        
        stationPanel.SetActive(true);
        StationData data = currentEntity.stationData;
        
        if (stationType != null)
            stationType.text = $"Tipo: {data.station_type}";
        
        if (stationQueue != null)
            stationQueue.text = $"Fila: {data.queue_length}/{data.max_queue}";
        
        if (stationCoords != null)
            stationCoords.text = $"Coordenadas: ({data.x}, {data.y})";
        
        if (stationOperational != null)
        {
            stationOperational.text = data.is_operational ? "✓ Operacional" : "✗ Inoperante";
            stationOperational.color = data.is_operational ? Color.green : Color.red;
        }
    }
    
    /// <summary>
    /// Updates vehicle-specific fields.
    /// </summary>
    void UpdateVehicleDisplay()
    {
        if (vehiclePanel == null || currentEntity.vehicleData == null) return;
        
        vehiclePanel.SetActive(true);
        VehicleData data = currentEntity.vehicleData;
        
        if (vehicleType != null)
            vehicleType.text = $"Tipo: {data.vehicle_type}";
        
        if (vehiclePassengers != null)
        {
            float occupancy = data.capacity > 0 ? (float)data.passengers / data.capacity : 0f;
            vehiclePassengers.text = $"Passageiros: {data.passengers}/{data.capacity} ({occupancy:P0})";
            
            if (vehiclePassengersBar != null)
                vehiclePassengersBar.fillAmount = occupancy;
        }
        
        if (vehicleFuel != null)
        {
            vehicleFuel.text = $"Combustível: {data.fuel_level:F1}%";
            
            if (vehicleFuelBar != null)
                vehicleFuelBar.fillAmount = data.fuel_level / 100f;
        }
        
        if (vehicleStation != null)
        {
            vehicleStation.text = string.IsNullOrEmpty(data.current_station_id) 
                ? "Estação: Em trânsito" 
                : $"Estação: {data.current_station_id.Substring(0, 8)}...";
        }
        
        // Update pause/resume button text
        if (pauseResumeButtonText != null)
        {
            pauseResumeButtonText.text = isPaused ? "▶ Retomar" : "⏸ Pausar";
        }
    }
    
    /// <summary>
    /// Updates agent-specific fields.
    /// </summary>
    void UpdateAgentDisplay()
    {
        if (agentPanel == null || currentEntity.agentData == null) return;
        
        agentPanel.SetActive(true);
        AgentData data = currentEntity.agentData;
        
        if (agentStatus != null)
            agentStatus.text = $"Status: {data.status}";
        
        if (agentEnergy != null)
        {
            float energyPercent = data.energy_level / 100f;
            agentEnergy.text = $"Energia: {data.energy_level}%";
            
            if (agentEnergyBar != null)
                agentEnergyBar.fillAmount = energyPercent;
        }
        
        if (agentWallet != null)
        {
            agentWallet.text = $"Carteira: R$ {data.wallet:F2}";
        }
        
        if (agentLocation != null)
        {
            string locType = data.location_type ?? "Desconhecido";
            string locId = string.IsNullOrEmpty(data.location_id) 
                ? "N/A" 
                : data.location_id.Substring(0, 8) + "...";
            agentLocation.text = $"Localização: {locType} ({locId})";
        }
    }
    
    /// <summary>
    /// Updates building-specific fields.
    /// </summary>
    void UpdateBuildingDisplay()
    {
        if (buildingPanel == null || currentEntity.buildingData == null) return;
        
        buildingPanel.SetActive(true);
        // TODO: Add building-specific fields when needed
    }
    
    /// <summary>
    /// Called when entity data is updated (Observer pattern callback).
    /// </summary>
    void OnEntityDataUpdated(object newData)
    {
        UpdateDisplay();
    }
    
    /// <summary>
    /// Handles tab changes.
    /// </summary>
    void OnTabChanged(string tabName)
    {
        AudioManager.PlayUISound("button_click");
        
        // TODO: Show/hide different content based on tab
        // For now, all tabs show the same content (basic info)
        Debug.Log($"[EntityInspectorPanel] Tab changed to: {tabName}");
    }
    
    /// <summary>
    /// Closes the panel and notifies UIManager to destroy it.
    /// </summary>
    public void Close()
    {
        AudioManager.PlayUISound("panel_close");
        
        // Unregister from entity updates
        if (currentEntity != null)
        {
            currentEntity.OnDataUpdated.RemoveListener(OnEntityDataUpdated);
            currentEntity = null;
        }
        
        // Notify UIManager to destroy this panel
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideInspector();
        }
        else
        {
            // Fallback: destroy self if UIManager doesn't exist
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Toggles panel expansion.
    /// </summary>
    void ToggleExpand()
    {
        isExpanded = !isExpanded;
        StartCoroutine(AnimateExpand(isExpanded ? expandedHeight : collapsedHeight));
        AudioManager.PlayUISound("button_click");
    }
    
    /// <summary>
    /// Handles Follow button click.
    /// </summary>
    void OnFollowClicked()
    {
        if (currentEntity == null) return;
        
        CameraController cameraController = Camera.main?.GetComponent<CameraController>();
        
        if (cameraController != null)
        {
            cameraController.FollowSelectedEntity();
            ToastNotificationManager.ShowSuccess($"Seguindo {currentEntity.GetDisplayName()}");
            AudioManager.PlayUISound("button_select");
        }
        else
        {
            Debug.LogError("[EntityInspectorPanel] CameraController not found on Main Camera!");
            ToastNotificationManager.ShowError("CameraController não encontrado!");
        }
    }
    
    /// <summary>
    /// Handles Pause/Resume button click for vehicles.
    /// </summary>
    void OnPauseResumeClicked()
    {
        if (currentEntity == null || currentEntity.entityType != SelectableEntity.EntityType.Vehicle)
            return;
        
        string vehicleId = currentEntity.vehicleData.id;
        string endpoint = isPaused ? "resume" : "pause";
        
        StartCoroutine(SendVehicleCommand(vehicleId, endpoint));
    }
    
    /// <summary>
    /// Sends pause/resume command to API.
    /// </summary>
    IEnumerator SendVehicleCommand(string vehicleId, string command)
    {
        string url = $"{apiBaseUrl}/api/vehicles/{vehicleId}/{command}";
        
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                isPaused = (command == "pause");
                UpdateVehicleDisplay();
                
                string message = isPaused 
                    ? $"Veículo {currentEntity.GetDisplayName()} pausado" 
                    : $"Veículo {currentEntity.GetDisplayName()} retomado";
                    
                ToastNotificationManager.ShowSuccess(message);
                AudioManager.PlayUISound("button_toggle");
            }
            else
            {
                ToastNotificationManager.ShowError($"Erro ao {command} veículo: {request.error}");
                AudioManager.PlayUISound("toast_error");
            }
        }
    }
    
    /// <summary>
    /// Handles Teleport button click for agents.
    /// </summary>
    void OnTeleportClicked()
    {
        if (currentEntity == null || currentEntity.entityType != SelectableEntity.EntityType.Agent)
            return;
        
        // Open teleport selector UI via UIManager
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowTeleportSelector(currentEntity.agentData);
            AudioManager.PlayUISound("panel_open");
        }
        else
        {
            ToastNotificationManager.ShowError("UIManager não encontrado!");
        }
    }
    
    /// <summary>
    /// Handles Modify Queue button click for stations.
    /// </summary>
    void OnModifyQueueClicked()
    {
        if (currentEntity == null || currentEntity.entityType != SelectableEntity.EntityType.Station)
            return;
        
        if (stationQueueInput == null) return;
        
        if (int.TryParse(stationQueueInput.text, out int newQueueLength))
        {
            string stationId = currentEntity.stationData.id;
            StartCoroutine(SendModifyQueueCommand(stationId, newQueueLength));
        }
        else
        {
            ToastNotificationManager.ShowWarning("Por favor, insira um número válido");
        }
    }
    
    /// <summary>
    /// Sends modify queue command to API.
    /// </summary>
    IEnumerator SendModifyQueueCommand(string stationId, int queueLength)
    {
        string url = $"{apiBaseUrl}/api/stations/{stationId}/queue";
        string jsonBody = $"{{\"queue_length\": {queueLength}}}";
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                ToastNotificationManager.ShowSuccess($"Fila da estação modificada para {queueLength}");
                AudioManager.PlayUISound("button_confirm");
                
                // Update display
                if (currentEntity.stationData != null)
                {
                    currentEntity.stationData.queue_length = queueLength;
                    UpdateStationDisplay();
                }
            }
            else
            {
                ToastNotificationManager.ShowError($"Erro ao modificar fila: {request.error}");
                AudioManager.PlayUISound("toast_error");
            }
        }
    }
    
    // ==================== DRAG HANDLERS ====================
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragOffset = eventData.position - (Vector2)rectTransform.position;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position - dragOffset;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        SavePosition();
    }
    
    // ==================== ANIMATIONS ====================
    
    /// <summary>
    /// Animates panel show.
    /// </summary>
    IEnumerator AnimateShow()
    {
        canvasGroup.alpha = 0f;
        rectTransform.localScale = Vector3.one * 0.8f;
        
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            canvasGroup.alpha = t;
            rectTransform.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, t);
            
            yield return null;
        }
        
        canvasGroup.alpha = 1f;
        rectTransform.localScale = Vector3.one;
    }
    
    /// <summary>
    /// Animates panel hide.
    /// </summary>
    IEnumerator AnimateHide()
    {
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            canvasGroup.alpha = 1f - t;
            rectTransform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, t);
            
            yield return null;
        }
        
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Animates panel expansion/collapse.
    /// </summary>
    IEnumerator AnimateExpand(float targetHeight)
    {
        LayoutElement layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = gameObject.AddComponent<LayoutElement>();
        }
        
        float startHeight = layoutElement.preferredHeight;
        float elapsed = 0f;
        
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            
            layoutElement.preferredHeight = Mathf.Lerp(startHeight, targetHeight, t);
            
            yield return null;
        }
        
        layoutElement.preferredHeight = targetHeight;
    }
    
    // ==================== PERSISTENCE ====================
    
    /// <summary>
    /// Saves panel position to PlayerPrefs.
    /// </summary>
    void SavePosition()
    {
        PlayerPrefs.SetFloat("InspectorPanelX", rectTransform.anchoredPosition.x);
        PlayerPrefs.SetFloat("InspectorPanelY", rectTransform.anchoredPosition.y);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Loads panel position from PlayerPrefs.
    /// </summary>
    void LoadPosition()
    {
        if (PlayerPrefs.HasKey("InspectorPanelX"))
        {
            float x = PlayerPrefs.GetFloat("InspectorPanelX");
            float y = PlayerPrefs.GetFloat("InspectorPanelY");
            rectTransform.anchoredPosition = new Vector2(x, y);
        }
    }
    
    void OnDestroy()
    {
        // Cleanup: unregister from entity updates
        if (currentEntity != null)
        {
            currentEntity.OnDataUpdated.RemoveListener(OnEntityDataUpdated);
        }
        
        // NOTE: Não precisamos mais desconectar do CameraController
        // porque o painel é instanciado dinamicamente e destruído quando fechado
    }
}

