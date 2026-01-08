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
    // IMPORTANTE: Índices sequenciais (0-63) para mapear corretamente no Texture Atlas 8x8
    // Layout do Atlas:
    //   Linha 0 (0-7):   Ar, Grama, GramaDesgastada, Terra, CaminhoDeTerra, Argila, Areia, Cascalho
    //   Linha 1 (8-15):  Laterita, Lama, Turfa, Granito, Diorito, Andesito, Basalto, Gneiss
    //   Linha 2 (16-23): Migmatito, Quartzito, Marmore, Arenito, Calcario, Xisto, Ardosia, Siltito
    //   Linha 3 (24-31): Concreto, Asfalto, Paralelepipedo, CaminhoDePedra, Tijolo, Cimento, Piso, Madeira
    //   Linha 4 (32-39): Agua, AguaRasa, Vegetacao, Musgo, Folhagem, Raiz, Neve, Gelo
    //   Linha 5 (40-47): Rocha, Pedregulho, Minerio, Ferro, Cobre, Ouro, Carvao, Cristal
    //   Linha 6 (48-55): Trilho, Ferrugem, Vidro, Metal, Ceramica, Plastico, Borracha, Tecido
    //   Linha 7 (56-63): Reservado para expansão futura
    public enum BlockType : byte {
        // === Linha 0: Terrenos naturais básicos ===
        Ar = 0,
        Grama = 1,
        GramaDesgastada = 2,
        Terra = 3,
        CaminhoDeTerra = 4,
        Argila = 5,
        Areia = 6,
        Cascalho = 7,

        // === Linha 1: Solos especiais e rochas ígneas ===
        Laterita = 8,
        Lama = 9,
        Turfa = 10,
        Granito = 11,
        Diorito = 12,
        Andesito = 13,
        Basalto = 14,
        Gneiss = 15,

        // === Linha 2: Rochas metamórficas e sedimentares ===
        Migmatito = 16,
        Quartzito = 17,
        Marmore = 18,
        Arenito = 19,
        Calcario = 20,
        Xisto = 21,
        Ardosia = 22,
        Siltito = 23,

        // === Linha 3: Urbanos / Construídos ===
        Concreto = 24,
        Asfalto = 25,
        Paralelepipedo = 26,
        CaminhoDePedra = 27,
        Tijolo = 28,
        Cimento = 29,
        Piso = 30,
        Madeira = 31,

        // === Linha 4: Água e vegetação ===
        Agua = 32,
        AguaRasa = 33,
        Vegetacao = 34,
        Musgo = 35,
        Folhagem = 36,
        Raiz = 37,
        Neve = 38,
        Gelo = 39,

        // === Linha 5: Rochas e minérios ===
        Rocha = 40,
        Pedregulho = 41,
        Minerio = 42,
        Ferro = 43,
        Cobre = 44,
        Ouro = 45,
        Carvao = 46,
        Cristal = 47,

        // === Linha 6: Materiais industriais/urbanos ===
        Trilho = 48,
        Ferrugem = 49,
        Vidro = 50,
        Metal = 51,
        Ceramica = 52,
        Plastico = 53,
        Borracha = 54,
        Tecido = 55,

        // === Linha 7: Reservado para expansão ===
        Reservado56 = 56,
        Reservado57 = 57,
        Reservado58 = 58,
        Reservado59 = 59,
        Reservado60 = 60,
        Reservado61 = 61,
        Reservado62 = 62,
        Reservado63 = 63
    }
}