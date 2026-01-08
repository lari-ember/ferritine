using UnityEngine;
using Voxel;

/// <summary>
/// Sistema de exemplo para usar a geologia de Curitiba no gameplay.
/// Demonstra cálculo de custos, drenagem e restrições de construção.
/// </summary>
public class GeologyGameplayExample : MonoBehaviour
{
    [Header("Referencias")]
    public TerrainWorld terrainWorld;
    
    [Header("Configuração de Custos")]
    [Tooltip("Custo base de construção por tipo de solo")]
    public float costGrama = 100f;
    public float costTerra = 120f;
    public float costArgila = 150f;
    public float costGranito = 300f;
    public float costAreia = 140f;
    
    [Header("Configuração de Drenagem")]
    [Tooltip("Chuva em mm/h (Curitiba média: 1500mm/ano)")]
    public float rainfallIntensity = 10f;
    [Tooltip("Limite de escoamento para enchente")]
    public float floodThreshold = 0.5f;

    // ==================== SISTEMA DE CUSTOS ====================
    
    /// <summary>
    /// Calcula o custo de fundação baseado no tipo de solo.
    /// Solos mais difíceis de escavar custam mais.
    /// </summary>
    public float CalculateFoundationCost(int x, int z)
    {
        BlockType soilType = terrainWorld.GetSoilBlockType(x, z);
        SoilStats stats = SoilProperties.Get(soilType);
        
        float baseCost = GetBaseCost(soilType);
        
        // Modificador por capacidade de suporte
        // Solos ruins precisam de reforço (custo adicional)
        if (stats.bearingCapacity < 60f)
        {
            baseCost *= 1.3f; // +30% para solos fracos
        }
        
        // Modificador por permeabilidade
        // Solos impermeáveis precisam de drenagem
        if (stats.permeability < 0.1f)
        {
            baseCost += 50f; // Custo de sistema de drenagem
        }
        
        return baseCost;
    }
    
    private float GetBaseCost(BlockType type)
    {
        switch (type)
        {
            case BlockType.Grama:
            case BlockType.Vegetacao:
                return costGrama;
            
            case BlockType.Terra:
                return costTerra;
            
            case BlockType.Argila:
            case BlockType.Laterita:
                return costArgila;
            
            case BlockType.Granito:
            case BlockType.Basalto:
            case BlockType.Rocha:
                return costGranito;
            
            case BlockType.Areia:
                return costAreia;
            
            case BlockType.Agua:
                return float.MaxValue; // Não pode construir em água
            
            default:
                return costTerra;
        }
    }

    // ==================== SISTEMA DE DRENAGEM ====================
    
    /// <summary>
    /// Simula chuva e calcula risco de enchente.
    /// Retorna quantidade de água que não foi absorvida (escoamento).
    /// </summary>
    public float SimulateRainfall(int x, int z, float rainfall)
    {
        SoilStats stats = terrainWorld.GetSoilStats(x, z);
        
        // Água absorvida pelo solo
        float absorbed = rainfall * stats.permeability;
        
        // Água que escoa (runoff)
        float runoff = rainfall - absorbed;
        
        return runoff;
    }
    
    /// <summary>
    /// Verifica se uma área está em risco de enchente.
    /// </summary>
    public bool IsFloodRisk(int x, int z)
    {
        float runoff = SimulateRainfall(x, z, rainfallIntensity);
        return runoff > floodThreshold;
    }
    
    /// <summary>
    /// Calcula o custo de infraestrutura de drenagem para uma área.
    /// </summary>
    public float CalculateDrainageCost(int x, int z)
    {
        SoilStats stats = terrainWorld.GetSoilStats(x, z);
        
        // Quanto menor a permeabilidade, maior o custo de drenagem
        float drainageFactor = 1.0f - stats.permeability;
        float baseDrainageCost = 200f;
        
        return baseDrainageCost * drainageFactor;
    }

    // ==================== SISTEMA DE VEGETAÇÃO ====================
    
    /// <summary>
    /// Verifica se é possível plantar vegetação em uma posição.
    /// </summary>
    public bool CanPlantVegetation(int x, int z)
    {
        SoilStats stats = terrainWorld.GetSoilStats(x, z);
        return stats.vegetationFriendly;
    }
    
    /// <summary>
    /// Calcula o custo de criar um parque/área verde.
    /// </summary>
    public float CalculateParkCost(int x, int z, int width, int height)
    {
        float totalCost = 0f;
        
        for (int dx = 0; dx < width; dx++)
        {
            for (int dz = 0; dz < height; dz++)
            {
                int posX = x + dx;
                int posZ = z + dz;
                
                SoilStats stats = terrainWorld.GetSoilStats(posX, posZ);
                
                // Custo base de terraplanagem
                float baseCost = 50f;
                
                // Se solo não é amigável para vegetação, precisa de tratamento
                if (!stats.vegetationFriendly)
                {
                    baseCost += 100f; // Adicionar terra vegetal
                }
                
                totalCost += baseCost;
            }
        }
        
        return totalCost;
    }

    // ==================== SISTEMA DE CONSTRUÇÃO ====================
    
