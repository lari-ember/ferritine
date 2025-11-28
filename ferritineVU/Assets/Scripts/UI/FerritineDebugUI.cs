using UnityEngine;
using TMPro;
using System;

public class FerritineDebugUI : MonoBehaviour
{
    [Header("References")]
    public FerritineAPIClient apiClient;
    public TextMeshProUGUI debugText;

    private WorldState lastState;

    void Awake()
    {
        // Awake garante que a subscrição acontece ANTES do polling iniciar
        if (apiClient != null)
        {
            apiClient.OnWorldStateReceived += UpdateDebugUI;
            apiClient.OnError += ShowError;
        }
        else
        {
            Debug.LogWarning("FerritineDebugUI: apiClient não atribuído no Inspector!");
        }
    }

    void UpdateDebugUI(WorldState state)
    {
        lastState = state;
        
        int vehicleCount = state.vehicles?.Count ?? 0;
        int agentCount = state.agents?.Count ?? 0;
        int stationCount = state.stations?.Count ?? 0;
        int routeCount = state.routes?.Count ?? 0;
        int operatorCount = state.operators?.Count ?? 0;

        // Timestamp ISO 8601 completo
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string isoTimestamp = state.timestamp ?? DateTime.UtcNow.ToString("o");

        debugText.text = 
            $"<b><size=130%><color=#00D9FF>⚙ FERRITINE SIMULATION</color></size></b>\n" +
            $"<color=#666666>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>\n\n" +
            
            $"<b><color=#FFD700>📡 DADOS EM TEMPO REAL</color></b>\n" +
            $"<mspace=0.6em>🚗 Veículos........: <b><color=#4CAF50>{vehicleCount,3}</color></b></mspace>\n" +
            $"<mspace=0.6em>👤 Agentes........: <b><color=#2196F3>{agentCount,3}</color></b></mspace>\n" +
            $"<mspace=0.6em>🚏 Estações.......: <b><color=#FF9800>{stationCount,3}</color></b></mspace>\n" +
            $"<mspace=0.6em>🛤 Rotas..........: <b><color=#9C27B0>{routeCount,3}</color></b></mspace>\n" +
            $"<mspace=0.6em>🏢 Operadores.....: <b><color=#E91E63>{operatorCount,3}</color></b></mspace>\n\n" +
            
            $"<color=#666666>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>\n" +
            $"<size=80%><color=#999999>⏱ Local: {timestamp}</color></size>\n" +
            $"<size=80%><color=#999999>🌐 Server: {isoTimestamp}</color></size>";
    }

    void ShowError(string error)
    {
        debugText.text = 
            $"<b><size=130%><color=#FF4444>❌ ERRO DE CONEXÃO</color></size></b>\n" +
            $"<color=#666666>━━━━━━━━━━━━━━━━━━━━━━━━━━━━━</color>\n\n" +
            $"<color=#FFAAAA>{error}</color>\n\n" +
            $"<size=80%><color=#999999>⏱ {DateTime.Now:HH:mm:ss}</color></size>";
    }

    void OnDestroy()
    {
        if (apiClient != null)
        {
            apiClient.OnWorldStateReceived -= UpdateDebugUI;
            apiClient.OnError -= ShowError;
        }
    }
}
