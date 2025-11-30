using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;

namespace Controllers
{
    /// <summary>
    /// Controller for the Entity Inspector Panel.
    /// Manages button actions, content switching, and API communication for entity interactions.
    /// Works in tandem with EntityInspectorPanel for display and InspectorPanelController for logic.
    /// </summary>
    public class InspectorPanelController : MonoBehaviour
    {
        [Header("Panel References")]
        [Tooltip("Reference to the main EntityInspectorPanel component")]
        public EntityInspectorPanel inspectorPanel;
        
        [Header("Content Panels")]
        public GameObject stationContent;
        public GameObject vehicleContent;
        public GameObject agentContent;
        public GameObject buildingContent;
        
        [Header("Action Buttons")]
        public Button followButton;
        public Button pauseButton;
        public Button teleportButton;
        public Button modifyQueueButton;
        
        [Header("Button Text Labels")]
        public TextMeshProUGUI pauseButtonText;
        
        [Header("API Configuration")]
        public string apiBaseUrl = "http://localhost:8000";
        
        [Header("Camera")]
        public CameraController cameraController;
        
        [Header("UI References")]
        public TeleportSelectorUI teleportSelectorUI;
        
        // State
        private SelectableEntity _currentEntity;
        private bool _isVehiclePaused;
        
        void Awake()
        {
            // Auto-find references if not assigned
            if (inspectorPanel == null)
            {
                inspectorPanel = GetComponent<EntityInspectorPanel>();
            }
            
            if (cameraController == null)
            {
                cameraController = Camera.main?.GetComponent<CameraController>();
            }
            
            if (teleportSelectorUI == null)
            {
                teleportSelectorUI = FindAnyObjectByType<TeleportSelectorUI>();
            }
            
            // Setup button listeners
            SetupButtonListeners();
        }
        
        /// <summary>
        /// Sets up all button click listeners.
        /// </summary>
        void SetupButtonListeners()
        {
            if (followButton != null)
            {
                followButton.onClick.RemoveAllListeners();
                followButton.onClick.AddListener(OnFollowPressed);
            }
            
            if (pauseButton != null)
            {
                pauseButton.onClick.RemoveAllListeners();
                pauseButton.onClick.AddListener(OnPausePressed);
            }
            
            if (teleportButton != null)
            {
                teleportButton.onClick.RemoveAllListeners();
                teleportButton.onClick.AddListener(OnTeleportPressed);
            }
            
            if (modifyQueueButton != null)
            {
                modifyQueueButton.onClick.RemoveAllListeners();
                modifyQueueButton.onClick.AddListener(OnModifyQueuePressed);
            }
        }
        
        /// <summary>
        /// Sets the current entity being inspected.
        /// Updates button states based on entity type.
        /// </summary>
        public void SetCurrentEntity(SelectableEntity entity)
        {
            _currentEntity = entity;
            _isVehiclePaused = false; // Reset pause state when changing entity
            
            UpdateButtonStates();
            ShowContentForEntityType(entity?.entityType ?? SelectableEntity.EntityType.Station);
        }
        
        /// <summary>
        /// Updates button enabled/disabled states based on current entity type.
        /// </summary>
        void UpdateButtonStates()
        {
            if (_currentEntity == null)
            {
                // Disable all buttons if no entity selected
                SetButtonState(followButton, false);
                SetButtonState(pauseButton, false);
                SetButtonState(teleportButton, false);
                SetButtonState(modifyQueueButton, false);
                return;
            }
            
            // Follow button: enabled for all entity types
            SetButtonState(followButton, true);
            
            // Pause button: only for vehicles
            bool isVehicle = _currentEntity.entityType == SelectableEntity.EntityType.Vehicle;
            SetButtonState(pauseButton, isVehicle);
            
            if (isVehicle && pauseButtonText != null)
            {
                pauseButtonText.text = _isVehiclePaused ? "▶ Retomar" : "⏸ Pausar";
            }
            
            // Teleport button: only for agents
            bool isAgent = _currentEntity.entityType == SelectableEntity.EntityType.Agent;
            SetButtonState(teleportButton, isAgent);
            
            // Modify Queue button: only for stations
            bool isStation = _currentEntity.entityType == SelectableEntity.EntityType.Station;
            SetButtonState(modifyQueueButton, isStation);
        }
        
        /// <summary>
        /// Helper to safely set button interactable state.
        /// </summary>
        void SetButtonState(Button button, bool isEnabled)
        {
            if (button != null)
            {
                button.interactable = isEnabled;
            }
        }
        
