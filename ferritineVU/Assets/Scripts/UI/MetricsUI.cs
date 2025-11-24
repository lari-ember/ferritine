using UnityEngine;
using UnityEngine.UI;

public class MetricsUI : MonoBehaviour
{
    public Text metricsText;

    public void UpdateMetrics(MetricsData metrics)
    {
        if (metricsText == null || metrics == null) return;

        // Usando os campos corretos de MetricsData (snake_case da API)
        metricsText.text = $"Passageiros Aguardando: {metrics.total_passengers_waiting}\n" +
                          $"Passageiros em Veículos: {metrics.total_passengers_in_vehicles}\n" +
                          $"Veículos: {metrics.total_vehicles}\n" +
                          $"Estações: {metrics.total_stations}\n" +
                          $"Rotas: {metrics.total_routes}\n" +
                          $"Receita Total: R$ {metrics.total_revenue:F2}\n" +
                          $"Fila Média: {metrics.avg_queue_length:F1}";
    }
}

