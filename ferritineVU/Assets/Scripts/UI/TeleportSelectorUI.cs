using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using Controllers;

/// <summary>
/// Teleport selector UI - allows selecting destination for agent teleportation.
/// Shows list of stations and buildings with preview functionality.
/// </summary>
public class TeleportSelectorUI : MonoBehaviour
{
    [Header("References")]
    public GameObject panel;
    public ScrollRect scrollView;
    public RectTransform contentContainer;
    public GameObject locationItemPrefab;
    
    [Header("Controls")]
    public Button confirmButton;
    public Button cancelButton;
    public TMP_Dropdown filterDropdown;
    public TMP_InputField searchField;
    
    [Header("Preview")]
    public TextMeshProUGUI previewText;
    
    // State
    private AgentData currentAgent;
    private string selectedLocationType;
    private string selectedLocationId;
    private GameObject selectedLocationObject;
    private GameObject previewParticle;
    
    // API
    private string apiBaseUrl = "http://localhost:8000";
    
    void Awake()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClicked);
        
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);
        
        if (filterDropdown != null)
            filterDropdown.onValueChanged.AddListener(OnFilterChanged);
        
        if (searchField != null)
            searchField.onValueChanged.AddListener(OnSearchChanged);
        
        // Start hidden
        if (panel != null)
            panel.SetActive(false);
    }
    
    /// <summary>
    /// Opens the teleport selector for an agent.
    /// </summary>
    public void Open(AgentData agent)
    {
        if (agent == null) return;
        
        currentAgent = agent;
        selectedLocationType = null;
        selectedLocationId = null;
        
        if (panel != null)
            panel.SetActive(true);
        
        // Populate location list
        PopulateLocationList();
        
        AudioManager.PlayUISound("panel_open");
    }
    
    /// <summary>
    /// Alias for Open() - usado pelo UIManager.
    /// </summary>
    public void ShowForAgent(AgentData agent)
    {
        Open(agent);
    }
    
    /// <summary>
    /// Closes the teleport selector.
    /// </summary>
    public void Close()
    {
        // Cleanup selected location highlight
        if (selectedLocationObject != null)
        {
            SelectableEntity entity = selectedLocationObject.GetComponent<SelectableEntity>();
            if (entity != null)
            {
                entity.Unhighlight();
            }
            selectedLocationObject = null;
        }
        
        // Remove preview particle
        if (previewParticle != null)
        {
            ParticleEffectPool.Instance?.Return("teleport_preview", previewParticle);
            previewParticle = null;
        }
        
        // Stop camera preview
        CameraController cameraController = Camera.main?.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.StopPreview();
        }
        
        // Clear selection state
        currentAgent = null;
        selectedLocationType = null;
        selectedLocationId = null;
        
        // Hide panel
        if (panel != null)
            panel.SetActive(false);
        
        AudioManager.PlayUISound("panel_close");
        
        // Notify UIManager to destroy this panel (se estiver gerenciado)
        UIManager.Instance?.HideTeleportSelector();
    }
    
    /// <summary>
    /// Populates the list of available teleport destinations.
    /// </summary>
    void PopulateLocationList()
    {
        // Clear existing items
        ClearLocationList();
        
        // Get stations from WorldController
        WorldController worldController = FindAnyObjectByType<WorldController>();
        if (worldController != null)
        {
            List<StationData> stations = worldController.GetAllStations();
            foreach (var station in stations)
            {
                CreateLocationItem("station", station.id, station.name, station.station_type, 
                                  new Vector3(station.x, 0, station.y));
            }
        }
        
        // Fetch buildings from API
        StartCoroutine(FetchAndAddBuildings());
    }
    
    /// <summary>
    /// Fetches buildings from API and adds them to the list.
    /// </summary>
    IEnumerator FetchAndAddBuildings()
    {
        string url = $"{apiBaseUrl}/api/buildings?limit=100";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string jsonResponse = request.downloadHandler.text;
                
                // Parse JSON manually (simple approach)
                // In production, use JsonUtility or Newtonsoft.Json
                BuildingListResponse response = JsonUtility.FromJson<BuildingListResponse>("{\"buildings\":" + jsonResponse + "}");
                
                if (response != null && response.buildings != null)
                {
                    foreach (var building in response.buildings)
                    {
                        CreateLocationItem("building", building.id, building.name, building.building_type,
                                          new Vector3(building.x, 0, building.y));
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[TeleportSelectorUI] Failed to fetch buildings: {request.error}");
            }
        }
    }
    
    [System.Serializable]
    private class BuildingListResponse
    {
        public List<BuildingData> buildings;
    }
    
    /// <summary>
    /// Creates a location item in the scroll view.
    /// </summary>
    void CreateLocationItem(string locationType, string locationId, string locationName, 
                           string subType, Vector3 worldPosition)
    {
        if (locationItemPrefab == null || contentContainer == null) return;
        
        GameObject item = Instantiate(locationItemPrefab, contentContainer);
        
        // Setup item data
        TeleportLocationItem itemComponent = item.GetComponent<TeleportLocationItem>();
        if (itemComponent == null)
        {
            itemComponent = item.AddComponent<TeleportLocationItem>();
        }
        
        itemComponent.Setup(locationType, locationId, locationName, subType, worldPosition, this);
    }
    
    /// <summary>
    /// Called when a location item is selected.
    /// </summary>
    public void OnLocationSelected(string locationType, string locationId, string locationName, Vector3 worldPosition)
    {
        selectedLocationType = locationType;
        selectedLocationId = locationId;
        
        // Update preview text
        if (previewText != null)
        {
            previewText.text = $"Destino: {locationName} ({locationType})";
        }
        
        // Highlight location in 3D world
        HighlightLocation(locationType, locationId);
        
        // Preview camera position
        CameraController cameraController = Camera.main?.GetComponent<CameraController>();
        if (cameraController != null)
        {
            cameraController.PreviewLocation(worldPosition);
        }
        
        // Spawn preview particle effect
        SpawnPreviewParticle(worldPosition);
        
        AudioManager.PlayUISound("button_select");
    }
    
    /// <summary>
    /// Highlights the selected location in the 3D world.
    /// </summary>
    void HighlightLocation(string locationType, string locationId)
    {
        WorldController worldController = FindAnyObjectByType<WorldController>();
        if (worldController == null) return;
        
        // Get the GameObject for this location
        GameObject locationObj = worldController.GetLocationGameObject(locationType, locationId);
        if (locationObj != null)
        {
            SelectableEntity entity = locationObj.GetComponent<SelectableEntity>();
            if (entity != null)
            {
                // Unhighlight previous selection
                if (selectedLocationObject != null)
                {
                    SelectableEntity prevEntity = selectedLocationObject.GetComponent<SelectableEntity>();
                    if (prevEntity != null)
                    {
                        prevEntity.Unhighlight();
                    }
                }
                
                // Highlight new selection
                entity.Highlight();
                selectedLocationObject = locationObj;
            }
        }
    }
    
    /// <summary>
    /// Spawns a preview particle effect at the destination.
    /// </summary>
    void SpawnPreviewParticle(Vector3 position)
    {
        // Remove previous preview particle
        if (previewParticle != null)
        {
            ParticleEffectPool.Instance?.Return("teleport_preview", previewParticle);
            previewParticle = null;
        }
        
        // Spawn new preview particle
        previewParticle = ParticleEffectPool.Instance?.Get("teleport_preview", position, Quaternion.identity);
    }
    
    /// <summary>
    /// Handles Confirm button click.
    /// </summary>
    void OnConfirmClicked()
    {
        if (currentAgent == null || string.IsNullOrEmpty(selectedLocationId))
        {
            ToastNotificationManager.ShowWarning("Por favor, selecione um destino");
            return;
        }
        
        // Send teleport request to API
        StartCoroutine(SendTeleportRequest());
    }
    
    /// <summary>
    /// Sends teleport request to API.
    /// </summary>
    IEnumerator SendTeleportRequest()
    {
        string url = $"{apiBaseUrl}/api/agents/{currentAgent.id}/teleport";
        string jsonBody = $"{{\"location_type\": \"{selectedLocationType}\", \"location_id\": \"{selectedLocationId}\"}}";
        
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                // Success - spawn teleport effects
                WorldController worldController = FindAnyObjectByType<WorldController>();
                if (worldController != null)
                {
                    GameObject agentObj = worldController.GetAgentGameObject(currentAgent.id);
                    if (agentObj != null)
                    {
                        // Spawn despawn effect at current position
                        ParticleEffectPool.Instance?.Play("teleport_despawn", agentObj.transform.position);
                        
                        // Spawn spawn effect at destination
                        GameObject destObj = worldController.GetLocationGameObject(selectedLocationType, selectedLocationId);
                        if (destObj != null)
                        {
                            ParticleEffectPool.Instance?.Play("teleport_spawn", destObj.transform.position);
                        }
                    }
                }
                
                AudioManager.PlayUISound("teleport_woosh");
                ToastNotificationManager.ShowSuccess($"Agente {currentAgent.name} teleportado com sucesso!");
                
                Close();
            }
            else
            {
                ToastNotificationManager.ShowError($"Erro ao teleportar agente: {request.error}");
                AudioManager.PlayUISound("toast_error");
            }
        }
    }
    
    /// <summary>
    /// Handles Cancel button click.
    /// </summary>
    void OnCancelClicked()
    {
        Close();
    }
    
    /// <summary>
    /// Handles filter dropdown change.
    /// </summary>
    void OnFilterChanged(int value)
    {
        // TODO: Implement filtering (All, Stations Only, Buildings Only)
        PopulateLocationList();
    }
    
    /// <summary>
    /// Handles search field change.
    /// </summary>
    void OnSearchChanged(string searchText)
    {
        // TODO: Implement search filtering
    }
    
    /// <summary>
    /// Clears all items from the location list.
    /// </summary>
    void ClearLocationList()
    {
        if (contentContainer == null) return;
        
        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }
    }
}

/// <summary>
/// Component for individual location items in the teleport selector list.
/// </summary>
public class TeleportLocationItem : MonoBehaviour
{
    private string locationType;
    private string locationId;
    private string locationName;
    private Vector3 worldPosition;
    private TeleportSelectorUI parentUI;
    
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI typeText;
    public TextMeshProUGUI coordsText;
    public Image iconImage;
    public Button selectButton;
    
    public void Setup(string type, string id, string name, string subType, Vector3 position, TeleportSelectorUI ui)
    {
        locationType = type;
        locationId = id;
        locationName = name;
        worldPosition = position;
        parentUI = ui;
        
        // Update UI elements
        if (nameText != null)
            nameText.text = name;
        
        if (typeText != null)
            typeText.text = $"{type}: {subType}";
        
        if (coordsText != null)
            coordsText.text = $"({position.x:F0}, {position.z:F0})";
        
        // Setup button
        if (selectButton != null)
        {
            selectButton.onClick.AddListener(OnClicked);
        }
    }
    
    void OnClicked()
    {
        if (parentUI != null)
        {
            parentUI.OnLocationSelected(locationType, locationId, locationName, worldPosition);
        }
    }
}

