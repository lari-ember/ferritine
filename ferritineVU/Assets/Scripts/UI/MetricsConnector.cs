using UnityEngine;

public class MetricsConnector : MonoBehaviour
{
    [Header("References")]
    public FerritineAPIClient apiClient;
    public MetricsUI metricsUI;

    void Start()
    {
        if (apiClient == null || metricsUI == null)
        {
            Debug.LogWarning("MetricsConnector: apiClient or metricsUI is not assigned in the Inspector.");
            return;
        }

        apiClient.OnMetricsReceived += metricsUI.UpdateMetrics;
    }

    void OnDestroy()
    {
        if (apiClient != null)
            apiClient.OnMetricsReceived -= metricsUI.UpdateMetrics;
    }
}