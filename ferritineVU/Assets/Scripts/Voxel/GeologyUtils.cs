using UnityEngine;

namespace Voxel {
    /// <summary>
    /// Utilitários para análise geológica e custos de construção.
    /// Baseado na geologia real de Curitiba (Primeiro Planalto Paranaense).
    /// </summary>
    public static class GeologyUtils {
        
        // ==================== CUSTOS DE CONSTRUÇÃO ====================
        
        /// <summary>
        /// Retorna o multiplicador de custo de fundação baseado no tipo de solo.
        /// Valores realistas para construção civil em Curitiba.
        /// </summary>
        /// <param name="soilType">Tipo de solo/rocha encontrado</param>
        /// <returns>Multiplicador de custo (1.0 = custo base)</returns>
        public static float GetFoundationCostMultiplier(BlockType soilType) {
            switch (soilType) {
                case BlockType.Grama:
                case BlockType.Terra:
                    return 1.0f; // Custo base - solo fácil de escavar
                
                case BlockType.Argila:
                    return 1.3f; // Requer compactação, risco de recalque
                
                case BlockType.Areia:
                    return 1.5f; // Fundação profunda necessária (baixa capacidade de carga)
                
                case BlockType.Laterita:
                    return 1.2f; // Solo duro mas manejável
                
                case BlockType.Cascalho:
                    return 1.1f; // Bom solo, boa drenagem
                
                case BlockType.Granito:
                case BlockType.Basalto:
                case BlockType.Gneiss:
                    return 2.0f; // Rocha sã: explosivos, perfuração, martelo hidráulico
                
                case BlockType.Arenito:
                case BlockType.Calcario:
                    return 1.6f; // Rocha sedimentar: mais fácil que granito, mas requer equipamento
                
                case BlockType.Agua:
                    return 3.0f; // Aterro hidráulico, contenção, bombeamento
                
                case BlockType.Concreto:
                case BlockType.Asfalto:
                    return 1.8f; // Demolição de infraestrutura existente
                
                default:
                    return 1.0f;
            }
        }
        
        /// <summary>
        /// Retorna o tempo de construção adicional (em dias de jogo) baseado no solo.
        /// </summary>
        public static int GetExtraConstructionDays(BlockType soilType) {
            switch (soilType) {
                case BlockType.Granito:
                case BlockType.Basalto:
                    return 30; // Escavação de rocha é lenta
                
                case BlockType.Agua:
                    return 45; // Aterro + drenagem + consolidação
                
                case BlockType.Argila:
                    return 15; // Compactação e estabilização
                
                case BlockType.Areia:
                    return 20; // Fundação profunda (estacas/tubulões)
                
                default:
                    return 0;
            }
        }
        
        // ==================== PERMEABILIDADE (DRENAGEM URBANA) ====================
        
        /// <summary>
        /// Retorna o coeficiente de permeabilidade do solo (0.0 = impermeável, 1.0 = totalmente permeável).
        /// Crucial para simular enchentes em Curitiba (histórico de alagamentos).
        /// </summary>
        /// <param name="soilType">Tipo de solo</param>
        /// <returns>Coeficiente de infiltração (0-1)</returns>
        public static float GetPermeability(BlockType soilType) {
            switch (soilType) {
                case BlockType.Areia:
                case BlockType.Cascalho:
                    return 0.9f; // Alta permeabilidade (k > 10^-3 cm/s)
                
                case BlockType.Grama:
                    return 0.7f; // Boa permeabilidade com cobertura vegetal
                
                case BlockType.Terra:
                    return 0.6f; // Permeabilidade média
                
                case BlockType.Laterita:
                    return 0.5f; // Permeabilidade moderada
                
                case BlockType.Arenito:
                    return 0.4f; // Rocha sedimentar porosa
                
                case BlockType.Argila:
                    return 0.2f; // Baixa permeabilidade (k < 10^-6 cm/s)
                
                case BlockType.Granito:
                case BlockType.Basalto:
                case BlockType.Gneiss:
                    return 0.05f; // Rocha impermeável (exceto fraturas)
                
                case BlockType.Concreto:
                case BlockType.Asfalto:
                    return 0.01f; // Impermeável (causa ilha de calor + enchentes)
                
                case BlockType.Agua:
                    return 1.0f; // Água já está saturada
                
                default:
                    return 0.5f;
            }
        }
        
