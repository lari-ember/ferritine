using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

/// <summary>
/// Sistema de teleporte com sincronização do backend.
/// Coordena a ação de teleporte entre frontend (visual) e backend (dados).
/// 
/// Fluxo:
/// 1. UI solicita teleporte (agente + destino)
/// 2. BackendTeleportManager envia requisição POST para /api/agents/{agent_id}/teleport
/// 3. Backend confirma e atualiza posição no banco de dados
/// 4. Frontend recebe confirmação e anima o teleporte localmente
/// 5. Próxima atualização do WorldState sincroniza a posição final
/// </summary>
public class BackendTeleportManager : MonoBehaviour
{
    [Header("API Configuration")]
    [SerializeField] private string apiBaseUrl = "http://localhost:8000";
    
    [Header("Teleport Settings")]
    [SerializeField] private float teleportDuration = 0.5f; // Duração da animação de teleporte
    
    [Header("Events")]
    public Action<bool, string> OnTeleportCompleted; // (success, message)
    
    // Singleton
    private static BackendTeleportManager _instance;
    public static BackendTeleportManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<BackendTeleportManager>();
                if (_instance == null)
                {
                    Debug.LogError("[BackendTeleportManager] Nenhum BackendTeleportManager encontrado na cena!");
                }
            }
            return _instance;
        }
    }
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Debug.LogWarning("[BackendTeleportManager] Duplicate found, destroying this one.");
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Inicia uma requisição de teleporte para o backend.
    /// </summary>
    /// <param name="agentId">UUID do agente a teleportar</param>
    /// <param name="locationType">Tipo do destino: "building", "station"</param>
    /// <param name="locationId">UUID do destino</param>
    /// <param name="onComplete">Callback quando a operação terminar (success, message)</param>
    public void TeleportAgent(string agentId, string locationType, string locationId, 
                              System.Action<bool, string> onComplete = null)
    {
        if (string.IsNullOrEmpty(agentId) || string.IsNullOrEmpty(locationId))
        {
            Debug.LogError("[BackendTeleportManager] Agente ou destino inválido!");
            onComplete?.Invoke(false, "Dados inválidos");
            return;
        }
        
        StartCoroutine(TeleportAgentCoroutine(agentId, locationType, locationId, onComplete));
    }
    
    /// <summary>
    /// Corrotina que executa o teleporte.
    /// </summary>
    IEnumerator TeleportAgentCoroutine(string agentId, string locationType, string locationId,
                                       System.Action<bool, string> onComplete)
    {
        string url = $"{apiBaseUrl}/api/agents/{agentId}/teleport";
        
        // Criar request body
        TeleportRequest request = new TeleportRequest
        {
            location_type = locationType,
            location_id = locationId
        };
        
        string json = JsonUtility.ToJson(request);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            
            yield return webRequest.SendWebRequest();
            
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string responseText = webRequest.downloadHandler.text;
                    TeleportResponse response = JsonUtility.FromJson<TeleportResponse>(responseText);
                    
                    Debug.Log($"[BackendTeleportManager] ✓ Teleporte confirmado para {agentId}");
                    onComplete?.Invoke(true, "Teleporte realizado com sucesso");
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[BackendTeleportManager] ✗ Erro ao parsear resposta: {e.Message}");
                    onComplete?.Invoke(false, "Erro ao processar resposta do servidor");
                }
            }
            else
            {
                string errorMsg = webRequest.error;
                if (!string.IsNullOrEmpty(webRequest.downloadHandler.text))
                {
                    errorMsg = webRequest.downloadHandler.text;
                }
                
                Debug.LogError($"[BackendTeleportManager] ✗ Erro HTTP {webRequest.responseCode}: {errorMsg}");
                onComplete?.Invoke(false, $"Erro: {errorMsg}");
            }
        }
    }
    
    /// <summary>
    /// Serialização para request do teleporte.
    /// </summary>
    [System.Serializable]
    public class TeleportRequest
    {
        public string location_type;
        public string location_id;
    }
    
    /// <summary>
    /// Estrutura de resposta do teleporte.
    /// </summary>
    [System.Serializable]
    public class TeleportResponse
    {
        public string agent_id;
        public string location_type;
        public string location_id;
        public string status;
        public string message;
    }
}

