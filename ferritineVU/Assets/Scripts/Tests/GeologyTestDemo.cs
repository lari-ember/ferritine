using UnityEngine;
using Voxel;

/// <summary>
/// Script de demonstração do sistema de geologia de Curitiba.
/// Anexe este componente a qualquer GameObject para testar as funcionalidades.
/// </summary>
public class GeologyTestDemo : MonoBehaviour {
    
    [Header("Teste de Análise de Terreno")]
    [SerializeField] private Vector2Int testLocation = new Vector2Int(100, 100);
    [SerializeField] private Vector2Int buildingSize = new Vector2Int(20, 15);
    
    [Header("Teste de Tipos de Solo")]
    [SerializeField] private float testSlope = 15f; // graus
    
    [Header("Debug")]
    [SerializeField] private bool runTestOnStart = true;
    
    private void Start() {
        if (runTestOnStart) {
            RunAllTests();
        }
    }
    
    /// <summary>
    /// Executa todos os testes de demonstração.
    /// </summary>
    [ContextMenu("Run All Tests")]
    public void RunAllTests() {
        Debug.Log("=== TESTE DO SISTEMA DE GEOLOGIA DE CURITIBA ===\n");
        
        TestFoundationCosts();
        TestDrainage();
        TestLandslideRisk();
        TestBearingCapacity();
        TestMineableResources();
        
        Debug.Log("\n=== TESTES CONCLUÍDOS ===");
    }
    
    /// <summary>
    /// Testa custos de fundação para diferentes tipos de solo.
    /// </summary>
    [ContextMenu("Test Foundation Costs")]
    public void TestFoundationCosts() {
        Debug.Log("\n--- TESTE: Custos de Fundação ---");
        
        BlockType[] soils = {
            BlockType.Grama,
            BlockType.Terra,
            BlockType.Argila,
            BlockType.Areia,
            BlockType.Granito,
            BlockType.Agua
        };
        
        float baseCost = 50000f; // R$ 50.000 (custo base)
        
        foreach (var soil in soils) {
            float multiplier = GeologyUtils.GetFoundationCostMultiplier(soil);
            float finalCost = baseCost * multiplier;
            int extraDays = GeologyUtils.GetExtraConstructionDays(soil);
            
            Debug.Log($"{soil.ToString().PadRight(10)} | " +
                      $"Custo: R$ {finalCost:N2} ({multiplier}x) | " +
                      $"Dias extras: {extraDays}");
        }
    }
    
    /// <summary>
    /// Testa permeabilidade e drenagem para enchentes.
    /// </summary>
    [ContextMenu("Test Drainage")]
    public void TestDrainage() {
        Debug.Log("\n--- TESTE: Drenagem e Risco de Enchentes ---");
        
        BlockType[] soils = {
            BlockType.Areia,
            BlockType.Grama,
            BlockType.Terra,
            BlockType.Argila,
            BlockType.Granito,
            BlockType.Asfalto
        };
        
        float rainIntensity = 50f; // mm/h (chuva típica de Curitiba)
        
        foreach (var soil in soils) {
            float permeability = GeologyUtils.GetPermeability(soil);
            float waterRetention = GeologyUtils.GetWaterRetentionCapacity(soil);
            float drainageCapacity = permeability * 100f;
            
            string floodRisk = (drainageCapacity < rainIntensity) ? "⚠️ ALTO" : "✅ Baixo";
            
            Debug.Log($"{soil.ToString().PadRight(10)} | " +
                      $"Permeabilidade: {permeability * 100:F0}% | " +
                      $"Retenção: {waterRetention:F1} L/m² | " +
                      $"Risco de Enchente: {floodRisk}");
        }
    }
    