        /// <summary>
        /// Calcula o volume de água que pode ser absorvido por um tile (em litros/m²).
        /// Baseado na capacidade de campo dos solos.
        /// </summary>
        public static float GetWaterRetentionCapacity(BlockType soilType) {
            float permeability = GetPermeability(soilType);
            float depth = 1.0f; // Profundidade considerada (1 metro)
            
            // Capacidade = permeabilidade × profundidade × fator de porosidade
            return permeability * depth * 100f; // litros/m²
        }
        
        // ==================== ESTABILIDADE DE TERRENO ====================
        
        /// <summary>
        /// Calcula o risco de deslizamento baseado no tipo de solo e inclinação.
        /// Retorna valor de 0.0 (estável) a 1.0 (alto risco de deslizamento).
        /// Importante para as encostas e morros de Curitiba.
        /// </summary>
        /// <param name="soilType">Tipo de solo</param>
        /// <param name="slopeDegrees">Inclinação em graus (0-90)</param>
        /// <returns>Índice de risco (0-1)</returns>
        public static float GetLandslideRisk(BlockType soilType, float slopeDegrees) {
            float baseRisk;
            
            switch (soilType) {
                case BlockType.Argila:
                    baseRisk = 0.7f; // Alto risco quando saturada (escorregamentos frequentes)
                    break;
                
                case BlockType.Terra:
                    baseRisk = 0.5f; // Risco moderado
                    break;
                
                case BlockType.Areia:
                    baseRisk = 0.6f; // Risco alto em encostas (ângulo de repouso ~30°)
                    break;
                
                case BlockType.Laterita:
                    baseRisk = 0.4f; // Mais coesa que argila comum
                    break;
                
                case BlockType.Cascalho:
                    baseRisk = 0.5f; // Depende da compactação
                    break;
                
                case BlockType.Granito:
                case BlockType.Basalto:
                case BlockType.Gneiss:
                    baseRisk = 0.1f; // Rocha sã é estável (risco apenas em fraturas)
                    break;
                
                case BlockType.Grama:
                    baseRisk = 0.3f; // Vegetação ajuda a estabilizar (raízes)
                    break;
                
                default:
                    baseRisk = 0.5f;
                    break;
            }
            
            // Inclinação aumenta o risco exponencialmente
            // Fator crítico: > 30° para solos, > 60° para rochas
            float criticalAngle = (soilType == BlockType.Granito || soilType == BlockType.Basalto) ? 60f : 30f;
            float slopeFactor = Mathf.Clamp01(slopeDegrees / criticalAngle);
            
            // Risco final = risco base × (1 + fator de inclinação²)
            return Mathf.Clamp01(baseRisk * (1f + slopeFactor * slopeFactor));
        }
        
        // ==================== CAPACIDADE DE CARGA ====================
        
        /// <summary>
        /// Retorna a capacidade de carga do solo (em tf/m²).
        /// Define quantos andares podem ser construídos sem fundação especial.
        /// </summary>
        /// <param name="soilType">Tipo de solo</param>
        /// <returns>Capacidade de carga admissível (toneladas por metro quadrado)</returns>
        public static float GetBearingCapacity(BlockType soilType) {
            switch (soilType) {
                case BlockType.Granito:
                case BlockType.Basalto:
                    return 100f; // Rocha sã: excelente (permite arranha-céus)
                
                case BlockType.Gneiss:
                    return 80f;
                
                case BlockType.Cascalho:
                    return 40f; // Bom solo
                
                case BlockType.Areia:
                    return 20f; // Razoável (compacta)
                
                case BlockType.Laterita:
                    return 30f; // Boa capacidade quando seca
                
                case BlockType.Terra:
                    return 15f; // Moderada
                
                case BlockType.Argila:
                    return 10f; // Baixa (risco de recalque diferencial)
                
                case BlockType.Arenito:
                    return 50f;
                
                case BlockType.Calcario:
                    return 45f;
                
                case BlockType.Agua:
                    return 0f; // Não suporta carga (requer aterro)
                
                case BlockType.Concreto:
                case BlockType.Asfalto:
                    return 25f; // Depende da base subjacente
                
                default:
                    return 10f;
            }
        }
        
