namespace Voxel {
    // Estrutura com atributos físicos/semânticos do solo
    [System.Serializable]
    public struct SoilStats {
        // permeabilidade do solo (0..1). Valores altos = água escoa facilmente.
        public float permeability;    // 0..1 (maior = mais permeável)
        // taxa relativa de erosão (0..1). Valores altos = mais sujeita a ser erodida
        public float erosionRate;     // 0..1 (maior = erosão rápida)
        // capacidade de suporte aproximada (kPa) para decisões simplificadas de construção
        public float bearingCapacity; // kPa (capacidade de suporte)
        // se o solo favorece crescimento/plantio
        public bool vegetationFriendly; // vegetal cresce bem
        // nota descritiva para debugging/edição
        public string note;
    }

    // Mapeamento centralizado entre BlockType e propriedades do solo.
    // Use isto para decisões de gameplay e física.
    public static class SoilProperties {
        public static SoilStats Get(BlockType t) {
            switch (t) {
                case BlockType.Argila:
                    return new SoilStats { permeability = 0.05f, erosionRate = 0.6f, bearingCapacity = 60f, vegetationFriendly = true, note = "Argila: baixa permeabilidade, compacta" };
                case BlockType.Areia:
                    return new SoilStats { permeability = 0.8f, erosionRate = 0.7f, bearingCapacity = 40f, vegetationFriendly = false, note = "Areia: muito permeável, baixa capacidade de suporte" };
                case BlockType.Cascalho:
                    return new SoilStats { permeability = 0.9f, erosionRate = 0.2f, bearingCapacity = 120f, vegetationFriendly = false, note = "Cascalho: ótimo para drenagem, bom suporte" };
                case BlockType.Laterita:
                    return new SoilStats { permeability = 0.3f, erosionRate = 0.8f, bearingCapacity = 50f, vegetationFriendly = false, note = "Laterita: suscetível à erosão quando removida" };
                case BlockType.Granito:
                case BlockType.Diorito:
                case BlockType.Basalto:
                case BlockType.Gneiss:
                case BlockType.Migmatito:
                case BlockType.Rocha:
                    return new SoilStats { permeability = 0.02f, erosionRate = 0.05f, bearingCapacity = 300f, vegetationFriendly = false, note = "Rocha sólida: pouca permeabilidade, excelente suporte" };
                case BlockType.Arenito:
                    return new SoilStats { permeability = 0.4f, erosionRate = 0.3f, bearingCapacity = 140f, vegetationFriendly = true, note = "Arenito: sedimentar com boa drenagem" };
                case BlockType.Calcario:
                    return new SoilStats { permeability = 0.2f, erosionRate = 0.25f, bearingCapacity = 160f, vegetationFriendly = true, note = "Calcário: suscetível a dissolução em água ácida" };
                case BlockType.Terra:
                case BlockType.Grama:
                case BlockType.Vegetacao:
                    return new SoilStats { permeability = 0.5f, erosionRate = 0.2f, bearingCapacity = 80f, vegetationFriendly = true, note = "Solo superficial: bom para vegetação" };
                case BlockType.Concreto:
                case BlockType.Asfalto:
                    return new SoilStats { permeability = 0.01f, erosionRate = 0.01f, bearingCapacity = 1000f, vegetationFriendly = false, note = "Pavimento urbano" };
                case BlockType.Agua:
                    return new SoilStats { permeability = 1.0f, erosionRate = 1.0f, bearingCapacity = 0f, vegetationFriendly = false, note = "Água" };
                default:
                    return new SoilStats { permeability = 0.5f, erosionRate = 0.5f, bearingCapacity = 80f, vegetationFriendly = true, note = "Genérico" };
            }
        }
    }
}
