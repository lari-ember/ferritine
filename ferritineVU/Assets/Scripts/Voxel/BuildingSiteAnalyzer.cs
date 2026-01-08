using UnityEngine;

namespace Voxel {
    /// <summary>
    /// Exemplo de integração do sistema geológico com construção de edifícios.
    /// Use este código como referência para implementar custos, restrições e análise de terreno.
    /// </summary>
    public class BuildingSiteAnalyzer : MonoBehaviour {
        
        [Header("Referências")]
        [SerializeField] private TerrainWorld terrainWorld;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugGizmos = true;
        
        /// <summary>
        /// Analisa um local de construção e retorna um relatório completo.
        /// </summary>
        /// <param name="worldX">Coordenada X no mundo</param>
        /// <param name="worldZ">Coordenada Z no mundo</param>
        /// <param name="buildingWidth">Largura do edifício (em metros)</param>
        /// <param name="buildingDepth">Profundidade do edifício (em metros)</param>
        /// <returns>Relatório de análise do terreno</returns>
        public SiteAnalysisReport AnalyzeBuildingSite(int worldX, int worldZ, int buildingWidth, int buildingDepth) {
            SiteAnalysisReport report = new SiteAnalysisReport();
            report.location = new Vector2Int(worldX, worldZ);
            report.buildingSize = new Vector2Int(buildingWidth, buildingDepth);
            
            // Coletar amostras de solo na área
            var soilSamples = CollectSoilSamples(worldX, worldZ, buildingWidth, buildingDepth);
            
            // Análise de custo
            report.foundationCost = CalculateFoundationCost(soilSamples);
            report.constructionDaysExtra = CalculateExtraConstructionTime(soilSamples);
            
            // Análise de risco
            report.floodRisk = CalculateFloodRisk(worldX, worldZ, buildingWidth, buildingDepth);
            report.landslideRisk = CalculateLandslideRisk(worldX, worldZ, buildingWidth, buildingDepth);
            
            // Análise estrutural
            report.maxFloorsRecommended = CalculateMaxFloors(soilSamples);
            report.deepFoundationRequired = report.maxFloorsRecommended < 3;
            
            // Recomendações
            report.recommendations = GenerateRecommendations(report, soilSamples);
            
            // Viabilidade geral (0-100)
            report.viabilityScore = CalculateViability(report);
            
            return report;
        }
        
        /// <summary>
        /// Coleta amostras de solo na área de construção.
        /// </summary>
        private SoilSample[] CollectSoilSamples(int worldX, int worldZ, int width, int depth) {
            // Amostrar em grid 3x3 na área de construção
            int samplesX = Mathf.Min(width, 3);
            int samplesZ = Mathf.Min(depth, 3);
            
            SoilSample[] samples = new SoilSample[samplesX * samplesZ];
            int index = 0;
            
            for (int x = 0; x < samplesX; x++) {
                for (int z = 0; z < samplesZ; z++) {
                    int sampleX = worldX + (x * width / samplesX);
                    int sampleZ = worldZ + (z * depth / samplesZ);
                    
                    samples[index] = new SoilSample {
                        position = new Vector3Int(sampleX, 0, sampleZ),
                        surfaceSoil = GetSoilTypeAt(sampleX, sampleZ, 0),
                        foundationDepthSoil = GetSoilTypeAt(sampleX, sampleZ, -3), // 3 metros de profundidade
                        bedrockSoil = GetSoilTypeAt(sampleX, sampleZ, -10) // 10 metros (rocha)
                    };
                    
                    index++;
                }
            }
            
            return samples;
        }
        
        /// <summary>
        /// Busca o tipo de solo em uma posição específica (com profundidade).
        /// </summary>
        private BlockType GetSoilTypeAt(int worldX, int worldZ, int depthOffset) {
            // TODO: Integrar com TerrainWorld para buscar voxel real
            // Por enquanto, retorna placeholder baseado em altura
            
            if (terrainWorld == null) {
                Debug.LogWarning("[BuildingSiteAnalyzer] TerrainWorld não atribuído!");
                return BlockType.Terra;
            }
            
            // Exemplo de lógica (substitua pela busca real no voxel array)
            int surfaceHeight = 10; // Placeholder: busque do heightmap
            int targetY = surfaceHeight + depthOffset;
            
            if (targetY < 0) return BlockType.Granito; // Rocha profunda
            if (targetY == surfaceHeight) return BlockType.Grama;
            if (targetY > surfaceHeight - 5) return BlockType.Terra;
            return BlockType.Argila;
        }
        