        /// <summary>
        /// Shows the appropriate content panel for the given entity type.
        /// </summary>
        public void ShowContentForEntityType(SelectableEntity.EntityType entityType)
        {
            // Hide all panels
            if (stationContent != null) stationContent.SetActive(false);
            if (vehicleContent != null) vehicleContent.SetActive(false);
            if (agentContent != null) agentContent.SetActive(false);
            if (buildingContent != null) buildingContent.SetActive(false);
            
            // Show relevant panel
            switch (entityType)
            {
                case SelectableEntity.EntityType.Station:
                    if (stationContent != null) stationContent.SetActive(true);
                    break;
                    
                case SelectableEntity.EntityType.Vehicle:
                    if (vehicleContent != null) vehicleContent.SetActive(true);
                    break;
                    
                case SelectableEntity.EntityType.Agent:
                    if (agentContent != null) agentContent.SetActive(true);
                    break;
                    
                case SelectableEntity.EntityType.Building:
                    if (buildingContent != null) buildingContent.SetActive(true);
                    break;
            }
        }
        
        /// <summary>
        /// Legacy method for backward compatibility.
        /// </summary>
        public void ShowContent(string type)
        {
            if (stationContent != null) stationContent.SetActive(type == "station");
            if (vehicleContent != null) vehicleContent.SetActive(type == "vehicle");
            if (agentContent != null) agentContent.SetActive(type == "agent");
            if (buildingContent != null) buildingContent.SetActive(type == "building");
        }
        
        // ==================== BUTTON HANDLERS ====================
        
        /// <summary>
        /// Handles Follow button press.
        /// Makes camera follow the selected entity.
        /// </summary>
        public void OnFollowPressed()
        {
            if (_currentEntity == null)
            {
                Debug.LogWarning("[InspectorPanelController] Cannot follow: no entity selected");
                ToastNotificationManager.ShowWarning("Nenhuma entidade selecionada");
                return;
            }
            
            if (cameraController == null)
            {
                Debug.LogError("[InspectorPanelController] CameraController not found!");
                ToastNotificationManager.ShowError("CameraController não encontrado!");
                AudioManager.PlayUISound("toast_error");
                return;
            }
            
            // Start following the entity
            cameraController.FollowSelectedEntity();
            
            string entityName = _currentEntity.GetDisplayName();
            ToastNotificationManager.ShowSuccess($"📹 Seguindo {entityName}");
            AudioManager.PlayUISound("button_select");
            
            Debug.Log($"[InspectorPanelController] Following entity: {entityName}");
        }
        
        /// <summary>
        /// Handles Pause/Resume button press for vehicles.
        /// Toggles vehicle movement state via API.
        /// </summary>
        public void OnPausePressed()
        {
            if (_currentEntity == null || _currentEntity.entityType != SelectableEntity.EntityType.Vehicle)
            {
                Debug.LogWarning("[InspectorPanelController] Cannot pause: not a vehicle");
                ToastNotificationManager.ShowWarning("Apenas veículos podem ser pausados");
                return;
            }
            
            if (_currentEntity.vehicleData == null)
            {
                Debug.LogError("[InspectorPanelController] Vehicle data is null!");
                ToastNotificationManager.ShowError("Dados do veículo não disponíveis");
                return;
            }
            
            string vehicleId = _currentEntity.vehicleData.id;
            string endpoint = _isVehiclePaused ? "resume" : "pause";
            
            StartCoroutine(SendVehicleCommand(vehicleId, endpoint));
            
            Debug.Log($"[InspectorPanelController] Sending {endpoint} command to vehicle {vehicleId}");
        }
        
