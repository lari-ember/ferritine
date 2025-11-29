using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class WorldController : MonoBehaviour
{
    [Header("References")]
    public FerritineAPIClient apiClient;
    
    [Header("Prefabs")]
    public GameObject stationPrefab;
    public GameObject vehiclePrefab;
    public GameObject agentPrefab;  // NOVO: Prefab para agentes/passageiros
    
    [Header("UI")]
    public TextMeshProUGUI debugText;
    
    // Dicionários para rastrear GameObjects usando UUID strings
    // Mantemos como string porque JSON deserializa UUIDs como strings
    // e a conversão é feita via helper methods quando necessário
    private Dictionary<string, GameObject> stations = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> vehicles = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> agents = new Dictionary<string, GameObject>(); // NOVO: Dicionário de agentes
    
    void Awake()
    {
        // Tentar achar automaticamente o FerritineAPIClient na cena se não foi atribuído
        if (apiClient == null)
        {
            apiClient = UnityEngine.Object.FindAnyObjectByType<FerritineAPIClient>();
        }
    }

    void Start()
    {
        if (apiClient == null)
        {
            Debug.LogError("WorldController: apiClient is not assigned and none was found in the scene. Please assign it in the Inspector.");
            return;
        }

        // Inscrever no evento de atualização
        apiClient.OnWorldStateReceived += UpdateWorld;
        apiClient.OnError += HandleError;
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
        foreach (var data in stationData)
        {
            if (!stations.ContainsKey(data.id))
            {
                // Criar nova estação
                Vector3 position = new Vector3(data.x, 0, data.y);
                GameObject station = Instantiate(stationPrefab, position, Quaternion.identity);
                station.name = data.name;
                stations[data.id] = station;
                
                // Configurar texto
                TextMeshPro text = station.GetComponentInChildren<TextMeshPro>();
                if (text != null)
                {
                    text.text = data.name;
                }
            }
            
            // Atualizar estado (cor baseada em fila)
            GameObject stationObj = stations[data.id];
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
    }
    
    void UpdateVehicles(List<VehicleData> vehicleData)
    {
        if (vehicleData == null) return;

        if (vehiclePrefab == null)
        {
            Debug.LogError("WorldController: vehiclePrefab is not assigned. Please assign it in the Inspector.");
            return;
        }

        foreach (var v in vehicleData)
        {
            if (v == null || string.IsNullOrEmpty(v.id)) continue;

            // cria/recupera veículo
            if (!vehicles.ContainsKey(v.id))
            {
                GameObject obj = Instantiate(vehiclePrefab);
                obj.name = $"Vehicle_{v.name}";

                // LIMPAR COMPONENTES DUPLICADOS (se o prefab já tiver)
                VehicleMover[] existingMovers = obj.GetComponents<VehicleMover>();
                if (existingMovers.Length > 1)
                {
                    Debug.LogWarning($"[Vehicle Cleanup] {v.name} tinha {existingMovers.Length} VehicleMovers! Removendo duplicatas.");
                    for (int i = 1; i < existingMovers.Length; i++)
                    {
                        Destroy(existingMovers[i]);
                    }
                }

                // garantir mover (apenas um)
                VehicleMover mover = obj.GetComponent<VehicleMover>();
                if (mover == null) mover = obj.AddComponent<VehicleMover>();
                mover.moveSpeed = 2f;      // velocidade reduzida
                mover.rotateSpeed = 180f;  // rotação suave
                mover.preserveY = true;

                // --- posição inicial ao instanciar: usar estação (se existir) + offset determinístico ---
                Vector3 spawnPos = Vector3.zero; // fallback

                if (!string.IsNullOrEmpty(v.current_station_id) && stations.ContainsKey(v.current_station_id))
                {
                    Vector3 basePos = stations[v.current_station_id].transform.position + Vector3.up * 0.5f;

                    // offset determinístico baseado no id (evita empilhamento)
                    int spawnHash = Mathf.Abs(v.id.GetHashCode());
                    float spawnSpacing = 0.6f;
                    float spawnAngle = (Mathf.Abs(spawnHash) % 360) * Mathf.Deg2Rad;
                    float spawnRadius = 0.3f + (Mathf.Abs(spawnHash) % 3) * 0.4f;
                    Vector3 spawnOffset = new Vector3(Mathf.Cos(spawnAngle), 0f, Mathf.Sin(spawnAngle)) * spawnRadius * spawnSpacing;

                    spawnPos = basePos + spawnOffset;
                }

                obj.transform.position = spawnPos;
                mover.targetPosition = spawnPos; // inicializar targetPosition

                vehicles[v.id] = obj;

                Debug.Log($"[Vehicle Created] {v.id} ({v.name}) - inicial pos: {spawnPos}");
            }

            GameObject vehObj = vehicles[v.id];

            // calcular posição alvo base (fallback mantém onde está)
            Vector3 baseTarget = vehObj.transform.position;

            // usar estação se disponível
            if (!string.IsNullOrEmpty(v.current_station_id) && stations.ContainsKey(v.current_station_id))
            {
                baseTarget = stations[v.current_station_id].transform.position + Vector3.up * 1.5f;
            }

            // offset determinístico para espalhar veículos
            int hash = Mathf.Abs(v.id.GetHashCode());
            
            // Usar índice no dicionário como seed adicional
            int vehicleIndex = 0;
            int idx = 0;
            foreach (var kvp in vehicles)
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
    }
    
    void UpdateAgents(List<AgentData> agentData)
    {
        if (agentData == null) return;

        if (agentPrefab == null)
        {
            Debug.LogError("WorldController: agentPrefab is not assigned. Please assign it in the Inspector.");
            return;
        }

        foreach (var a in agentData)
        {
            if (a == null || string.IsNullOrEmpty(a.id)) continue;

            // cria/recupera agente
            if (!agents.ContainsKey(a.id))
            {
                GameObject obj = Instantiate(agentPrefab);
                obj.name = $"Agent_{a.name}";

                // garantir mover (apenas um) - reutilizando VehicleMover para agentes
                VehicleMover mover = obj.GetComponent<VehicleMover>();
                if (mover == null) mover = obj.AddComponent<VehicleMover>();
                mover.moveSpeed = 1.5f;    // velocidade de caminhada (mais lento que veículos)
                mover.rotateSpeed = 360f;  // rotação normal
                mover.preserveY = true;

                // --- posição inicial ao instanciar: usar localização ---
                Vector3 spawnPos = Vector3.zero; // fallback

                // Verificar se está em estação ou veículo
                if (a.location_type == "station" && !string.IsNullOrEmpty(a.location_id) && stations.ContainsKey(a.location_id))
                {
                    Vector3 basePos = stations[a.location_id].transform.position + Vector3.up * 0.2f;

                    // offset determinístico baseado no id (evita empilhamento)
                    int spawnHash = Mathf.Abs(a.id.GetHashCode());
                    float spawnAngle = (Mathf.Abs(spawnHash) % 360) * Mathf.Deg2Rad;
                    float spawnRadius = 0.2f + (Mathf.Abs(spawnHash) % 5) * 0.15f;
                    Vector3 spawnOffset = new Vector3(Mathf.Cos(spawnAngle), 0f, Mathf.Sin(spawnAngle)) * spawnRadius;

                    spawnPos = basePos + spawnOffset;
                }
                else if (a.location_type == "vehicle" && !string.IsNullOrEmpty(a.location_id) && vehicles.ContainsKey(a.location_id))
                {
                    // Se estiver em veículo, posicionar próximo ao veículo
                    spawnPos = vehicles[a.location_id].transform.position + Vector3.up * 0.3f;
                }

                obj.transform.position = spawnPos;
                mover.targetPosition = spawnPos; // inicializar targetPosition

                agents[a.id] = obj;

                Debug.Log($"[Agent Created] {a.id} ({a.name}) - inicial pos: {spawnPos}");
            }

            GameObject agentObj = agents[a.id];

            // calcular posição alvo base (fallback mantém onde está)
            Vector3 baseTarget = agentObj.transform.position;

            // usar localização se disponível
            if (a.location_type == "station" && !string.IsNullOrEmpty(a.location_id) && stations.ContainsKey(a.location_id))
            {
                baseTarget = stations[a.location_id].transform.position + Vector3.up * 0.2f;
            }
            else if (a.location_type == "vehicle" && !string.IsNullOrEmpty(a.location_id) && vehicles.ContainsKey(a.location_id))
            {
                // Se estiver em veículo, seguir o veículo
                baseTarget = vehicles[a.location_id].transform.position + Vector3.up * 0.3f;
            }

            // offset determinístico para espalhar agentes
            int hash = Mathf.Abs(a.id.GetHashCode());
            
            // Usar índice no dicionário como seed adicional
            int agentIndex = 0;
            int idx = 0;
            foreach (var kvp in agents)
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
            VehicleMover am = agentObj.GetComponent<VehicleMover>();
            
            // Se não tiver VehicleMover, algo está errado - logar erro
            if (am == null)
            {
                Debug.LogError($"[Agent Error] {a.id} ({a.name}) não tem VehicleMover! Isso não deveria acontecer.");
                continue;
            }

            // THRESHOLD para evitar micro-atualizações
            float minChange = 0.2f;
            float distance = Vector3.Distance(am.targetPosition, target);
            
            if (distance > minChange)
            {
                am.targetPosition = target;
            }

            // Atualizar cor baseado em nível de energia
            Renderer agentRenderer = agentObj.GetComponent<Renderer>();
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
}
