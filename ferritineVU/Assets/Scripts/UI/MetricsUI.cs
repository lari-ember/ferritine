using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MetricsUI : MonoBehaviour
{
    public Text metricsText;

    public void UpdateMetrics(MetricsData metrics)
    {
        if (metricsText == null || metrics == null) return;

        metricsText.text = $"Agents: {metrics.totalAgents}\nVehicles: {metrics.activeVehicles}\nStations: {metrics.totalStations}\nAvg Wait: {metrics.averageWaitTime:F2}s";
    }
}

