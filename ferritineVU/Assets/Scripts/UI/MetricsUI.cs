using TMPro;
using UnityEngine;
using System.Globalization;

public class MetricsUI : MonoBehaviour
{
    public TextMeshProUGUI metricsText;
    
    [Header("Threshold Settings")]
    [Tooltip("Fila média abaixo deste valor = verde")]
    public float queueLowThreshold = 5f;
    
    [Tooltip("Fila média acima deste valor = vermelho")]
    public float queueHighThreshold = 15f;

    public void UpdateMetrics(MetricsData metrics)
    {
        if (metricsText == null || metrics == null) return;

        // Formatação de moeda em pt-BR
        string revenue = FormatCurrency(metrics.total_revenue);
        
        // Cor dinâmica para fila média baseada em threshold
        string queueColor = GetQueueColor(metrics.avg_queue_length);
        
        metricsText.text = 
            $"<b><size=120%>📊 MÉTRICAS DO SISTEMA</size></b>\n" +
            $"<color=#CCCCCC>━━━━━━━━━━━━━━━━━━━━━━━━━━</color>\n\n" +
            
            $"<b>👤 Passageiros</b>\n" +
            $"  • Aguardando: <color=#FFA500><b>{metrics.total_passengers_waiting}</b></color>\n" +
            $"  • Em Veículos: <color=#4169E1><b>{metrics.total_passengers_in_vehicles}</b></color>\n\n" +
            
            $"<b>🚗 Infraestrutura</b>\n" +
            $"  • Veículos: <b>{metrics.total_vehicles}</b>\n" +
            $"  • Estações: <b>{metrics.total_stations}</b>\n" +
            $"  • Rotas Ativas: <b>{metrics.total_routes}</b>\n\n" +
            
            $"<b>💰 Financeiro</b>\n" +
            $"  • Receita Total: <color=#2ECC40><b>{revenue}</b></color>\n\n" +
            
            $"<b>📈 Indicadores</b>\n" +
            $"  • Fila Média: <color={queueColor}><b>{metrics.avg_queue_length:F1}</b></color> passageiros";
    }
    
    /// <summary>
    /// Formata valor em moeda brasileira (R$ 1.234,56)
    /// </summary>
    private string FormatCurrency(float value)
    {
        CultureInfo ptBR = new CultureInfo("pt-BR");
        return value.ToString("C2", ptBR);
    }
    
    /// <summary>
    /// Retorna cor baseada no tamanho da fila média
    /// Verde (baixo) → Amarelo (médio) → Vermelho (alto)
    /// </summary>
    private string GetQueueColor(float avgQueue)
    {
        if (avgQueue <= queueLowThreshold)
            return "#2ECC40"; // Verde
        else if (avgQueue <= queueHighThreshold)
            return "#FFDC00"; // Amarelo
        else
            return "#FF4136"; // Vermelho
    }
}