        /// <summary>
        /// Sends pause/resume command to the vehicle API.
        /// </summary>
        IEnumerator SendVehicleCommand(string vehicleId, string command)
        {
            string url = $"{apiBaseUrl}/api/vehicles/{vehicleId}/{command}";
            
            using (UnityWebRequest request = UnityWebRequest.PostWwwForm(url, ""))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    _isVehiclePaused = (command == "pause");
                    
                    // Update button text
                    if (pauseButtonText != null)
                    {
                        pauseButtonText.text = _isVehiclePaused ? "▶ Retomar" : "⏸ Pausar";
                    }
                    
                    string vehicleName = _currentEntity.GetDisplayName();
                    string message = _isVehiclePaused 
                        ? $"⏸ Veículo {vehicleName} pausado" 
                        : $"▶ Veículo {vehicleName} retomado";
                    
                    ToastNotificationManager.ShowSuccess(message);
                    AudioManager.PlayUISound("button_toggle");
                    
                    Debug.Log($"[InspectorPanelController] Vehicle {vehicleId} {command} successful");
                }
                else
                {
                    string errorMessage = $"Erro ao {(command == "pause" ? "pausar" : "retomar")} veículo: {request.error}";
                    ToastNotificationManager.ShowError(errorMessage);
                    AudioManager.PlayUISound("toast_error");
                    
                    Debug.LogError($"[InspectorPanelController] {errorMessage}");
                }
            }
        }
        
        /// <summary>
        /// Handles Teleport button press for agents.
        /// Opens the teleport location selector UI.
        /// </summary>
        public void OnTeleportPressed()
        {
            if (_currentEntity == null || _currentEntity.entityType != SelectableEntity.EntityType.Agent)
            {
                Debug.LogWarning("[InspectorPanelController] Cannot teleport: not an agent");
                ToastNotificationManager.ShowWarning("Apenas agentes podem ser teleportados");
                return;
            }
            
            if (_currentEntity.agentData == null)
            {
                Debug.LogError("[InspectorPanelController] Agent data is null!");
                ToastNotificationManager.ShowError("Dados do agente não disponíveis");
                return;
            }
            
            if (teleportSelectorUI == null)
            {
                Debug.LogError("[InspectorPanelController] TeleportSelectorUI not found!");
                ToastNotificationManager.ShowError("TeleportSelectorUI não encontrado!");
                AudioManager.PlayUISound("toast_error");
                return;
            }
            
            // Open teleport selector
            teleportSelectorUI.Open(_currentEntity.agentData);
            AudioManager.PlayUISound("panel_open");
            
            string agentName = _currentEntity.GetDisplayName();
            Debug.Log($"[InspectorPanelController] Opening teleport selector for agent: {agentName}");
        }
        
        /// <summary>
        /// Handles Modify Queue button press for stations.
        /// Opens input dialog or directly modifies queue based on implementation.
        /// </summary>
        public void OnModifyQueuePressed()
        {
            if (_currentEntity == null || _currentEntity.entityType != SelectableEntity.EntityType.Station)
            {
                Debug.LogWarning("[InspectorPanelController] Cannot modify queue: not a station");
                ToastNotificationManager.ShowWarning("Apenas estações podem ter filas modificadas");
                return;
            }
            
            if (_currentEntity.stationData == null)
            {
                Debug.LogError("[InspectorPanelController] Station data is null!");
                ToastNotificationManager.ShowError("Dados da estação não disponíveis");
                return;
            }
            
            // For now, show info message
            // TODO: Implement queue modification UI with input field
            string stationName = _currentEntity.stationData.name;
            int currentQueue = _currentEntity.stationData.queue_length;
            int maxQueue = _currentEntity.stationData.max_queue;
            
            ToastNotificationManager.ShowInfo($"📊 Fila de {stationName}: {currentQueue}/{maxQueue}");
            AudioManager.PlayUISound("button_click");
            
            Debug.Log($"[InspectorPanelController] Modify queue requested for station: {stationName}");
            
            // Example: Send test command to increase queue by 1
            // StartCoroutine(SendModifyQueueCommand(_currentEntity.stationData.id, currentQueue + 1));
        }
        
        /// <summary>
        /// Sends modify queue command to the station API.
        /// </summary>
        IEnumerator SendModifyQueueCommand(string stationId, int newQueueLength)
        {
            string url = $"{apiBaseUrl}/api/stations/{stationId}/queue";
            string jsonBody = $"{{\"queue_length\": {newQueueLength}}}";
            
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    ToastNotificationManager.ShowSuccess($"✅ Fila da estação modificada para {newQueueLength}");
                    AudioManager.PlayUISound("button_confirm");
                    
                    // Update local data
                    if (_currentEntity.stationData != null)
                    {
                        _currentEntity.stationData.queue_length = newQueueLength;
                        _currentEntity.UpdateData(_currentEntity.stationData);
                    }
                    
                    Debug.Log($"[InspectorPanelController] Station {stationId} queue updated to {newQueueLength}");
                }
                else
                {
                    string errorMessage = $"Erro ao modificar fila: {request.error}";
                    ToastNotificationManager.ShowError(errorMessage);
                    AudioManager.PlayUISound("toast_error");
                    
                    Debug.LogError($"[InspectorPanelController] {errorMessage}");
                }
            }
        }
        
        /// <summary>
        /// Public method to update entity from external source (e.g., CameraController).
        /// </summary>
        public void OnEntitySelected(SelectableEntity entity)
        {
            SetCurrentEntity(entity);
        }
        
        void OnDestroy()
        {
            // Clean up button listeners
            if (followButton != null) followButton.onClick.RemoveAllListeners();
            if (pauseButton != null) pauseButton.onClick.RemoveAllListeners();
            if (teleportButton != null) teleportButton.onClick.RemoveAllListeners();
            if (modifyQueueButton != null) modifyQueueButton.onClick.RemoveAllListeners();
        }
    }
}

