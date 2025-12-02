using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace Controllers
{
public class WorldController : MonoBehaviour
{
    [Header("References")]
    public FerritineAPIClient apiClient;
    public ObjectPool objectPool;  // Referência ao gerenciador de pools
    
    [Header("Prefabs")]
    public GameObject stationPrefab;
    public GameObject vehiclePrefab;
    public GameObject agentPrefab;  //  Prefab para agentes/passageiros
    
    [Header("UI")]
    public TextMeshProUGUI debugText;
    
    // Parent containers para organização na hierarquia
    private Transform _stationsContainer;
    private Transform _vehiclesContainer;
    private Transform _agentsContainer;
    
    // Dicionários para rastrear GameObjects usando UUID strings
    // Mantemos como string porque JSON deserializa UUIDs como strings
    // e a conversão é feita via helper methods quando necessário
    private Dictionary<string, GameObject> _stations = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _vehicles = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> _agents = new Dictionary<string, GameObject>(); // NOVO: Dicionário de agentes
    
    void Awake()
    {
        // Tentar achar automaticamente o FerritineAPIClient na cena se não foi atribuído
        if (apiClient == null)
        {
            apiClient = UnityEngine.Object.FindAnyObjectByType<FerritineAPIClient>();
        }
        
        // Tentar achar automaticamente o ObjectPool na cena se não foi atribuído
        if (objectPool == null)
        {
            objectPool = UnityEngine.Object.FindAnyObjectByType<ObjectPool>();
        }
        
        // Criar containers para organização hierárquica
        CreateContainers();
    }

    void Start()
    {
        if (apiClient == null)
        {
            Debug.LogError("WorldController: apiClient is not assigned and none was found in the scene. Please assign it in the Inspector.");
            return;
        }
        
        if (objectPool == null)
        {
            Debug.LogError("WorldController: objectPool is not assigned and none was found in the scene. Please assign it in the Inspector.");
            return;
        }

        // Inicializar pools com prewarm
        InitializePools();

        // Inscrever no evento de atualização
        apiClient.OnWorldStateReceived += UpdateWorld;
        apiClient.OnError += HandleError;
    }
    
    /// <summary>
    /// Cria containers vazios para organizar objetos na hierarquia
    /// </summary>
    void CreateContainers()
    {
        _stationsContainer = new GameObject("StationsContainer").transform;
        _stationsContainer.SetParent(transform);
        
        _vehiclesContainer = new GameObject("VehiclesContainer").transform;
        _vehiclesContainer.SetParent(transform);
        
        _agentsContainer = new GameObject("AgentsContainer").transform;
        _agentsContainer.SetParent(transform);
    }
    
    /// <summary>
    /// Inicializa os pools com prewarm baseado em dados de seed_unity_ready.py
    /// Stations: 4, Vehicles: 5, Agents: 50
    /// </summary>
    void InitializePools()
    {
        // Pool de estações (raramente mudam)
        if (stationPrefab != null)
        {
            objectPool.InitializePool("stations", stationPrefab, _stationsContainer, prewarmCount: 10);
        }
        else
        {
            Debug.LogWarning("[WorldController] stationPrefab não atribuído. Pool de stations não será criado.");
        }
        
        // Pool de veículos (quantidade moderada)
        if (vehiclePrefab != null)
        {
            objectPool.InitializePool("vehicles", vehiclePrefab, _vehiclesContainer, prewarmCount: 15);
        }
        else
        {
            Debug.LogWarning("[WorldController] vehiclePrefab não atribuído. Pool de vehicles não será criado.");
        }
        
        // Pool de agentes (alta rotatividade)
        if (agentPrefab != null)
        {
            objectPool.InitializePool("agents", agentPrefab, _agentsContainer, prewarmCount: 50);
        }
        else
        {
            Debug.LogWarning("[WorldController] agentPrefab não atribuído. Pool de agents não será criado.");
        }
        
        Debug.Log("[WorldController] Todos os pools inicializados com sucesso.");
    }
    
    void UpdateWorld(WorldState state)
    {
        if (state == null) return;

        // Atualizar estações (checar null)
        if (state.stations != null)
            UpdateStations(state.stations);
        
        // Atualizar veículos
        if (state.vehicles != null)
            UpdateVehicles(state.vehicles);
        
        // NOVO: Atualizar agentes/passageiros
        if (state.agents != null)
            UpdateAgents(state.agents);
        
        // Atualizar UI de debug, com proteções adicionais
        if (debugText != null)
        {
            int stationCount = state.stations?.Count ?? 0;
            int vehicleCount = state.vehicles?.Count ?? 0;
            int agentCount = state.agents?.Count ?? 0; // NOVO
            int waiting = state.metrics != null ? state.metrics.total_passengers_waiting : 0;
            int inVehicles = state.metrics != null ? state.metrics.total_passengers_in_vehicles : 0;

            debugText.text = $"Tempo: {state.timestamp}\n" +
                             $"Estações: {stationCount}\n" +
                             $"Veículos: {vehicleCount}\n" +
                             $"Agentes: {agentCount}\n" + // NOVO
                             $"Passageiros em fila: {waiting}\n" +
                             $"Passageiros em veículos: {inVehicles}";
        }
    }
    
    void UpdateStations(List<StationData> stationData)
    {
        HashSet<string> activeStationIds = new HashSet<string>();
        
        foreach (var data in stationData)
        {
            activeStationIds.Add(data.id);
            
            if (!_stations.ContainsKey(data.id))
            {
                // Obter estação do pool ao invés de Instantiate
                GameObject station = objectPool.Get("stations");
                if (station == null)
                {
                    Debug.LogError("[WorldController] Falha ao obter station do pool.");
                    continue;
                }
                
                station.name = data.name;
                
                // Posicionar estação
                Vector3 position = new Vector3(data.x, 0, data.y);
                station.transform.position = position;
                
                _stations[data.id] = station;
                
                // Attach SelectableEntity component
                AttachSelectableEntity(station, SelectableEntity.EntityType.Station, data);
                
                // Configurar texto
                TextMeshPro text = station.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = data.name;
                }
            }
            
            // Atualizar estado (cor baseada em fila)
            GameObject stationObj = _stations[data.id];
            
            // Update existing station's data
            SelectableEntity selectable = stationObj.GetComponent<SelectableEntity>();
            if (selectable != null)
            {
                selectable.UpdateData(data);
            }
            Renderer stationRenderer = stationObj.GetComponent<Renderer>();
            
            if (stationRenderer != null)
            {
                // Verde se vazio, amarelo se médio, vermelho se cheio
                float queueRatio = (float)data.queue_length / data.max_queue;
                
                if (queueRatio < 0.3f)
                    stationRenderer.material.color = Color.green;
                else if (queueRatio < 0.7f)
                    stationRenderer.material.color = Color.yellow;
                else
                    stationRenderer.material.color = Color.red;
            }
            
            // Atualizar texto de fila
            TextMeshPro queueText = stationObj.GetComponentInChildren<TextMeshPro>();
            if (queueText != null)
            {
                queueText.text = $"{data.name}\n🚶 {data.queue_length}/{data.max_queue}";
            }
        }
        
        // Cleanup: retornar estações não utilizadas ao pool
        CleanupUnusedEntities(_stations, activeStationIds, "stations");
    }
    
    void UpdateVehicles(List<VehicleData> vehicleData)
    {
        if (vehicleData == null) return;

        HashSet<string> activeVehicleIds = new HashSet<string>();

        foreach (var v in vehicleData)
        {
            if (v == null || string.IsNullOrEmpty(v.id)) continue;
            
            activeVehicleIds.Add(v.id);

            // cria/recupera veículo
            if (!_vehicles.ContainsKey(v.id))
            {
                // Obter veículo do pool ao invés de Instantiate
                GameObject vehicleObj = objectPool.Get("vehicles");
                if (vehicleObj == null)
                {
                    Debug.LogError("[WorldController] Falha ao obter vehicle do pool.");
                    continue;
                }
                
                vehicleObj.name = $"Vehicle_{v.name}";

                // LIMPAR COMPONENTES DUPLICADOS (se o prefab já tiver)
                VehicleMover[] existingMovers = vehicleObj.GetComponents<VehicleMover>();
                if (existingMovers.Length > 1)
                {
                    Debug.LogWarning($"[Vehicle Cleanup] {v.name} tinha {existingMovers.Length} VehicleMovers! Removendo duplicatas.");
                    for (int i = 1; i < existingMovers.Length; i++)
                    {
                        Destroy(existingMovers[i]);
                    }
                }

                // garantir mover (apenas um)
                VehicleMover mover = vehicleObj.GetComponent<VehicleMover>();
                if (mover == null) mover = vehicleObj.AddComponent<VehicleMover>();
                mover.moveSpeed = 2f;      // velocidade reduzida
                mover.rotateSpeed = 180f;  // rotação suave
                mover.preserveY = true;

                // --- posição inicial ao instanciar: usar estação (se existir) + offset determinístico ---
                Vector3 spawnPos = Vector3.zero; // fallback

                if (!string.IsNullOrEmpty(v.current_station_id) && _stations.ContainsKey(v.current_station_id))
                {
                    Vector3 basePos = _stations[v.current_station_id].transform.position + Vector3.up * 0.5f;

                    // offset determinístico baseado no id (evita empilhamento)
                    int spawnHash = Mathf.Abs(v.id.GetHashCode());
                    float spawnSpacing = 0.6f;
                    float spawnAngle = (Mathf.Abs(spawnHash) % 360) * Mathf.Deg2Rad;
                    float spawnRadius = 0.3f + (Mathf.Abs(spawnHash) % 3) * 0.4f;
                    Vector3 spawnOffset = new Vector3(Mathf.Cos(spawnAngle), 0f, Mathf.Sin(spawnAngle)) * (spawnRadius * spawnSpacing);

                    spawnPos = basePos + spawnOffset;
                }

                vehicleObj.transform.position = spawnPos;
                mover.targetPosition = spawnPos; // inicializar targetPosition

                _vehicles[v.id] = vehicleObj;
                
                // Attach SelectableEntity component
                AttachSelectableEntity(vehicleObj, SelectableEntity.EntityType.Vehicle, v);

                Debug.Log($"[Vehicle Created] {v.id} ({v.name}) - inicial pos: {spawnPos}");
            }

            GameObject vehObj = _vehicles[v.id];
            
            // Update existing vehicle's data
            SelectableEntity selectable = vehObj.GetComponent<SelectableEntity>();
            if (selectable != null)
            {
                selectable.UpdateData(v);
            }

            // calcular posição alvo base (fallback mantém onde está)
            Vector3 baseTarget = vehObj.transform.position;

            // usar estação se disponível
            if (!string.IsNullOrEmpty(v.current_station_id) && _stations.ContainsKey(v.current_station_id))
            {
                baseTarget = _stations[v.current_station_id].transform.position + Vector3.up * 1.5f;
            }

            // offset determinístico para espalhar veículos
            int hash = Mathf.Abs(v.id.GetHashCode());
            
            // Usar índice no dicionário como seed adicional
            int vehicleIndex = 0;
            int idx = 0;
            foreach (var kvp in _vehicles)
            {
                if (kvp.Key == v.id) vehicleIndex = idx;
                idx++;
            }
            
            // Ângulo determinístico baseado em hash + índice
            float angle = ((hash + vehicleIndex * 137.5f) % 360f) * Mathf.Deg2Rad; // golden angle
            
            // Raio para espalhar
            float radius = 1.5f + ((hash % 5) * 0.8f); // 1.5 a 4.7 unidades
            
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius
            );

            Vector3 target = baseTarget + offset;

            // atribuir ao mover somente se mudou significativamente
            VehicleMover vm = vehObj.GetComponent<VehicleMover>();
            
            // Se não tiver VehicleMover, algo está errado - logar erro
            if (vm == null)
            {
                Debug.LogError($"[Vehicle Error] {v.id} ({v.name}) não tem VehicleMover! Isso não deveria acontecer.");
                continue;
            }

            // THRESHOLD para evitar micro-atualizações
            float minChange = 0.2f;
            float distance = Vector3.Distance(vm.targetPosition, target);
            
            if (distance > minChange)
            {
                Debug.Log($"[Vehicle Move] {v.id} ({v.name}) " +
                         $"station={v.current_station_id} " +
                         $"from {vm.targetPosition} to {target} " +
                         $"distance={distance:F2}");
                
                vm.targetPosition = target;
            }

            // Atualizar cor baseado em ocupação
            Renderer vehicleRenderer = vehObj.GetComponent<Renderer>();
            if (vehicleRenderer != null)
            {
                float occupancy = v.capacity > 0 ? (float)v.passengers / v.capacity : 0f;
                vehicleRenderer.material.color = Color.Lerp(Color.blue, Color.magenta, occupancy);
            }
            
            // Atualizar texto
            TextMeshPro text = vehObj.GetComponentInChildren<TextMeshPro>();
            if (text != null)
            {
                text.text = $"{v.name}\n👥 {v.passengers}/{v.capacity}";
            }
        }
        
        // Cleanup: retornar veículos não utilizados ao pool
        CleanupUnusedEntities(_vehicles, activeVehicleIds, "vehicles");
    }
    
    void UpdateAgents(List<AgentData> agentData)
    {
        if (agentData == null) return;

        HashSet<string> activeAgentIds = new HashSet<string>();

        foreach (var a in agentData)
        {
            if (a == null || string.IsNullOrEmpty(a.id)) continue;
            
            activeAgentIds.Add(a.id);

            // cria/recupera agente
            if (!_agents.ContainsKey(a.id))
            {
                // Obter agente do pool ao invés de Instantiate
                GameObject agentObj = objectPool.Get("agents");
                if (agentObj == null)
                {
                    Debug.LogError("[WorldController] Falha ao obter agent do pool.");
                    continue;
                }
                
                agentObj.name = $"Agent_{a.name}";

                // garantir mover (apenas um) - reutilizando VehicleMover para agentes
                VehicleMover mover = agentObj.GetComponent<VehicleMover>();
                if (mover == null) mover = agentObj.AddComponent<VehicleMover>();
                mover.moveSpeed = 1.2f;    // velocidade de caminhada suave (mais lento que veículos)
                mover.rotateSpeed = 180f;  // rotação suave (igual aos veículos)
                mover.preserveY = true;

                // --- posição inicial ao instanciar: usar localização ---
                Vector3 spawnPos = Vector3.zero; // fallback
                
                // altura desejada para agentes
                float agentHeight = 0.2f;
                
                // espaçamento determinístico entre agentes
                float spawnSpacing = 0.35f;
                
                if (a.location_type == "station" && !string.IsNullOrEmpty(a.location_id) && _stations.ContainsKey(a.location_id))
                {
                    Vector3 basePos = _stations[a.location_id].transform.position + Vector3.up * agentHeight;
                
                    // offset determinístico baseado no id (evita empilhamento)
                    int spawnHash = Mathf.Abs(a.id.GetHashCode());
                    float spawnAngle = (spawnHash % 360) * Mathf.Deg2Rad;
                    float spawnRadius = 0.2f + (spawnHash % 5) * 0.15f;
                    Vector3 spawnOffset = new Vector3(Mathf.Cos(spawnAngle), 0f, Mathf.Sin(spawnAngle)) * (spawnRadius * spawnSpacing);
                
                    spawnPos = basePos + spawnOffset;
                }
                else if (a.location_type == "vehicle" && !string.IsNullOrEmpty(a.location_id) && _vehicles.ContainsKey(a.location_id))
                {
                    // Se estiver em veículo, posicionar próximo ao veículo na mesma altura
                    Vector3 basePos = _vehicles[a.location_id].transform.position + Vector3.up * agentHeight;
                
                    int spawnHash = Mathf.Abs(a.id.GetHashCode());
                    float spawnAngle = (spawnHash % 360) * Mathf.Deg2Rad;
                    float spawnRadius = 0.25f + (spawnHash % 3) * 0.12f;
                    Vector3 spawnOffset = new Vector3(Mathf.Cos(spawnAngle), 0f, Mathf.Sin(spawnAngle)) * (spawnRadius * spawnSpacing);
                
                    spawnPos = basePos + spawnOffset;
                }
                
                agentObj.transform.position = spawnPos;
                mover.targetPosition = spawnPos; // inicializar targetPosition

                _agents[a.id] = agentObj;
                
                // Attach SelectableEntity component
                AttachSelectableEntity(agentObj, SelectableEntity.EntityType.Agent, a);

                Debug.Log($"[Agent Created] {a.id} ({a.name}) - inicial pos: {spawnPos}");
            }

            GameObject currentAgentObj = _agents[a.id];
            
            // Update existing agent's data
            SelectableEntity selectable = currentAgentObj.GetComponent<SelectableEntity>();
            if (selectable != null)
            {
                selectable.UpdateData(a);
            }

            // calcular posição alvo base (fallback mantém onde está)
            Vector3 baseTarget = currentAgentObj.transform.position;

            // usar localização se disponível
            if (a.location_type == "station" && !string.IsNullOrEmpty(a.location_id) && _stations.ContainsKey(a.location_id))
            {
                baseTarget = _stations[a.location_id].transform.position + Vector3.up * 0.2f;
            }
            else if (a.location_type == "vehicle" && !string.IsNullOrEmpty(a.location_id) && _vehicles.ContainsKey(a.location_id))
            {
                // Se estiver em veículo, seguir o veículo
                baseTarget = _vehicles[a.location_id].transform.position + Vector3.up * 0.3f;
            }

            // offset determinístico para espalhar agentes
            int hash = Mathf.Abs(a.id.GetHashCode());
            
            // Usar índice no dicionário como seed adicional
            int agentIndex = 0;
            int idx = 0;
            foreach (var kvp in _agents)
            {
                if (kvp.Key == a.id) agentIndex = idx;
                idx++;
            }
            
            // Ângulo determinístico baseado em hash + índice
            float angle = ((hash + agentIndex * 137.5f) % 360f) * Mathf.Deg2Rad; // golden angle
            
            // Raio menor para agentes (são menores que veículos)
            float radius = 0.5f + ((hash % 5) * 0.2f); // 0.5 a 1.3 unidades
            
            Vector3 offset = new Vector3(
                Mathf.Cos(angle) * radius,
                0f,
                Mathf.Sin(angle) * radius
            );

            Vector3 target = baseTarget + offset;

            // atribuir ao mover somente se mudou significativamente
            VehicleMover am = currentAgentObj.GetComponent<VehicleMover>();
            
            // Se não tiver VehicleMover, algo está errado - logar erro
            if (am == null)
            {
                Debug.LogError($"[Agent Error] {a.id} ({a.name}) não tem VehicleMover! Isso não deveria acontecer.");
                continue;
            }

            // THRESHOLD para movimento suave mas responsivo
            float minChange = 0.1f; // threshold menor = movimento mais suave
            float distance = Vector3.Distance(am.targetPosition, target);
            
            if (distance > minChange)
            {
                am.targetPosition = target;
            }

            // REMOVIDO: Alteração de cor para manter a cor original do prefab
            // A cor do agente agora permanece como configurada no prefab
            /*
            // Atualizar cor baseado em nível de energia
            Renderer agentRenderer = currentAgentObj.GetComponent<Renderer>();
            if (agentRenderer != null)
            {
                // Cor baseada em energia (verde = cheio, amarelo = médio, vermelho = baixo)
                float energyRatio = a.energy_level / 100f;
                if (energyRatio > 0.6f)
                    agentRenderer.material.color = Color.green;
                else if (energyRatio > 0.3f)
                    agentRenderer.material.color = Color.yellow;
                else
                    agentRenderer.material.color = Color.red;
            }
            */
        }
        
        // Cleanup: retornar agentes não utilizados ao pool
        CleanupUnusedEntities(_agents, activeAgentIds, "agents");
    }
    
    /// <summary>
    /// Remove entidades não utilizadas e retorna ao pool
    /// </summary>
    void CleanupUnusedEntities(Dictionary<string, GameObject> entityDict, HashSet<string> activeIds, string poolName)
    {
        List<string> toRemove = new List<string>();
        
        // Identificar entidades que não estão mais ativas
        foreach (var kvp in entityDict)
        {
            if (!activeIds.Contains(kvp.Key))
            {
                toRemove.Add(kvp.Key);
            }
        }
        
        // Retornar ao pool e remover do dicionário
        foreach (var id in toRemove)
        {
            GameObject obj = entityDict[id];
            objectPool.Return(poolName, obj);
            entityDict.Remove(id);
            Debug.Log($"[Cleanup] {poolName} - ID {id} retornado ao pool.");
        }
    }
    
    void HandleError(string error)
    {
        Debug.LogError($"API Error: {error}");
        
        if (debugText != null)
        {
            debugText.text = $"❌ Erro: {error}\nVerifique se a API está rodando!";
            debugText.color = Color.red;
        }
    }
    
    // ==================== HELPER METHODS FOR ENTITY SELECTION ====================
    
    /// <summary>
    /// Gets all current station data as a list.
    /// Used by TeleportSelectorUI.
    /// </summary>
    public List<StationData> GetAllStations()
    {
        List<StationData> stationList = new List<StationData>();
        
        foreach (var kvp in _stations)
        {
            GameObject stationObj = kvp.Value;
            SelectableEntity entity = stationObj.GetComponent<SelectableEntity>();
            if (entity != null && entity.stationData != null)
            {
                stationList.Add(entity.stationData);
            }
        }
        
        return stationList;
    }
    
    /// <summary>
    /// Gets all current building data as a list.
    /// Used by TeleportSelectorUI.
    /// </summary>
    public List<BuildingData> GetAllBuildings()
    {
        List<BuildingData> buildingList = new List<BuildingData>();
        
        // TODO: Implement buildings tracking similar to stations/vehicles/agents
        // For now, return empty list
        
        return buildingList;
    }
    
    /// <summary>
    /// Gets GameObject for a specific location (station or building).
    /// Used by TeleportSelectorUI for highlighting.
    /// </summary>
    public GameObject GetLocationGameObject(string locationType, string locationId)
    {
        if (locationType == "station" && _stations.ContainsKey(locationId))
        {
            return _stations[locationId];
        }
        
        // TODO: Add building support when implemented
        
        return null;
    }
    
    /// <summary>
    /// Gets GameObject for a specific agent.
    /// Used by TeleportSelectorUI for particle effects.
    /// </summary>
    public GameObject GetAgentGameObject(string agentId)
    {
        if (_agents.ContainsKey(agentId))
        {
            return _agents[agentId];
        }
        
        return null;
    }
    
    /// <summary>
    /// Attaches SelectableEntity component to a GameObject.
    /// </summary>
    void AttachSelectableEntity(GameObject obj, SelectableEntity.EntityType entityType, object data)
    {
        SelectableEntity selectable = obj.GetComponent<SelectableEntity>();
        if (selectable == null)
        {
            selectable = obj.AddComponent<SelectableEntity>();
        }
        
        selectable.entityType = entityType;
        
        // Assign data based on type
        switch (entityType)
        {
            case SelectableEntity.EntityType.Station:
                selectable.stationData = data as StationData;
                break;
            case SelectableEntity.EntityType.Vehicle:
                selectable.vehicleData = data as VehicleData;
                break;
            case SelectableEntity.EntityType.Agent:
                selectable.agentData = data as AgentData;
                break;
            case SelectableEntity.EntityType.Building:
                selectable.buildingData = data as BuildingData;
                break;
        }
        
        // Set layer for raycasting - with validation
        int selectableLayerIndex = LayerMask.NameToLayer("Selectable");
        
        if (selectableLayerIndex == -1)
        {
            Debug.LogError("[WorldController] ⚠️ ATENÇÃO: Layer 'Selectable' não encontrada! " +
                          "Crie a layer em Edit → Project Settings → Tags and Layers. " +
                          "Usando layer Default como fallback (entidades NÃO serão clicáveis!)");
            obj.layer = 0; // Default layer
        }
        else
        {
            obj.layer = selectableLayerIndex;
        }
    }
    
    void OnDestroy()
    {
        // Limpar inscrições de eventos
        if (apiClient != null)
        {
            apiClient.OnWorldStateReceived -= UpdateWorld;
            apiClient.OnError -= HandleError;
        }
        
        // Log de estatísticas dos pools antes de destruir
        if (objectPool != null)
        {
            objectPool.LogPoolStats("stations");
            objectPool.LogPoolStats("vehicles");
            objectPool.LogPoolStats("agents");
        }
    }
    }
}