    /// <summary>
    /// Verifica se é possível construir em uma posição.
    /// Retorna true se permitido, false caso contrário.
    /// </summary>
    public bool CanBuild(int x, int z, out string reason)
    {
        BlockType soilType = terrainWorld.GetSoilBlockType(x, z);
        SoilStats stats = SoilProperties.Get(soilType);
        
        // Não pode construir em água
        if (soilType == BlockType.Agua)
        {
            reason = "Não é possível construir em água";
            return false;
        }
        
        // Verifica capacidade de suporte mínima
        if (stats.bearingCapacity < 40f)
        {
            reason = "Solo muito fraco (capacidade de suporte insuficiente)";
            return false;
        }
        
        // Verifica risco de erosão
        if (stats.erosionRate > 0.7f)
        {
            float slope = terrainWorld.GetSlope(x, z);
            if (slope > 2f) // Inclinação muito alta
            {
                reason = "Área com alto risco de erosão e inclinação excessiva";
                return false;
            }
        }
        
        reason = "OK";
        return true;
    }
    
    /// <summary>
    /// Calcula o tempo necessário para escavar até uma profundidade.
    /// Retorna tempo em horas (pode ser convertido para dias de gameplay).
    /// </summary>
    public float CalculateExcavationTime(int x, int z, int depth)
    {
        float totalTime = 0f;
        
        // Simula escavação camada por camada
        int surfaceHeight = Mathf.FloorToInt(terrainWorld.GetHeight(x, z) / terrainWorld.escalaVoxel);
        
        for (int y = surfaceHeight; y > surfaceHeight - depth && y >= 0; y--)
        {
            // Determina o tipo de bloco nesta profundidade
            BlockType blockType = DetermineBlockTypeAtDepth(surfaceHeight, y);
            
            // Tempo baseado na dureza do material
            float timePerBlock = GetExcavationTime(blockType);
            totalTime += timePerBlock;
        }
        
        return totalTime;
    }
    
    private BlockType DetermineBlockTypeAtDepth(int surfaceHeight, int y)
    {
        int depthFromSurface = surfaceHeight - y;
        
        if (depthFromSurface == 0)
            return BlockType.Grama;
        else if (depthFromSurface <= 5)
            return BlockType.Terra;
        else if (depthFromSurface <= 12)
            return BlockType.Argila;
        else
            return BlockType.Granito;
    }
    
    private float GetExcavationTime(BlockType type)
    {
        switch (type)
        {
            case BlockType.Grama:
            case BlockType.Terra:
                return 0.5f; // 30 minutos por bloco
            
            case BlockType.Argila:
            case BlockType.Laterita:
                return 1.0f; // 1 hora por bloco
            
            case BlockType.Areia:
                return 0.3f; // 18 minutos (rápido mas instável)
            
            case BlockType.Granito:
            case BlockType.Basalto:
            case BlockType.Rocha:
                return 5.0f; // 5 horas (requer explosivos)
            
            default:
                return 1.0f;
        }
    }

    // ==================== EXEMPLO DE USO ====================
    
    void Start()
    {
        if (terrainWorld == null)
        {
            terrainWorld = FindFirstObjectByType<TerrainWorld>();
        }
        
        // Exemplo: Testar construção em posição (100, 100)
        TestConstructionAt(100, 100);
    }
    
    void TestConstructionAt(int x, int z)
    {
        Debug.Log($"=== Análise de Terreno em ({x}, {z}) ===");
        
        // 1. Tipo de solo
        BlockType soilType = terrainWorld.GetSoilBlockType(x, z);
        Debug.Log($"Tipo de solo: {soilType}");
        
        // 2. Propriedades físicas
        SoilStats stats = terrainWorld.GetSoilStats(x, z);
        Debug.Log($"Permeabilidade: {stats.permeability:F2}");
        Debug.Log($"Capacidade de suporte: {stats.bearingCapacity:F0} kPa");
        Debug.Log($"Taxa de erosão: {stats.erosionRate:F2}");
        Debug.Log($"Amigável para vegetação: {stats.vegetationFriendly}");
        
        // 3. Viabilidade de construção
        if (CanBuild(x, z, out string reason))
        {
            float cost = CalculateFoundationCost(x, z);
            Debug.Log($"✅ Construção permitida");
            Debug.Log($"💰 Custo estimado: ${cost:F2}");
            
            float excavationTime = CalculateExcavationTime(x, z, 10);
            Debug.Log($"⏱️ Tempo de escavação (10 blocos): {excavationTime:F1} horas");
        }
        else
        {
            Debug.Log($"❌ Construção não permitida: {reason}");
        }
        
        // 4. Risco de enchente
        if (IsFloodRisk(x, z))
        {
            float drainageCost = CalculateDrainageCost(x, z);
            Debug.Log($"⚠️ RISCO DE ENCHENTE!");
            Debug.Log($"💧 Custo de drenagem: ${drainageCost:F2}");
        }
        
        // 5. Viabilidade de parque
        if (CanPlantVegetation(x, z))
        {
            Debug.Log($"🌳 Adequado para área verde");
        }
    }
}

