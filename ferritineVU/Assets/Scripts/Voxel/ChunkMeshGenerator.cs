using UnityEngine;
using System.Collections.Generic;

namespace Voxel {
    /// <summary>
    /// Gerador de mesh para chunks de voxel.
    /// 
    /// Responsável por converter dados de voxel (ChunkData) em meshes renderizáveis.
    /// Utiliza face culling para só desenhar faces visíveis (expostas ao ar).
    /// 
    /// Otimizações implementadas:
    /// - Arrays estáticos para direções e vértices (evita GC)
    /// - UV mapping via texture atlas 8x8
    /// - Consulta cross-chunk para culling correto nas bordas
    /// </summary>
    public static class ChunkMeshGenerator {
        
        #region Dados Estáticos (Evita Alocações)
        
        /// <summary>
        /// Direções das 6 faces do cubo: X+, X-, Y+, Y-, Z+, Z-
        /// </summary>
        private static readonly Vector3Int[] Directions = {
            new Vector3Int(1, 0, 0),   // 0: X+ (direita)
            new Vector3Int(-1, 0, 0),  // 1: X- (esquerda)
            new Vector3Int(0, 1, 0),   // 2: Y+ (cima)
            new Vector3Int(0, -1, 0),  // 3: Y- (baixo)
            new Vector3Int(0, 0, 1),   // 4: Z+ (frente)
            new Vector3Int(0, 0, -1)   // 5: Z- (trás)
        };
        
        /// <summary>
        /// Vértices para cada face do cubo (4 vértices por face).
        /// Pré-calculados para evitar switch em runtime.
        /// </summary>
        private static readonly Vector3[][] FaceVertices = {
            // Face X+ (direita)
            new[] { new Vector3(1, 0, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(1, 0, 1) },
            // Face X- (esquerda)
            new[] { new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(0, 1, 0), new Vector3(0, 0, 0) },
            // Face Y+ (cima)
            new[] { new Vector3(0, 1, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0), new Vector3(0, 1, 0) },
            // Face Y- (baixo)
            new[] { new Vector3(0, 0, 0), new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(0, 0, 1) },
            // Face Z+ (frente)
            new[] { new Vector3(0, 0, 1), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(0, 1, 1) },
            // Face Z- (trás)
            new[] { new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0) }
        };
        
        /// <summary>
        /// Configuração do texture atlas.
        /// </summary>
        private const int AtlasSize = 8;
        private const float TileSize = 1.0f / AtlasSize;
        private const float UVPadding = 0.001f; // Evita bleeding entre tiles
        
        #endregion
        
        /// <summary>
        /// Gera a mesh do chunk renderizando todas as faces expostas.
        /// Faces são consideradas expostas quando o voxel vizinho é ar (0).
        /// </summary>
        /// <param name="world">Referência ao TerrainWorld para consulta cross-chunk</param>
        /// <param name="data">Dados do chunk a ser renderizado</param>
        /// <param name="scale">Escala do voxel em metros</param>
        /// <returns>Mesh pronta para renderização</returns>
        public static Mesh BuildMesh(TerrainWorld world, ChunkData data, float scale) {
            // Listas para construir a mesh
            List<Vector3> vertices = new List<Vector3>(4096);  // Capacidade inicial estimada
            List<int> triangles = new List<int>(6144);
            List<Vector2> uvs = new List<Vector2>(4096);

            int w = ChunkData.Largura;
            int h = data.Altura;
            int d = ChunkData.Largura;
            
            // Posição global do chunk para consultas cross-chunk
            int chunkGlobalX = data.pos.x * ChunkData.Largura;
            int chunkGlobalZ = data.pos.y * ChunkData.Largura;

            // Itera sobre todos os voxels do chunk
            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    for (int z = 0; z < d; z++) {
                        byte voxelType = data.voxels[x, y, z];
                        if (voxelType == 0) continue; // Ar, pula
                        
                        // Verifica cada uma das 6 faces
                        for (int dir = 0; dir < 6; dir++) {
                            if (IsFaceExposed(data, world, x, y, z, dir, w, h, d, chunkGlobalX, chunkGlobalZ)) {
                                AddFace(vertices, triangles, uvs, x, y, z, dir, scale, voxelType);
                            }
                        }
                    }
                }
            }

            // Cria e configura a mesh
            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // Suporta meshes grandes
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            
            return mesh;
        }
        
        /// <summary>
        /// Verifica se uma face está exposta (vizinho é ar ou borda do chunk).
        /// </summary>
        private static bool IsFaceExposed(ChunkData data, TerrainWorld world, 
            int x, int y, int z, int dir, int w, int h, int d, int chunkGlobalX, int chunkGlobalZ) {
            
            Vector3Int offset = Directions[dir];
            int nx = x + offset.x;
            int ny = y + offset.y;
            int nz = z + offset.z;
            
            // Vizinho dentro do chunk?
            if (nx >= 0 && nx < w && ny >= 0 && ny < h && nz >= 0 && nz < d) {
                return data.voxels[nx, ny, nz] == 0;
            }
            
            // Vizinho fora do chunk - consulta TerrainWorld
            int gX = chunkGlobalX + nx;
            int gY = ny;
            int gZ = chunkGlobalZ + nz;
            
            return world.GetVoxelAt(gX, gY, gZ) == 0;
        }

        /// <summary>
        /// Adiciona uma face (quad) à mesh.
        /// </summary>
        private static void AddFace(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs,
            int x, int y, int z, int dir, float scale, byte blockType) {
            
            int vCount = vertices.Count;
            Vector3[] faceVerts = FaceVertices[dir];
            Vector3 pos = new Vector3(x, y, z);
            
            // Adiciona os 4 vértices da face
            for (int i = 0; i < 4; i++) {
                vertices.Add((pos + faceVerts[i]) * scale);
            }
            
            // Adiciona UVs do texture atlas
            AddFaceUVs(uvs, blockType);
            
            // Adiciona os 2 triângulos da face (quad)
            triangles.Add(vCount);
            triangles.Add(vCount + 1);
            triangles.Add(vCount + 2);
            triangles.Add(vCount);
            triangles.Add(vCount + 2);
            triangles.Add(vCount + 3);
        }

        /// <summary>
        /// Calcula e adiciona coordenadas UV para uma face baseado no texture atlas.
        /// 
        /// O atlas é uma grade 8x8 (64 texturas).
        /// BlockType mapeia diretamente para índice no atlas.
        /// </summary>
        private static void AddFaceUVs(List<Vector2> uvs, byte blockType) {
            // Calcula posição no atlas
            int row = blockType / AtlasSize;
            int col = blockType % AtlasSize;
            
            // UV base (canto inferior esquerdo)
            float u = col * TileSize;
            float v = row * TileSize;
            
            // Aplica padding para evitar bleeding
            float uMin = u + UVPadding;
            float uMax = u + TileSize - UVPadding;
            float vMin = v + UVPadding;
            float vMax = v + TileSize - UVPadding;
            
            // 4 vértices do quad
            uvs.Add(new Vector2(uMin, vMin));
            uvs.Add(new Vector2(uMin, vMax));
            uvs.Add(new Vector2(uMax, vMax));
            uvs.Add(new Vector2(uMax, vMin));
        }
    }
}