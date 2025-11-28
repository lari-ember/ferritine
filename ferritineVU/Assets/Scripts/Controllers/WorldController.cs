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
    
    [Header("UI")]
    public TextMeshProUGUI debugText;
    
    // Dicionários para rastrear GameObjects usando UUID strings
    // Mantemos como string porque JSON deserializa UUIDs como strings
    // e a conversão é feita via helper methods quando necessário
    private Dictionary<string, GameObject> stations = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> vehicles = new Dictionary<string, GameObject>();
    
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
        
        // Atualizar UI de debug, com proteções adicionais
        if (debugText != null)
        {
            int stationCount = state.stations?.Count ?? 0;
            int vehicleCount = state.vehicles?.Count ?? 0;
            int waiting = state.metrics != null ? state.metrics.total_passengers_waiting : 0;
            int inVehicles = state.metrics != null ? state.metrics.total_passengers_in_vehicles : 0;

            debugText.text = $"Tempo: {state.timestamp}\n" +
                             $"Estações: {stationCount}\n" +
                             $"Veículos: {vehicleCount}\n" +
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
        foreach (var data in vehicleData)
        {
            if (!vehicles.ContainsKey(data.id))
            {
                // Criar novo veículo
                GameObject vehicle = Instantiate(vehiclePrefab, Vector3.zero, Quaternion.identity);
                vehicle.name = data.name;
                vehicles[data.id] = vehicle;
            }
            
            GameObject vehicleObj = vehicles[data.id];
            
            // Mover para estação atual (se tiver)
            if (!string.IsNullOrEmpty(data.current_station_id) && 
                stations.ContainsKey(data.current_station_id))
            {
                Vector3 targetPos = stations[data.current_station_id].transform.position;
                targetPos.y = 1f; // Elevado para ficar acima da estação
                
                // Movimento suave
                vehicleObj.transform.position = Vector3.Lerp(
                    vehicleObj.transform.position,
                    targetPos,
                    Time.deltaTime * 2f
                );
            }
            
            // Atualizar cor baseado em ocupação
            Renderer vehicleRenderer = vehicleObj.GetComponent<Renderer>();
            if (vehicleRenderer != null)
            {
                float occupancy = data.capacity > 0 ? (float)data.passengers / data.capacity : 0f;
                vehicleRenderer.material.color = Color.Lerp(Color.blue, Color.magenta, occupancy);
            }
            
            // Atualizar texto
            TextMeshPro text = vehicleObj.GetComponentInChildren<TextMeshPro>();
            if (text != null)
            {
                text.text = $"{data.name}\n👥 {data.passengers}/{data.capacity}";
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