    /// <summary>
    /// Testa risco de deslizamento em diferentes encostas.
    /// </summary>
    [ContextMenu("Test Landslide Risk")]
    public void TestLandslideRisk() {
        Debug.Log("\n--- TESTE: Risco de Deslizamento ---");
        
        BlockType[] soils = {
            BlockType.Argila,
            BlockType.Terra,
            BlockType.Areia,
            BlockType.Grama,
            BlockType.Granito
        };
        
        float[] slopes = { 0f, 15f, 30f, 45f }; // graus
        
        foreach (var soil in soils) {
            string line = $"{soil.ToString().PadRight(10)} |";
            
            foreach (var slope in slopes) {
                float risk = GeologyUtils.GetLandslideRisk(soil, slope);
                string riskLevel = risk > 0.7f ? "🔴" : risk > 0.4f ? "🟡" : "🟢";
                line += $" {slope}°: {riskLevel}{risk * 100:F0}% |";
            }
            
            Debug.Log(line);
        }
    }
    
    /// <summary>
    /// Testa capacidade de carga e limite de andares.
    /// </summary>
    [ContextMenu("Test Bearing Capacity")]
    public void TestBearingCapacity() {
        Debug.Log("\n--- TESTE: Capacidade de Carga (Estrutural) ---");
        
        BlockType[] soils = {
            BlockType.Agua,
            BlockType.Argila,
            BlockType.Terra,
            BlockType.Areia,
            BlockType.Cascalho,
            BlockType.Granito
        };
        
        foreach (var soil in soils) {
            float capacity = GeologyUtils.GetBearingCapacity(soil);
            int maxFloors = GeologyUtils.GetMaxFloorsWithoutDeepFoundation(soil);
            
            string recommendation = maxFloors < 3 ? 
                "⚠️ Fundação profunda obrigatória" : 
                "✅ Fundação rasa adequada";
            
            Debug.Log($"{soil.ToString().PadRight(10)} | " +
                      $"Capacidade: {capacity:F1} tf/m² | " +
                      $"Max andares: {maxFloors} | " +
                      $"{recommendation}");
        }
    }
    
    /// <summary>
    /// Testa recursos mineráveis e seus valores econômicos.
    /// </summary>
    [ContextMenu("Test Mineable Resources")]
    public void TestMineableResources() {
        Debug.Log("\n--- TESTE: Recursos Mineráveis ---");
        
        BlockType[] blocks = {
            BlockType.Grama,
            BlockType.Argila,
            BlockType.Areia,
            BlockType.Granito,
            BlockType.Basalto,
            BlockType.Arenito,
            BlockType.Calcario
        };
        
        foreach (var block in blocks) {
            bool mineable = GeologyUtils.IsMineableResource(block);
            float value = GeologyUtils.GetResourceValue(block);
            
            if (mineable) {
                Debug.Log($"⛏️ {block.ToString().PadRight(10)} | " +
                          $"Minerável: SIM | Valor: R$ {value:F2}/m³ | " +
                          $"Total (100m³): R$ {value * 100:N2}");
            } else {
                Debug.Log($"❌ {block.ToString().PadRight(10)} | Não minerável");
            }
        }
    }
    
    /// <summary>
    /// Demonstração de descrições textuais para UI.
    /// </summary>
    [ContextMenu("Test Soil Descriptions")]
    public void TestSoilDescriptions() {
        Debug.Log("\n--- TESTE: Descrições de Solos (para UI) ---");
        
        BlockType[] soils = {
            BlockType.Grama,
            BlockType.Terra,
            BlockType.Argila,
            BlockType.Granito,
            BlockType.Agua
        };
        
        foreach (var soil in soils) {
            string description = GeologyUtils.GetSoilDescription(soil);
            Color debugColor = GeologyUtils.GetDebugColor(soil);
            
            Debug.Log($"<color=#{ColorUtility.ToHtmlStringRGB(debugColor)}>" +
                      $"■ {soil}: {description}</color>");
        }
    }
    
