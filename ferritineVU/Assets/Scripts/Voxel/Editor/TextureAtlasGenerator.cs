using UnityEngine;
using UnityEditor;
using System.IO;

namespace Voxel {
    /// <summary>
    /// Editor utility para gerar um Texture Atlas de placeholder com cores sólidas.
    /// Use isso para testar o sistema de voxels antes de criar as texturas finais.
    /// 
    /// Uso: Menu > Voxel > Generate Placeholder Atlas
    /// </summary>
    public static class TextureAtlasGenerator {
        
        // Tamanho do atlas (8x8 tiles)
        private const int ATLAS_SIZE = 8;
        private const int TILE_SIZE = 64; // Cada tile 64x64 pixels
        private const int TEXTURE_SIZE = ATLAS_SIZE * TILE_SIZE; // 512x512
        
        /// <summary>
        /// Paleta de cores para cada BlockType (índice = valor do enum)
        /// Cores baseadas na geologia e urbanismo de Curitiba/PR
        /// </summary>
        private static readonly Color32[] BlockColors = new Color32[64] {
            // === Linha 0: Terrenos naturais básicos ===
            new Color32(0, 0, 0, 0),         // 0: Ar (transparente)
            new Color32(76, 175, 80, 255),   // 1: Grama (verde vivo)
            new Color32(139, 195, 74, 255),  // 2: GramaDesgastada (verde amarelado)
            new Color32(121, 85, 72, 255),   // 3: Terra (marrom)
            new Color32(161, 136, 127, 255), // 4: CaminhoDeTerra (marrom claro)
            new Color32(191, 54, 12, 255),   // 5: Argila (laranja queimado)
            new Color32(215, 204, 200, 255), // 6: Areia (bege)
            new Color32(158, 158, 158, 255), // 7: Cascalho (cinza)
            
            // === Linha 1: Solos especiais e rochas ígneas ===
            new Color32(198, 40, 40, 255),   // 8: Laterita (vermelho ferroso)
            new Color32(78, 52, 46, 255),    // 9: Lama (marrom escuro)
            new Color32(33, 33, 33, 255),    // 10: Turfa (preto orgânico)
            new Color32(189, 189, 189, 255), // 11: Granito (cinza mosqueado)
            new Color32(117, 117, 117, 255), // 12: Diorito (cinza médio)
            new Color32(97, 97, 97, 255),    // 13: Andesito (cinza)
            new Color32(66, 66, 66, 255),    // 14: Basalto (preto/cinza escuro)
            new Color32(141, 141, 141, 255), // 15: Gneiss (cinza listrado)
            
            // === Linha 2: Rochas metamórficas e sedimentares ===
            new Color32(161, 136, 127, 255), // 16: Migmatito (bege/cinza)
            new Color32(245, 245, 245, 255), // 17: Quartzito (branco)
            new Color32(250, 250, 250, 255), // 18: Marmore (branco com veios)
            new Color32(188, 170, 164, 255), // 19: Arenito (bege texturizado)
            new Color32(238, 238, 238, 255), // 20: Calcario (branco acinzentado)
            new Color32(96, 125, 139, 255),  // 21: Xisto (cinza azulado)
            new Color32(69, 90, 100, 255),   // 22: Ardosia (cinza escuro azul)
            new Color32(141, 110, 99, 255),  // 23: Siltito (marrom acinzentado)
            
            // === Linha 3: Urbanos / Construídos ===
            new Color32(189, 189, 189, 255), // 24: Concreto (cinza claro)
            new Color32(55, 71, 79, 255),    // 25: Asfalto (cinza escuro)
            new Color32(96, 125, 139, 255),  // 26: Paralelepipedo (cinza irregular)
            new Color32(141, 141, 141, 255), // 27: CaminhoDePedra (cinza médio)
            new Color32(230, 74, 25, 255),   // 28: Tijolo (vermelho tijolo)
            new Color32(158, 158, 158, 255), // 29: Cimento (cinza)
            new Color32(215, 204, 200, 255), // 30: Piso (bege liso)
            new Color32(141, 110, 99, 255),  // 31: Madeira (marrom madeira)
            
            // === Linha 4: Água e vegetação ===
            new Color32(25, 118, 210, 200),  // 32: Agua (azul escuro, semi-transparente)
            new Color32(100, 181, 246, 180), // 33: AguaRasa (azul claro, semi-transparente)
            new Color32(46, 125, 50, 255),   // 34: Vegetacao (verde floresta)
            new Color32(85, 139, 47, 255),   // 35: Musgo (verde musgo)
            new Color32(104, 159, 56, 255),  // 36: Folhagem (verde folha)
            new Color32(93, 64, 55, 255),    // 37: Raiz (marrom escuro)
            new Color32(255, 255, 255, 255), // 38: Neve (branco)
            new Color32(179, 229, 252, 200), // 39: Gelo (azul claro transparente)
            
            // === Linha 5: Rochas e minérios ===
            new Color32(117, 117, 117, 255), // 40: Rocha (cinza)
            new Color32(97, 97, 97, 255),    // 41: Pedregulho (cinza irregular)
            new Color32(120, 144, 156, 255), // 42: Minerio (cinza com brilho)
            new Color32(121, 85, 72, 255),   // 43: Ferro (marrom metálico)
            new Color32(230, 126, 34, 255),  // 44: Cobre (laranja metálico)
            new Color32(255, 193, 7, 255),   // 45: Ouro (amarelo dourado)
            new Color32(33, 33, 33, 255),    // 46: Carvao (preto)
            new Color32(224, 247, 250, 255), // 47: Cristal (azul cristalino)
            
            // === Linha 6: Materiais industriais/urbanos ===
            new Color32(78, 52, 46, 255),    // 48: Trilho (metal + madeira)
            new Color32(191, 87, 0, 255),    // 49: Ferrugem (laranja escuro)
            new Color32(179, 229, 252, 100), // 50: Vidro (transparente azulado)
            new Color32(176, 190, 197, 255), // 51: Metal (cinza brilhante)
            new Color32(255, 224, 178, 255), // 52: Ceramica (bege cerâmico)
            new Color32(255, 255, 255, 255), // 53: Plastico (branco)
            new Color32(33, 33, 33, 255),    // 54: Borracha (preto)
            new Color32(206, 147, 216, 255), // 55: Tecido (roxo claro)
            
            // === Linha 7: Reservado para expansão ===
            new Color32(255, 0, 255, 255),   // 56: Reservado (magenta - debug)
            new Color32(255, 0, 255, 255),   // 57: Reservado
            new Color32(255, 0, 255, 255),   // 58: Reservado
            new Color32(255, 0, 255, 255),   // 59: Reservado
            new Color32(255, 0, 255, 255),   // 60: Reservado
            new Color32(255, 0, 255, 255),   // 61: Reservado
            new Color32(255, 0, 255, 255),   // 62: Reservado
            new Color32(255, 0, 255, 255),   // 63: Reservado
        };
        
#if UNITY_EDITOR
        [MenuItem("Voxel/Generate Placeholder Atlas")]
        public static void GeneratePlaceholderAtlas() {
            Texture2D atlas = new Texture2D(TEXTURE_SIZE, TEXTURE_SIZE, TextureFormat.RGBA32, true);
            atlas.filterMode = FilterMode.Point;
            atlas.wrapMode = TextureWrapMode.Clamp;
            
            // Preencher cada tile
            for (int tileIndex = 0; tileIndex < 64; tileIndex++) {
                int row = tileIndex / ATLAS_SIZE;
                int col = tileIndex % ATLAS_SIZE;
                
                Color32 baseColor = BlockColors[tileIndex];
                
                // Preencher o tile com a cor base e adicionar variação
                for (int y = 0; y < TILE_SIZE; y++) {
                    for (int x = 0; x < TILE_SIZE; x++) {
                        int pixelX = col * TILE_SIZE + x;
                        int pixelY = row * TILE_SIZE + y;
                        
                        // Adicionar pequena variação de cor para textura
                        Color32 pixelColor = AddNoiseToColor(baseColor, x, y, tileIndex);
                        
                        // Adicionar borda sutil para debug (1px)
                        if (x == 0 || y == 0 || x == TILE_SIZE - 1 || y == TILE_SIZE - 1) {
                            pixelColor = DarkenColor(pixelColor, 0.8f);
                        }
                        
                        atlas.SetPixel(pixelX, pixelY, pixelColor);
                    }
                }
            }
            
            atlas.Apply();
            
            // Salvar como PNG
            string path = "Assets/Textures/VoxelAtlas_Placeholder.png";
            
            // Criar diretório se não existir
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory)) {
                Directory.CreateDirectory(directory);
            }
            
