using UnityEngine;
using System.Collections.Generic;

namespace Voxel {
    public static class ChunkMeshGenerator {
        /// <summary>
        /// Gera a mesh do chunk sem greedy meshing: desenha todas as faces expostas de cada voxel.
        /// </summary>
        public static Mesh BuildMesh(TerrainWorld world, ChunkData data, float scale) {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int w = ChunkData.Largura;
            int h = data.Altura; // Usa altura dinâmica do chunk
            int d = ChunkData.Largura;

            // Direções das 6 faces (X+, X-, Y+, Y-, Z+, Z-)
            Vector3Int[] directions = new Vector3Int[] {
                new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)
            };

            // Para cada voxel do chunk
            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    for (int z = 0; z < d; z++) {
                        byte v = data.voxels[x, y, z];
                        if (v == 0) continue; // Ar, não desenha
                        Vector3Int pos = new Vector3Int(x, y, z);
                        // Para cada face
                        for (int dir = 0; dir < 6; dir++) {
                            Vector3Int offset = directions[dir];
                            int nx = x + offset.x;
                            int ny = y + offset.y;
                            int nz = z + offset.z;
                            bool faceExposta;
                            
                            // Checa se está dentro do chunk
                            if (nx >= 0 && nx < w && ny >= 0 && ny < h && nz >= 0 && nz < d) {
                                // Vizinho dentro do mesmo chunk
                                faceExposta = (data.voxels[nx, ny, nz] == 0);
                            } else {
                                // Vizinho está em outro chunk ou fora dos limites Y
                                // Consulta o TerrainWorld para culling entre chunks
                                int gX = data.pos.x * ChunkData.Largura + x + offset.x;
                                int gY = y + offset.y;
                                int gZ = data.pos.y * ChunkData.Largura + z + offset.z;
                                
                                // GetVoxelAt retorna 0 (ar) se o chunk não existir ou coordenada inválida
                                faceExposta = (world.GetVoxelAt(gX, gY, gZ) == 0);
                            }
                            
                            if (faceExposta) {
                                AddFace(vertices, triangles, uvs, pos, dir, scale, v);
                            }
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        // Adiciona uma face do cubo na direção dir
        // blockType: tipo do bloco para definir UV mapping
        private static void AddFace(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, Vector3Int pos, int dir, float scale, byte blockType) {
            // Posições dos vértices de um cubo unitário
            Vector3[] faceVerts = new Vector3[4];
            switch (dir) {
                case 0: // X+
                    faceVerts[0] = new Vector3(1, 0, 0);
                    faceVerts[1] = new Vector3(1, 1, 0);
                    faceVerts[2] = new Vector3(1, 1, 1);
                    faceVerts[3] = new Vector3(1, 0, 1);
                    break;
                case 1: // X-
                    faceVerts[0] = new Vector3(0, 0, 1);
                    faceVerts[1] = new Vector3(0, 1, 1);
                    faceVerts[2] = new Vector3(0, 1, 0);
                    faceVerts[3] = new Vector3(0, 0, 0);
                    break;
                case 2: // Y+
                    faceVerts[0] = new Vector3(0, 1, 1);
                    faceVerts[1] = new Vector3(1, 1, 1);
                    faceVerts[2] = new Vector3(1, 1, 0);
                    faceVerts[3] = new Vector3(0, 1, 0);
                    break;
                case 3: // Y-
                    faceVerts[0] = new Vector3(0, 0, 0);
                    faceVerts[1] = new Vector3(1, 0, 0);
                    faceVerts[2] = new Vector3(1, 0, 1);
                    faceVerts[3] = new Vector3(0, 0, 1);
                    break;
                case 4: // Z+
                    faceVerts[0] = new Vector3(0, 0, 1);
                    faceVerts[1] = new Vector3(1, 0, 1);
                    faceVerts[2] = new Vector3(1, 1, 1);
                    faceVerts[3] = new Vector3(0, 1, 1);
                    break;
                case 5: // Z-
                    faceVerts[0] = new Vector3(1, 0, 0);
                    faceVerts[1] = new Vector3(0, 0, 0);
                    faceVerts[2] = new Vector3(0, 1, 0);
                    faceVerts[3] = new Vector3(1, 1, 0);
                    break;
            }
            int vCount = vertices.Count;
            for (int i = 0; i < 4; i++) {
                vertices.Add((pos + faceVerts[i]) * scale);
            }
            
            // ==================== UV MAPPING POR BLOCKTYPE ====================
            // Calcula UVs baseado em um Texture Atlas (grade de texturas)
            // Exemplo: Atlas 8x8 = 64 texturas diferentes
            AddFaceUVs(uvs, blockType);
            
            triangles.Add(vCount);
            triangles.Add(vCount + 1);
            triangles.Add(vCount + 2);
            triangles.Add(vCount);
            triangles.Add(vCount + 2);
            triangles.Add(vCount + 3);
        }

        /// <summary>
        /// Calcula coordenadas UV para um bloco baseado em texture atlas.
        /// Atlas configurado como grade 8x8 (64 texturas).
        /// Cada BlockType mapeia para uma célula do atlas via índice sequencial.
        /// 
        /// Layout do Atlas (8x8):
        ///   Linha 0: Ar, Grama, GramaDesgastada, Terra, CaminhoDeTerra, Argila, Areia, Cascalho
        ///   Linha 1: Laterita, Lama, Turfa, Granito, Diorito, Andesito, Basalto, Gneiss
        ///   Linha 2: Migmatito, Quartzito, Marmore, Arenito, Calcario, Xisto, Ardosia, Siltito
        ///   Linha 3: Concreto, Asfalto, Paralelepipedo, CaminhoDePedra, Tijolo, Cimento, Piso, Madeira
        ///   Linha 4: Agua, AguaRasa, Vegetacao, Musgo, Folhagem, Raiz, Neve, Gelo
        ///   Linha 5: Rocha, Pedregulho, Minerio, Ferro, Cobre, Ouro, Carvao, Cristal
        ///   Linha 6: Trilho, Ferrugem, Vidro, Metal, Ceramica, Plastico, Borracha, Tecido
        ///   Linha 7: Reservado para expansão
        /// </summary>
        private static void AddFaceUVs(List<Vector2> uvs, byte blockType) {
            // Configuração do atlas: 8x8 texturas
            const int atlasSize = 8;
            const float tileSize = 1.0f / atlasSize;
            
            // Padding para evitar bleeding entre tiles (ajuste conforme resolução do atlas)
            // Para atlas 512x512 com tiles 64x64, use ~0.5 pixel = 0.001
            // Para atlas 1024x1024 com tiles 128x128, use ~0.5 pixel = 0.0005
            const float padding = 0.001f;
            
            // Mapeia blockType diretamente para posição no atlas (índices sequenciais 0-63)
            int tileIndex = blockType;
            int row = tileIndex / atlasSize;
            int col = tileIndex % atlasSize;
            
            // Calcula UV base (canto inferior esquerdo da textura)
            // Unity usa Y=0 em baixo, então row 0 fica em v=0
            float u = col * tileSize;
            float v = row * tileSize;
            
            // Aplica padding para evitar bleeding nas bordas
            float uMin = u + padding;
            float uMax = u + tileSize - padding;
            float vMin = v + padding;
            float vMax = v + tileSize - padding;
            
            // Adiciona os 4 vértices da face (quad)
            // Ordem deve corresponder à ordem dos vértices em AddFace
            uvs.Add(new Vector2(uMin, vMin));  // 0: baixo-esquerdo
            uvs.Add(new Vector2(uMin, vMax));  // 1: cima-esquerdo
            uvs.Add(new Vector2(uMax, vMax));  // 2: cima-direito
            uvs.Add(new Vector2(uMax, vMin));  // 3: baixo-direito
        }
    }
}