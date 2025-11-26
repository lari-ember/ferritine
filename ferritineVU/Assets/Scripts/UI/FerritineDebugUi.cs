using UnityEngine;
using TMPro;

public class FerritineDebugUI : MonoBehaviour
{
    [Header("References")]
    public FerritineAPIClient apiClient;
    public TextMeshProUGUI debugText;

    private WorldState lastState;

    void Start()
    {
        if (apiClient != null)
        {
            apiClient.OnWorldStateReceived += UpdateDebugUI;
            apiClient.OnError += ShowError;
        }
    }

    void UpdateDebugUI(WorldState state)
    {
        lastState = state;
        
        int vehicleCount = state.vehicles?.Count ?? 0;
        int agentCount = state.agents?.Count ?? 0;
        int stationCount = state.stations?.Count ?? 0;

        debugText.text = $"<b>Ferritine Simulation</b>\n" +
                        $"─────────────────\n" +
                        $"🚗 Veículos: {vehicleCount}\n" +
                        $"👤 Agentes: {agentCount}\n" +
                        $"🚏 Estações: {stationCount}\n" +
                        $"─────────────────\n" +
                        $"⏱️ {System.DateTime.Now:HH:mm:ss}";
    }

    void ShowError(string error)
    {
        debugText.text = $"<color=red>❌ ERRO:</color>\n{error}";
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