            byte[] pngData = atlas.EncodeToPNG();
            File.WriteAllBytes(path, pngData);
            
            AssetDatabase.Refresh();
            
            // Configurar import settings
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null) {
                importer.textureType = TextureImporterType.Default;
                importer.filterMode = FilterMode.Point;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.mipmapEnabled = true;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
            
            Debug.Log($"✅ Atlas de placeholder gerado em: {path}\n" +
                      $"   Tamanho: {TEXTURE_SIZE}x{TEXTURE_SIZE}\n" +
                      $"   Tiles: {ATLAS_SIZE}x{ATLAS_SIZE} ({ATLAS_SIZE * ATLAS_SIZE} texturas)\n" +
                      $"   Tile Size: {TILE_SIZE}x{TILE_SIZE} pixels\n\n" +
                      $"Próximos passos:\n" +
                      $"1. Crie um Material (Create > Material)\n" +
                      $"2. Arraste VoxelAtlas_Placeholder.png para o slot 'Base Map'\n" +
                      $"3. Atribua o material ao campo 'voxelMaterial' do TerrainWorld");
            
            // Selecionar o asset criado
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }
        
        /// <summary>
        /// Adiciona ruído sutil à cor para dar textura
        /// </summary>
        private static Color32 AddNoiseToColor(Color32 baseColor, int x, int y, int seed) {
            // Ruído baseado em posição e seed
            float noise = Mathf.PerlinNoise((x + seed * 7) * 0.1f, (y + seed * 13) * 0.1f);
            float variation = (noise - 0.5f) * 20f; // ±10 de variação
            
            return new Color32(
                (byte)Mathf.Clamp(baseColor.r + variation, 0, 255),
                (byte)Mathf.Clamp(baseColor.g + variation, 0, 255),
                (byte)Mathf.Clamp(baseColor.b + variation, 0, 255),
                baseColor.a
            );
        }
        
        /// <summary>
        /// Escurece uma cor pelo fator especificado
        /// </summary>
        private static Color32 DarkenColor(Color32 color, float factor) {
            return new Color32(
                (byte)(color.r * factor),
                (byte)(color.g * factor),
                (byte)(color.b * factor),
                color.a
            );
        }
#endif
    }
}

