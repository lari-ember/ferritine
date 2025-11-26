using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class FerritineAPIClient : MonoBehaviour
{
    [Header("API Configuration")]
    public string apiUrl = "http://localhost:5000";
    public float pollInterval = 1f; // Segundos entre requisições
    
    [Header("Events")]
    public Action<WorldState> OnWorldStateReceived;
    public Action<string> OnError;
    public Action<MetricsData> OnMetricsReceived;
    
    private bool isPolling = false;
    
    void Start()
    {
        StartPolling();
    }
    
    public void StartPolling()
    {
        if (!isPolling)
        {
            isPolling = true;
            StartCoroutine(PollWorldState());
        }
    }
    
    public void StopPolling()
    {
        isPolling = false;
        StopAllCoroutines();
    }
    
    IEnumerator PollWorldState()
    {
        while (isPolling)
        {
            yield return StartCoroutine(GetWorldState());
            yield return StartCoroutine(GetMetrics());
            yield return new WaitForSeconds(pollInterval);
        }
    }
    
    IEnumerator GetWorldState()
    {
        string url = $"{apiUrl}/api/world/state";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string json = request.downloadHandler.text;
                    WorldState state = JsonUtility.FromJson<WorldState>(json);
                    ValidateUUIDs(state);
                    OnWorldStateReceived?.Invoke(state);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Erro ao parsear JSON: {e.Message}");
                    OnError?.Invoke(e.Message);
                }
            }
            else
            {
                Debug.LogError($"Erro na API: {request.error}");
                OnError?.Invoke(request.error);
            }
        }
    }
    
    // Ele valida os UUIDs recebidos
    
    private void ValidateUUIDs(WorldState state)
    {
        if (state == null) return;
        
        int invalid = 0;
        
        // Validar vehicles
        if (state.vehicles != null)
        {
            foreach (var v in state.vehicles)
                if (!v.HasValidId()) {
                    invalid++;
                    Debug.LogWarning($"⚠️ Vehicle UUID inválido: {v.id}");
                }
        }
        
        // Validar agents
        if (state.agents != null)
        {
            foreach (var a in state.agents)
                if (!a.HasValidId()) {
                    invalid++;
                    Debug.LogWarning($"⚠️ Agent UUID inválido: {a.id}");
                }
        }
        
        // Validar stations
        if (state.stations != null)
        {
            foreach (var s in state.stations)
                if (!s.HasValidId()) {
                    invalid++;
                    Debug.LogWarning($"⚠️ Station UUID inválido: {s.id}");
                }
        }
        
        if (invalid == 0)
            Debug.Log("✅ Todos os UUIDs são válidos!");
        else
            Debug.LogError($"❌ {invalid} UUIDs inválidos encontrados!");
    }
    
    
    // Método auxiliar para buscar apenas estações
    public IEnumerator GetStations(Action<StationData[]> callback)
    {
        string url = $"{apiUrl}/api/stations";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                // Wrapper necessário porque Unity não deserializa arrays direto
                string wrappedJson = $"{{\"items\":{json}}}";
                StationDataWrapper wrapper = JsonUtility.FromJson<StationDataWrapper>(wrappedJson);
                callback?.Invoke(wrapper.items);
            }
        }
    }
    
    public IEnumerator GetMetrics()
    {
        string url = $"{apiUrl}/api/metrics";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string json = request.downloadHandler.text;
                    MetricsData metrics = JsonUtility.FromJson<MetricsData>(json);
                    OnMetricsReceived?.Invoke(metrics);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Erro ao parsear Metrics JSON: {e.Message}");
                    OnError?.Invoke(e.Message);
                }
            }
            else
            {
                Debug.LogError($"Erro na API (metrics): {request.error}");
                OnError?.Invoke(request.error);
            }
        }
    }

    
    [Serializable]
    private class StationDataWrapper
    {
        public StationData[] items;
    }
}