        /// <summary>
        /// Retorna o número máximo de andares recomendado sem fundação profunda.
        /// Baseado na capacidade de carga.
        /// </summary>
        public static int GetMaxFloorsWithoutDeepFoundation(BlockType soilType) {
            float capacity = GetBearingCapacity(soilType);
            
            // Estimativa: cada andar ≈ 10 tf/m² (carga distribuída)
            int maxFloors = Mathf.FloorToInt(capacity / 10f);
            return Mathf.Max(1, maxFloors); // Mínimo 1 andar
        }
        
        // ==================== RECURSOS NATURAIS ====================
        
        /// <summary>
        /// Verifica se o bloco pode ser minerado/extraído para recursos.
        /// </summary>
        public static bool IsMineableResource(BlockType blockType) {
            switch (blockType) {
                case BlockType.Granito:
                case BlockType.Basalto:
                case BlockType.Arenito:
                case BlockType.Calcario:
                    return true; // Pedreira (agregados para construção)
                
                case BlockType.Argila:
                    return true; // Cerâmica, tijolos
                
                case BlockType.Areia:
                    return true; // Construção civil
                
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Retorna o valor econômico do recurso (em $ por m³).
        /// Preços aproximados de Curitiba (2026).
        /// </summary>
        public static float GetResourceValue(BlockType blockType) {
            switch (blockType) {
                case BlockType.Granito:
                    return 150f; // Pedra ornamental (alto valor)
                
                case BlockType.Basalto:
                    return 80f; // Agregado para asfalto/concreto
                
                case BlockType.Arenito:
                    return 60f;
                
                case BlockType.Calcario:
                    return 100f; // Cal, cimento
                
                case BlockType.Argila:
                    return 40f; // Cerâmica
                
                case BlockType.Areia:
                    return 50f; // Construção
                
                default:
                    return 0f;
            }
        }
        
        // ==================== HELPER METHODS ====================
        
        /// <summary>
        /// Retorna uma descrição textual do tipo de solo (para UI/debug).
        /// </summary>
        public static string GetSoilDescription(BlockType soilType) {
            switch (soilType) {
                case BlockType.Ar:
                    return "Ar (vazio)";
                case BlockType.Grama:
                    return "Cobertura vegetal - fácil de construir";
                case BlockType.Terra:
                    return "Solo orgânico - adequado para fundação rasa";
                case BlockType.Argila:
                    return "Argila laterítica - atenção a recalques";
                case BlockType.Areia:
                    return "Solo arenoso - fundação profunda recomendada";
                case BlockType.Cascalho:
                    return "Cascalho - excelente para drenagem";
                case BlockType.Laterita:
                    return "Laterita - solo típico de Curitiba";
                case BlockType.Granito:
                    return "Granito - rocha matriz (escavação custosa)";
                case BlockType.Basalto:
                    return "Basalto - rocha vulcânica";
                case BlockType.Gneiss:
                    return "Gneisse - rocha metamórfica";
                case BlockType.Arenito:
                    return "Arenito - rocha sedimentar";
                case BlockType.Calcario:
                    return "Calcário - risco de cavernas";
                case BlockType.Agua:
                    return "Corpo d'água - aterro necessário";
                case BlockType.Concreto:
                    return "Concreto existente - demolição requerida";
                case BlockType.Asfalto:
                    return "Pavimentação existente";
                default:
                    return "Solo não identificado";
            }
        }
        
        /// <summary>
        /// Retorna a cor de debug para visualização de tipos de solo.
        /// </summary>
        public static Color GetDebugColor(BlockType soilType) {
            switch (soilType) {
                case BlockType.Grama:
                    return new Color(0.3f, 0.7f, 0.3f); // Verde
                case BlockType.Terra:
                    return new Color(0.55f, 0.27f, 0.07f); // Marrom
                case BlockType.Argila:
                    return new Color(0.8f, 0.36f, 0.36f); // Vermelho
                case BlockType.Areia:
                    return new Color(0.96f, 0.64f, 0.38f); // Bege
                case BlockType.Granito:
                    return new Color(0.41f, 0.41f, 0.41f); // Cinza escuro
                case BlockType.Agua:
                    return new Color(0.12f, 0.56f, 1.0f); // Azul
                default:
                    return Color.gray;
            }
        }
    }
}

