using UnityEngine;
using System;
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
    
    void Start()
    {
        if (apiClient == null)
        {
            apiClient = GetComponent<FerritineAPIClient>();
        }
        
        // Inscrever no evento de atualização
        apiClient.OnWorldStateReceived += UpdateWorld;
        apiClient.OnError += HandleError;
    }
    
    void UpdateWorld(WorldState state)
    {
        // Atualizar estações
        UpdateStations(state.stations);
        
        // Atualizar veículos
        UpdateVehicles(state.vehicles);
        
        // Atualizar UI de debug
        if (debugText != null)
        {
            debugText.text = $"Tempo: {state.timestamp}\n" +
                           $"Estações: {state.stations.Count}\n" +
                           $"Veículos: {state.vehicles.Count}\n" +
                           $"Passageiros em fila: {state.metrics.total_passengers_waiting}\n" +
                           $"Passageiros em veículos: {state.metrics.total_passengers_in_vehicles}";
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
            Renderer renderer = stationObj.GetComponent<Renderer>();
            
            if (renderer != null)
            {
                // Verde se vazio, amarelo se médio, vermelho se cheio
                float queueRatio = (float)data.queue_length / data.max_queue;
                
                if (queueRatio < 0.3f)
                    renderer.material.color = Color.green;
                else if (queueRatio < 0.7f)
                    renderer.material.color = Color.yellow;
                else
                    renderer.material.color = Color.red;
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
            Renderer renderer = vehicleObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                float occupancy = data.capacity > 0 ? (float)data.passengers / data.capacity : 0f;
                renderer.material.color = Color.Lerp(Color.blue, Color.magenta, occupancy);
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