    /// <summary>
    /// Simulação de análise de terreno para construção.
    /// </summary>
    [ContextMenu("Simulate Building Site Analysis")]
    public void SimulateBuildingSiteAnalysis() {
        Debug.Log("\n--- SIMULAÇÃO: Análise de Terreno para Construção ---");
        Debug.Log($"Localização: ({testLocation.x}, {testLocation.y})");
        Debug.Log($"Dimensões do Edifício: {buildingSize.x}m × {buildingSize.y}m\n");
        
        // Simular amostragem de solo (em produção, buscar do TerrainWorld)
        BlockType[] simulatedSoils = {
            BlockType.Grama,    // Superfície
            BlockType.Terra,    // -3m (fundação)
            BlockType.Argila,   // -5m
            BlockType.Granito   // -10m (rocha matriz)
        };
        
        Debug.Log("🔬 Amostras de Solo:");
        for (int i = 0; i < simulatedSoils.Length; i++) {
            int depth = i * 3;
            Debug.Log($"  Profundidade -{depth}m: {simulatedSoils[i]}");
        }
        
        // Cálculos
        BlockType foundationSoil = simulatedSoils[1]; // Solo na profundidade de fundação
        
        float costMultiplier = GeologyUtils.GetFoundationCostMultiplier(foundationSoil);
        float baseCost = 100000f;
        float totalCost = baseCost * costMultiplier;
        
        int extraDays = GeologyUtils.GetExtraConstructionDays(foundationSoil);
        int baseDays = 180;
        int totalDays = baseDays + extraDays;
        
        float permeability = GeologyUtils.GetPermeability(foundationSoil);
        float floodRisk = (1f - permeability) * 0.5f; // Simplificado
        
        float landslideRisk = GeologyUtils.GetLandslideRisk(foundationSoil, testSlope);
        
        int maxFloors = GeologyUtils.GetMaxFloorsWithoutDeepFoundation(foundationSoil);
        
        // Relatório
        Debug.Log("\n📋 RELATÓRIO DE VIABILIDADE:");
        Debug.Log($"💰 Custo Total: R$ {totalCost:N2} (base: R$ {baseCost:N2}, multiplicador: {costMultiplier}x)");
        Debug.Log($"⏱️ Tempo de Construção: {totalDays} dias (base: {baseDays}, extra: {extraDays})");
        Debug.Log($"🌊 Risco de Enchente: {floodRisk * 100:F1}% ({(floodRisk > 0.5f ? "ALTO" : "Baixo")})");
        Debug.Log($"⛰️ Risco de Deslizamento: {landslideRisk * 100:F1}% (inclinação: {testSlope}°)");
        Debug.Log($"🏗️ Andares Máximos (sem fundação profunda): {maxFloors}");
        
        // Recomendações
        Debug.Log("\n💡 RECOMENDAÇÕES:");
        if (maxFloors < 3) {
            Debug.Log("  ⚠️ Fundação profunda (estacas ou tubulões) necessária");
        }
        if (floodRisk > 0.5f) {
            Debug.Log("  🌊 Sistema de drenagem reforçado obrigatório");
        }
        if (landslideRisk > 0.5f) {
            Debug.Log("  ⛰️ Muro de contenção e drenagem de encosta obrigatórios");
        }
        if (foundationSoil == BlockType.Argila) {
            Debug.Log("  🧱 Atenção: solo argiloso - monitorar recalques durante obra");
        }
        
        // Score de viabilidade
        float viabilityScore = 100f - (floodRisk * 30f) - (landslideRisk * 25f);
        if (maxFloors < 3) viabilityScore -= 15f;
        viabilityScore = Mathf.Clamp(viabilityScore, 0f, 100f);
        
        string viabilityLevel = viabilityScore > 80f ? "EXCELENTE ✅" : 
                                viabilityScore > 60f ? "BOM 🟢" :
                                viabilityScore > 40f ? "REGULAR 🟡" : "INADEQUADO 🔴";
        
        Debug.Log($"\n📊 VIABILIDADE GERAL: {viabilityScore:F1}/100 - {viabilityLevel}");
    }
    
    // Botões de teste no Inspector (para facilitar testes durante desenvolvimento)
    private void OnValidate() {
        // Validar valores
        testSlope = Mathf.Clamp(testSlope, 0f, 90f);
    }
}
