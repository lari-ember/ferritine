using API.Models;
using UnityEngine;

public class WeatherSyncManager : MonoBehaviour
{
    private void OnEnable()
    {
        if (Managers.WorldStateManager.Instance != null)
            Managers.WorldStateManager.Instance.OnWorldStateChanged += HandleWorldStateChanged;
    }

    private void OnDisable()
    {
        if (Managers.WorldStateManager.Instance != null)
            Managers.WorldStateManager.Instance.OnWorldStateChanged -= HandleWorldStateChanged;
    }

    private void HandleWorldStateChanged(WorldState state)
    {
        ApplyWeather(state);
    }

    private void ApplyWeather(WorldState state)
    {
        _ = state;
        // Exemplo: Debug.Log($"Novo clima: {state.weatherType}");
    }
}