        /// <summary>
        /// Calcula o custo de fundação baseado nas amostras de solo.
        /// </summary>
        private float CalculateFoundationCost(SoilSample[] samples) {
            float baseCost = 10000f; // Custo base em $ (fictício)
            float totalMultiplier = 0f;
            
            foreach (var sample in samples) {
                // Considerar o solo na profundidade de fundação (3m)
                float multiplier = GeologyUtils.GetFoundationCostMultiplier(sample.foundationDepthSoil);
                totalMultiplier += multiplier;
            }
            
            float averageMultiplier = totalMultiplier / samples.Length;
            return baseCost * averageMultiplier;
        }
        
        /// <summary>
        /// Calcula o tempo extra de construção baseado no solo.
        /// </summary>
        private int CalculateExtraConstructionTime(SoilSample[] samples) {
            int maxExtraDays = 0;
            
            foreach (var sample in samples) {
                int days = GeologyUtils.GetExtraConstructionDays(sample.foundationDepthSoil);
                if (days > maxExtraDays) maxExtraDays = days;
            }
            
            return maxExtraDays;
        }
        
        /// <summary>
        /// Calcula o risco de enchente baseado na permeabilidade e altitude.
        /// </summary>
        private float CalculateFloodRisk(int worldX, int worldZ, int width, int depth) {
            // Fatores: altitude baixa + solo impermeável = alto risco
            float altitude = 10f; // TODO: Buscar altura real do terreno
            float permeability = 0.5f; // TODO: Média da permeabilidade das amostras
            
            float altitudeRisk = Mathf.Clamp01((20f - altitude) / 20f); // Risco maior em altitude < 20m
            float soilRisk = 1f - permeability; // Solo impermeável retém água
            
            return (altitudeRisk * 0.6f + soilRisk * 0.4f); // Ponderado
        }
        
        /// <summary>
        /// Calcula o risco de deslizamento baseado na inclinação e tipo de solo.
        /// </summary>
        private float CalculateLandslideRisk(int worldX, int worldZ, int width, int depth) {
            // Calcular inclinação média do terreno
            float slope = CalculateAverageSlope(worldX, worldZ, width, depth);
            
            // TODO: Obter tipo de solo predominante
            BlockType dominantSoil = BlockType.Terra;
            
            return GeologyUtils.GetLandslideRisk(dominantSoil, slope);
        }
        
        /// <summary>
        /// Calcula a inclinação média do terreno (em graus).
        /// </summary>
        private float CalculateAverageSlope(int worldX, int worldZ, int width, int depth) {
            // Placeholder: calcular diferença de altura nos cantos
            float h1 = 10f; // TODO: Buscar altura real
            float h2 = 12f;
            float h3 = 11f;
            float h4 = 13f;
            
            float heightDiff = Mathf.Max(Mathf.Abs(h1 - h2), Mathf.Abs(h3 - h4));
            float distance = Mathf.Max(width, depth);
            
            return Mathf.Atan(heightDiff / distance) * Mathf.Rad2Deg;
        }
        
        /// <summary>
        /// Calcula o número máximo de andares recomendado.
        /// </summary>
        private int CalculateMaxFloors(SoilSample[] samples) {
            int minFloors = int.MaxValue;
            
            foreach (var sample in samples) {
                int floors = GeologyUtils.GetMaxFloorsWithoutDeepFoundation(sample.foundationDepthSoil);
                if (floors < minFloors) minFloors = floors;
            }
            
            return minFloors;
        }
        
