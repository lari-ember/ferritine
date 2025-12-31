using UnityEngine;

namespace Voxel {
    // Enum para os tipos de zona (expandido com granularidade do GDD / backend)
    public enum ZonaTipo {
        Nenhuma,

        // Residenciais por densidade (R1/R2/R3 equivalentes)
        ResidencialBaixaDensidade,
        ResidencialMediaDensidade,
        ResidencialAltaDensidade,

        // Comerciais
        ComercialLocal,
        ComercialCentral,

        // Industrial
        IndustrialLeve,
        IndustrialPesada,

        // Uso misto / rural / especial
        Misto,
        Rural,
        Agricultura,
        Especial,

        // Institucional / Infraestrutura / Recreação
        Institucional,   // escolas, hospitais, órgãos públicos
        Infraestrutura,  // estações, pontes, viadutos
        Parque,

        // Transporte / Via pública
        Via,

        // Reservado para extensões futuras (ex: turismo, zona histórica)
        Historico,
        Turismo
    }

    // Classe de dados para o Lote
    [System.Serializable]
    public class Lote {
        public Vector2Int pos;
        public ZonaTipo zona;
        public bool estaValido;
        public string motivo;
    }

    // Enum auxiliar para tipos de bloco (baseado em geologia/solos brasileiros + urbanos)
    public enum BlockType : byte {
        Ar = 0,
        Grama = 1,
        Terra = 2,
        Argila = 3,
        Areia = 4,
        Cascalho = 5,
        Laterita = 6,

        // Rochas ígneas e metamórficas comuns
        Granito = 10,
        Diorito = 11,
        Andesito = 12,
        Basalto = 13,
        Gneiss = 14,
        Migmatito = 15,

        // Sedimentares
        Arenito = 20,
        Calcario = 21,

        // Urbanos / Construídos
        Concreto = 30,
        Asfalto = 31,

        // Água e vegetação
        Agua = 40,
        Vegetacao = 41,

        // Genérico para pedras/rochas não especificadas
        Rocha = 50
    }
}