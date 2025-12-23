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
    
    [Header("List References")]
    [SerializeField] private Transform destinationsContent;
    [SerializeField] private GameObject destinationItemPrefab;
    
    [Header("UI Controls")]
    [SerializeField] private TMP_Dropdown filterDropdown;
    [SerializeField] private TMP_InputField searchInput;
    
    [Header("Footer Buttons")]
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    
    [Header("Preview")]
    public TextMeshProUGUI previewText;
    
    // ==================== DESTINATION DATA ====================
    private class DestinationData
    {
        public string id;
        public string name;
        public Vector3 position;
        public string type; // "station" | "building"
    }
    
    // ==================== STATE ====================
    private List<DestinationData> allDestinations = new List<DestinationData>();
    private DestinationData selectedDestination;
    private GameObject selectedItemGO;
    
    // Entidade sendo teleportada
    private AgentData currentAgent;
    private VehicleData currentVehicle;
    private SelectableEntity.EntityType currentEntityType;
    
    // Legacy state
    private string selectedLocationType;
    private string selectedLocationId;
    private GameObject selectedLocationObject;
    private GameObject previewParticle;
    
    // API
    private string apiBaseUrl = "http://localhost:8000";

    // Colors for selection
    private readonly Color32 defaultItemColor = new Color32(58, 58, 58, 255);
    private readonly Color32 selectedItemColor = new Color32(80, 80, 80, 255);
    
    // ==================== VALIDATION ====================
    private enum ValidationType { Agent, Destination }
    
    void Awake()
    {
        if (confirmButton != null)
            confirmButton.onClick.AddListener(OnConfirmClicked);
        
        if (cancelButton != null)
            cancelButton.onClick.AddListener(OnCancelClicked);
        
        if (filterDropdown != null)
            filterDropdown.onValueChanged.AddListener(OnFilterChanged);
        
        if (searchInput != null)
            searchInput.onValueChanged.AddListener(OnSearchChanged);
        
        if (destinationsContent == null && contentContainer != null)
            destinationsContent = contentContainer;
        
        if (destinationItemPrefab == null && locationItemPrefab != null)
            destinationItemPrefab = locationItemPrefab;
        
        if (panel != null)
            panel.SetActive(false);
    }
    
    // ==================== OPEN METHODS ====================
    
    public void Open(AgentData agent)
    {
        if (agent == null) return;
        currentAgent = agent;
        currentVehicle = null;
        currentEntityType = SelectableEntity.EntityType.Agent;
        OpenInternal();
    }
    
    public void Open(VehicleData vehicle)
    {
        if (vehicle == null) return;
        currentVehicle = vehicle;
        currentAgent = null;
        currentEntityType = SelectableEntity.EntityType.Vehicle;
        OpenInternal();
    }
    
    private void OpenInternal()
    {
        selectedLocationType = null;
        selectedLocationId = null;
        selectedDestination = null;
        selectedItemGO = null;
        
        if (panel != null)
            panel.SetActive(true);
        
        if (previewText != null)
            previewText.text = "Selecione um destino";
        
        AddTestDestinations();
        AudioManager.PlayUISound("panel_open");
        
        string entityName = currentAgent?.name ?? currentVehicle?.name ?? "?";
        Debug.Log($"[TeleportSelectorUI] Opened for {currentEntityType}: {entityName}");
    }
    
    public void ShowForAgent(AgentData agent) => Open(agent);
    
    // ==================== CLOSE ====================
    
    public void Close()
    {
        if (selectedLocationObject != null)
        {
            SelectableEntity entity = selectedLocationObject.GetComponent<SelectableEntity>();
            if (entity != null) entity.Unhighlight();
            selectedLocationObject = null;
        }
        
        if (previewParticle != null)
        {
            ParticleEffectPool.Instance?.Return("teleport_preview", previewParticle);
            previewParticle = null;
        }
        
        CameraController cameraController = Camera.main?.GetComponent<CameraController>();
        if (cameraController != null) cameraController.StopPreview();
        
        selectedDestination = null;
        selectedItemGO = null;
        allDestinations.Clear();
        currentAgent = null;
        currentVehicle = null;
        selectedLocationType = null;
        selectedLocationId = null;
        
        if (panel != null)
            panel.SetActive(false);
        
        AudioManager.PlayUISound("panel_close");
        UIManager.Instance?.HideTeleportSelector();
        Debug.Log("[TeleportSelectorUI] Closed");
    }
    
    // ==================== TEST DATA ====================
    
    void AddTestDestinations()
    {
        allDestinations.Clear();
        
        allDestinations.Add(new DestinationData {
            id = "station_01", name = "Central Station",
            position = new Vector3(120, 0, 450), type = "station"
        });
        allDestinations.Add(new DestinationData {
            id = "station_02", name = "Estação Jabaquara",
            position = new Vector3(0, 0, 0), type = "station"
        });
        allDestinations.Add(new DestinationData {
            id = "building_01", name = "Prefeitura",
            position = new Vector3(300, 0, 210), type = "building"
        });
        allDestinations.Add(new DestinationData {
            id = "building_02", name = "Hospital Municipal",
            position = new Vector3(450, 0, 180), type = "building"
        });
        allDestinations.Add(new DestinationData {
            id = "station_03", name = "Terminal Rodoviário",
            position = new Vector3(200, 0, 300), type = "station"
        });
        
        Debug.Log($"[TeleportSelectorUI] Added {allDestinations.Count} test destinations");
        RefreshList();
    }
    
    // ==================== REFRESH LIST ====================
    
    void RefreshList()
    {
        Transform content = destinationsContent != null ? destinationsContent : contentContainer;
        if (content == null)
        {
            Debug.LogError("[TeleportSelectorUI] No content container found!");
            return;
        }
        
        GameObject prefab = destinationItemPrefab != null ? destinationItemPrefab : locationItemPrefab;
        if (prefab == null)
        {
            Debug.LogError("[TeleportSelectorUI] No destination item prefab found!");
            return;
        }
        
        foreach (Transform child in content)
            Destroy(child.gameObject);
        
        foreach (var dest in allDestinations)
        {
            GameObject itemGO = Instantiate(prefab, content);
            itemGO.SetActive(true);
            
            DestinationItemVisual itemVisual = itemGO.GetComponent<DestinationItemVisual>();
            if (itemVisual == null)
            {
                itemVisual = itemGO.AddComponent<DestinationItemVisual>();
                itemVisual.CreateBorderIfNeeded();
            }
            
            DestinationData capturedDest = dest;
            GameObject capturedGO = itemGO;
            string coordsStr = $"({dest.position.x:F0}, {dest.position.y:F0}, {dest.position.z:F0})";
            itemVisual.Setup(dest.name, coordsStr, () => OnDestinationSelected(capturedDest, capturedGO));
            
            Transform textGroup = itemGO.transform.Find("TextGroup");
            if (textGroup != null)
            {
                var nameText = textGroup.Find("NameText")?.GetComponent<TextMeshProUGUI>();
                var coordsText = textGroup.Find("CoordsText")?.GetComponent<TextMeshProUGUI>();
                if (nameText != null) nameText.text = dest.name;
                if (coordsText != null) coordsText.text = coordsStr;
            }
            
            Button btn = itemGO.GetComponent<Button>();
            if (btn != null) btn.enabled = false;
        }
        
        Debug.Log($"[TeleportSelectorUI] Refreshed list with {allDestinations.Count} items");
    }
    
    // ==================== SELECTION ====================
    
    void OnDestinationSelected(DestinationData dest, GameObject itemGO)
    {
        if (selectedItemGO != null)
        {
            DestinationItemVisual prevVisual = selectedItemGO.GetComponent<DestinationItemVisual>();
            if (prevVisual != null) prevVisual.SetSelected(false);
        }
        
        selectedDestination = dest;
        selectedItemGO = itemGO;
        selectedLocationType = dest.type;
        selectedLocationId = dest.id;
        
        DestinationItemVisual itemVisual = itemGO.GetComponent<DestinationItemVisual>();
        if (itemVisual != null) itemVisual.SetSelected(true);
        
        if (previewText != null)
        {
            string typeLabel = dest.type == "station" ? "Estação" : "Edifício";
            previewText.text = $"Destino: {dest.name} ({typeLabel})";
        }
        
        AudioManager.PlayUISound("button_select");
        HighlightLocation(dest.type, dest.id);
        
        CameraController cameraController = Camera.main?.GetComponent<CameraController>();
        if (cameraController != null) cameraController.PreviewLocation(dest.position);
        
        SpawnPreviewParticle(dest.position);
        Debug.Log($"[TeleportSelectorUI] Selected: {dest.name} ({dest.type}) at {dest.position}");
    }
    
    // ==================== CONFIRM / TELEPORT ====================
    
    void OnConfirmClicked()
    {
        if (currentAgent == null && currentVehicle == null)
        {
            ShowValidationError("Nenhuma entidade selecionada", ValidationType.Agent);
            return;
        }
        
        if (selectedDestination == null && string.IsNullOrEmpty(selectedLocationId))
        {
            ShowValidationError("Selecione um destino na lista", ValidationType.Destination);
            return;
        }
        
        if (currentVehicle != null)
        {
            string error = ValidateVehicleDestination(currentVehicle, selectedDestination);
            if (!string.IsNullOrEmpty(error))
            {
                ShowValidationError(error, ValidationType.Destination);
                return;
            }
        }
        
        ExecuteTeleportWithBackendSync();
    }
    
    string ValidateVehicleDestination(VehicleData vehicle, DestinationData dest)
    {
        if (vehicle == null || dest == null) return "Dados inválidos";
        
        string vType = vehicle.vehicle_type?.ToLower() ?? "";
        string dType = dest.type?.ToLower() ?? "";
        string dName = dest.name?.ToLower() ?? "";
        
        // Trens: só estações de trem
        if (vType.Contains("train") || vType.Contains("trem") || vType.Contains("metro"))
        {
            if (dType != "station") return "Trens só podem ir para estações";
            if (dName.Contains("ônibus") || dName.Contains("bus")) return "Trens não podem ir para pontos de ônibus";
        }
        
        // Ônibus: só estações/terminais
        if (vType.Contains("bus") || vType.Contains("ônibus"))
        {
            if (dType != "station") return "Ônibus só podem ir para estações/terminais";
        }
        
        // Barcos: precisam de portos
        if (vType.Contains("boat") || vType.Contains("ferry") || vType.Contains("barco"))
        {
            if (!dName.Contains("porto") && !dName.Contains("pier") && !dName.Contains("marina"))
                return "Embarcações precisam de porto ou atracadouro";
        }
        
        // Carros: não podem ir para trilhos
        if (vType.Contains("car") || vType.Contains("carro") || vType.Contains("taxi"))
        {
            if (dName.Contains("trilho") || dName.Contains("ferrovia") || dName.Contains("metrô"))
                return "Veículos rodoviários não podem ir para trilhos";
        }
        
        return null;
    }
    
    /// <summary>
    /// Executa o teleporte com sincronização do backend.
    /// Envia requisição para backend antes de atualizar o frontend.
    /// </summary>
    void ExecuteTeleportWithBackendSync()
    {
        string locType = selectedDestination?.type ?? selectedLocationType;
        string locId = selectedDestination?.id ?? selectedLocationId;
        string locName = selectedDestination?.name ?? "destino";
        Vector3 targetPos = selectedDestination?.position ?? Vector3.zero;
        
        // Validar entidade
        string entityId = currentAgent?.id ?? currentVehicle?.id;
        string entityName = currentAgent?.name ?? currentVehicle?.name ?? "entidade";
        
        if (string.IsNullOrEmpty(entityId))
        {
            ShowValidationError("Entidade inválida", ValidationType.Agent);
            return;
        }
        
        // Encontrar GameObject da entidade
        WorldController worldController = FindAnyObjectByType<WorldController>();
        if (worldController == null)
        {
            ToastNotificationManager.ShowError("WorldController não encontrado!");
            return;
        }
        
        GameObject entityObj = null;
        if (currentAgent != null)
        {
            entityObj = worldController.GetAgentGameObject(currentAgent.id);
        }
        else if (currentVehicle != null)
        {
            entityObj = worldController.GetVehicleGameObject(currentVehicle.id);
        }
        
        if (entityObj == null)
        {
            ToastNotificationManager.ShowError($"'{entityName}' não encontrado na cena!");
            return;
        }
        
        // Desabilitar botões enquanto aguarda resposta
        if (confirmButton != null) confirmButton.interactable = false;
        if (cancelButton != null) cancelButton.interactable = false;
        
        // Tentar enviar requisição para backend
        BackendTeleportManager teleportManager = BackendTeleportManager.Instance;
        if (teleportManager != null)
        {
            Debug.Log($"[TeleportSelectorUI] Enviando teleporte para backend: {entityId} -> {locType}/{locId}");
            teleportManager.TeleportAgent(entityId, locType, locId, 
                (success, message) => OnTeleportBackendResponse(success, message, entityObj, targetPos, locName, entityName));
        }
        else
        {
            // FALLBACK: BackendTeleportManager não encontrado - executar teleporte local
            Debug.LogWarning("[TeleportSelectorUI] BackendTeleportManager não encontrado. Executando teleporte local.");
            
            if (confirmButton != null) confirmButton.interactable = true;
            if (cancelButton != null) cancelButton.interactable = true;
            
            // Executar teleporte local diretamente
            StartCoroutine(ExecuteTeleportAnimation(entityObj, targetPos, locName, entityName));
        }
    }
    
    /// <summary>
    /// Callback quando o backend responde ao teleporte.
    /// </summary>
    void OnTeleportBackendResponse(bool success, string message, GameObject entityObj, 
                                     Vector3 targetPos, string destName, string entityName)
    {
        if (confirmButton != null) confirmButton.interactable = true;
        if (cancelButton != null) cancelButton.interactable = true;
        
        if (success)
        {
            // Backend confirmou - executar animação local
            Debug.Log($"[TeleportSelectorUI] Backend confirmou teleporte de {entityName}");
            StartCoroutine(ExecuteTeleportAnimation(entityObj, targetPos, destName, entityName));
        }
        else
        {
            // FALLBACK: Erro no backend - executar teleporte local mesmo assim
            Debug.LogWarning($"[TeleportSelectorUI] Backend falhou ({message}), executando teleporte local.");
            StartCoroutine(ExecuteTeleportAnimation(entityObj, targetPos, destName, entityName));
        }
    }
    
    /// <summary>
    /// Anima o teleporte localmente após confirmação do backend.
    /// </summary>
    IEnumerator ExecuteTeleportAnimation(GameObject entityObj, Vector3 targetPos, string destName, string entityName)
    {
        // Spawn efeito de desaparecimento
        SpawnTeleportEffect("teleport_despawn", entityObj.transform.position);
        AudioManager.PlayUISound("teleport_woosh");
        
        // Aguardar animação
        yield return new WaitForSeconds(0.3f);
        
        // Mover entidade
        Vector3 finalPos = CalculateTeleportPosition(targetPos, selectedDestination?.type ?? selectedLocationType);
        entityObj.transform.position = finalPos;
        
        VehicleMover mover = entityObj.GetComponent<VehicleMover>();
        if (mover != null) mover.targetPosition = finalPos;
        
        // Spawn efeito de aparecimento
        SpawnTeleportEffect("teleport_spawn", finalPos);
        AudioManager.PlayUISound("teleport_arrive");
        
        // Atualizar dados locais
        SelectableEntity selectable = entityObj.GetComponent<SelectableEntity>();
        if (selectable != null)
        {
            if (selectable.agentData != null)
            {
                selectable.agentData.location_type = selectedDestination?.type ?? selectedLocationType;
                selectable.agentData.location_id = selectedDestination?.id ?? selectedLocationId;
            }
            else if (selectable.vehicleData != null)
            {
                selectable.vehicleData.current_station_id = selectedDestination?.id ?? selectedLocationId;
            }
        }
        
        string entityType = currentAgent != null ? "Agente" : "Veículo";
        ToastNotificationManager.ShowSuccess($"{entityType} '{entityName}' teleportado para {destName}!");
        
        Debug.Log($"[TeleportSelectorUI] ✓ Teleport complete: {entityName} → {destName}");
        Close();
    }

    void ExecuteTeleport()
    {
        string locType = selectedDestination?.type ?? selectedLocationType;
        string locId = selectedDestination?.id ?? selectedLocationId;
        string locName = selectedDestination?.name ?? "destino";
        Vector3 targetPos = selectedDestination?.position ?? Vector3.zero;
        
        WorldController worldController = FindAnyObjectByType<WorldController>();
        if (worldController == null)
        {
            ToastNotificationManager.ShowError("WorldController não encontrado!");
            return;
        }
        
        GameObject entityObj = null;
        string entityName = "";
        
        if (currentAgent != null)
        {
            entityObj = worldController.GetAgentGameObject(currentAgent.id);
            entityName = currentAgent.name;
        }
        else if (currentVehicle != null)
        {
            entityObj = worldController.GetVehicleGameObject(currentVehicle.id);
            entityName = currentVehicle.name;
        }
        
        if (entityObj == null)
        {
            ToastNotificationManager.ShowError($"'{entityName}' não encontrado na cena!");
            return;
        }
        
        Vector3 finalPos = CalculateTeleportPosition(targetPos, locType);
        
        SpawnTeleportEffect("teleport_despawn", entityObj.transform.position);
        AudioManager.PlayUISound("teleport_woosh");
        
        StartCoroutine(TeleportWithDelay(entityObj, finalPos, locName, entityName));
    }
    
    Vector3 CalculateTeleportPosition(Vector3 basePos, string destType)
    {
        float offsetX = Random.Range(-2f, 2f);
        float offsetZ = Random.Range(-2f, 2f);
        float heightOffset = destType == "station" ? 0.1f : 0f;
        return new Vector3(basePos.x + offsetX, basePos.y + heightOffset, basePos.z + offsetZ);
    }
    
    IEnumerator TeleportWithDelay(GameObject entityObj, Vector3 targetPos, string destName, string entityName)
    {
        yield return new WaitForSeconds(0.3f);
        
        entityObj.transform.position = targetPos;
        
        VehicleMover mover = entityObj.GetComponent<VehicleMover>();
        if (mover != null) mover.targetPosition = targetPos;
        
        SpawnTeleportEffect("teleport_spawn", targetPos);
        
        SelectableEntity selectable = entityObj.GetComponent<SelectableEntity>();
        if (selectable != null)
        {
            if (selectable.agentData != null)
            {
                selectable.agentData.location_type = selectedDestination?.type ?? selectedLocationType;
                selectable.agentData.location_id = selectedDestination?.id ?? selectedLocationId;
            }
            else if (selectable.vehicleData != null)
            {
                selectable.vehicleData.current_station_id = selectedDestination?.id ?? selectedLocationId;
            }
        }
        
        string entityType = currentAgent != null ? "Agente" : "Veículo";
        ToastNotificationManager.ShowSuccess($"{entityType} '{entityName}' teleportado para {destName}!");
        AudioManager.PlayUISound("teleport_arrive");
        
        Debug.Log($"[TeleportSelectorUI] Teleport complete: {entityName} → {destName}");
        Close();
    }
    
    void SpawnTeleportEffect(string effectName, Vector3 position)
    {
        if (ParticleEffectPool.Instance != null)
            ParticleEffectPool.Instance.Play(effectName, position);
        else
            Debug.Log($"[TeleportSelectorUI] Particle '{effectName}' at {position} (no pool)");
    }
    
    // ==================== VALIDATION FEEDBACK ====================
    
    void ShowValidationError(string message, ValidationType type)
    {
        ToastNotificationManager.ShowWarning(message);
        AudioManager.PlayUISound("toast_warning");
        
        if (type == ValidationType.Destination)
        {
            StartCoroutine(FlashDestinationList());
            if (previewText != null)
            {
                previewText.text = "⚠️ Selecione um destino acima";
                previewText.color = new Color(1f, 0.6f, 0.2f);
                StartCoroutine(ResetPreviewTextColor());
            }
        }
        else if (type == ValidationType.Agent)
        {
            if (confirmButton != null)
                StartCoroutine(FlashButton(confirmButton, new Color(0.8f, 0.2f, 0.2f)));
        }
        
        Debug.LogWarning($"[TeleportSelectorUI] Validation failed: {message}");
    }
    
    IEnumerator FlashDestinationList()
    {
        Transform content = destinationsContent != null ? destinationsContent : contentContainer;
        if (content == null) yield break;
        
        for (int i = 0; i < 2; i++)
        {
            foreach (Transform child in content)
            {
                child.localScale = Vector3.one * 1.02f;
                var img = child.GetComponent<Image>();
                if (img != null) img.color = new Color(0.4f, 0.3f, 0.1f);
            }
            yield return new WaitForSeconds(0.15f);
            
            foreach (Transform child in content)
            {
                child.localScale = Vector3.one;
                var img = child.GetComponent<Image>();
                if (img != null) img.color = new Color32(45, 45, 48, 255);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
    
    IEnumerator FlashButton(Button btn, Color flashColor)
    {
        var img = btn.GetComponent<Image>();
        if (img == null) yield break;
        
        var original = img.color;
        float dur = 0.15f;
        
        for (int i = 0; i < 2; i++)
        {
            float t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                img.color = Color.Lerp(original, flashColor, t / dur);
                yield return null;
            }
            t = 0f;
            while (t < dur)
            {
                t += Time.deltaTime;
                img.color = Color.Lerp(flashColor, original, t / dur);
                yield return null;
            }
        }
        img.color = original;
    }
    
    IEnumerator ResetPreviewTextColor()
    {
        yield return new WaitForSeconds(2f);
        if (previewText != null)
        {
            previewText.color = Color.white;
            if (selectedDestination == null) previewText.text = "Selecione um destino";
        }
    }
    
    // ==================== CANCEL ====================
    
    void OnCancelClicked() => Close();
    
    // ==================== FILTER / SEARCH ====================
    
    void OnFilterChanged(int value)
    {
        RefreshFilteredList(value, searchInput?.text ?? "");
    }
    
    void OnSearchChanged(string searchText)
    {
        int filterValue = filterDropdown?.value ?? 0;
        RefreshFilteredList(filterValue, searchText);
    }
    
    void RefreshFilteredList(int filterValue, string searchText)
    {
        Transform content = destinationsContent != null ? destinationsContent : contentContainer;
        if (content == null) return;
        
        GameObject prefab = destinationItemPrefab != null ? destinationItemPrefab : locationItemPrefab;
        if (prefab == null) return;
        
        foreach (Transform child in content)
            Destroy(child.gameObject);
        
        string searchLower = searchText?.ToLower() ?? "";
        
        foreach (var dest in allDestinations)
        {
            bool passesTypeFilter = filterValue == 0 ||
                (filterValue == 1 && dest.type == "station") ||
                (filterValue == 2 && dest.type == "building");
            
            bool passesSearchFilter = string.IsNullOrEmpty(searchLower) ||
                dest.name.ToLower().Contains(searchLower);
            
            if (!passesTypeFilter || !passesSearchFilter) continue;
            
            GameObject itemGO = Instantiate(prefab, content);
            itemGO.SetActive(true);
            
            DestinationItemVisual itemVisual = itemGO.GetComponent<DestinationItemVisual>();
            if (itemVisual == null)
            {
                itemVisual = itemGO.AddComponent<DestinationItemVisual>();
                itemVisual.CreateBorderIfNeeded();
            }
            
            DestinationData capturedDest = dest;
            GameObject capturedGO = itemGO;
            string coordsStr = $"({dest.position.x:F0}, {dest.position.y:F0}, {dest.position.z:F0})";
            itemVisual.Setup(dest.name, coordsStr, () => OnDestinationSelected(capturedDest, capturedGO));
            
            bool isSelected = selectedDestination != null && selectedDestination.id == dest.id;
            itemVisual.SetSelected(isSelected);
            if (isSelected) selectedItemGO = itemGO;
            
            Transform textGroup = itemGO.transform.Find("TextGroup");
            if (textGroup != null)
            {
                var nameText = textGroup.Find("NameText")?.GetComponent<TextMeshProUGUI>();
                var coordsText = textGroup.Find("CoordsText")?.GetComponent<TextMeshProUGUI>();
                if (nameText != null) nameText.text = dest.name;
                if (coordsText != null) coordsText.text = coordsStr;
            }
            
            Button btn = itemGO.GetComponent<Button>();
            if (btn != null) btn.enabled = false;
        }
    }
    
    // ==================== HELPER METHODS ====================
    
    void HighlightLocation(string locationType, string locationId)
    {
        WorldController worldController = FindAnyObjectByType<WorldController>();
        if (worldController == null) return;
        
        GameObject locationObj = worldController.GetLocationGameObject(locationType, locationId);
        if (locationObj != null)
        {
            SelectableEntity entity = locationObj.GetComponent<SelectableEntity>();
            if (entity != null)
            {
                if (selectedLocationObject != null)
                {
                    SelectableEntity prevEntity = selectedLocationObject.GetComponent<SelectableEntity>();
                    if (prevEntity != null) prevEntity.Unhighlight();
                }
                entity.Highlight();
                selectedLocationObject = locationObj;
            }
        }
    }
    
    void SpawnPreviewParticle(Vector3 position)
    {
        if (previewParticle != null)
        {
            ParticleEffectPool.Instance?.Return("teleport_preview", previewParticle);
            previewParticle = null;
        }
        previewParticle = ParticleEffectPool.Instance?.Get("teleport_preview", position, Quaternion.identity);
    }
    
    public void OnLocationSelected(string locationType, string locationId, string locationName, Vector3 worldPosition)
    {
        selectedLocationType = locationType;
        selectedLocationId = locationId;
        
        if (previewText != null)
            previewText.text = $"Destino: {locationName} ({locationType})";
        
        HighlightLocation(locationType, locationId);
        
        CameraController cameraController = Camera.main?.GetComponent<CameraController>();
        if (cameraController != null) cameraController.PreviewLocation(worldPosition);
        
        SpawnPreviewParticle(worldPosition);
        AudioManager.PlayUISound("button_select");
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
        
        if (nameText != null) nameText.text = name;
        if (typeText != null) typeText.text = $"{type}: {subType}";
        if (coordsText != null) coordsText.text = $"({position.x:F0}, {position.z:F0})";
        
        if (selectButton != null)
            selectButton.onClick.AddListener(OnClicked);
    }
    
    void OnClicked()
    {
        if (parentUI != null)
            parentUI.OnLocationSelected(locationType, locationId, locationName, worldPosition);
    }
}