        /// <summary>
        /// Gera recomendações técnicas baseadas na análise.
        /// </summary>
        private string[] GenerateRecommendations(SiteAnalysisReport report, SoilSample[] samples) {
            var recommendations = new System.Collections.Generic.List<string>();
            
            if (report.deepFoundationRequired) {
                recommendations.Add("⚠️ Fundação profunda (estacas) necessária para estruturas altas");
            }
            
            if (report.floodRisk > 0.6f) {
                recommendations.Add("🌊 Alto risco de enchente - considerar drenagem reforçada e elevação do térreo");
            }
            
            if (report.landslideRisk > 0.5f) {
                recommendations.Add("⛰️ Risco de deslizamento - muro de contenção e drenagem obrigatórios");
            }
            
            if (report.constructionDaysExtra > 30) {
                recommendations.Add($"⏱️ Terreno rochoso: adicionar {report.constructionDaysExtra} dias ao cronograma");
            }
            
            // Verificar se há amostras problemáticas
            foreach (var sample in samples) {
                if (sample.foundationDepthSoil == BlockType.Agua) {
                    recommendations.Add("💧 Solo saturado detectado - rebaixamento do lençol freático necessário");
                    break;
                }
                
                if (sample.foundationDepthSoil == BlockType.Argila) {
                    recommendations.Add("🧱 Solo argiloso - atenção a recalques diferenciais");
                    break;
                }
            }
            
            if (recommendations.Count == 0) {
                recommendations.Add("✅ Terreno adequado para construção sem restrições especiais");
            }
            
            return recommendations.ToArray();
        }
        
        /// <summary>
        /// Calcula a viabilidade geral do terreno (0-100).
        /// </summary>
        private float CalculateViability(SiteAnalysisReport report) {
            float score = 100f;
            
            // Penalidades
            score -= report.floodRisk * 30f;
            score -= report.landslideRisk * 25f;
            
            if (report.deepFoundationRequired) score -= 15f;
            if (report.constructionDaysExtra > 30) score -= 10f;
            
            // Bônus: terreno plano e seco
            if (report.landslideRisk < 0.2f) score += 5f;
            if (report.floodRisk < 0.2f) score += 5f;
            
            return Mathf.Clamp(score, 0f, 100f);
        }
        
        // ==================== DEBUG VISUALIZATION ====================
        
        private void OnDrawGizmos() {
            if (!showDebugGizmos) return;
            
            // TODO: Desenhar grid de amostras, cores por tipo de solo, etc.
        }
    }
    
    // ==================== DATA STRUCTURES ====================
    
    [System.Serializable]
    public struct SoilSample {
        public Vector3Int position;
        public BlockType surfaceSoil;
        public BlockType foundationDepthSoil;
        public BlockType bedrockSoil;
    }
    
    [System.Serializable]
    public class SiteAnalysisReport {
        public Vector2Int location;
        public Vector2Int buildingSize;
        
        // Custos e tempo
        public float foundationCost;
        public int constructionDaysExtra;
        
        // Riscos
        public float floodRisk;         // 0-1
        public float landslideRisk;     // 0-1
        
        // Capacidade estrutural
        public int maxFloorsRecommended;
        public bool deepFoundationRequired;
        
        // Análise geral
        public float viabilityScore;    // 0-100
        public string[] recommendations;
        
        public override string ToString() {
            string report = $"=== RELATÓRIO DE ANÁLISE DE TERRENO ===\n";
            report += $"Localização: ({location.x}, {location.y})\n";
            report += $"Dimensões: {buildingSize.x}m × {buildingSize.y}m\n\n";
            
            report += $"💰 Custo de Fundação: ${foundationCost:N2}\n";
            report += $"⏱️ Tempo Extra: {constructionDaysExtra} dias\n\n";
            
            report += $"🌊 Risco de Enchente: {floodRisk * 100f:F1}%\n";
            report += $"⛰️ Risco de Deslizamento: {landslideRisk * 100f:F1}%\n\n";
            
            report += $"🏗️ Andares Recomendados: {maxFloorsRecommended}\n";
            report += $"🔩 Fundação Profunda: {(deepFoundationRequired ? "SIM" : "NÃO")}\n\n";
            
            report += $"📊 Viabilidade Geral: {viabilityScore:F1}/100\n\n";
            
            report += "📋 Recomendações:\n";
            foreach (var rec in recommendations) {
                report += $"  • {rec}\n";
            }
            
            return report;
        }
    }
}

