using UnityEngine;
using System.Collections.Generic;

namespace Voxel.GreedyMeshing {
    /// <summary>
    /// GreedyMeshBuilder: Implementa o algoritmo de Greedy Meshing para otimização de malhas.
    /// 
    /// O algoritmo funciona em 3 passos principais, repetidos para cada eixo:
    /// 
    /// Passo A: Slice (Fatiamento)
    ///   - Percorre o chunk camada por camada em cada eixo
    ///   - Em cada fatia, temos um plano 2D de faces
    /// 
    /// Passo B: Scanning (Varredura)
    ///   - Para cada face na fatia, expande horizontalmente (largura)
    ///   - Depois expande verticalmente (altura) como bloco único
    ///   
    /// Passo C: Masking (Máscara)
    ///   - Usa máscara booleana para marcar faces já processadas
    ///   - Evita processar a mesma face duas vezes
    /// 
    /// A lógica central: "Se tenho vários quadrados adjacentes da mesma cor/tipo,
    /// por que não fazer um retângulo único que cubra todos?"
    /// </summary>
    public static class GreedyMeshBuilder {
        
        #region Configuração de Atlas
        
        private const int AtlasSize = 8;
        private const float TileSize = 1.0f / AtlasSize;
        private const float UVPadding = 0.001f;
        
        #endregion
        
        #region Dados Estáticos de Faces
        
        /// <summary>
        /// Normais para cada uma das 6 direções de face.
        /// </summary>
        private static readonly Vector3[] FaceNormals = {
            Vector3.right,    // 0: X+
            Vector3.left,     // 1: X-
            Vector3.up,       // 2: Y+
            Vector3.down,     // 3: Y-
            Vector3.forward,  // 4: Z+
            Vector3.back      // 5: Z-
        };
        
        /// <summary>
        /// Offsets para verificar vizinhos em cada direção.
        /// </summary>
        private static readonly Vector3Int[] DirectionOffsets = {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };
        
        /// <summary>
        /// Para cada direção de face, define os eixos U e V do plano.
        /// [direction][0] = eixo U, [direction][1] = eixo V
        /// </summary>
        private static readonly int[][] PlaneAxes = {
            new[] { 2, 1 }, // X+: U=Z, V=Y
            new[] { 2, 1 }, // X-: U=Z, V=Y
            new[] { 0, 2 }, // Y+: U=X, V=Z
            new[] { 0, 2 }, // Y-: U=X, V=Z
            new[] { 0, 1 }, // Z+: U=X, V=Y
            new[] { 0, 1 }  // Z-: U=X, V=Y
        };
        
        /// <summary>
        /// Eixo perpendicular ao plano para cada direção.
        /// </summary>
        private static readonly int[] PerpendicularAxis = { 0, 0, 1, 1, 2, 2 };
        
        #endregion
        
        #region Métodos Principais de Meshing
        
        /// <summary>
        /// Gera mesh otimizada usando Greedy Meshing para um chunk completo.
        /// </summary>
        /// <param name="voxels">Array 3D de tipos de voxel</param>
        /// <param name="width">Largura do chunk</param>
        /// <param name="height">Altura do chunk</param>
        /// <param name="depth">Profundidade do chunk</param>
        /// <param name="scale">Escala em metros por voxel</param>
        /// <param name="world">TerrainWorld para consultas cross-chunk (opcional)</param>
        /// <param name="chunkPos">Posição do chunk para cross-chunk queries</param>
        /// <returns>Mesh otimizada</returns>
        public static Mesh BuildGreedyMesh(
            byte[,,] voxels,
            int width, int height, int depth,
            float scale,
            TerrainWorld world = null,
            Vector2Int chunkPos = default) {
            
            var meshData = new MeshData(width * height);
            int chunkGlobalX = chunkPos.x * width;
            int chunkGlobalZ = chunkPos.y * width;
            
            // Debug: conta voxels sólidos
            int solidCount = 0;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    for (int z = 0; z < depth; z++)
                        if (voxels[x, y, z] != 0) solidCount++;
            
            // Processa cada uma das 6 direções de face
            for (int dir = 0; dir < 6; dir++) {
                ProcessDirection(voxels, width, height, depth, dir, scale, 
                    meshData, world, chunkGlobalX, chunkGlobalZ);
            }
            // Debug: log mesh stats
            Debug.Log($"[GreedyMeshBuilder] Mesh stats: VertexCount={meshData.VertexCount}, TriangleCount={meshData.TriangleCount}");
            // Debug: log apenas se houver poucos vértices (possível problema)
            if (solidCount > 0 && meshData.VertexCount == 0) {
                Debug.LogWarning($"[GreedyMeshBuilder] Chunk {chunkPos} tem {solidCount} voxels sólidos mas 0 vértices gerados!");
            }
            
            return meshData.ToMesh();
        }
        
        /// <summary>
        /// Gera mesh apenas para faces horizontais (laterais).
        /// Ignora faces Y+ e Y- conforme especificação.
        /// </summary>
        public static Mesh BuildGreedyMeshHorizontalOnly(
            byte[,,] voxels,
            int width, int height, int depth,
            float scale,
            TerrainWorld world = null,
            Vector2Int chunkPos = default) {
            
            var meshData = new MeshData(width * height);
            int chunkGlobalX = chunkPos.x * width;
            int chunkGlobalZ = chunkPos.y * width;
            
            // Processa apenas direções horizontais: 0,1 (X+,X-) e 4,5 (Z+,Z-)
            int[] horizontalDirs = { 0, 1, 4, 5 };
            foreach (int dir in horizontalDirs) {
                ProcessDirection(voxels, width, height, depth, dir, scale, 
                    meshData, world, chunkGlobalX, chunkGlobalZ);
            }
            
            return meshData.ToMesh();
        }
        
        /// <summary>
        /// Processa todas as faces em uma direção específica usando Greedy Meshing.
        /// </summary>
        private static void ProcessDirection(
            byte[,,] voxels,
            int width, int height, int depth,
            int direction,
            float scale,
            MeshData meshData,
            TerrainWorld world,
            int chunkGlobalX, int chunkGlobalZ) {
            
            // Determina os eixos do plano baseado na direção
            int axisU = PlaneAxes[direction][0]; // Eixo horizontal do quad
            int axisV = PlaneAxes[direction][1]; // Eixo vertical do quad
            int axisW = PerpendicularAxis[direction]; // Eixo perpendicular (profundidade)
            
            // Tamanhos nos três eixos
            int[] sizes = { width, height, depth };
            int sizeW = sizes[axisW];
            int sizeU = sizes[axisU];
            int sizeV = sizes[axisV];
            
            // Direção do offset para verificar vizinho
            int wOffset = (direction % 2 == 0) ? 1 : -1;
            
            // Array para coordenadas de trabalho
            int[] pos = new int[3];
            int[] neighborPos = new int[3];
            
            // Máscara para marcar faces já processadas (reutilizada por slice)
            bool[] mask = new bool[sizeU * sizeV];
            byte[] maskTypes = new byte[sizeU * sizeV];
            
            // Passo A: Fatia por fatia no eixo W
            for (int w = 0; w < sizeW; w++) {
                // Limpa máscaras para esta fatia
                System.Array.Clear(mask, 0, mask.Length);
                System.Array.Clear(maskTypes, 0, maskTypes.Length);
                
                pos[axisW] = w;
                neighborPos[axisW] = w + wOffset;
                
                // Preenche a máscara com faces visíveis nesta fatia
                for (int v = 0; v < sizeV; v++) {
                    for (int u = 0; u < sizeU; u++) {
                        pos[axisU] = u;
                        pos[axisV] = v;
                        
                        byte voxelType = voxels[pos[0], pos[1], pos[2]];
                        
                        // Voxel atual precisa ser sólido
                        if (voxelType == 0) continue;
                        
                        // Verifica se face está exposta
                        bool faceExposed = false;
                        
                        neighborPos[axisU] = u;
                        neighborPos[axisV] = v;
                        
                        if (neighborPos[axisW] < 0 || neighborPos[axisW] >= sizeW) {
                            // Borda do chunk - verifica cross-chunk ou assume exposto
                            if (world != null) {
                                int gX = chunkGlobalX + neighborPos[0];
                                int gY = neighborPos[1];
                                int gZ = chunkGlobalZ + neighborPos[2];
                                faceExposed = (world.GetVoxelAt(gX, gY, gZ) == 0);
                            }
                            else {
                                faceExposed = true;
                            }
                        }
                        else {
                            faceExposed = (voxels[neighborPos[0], neighborPos[1], neighborPos[2]] == 0);
                        }
                        
                        if (faceExposed) {
                            int idx = v * sizeU + u;
                            mask[idx] = true;
                            maskTypes[idx] = voxelType;
                        }
                    }
                }
                
                // Passo B: Greedy expansion - encontra retângulos máximos
                for (int v = 0; v < sizeV; v++) {
                    for (int u = 0; u < sizeU; ) {
                        int idx = v * sizeU + u;
                        
                        if (!mask[idx]) {
                            u++;
                            continue;
                        }
                        
                        byte currentType = maskTypes[idx];
                        
                        // Expansão horizontal (largura)
                        int quadWidth = 1;
                        while (u + quadWidth < sizeU) {
                            int nextIdx = v * sizeU + (u + quadWidth);
                            if (!mask[nextIdx] || maskTypes[nextIdx] != currentType) {
                                break;
                            }
                            quadWidth++;
                        }
                        
                        // Expansão vertical (altura) - tenta expandir a linha inteira
                        int quadHeight = 1;
                        bool canExpandV = true;
                        
                        while (v + quadHeight < sizeV && canExpandV) {
                            // Verifica se toda a próxima linha pode ser incluída
                            for (int checkU = 0; checkU < quadWidth; checkU++) {
                                int checkIdx = (v + quadHeight) * sizeU + (u + checkU);
                                if (!mask[checkIdx] || maskTypes[checkIdx] != currentType) {
                                    canExpandV = false;
                                    break;
                                }
                            }
                            
                            if (canExpandV) {
                                quadHeight++;
                            }
                        }
                        
                        // Passo C: Marca faces usadas na máscara
                        for (int dv = 0; dv < quadHeight; dv++) {
                            for (int du = 0; du < quadWidth; du++) {
                                int clearIdx = (v + dv) * sizeU + (u + du);
                                mask[clearIdx] = false;
                            }
                        }
                        
                        // Emite o quad mesclado
                        EmitQuad(meshData, direction, axisU, axisV, axisW,
                            u, v, w, quadWidth, quadHeight, scale, currentType);
                        
                        u += quadWidth;
                    }
                }
            }
        }
        
        #endregion
        
        #region Emissão de Quads
        
        /// <summary>
        /// Emite um quad (4 vértices, 6 índices) na mesh.
        /// </summary>
        private static void EmitQuad(
            MeshData meshData,
            int direction,
            int axisU, int axisV, int axisW,
            int u, int v, int w,
            int quadWidth, int quadHeight,
            float scale,
            byte blockType) {
            meshData.EnsureCapacity(4, 6);
            int vStart = meshData.VertexCount;
            float offset = (direction % 2 == 0) ? 1.0f : 0.0f;
            Vector3 v0, v1, v2, v3;
            float[] pos0 = new float[3];
            float[] pos1 = new float[3];
            float[] pos2 = new float[3];
            float[] pos3 = new float[3];
            pos0[axisW] = (w + offset) * scale;
            pos1[axisW] = (w + offset) * scale;
            pos2[axisW] = (w + offset) * scale;
            pos3[axisW] = (w + offset) * scale;
            pos0[axisU] = u * scale;
            pos0[axisV] = v * scale;
            pos1[axisU] = (u + quadWidth) * scale;
            pos1[axisV] = v * scale;
            pos2[axisU] = (u + quadWidth) * scale;
            pos2[axisV] = (v + quadHeight) * scale;
            pos3[axisU] = u * scale;
            pos3[axisV] = (v + quadHeight) * scale;
            v0 = new Vector3(pos0[0], pos0[1], pos0[2]);
            v1 = new Vector3(pos1[0], pos1[1], pos1[2]);
            v2 = new Vector3(pos2[0], pos2[1], pos2[2]);
            v3 = new Vector3(pos3[0], pos3[1], pos3[2]);
            // UVs padrão universal para todos os quads
            Vector2[] quadUVs = new Vector2[4];
            int row = blockType / AtlasSize;
            int col = blockType % AtlasSize;
            float u0 = col * TileSize + UVPadding;
            float v0uv = row * TileSize + UVPadding;
            float u1 = u0 + TileSize - UVPadding * 2;
            float v1uv = v0uv + TileSize - UVPadding * 2;
            quadUVs[0] = new Vector2(u0, v0uv);
            quadUVs[1] = new Vector2(u1, v0uv);
            quadUVs[2] = new Vector2(u1, v1uv);
            quadUVs[3] = new Vector2(u0, v1uv);
            meshData.Vertices[vStart] = v0;
            meshData.Vertices[vStart + 1] = v1;
            meshData.Vertices[vStart + 2] = v2;
            meshData.Vertices[vStart + 3] = v3;
            Vector3 normal = FaceNormals[direction];
            meshData.Normals[vStart] = normal;
            meshData.Normals[vStart + 1] = normal;
            meshData.Normals[vStart + 2] = normal;
            meshData.Normals[vStart + 3] = normal;
            meshData.UVs[vStart] = quadUVs[0];
            meshData.UVs[vStart + 1] = quadUVs[1];
            meshData.UVs[vStart + 2] = quadUVs[2];
            meshData.UVs[vStart + 3] = quadUVs[3];
            Color32 color = GetBlockColor(blockType);
            meshData.Colors[vStart] = color;
            meshData.Colors[vStart + 1] = color;
            meshData.Colors[vStart + 2] = color;
            meshData.Colors[vStart + 3] = color;
            int tStart = meshData.TriangleCount;
            bool useMainDiagonal = ((u + v + w) % 2 == 0);
            // Corrige winding para Z+ e Z-
            if (direction == 4 || direction == 5) {
                // Faces Z: inverta o winding dos triângulos
                if (useMainDiagonal) {
                    meshData.Triangles[tStart] = vStart;
                    meshData.Triangles[tStart + 1] = vStart + 1;
                    meshData.Triangles[tStart + 2] = vStart + 3;
                    meshData.Triangles[tStart + 3] = vStart + 1;
                    meshData.Triangles[tStart + 4] = vStart + 2;
                    meshData.Triangles[tStart + 5] = vStart + 3;
                } else {
                    meshData.Triangles[tStart] = vStart;
                    meshData.Triangles[tStart + 1] = vStart + 2;
                    meshData.Triangles[tStart + 2] = vStart + 3;
                    meshData.Triangles[tStart + 3] = vStart;
                    meshData.Triangles[tStart + 4] = vStart + 1;
                    meshData.Triangles[tStart + 5] = vStart + 2;
                }
            } else if (direction % 2 == 0) {
                if (useMainDiagonal) {
                    meshData.Triangles[tStart] = vStart;
                    meshData.Triangles[tStart + 1] = vStart + 3;
                    meshData.Triangles[tStart + 2] = vStart + 1;
                    meshData.Triangles[tStart + 3] = vStart + 1;
                    meshData.Triangles[tStart + 4] = vStart + 3;
                    meshData.Triangles[tStart + 5] = vStart + 2;
                } else {
                    meshData.Triangles[tStart] = vStart;
                    meshData.Triangles[tStart + 1] = vStart + 3;
                    meshData.Triangles[tStart + 2] = vStart + 2;
                    meshData.Triangles[tStart + 3] = vStart;
                    meshData.Triangles[tStart + 4] = vStart + 2;
                    meshData.Triangles[tStart + 5] = vStart + 1;
                }
            } else {
                if (useMainDiagonal) {
                    meshData.Triangles[tStart] = vStart;
                    meshData.Triangles[tStart + 1] = vStart + 1;
                    meshData.Triangles[tStart + 2] = vStart + 3;
                    meshData.Triangles[tStart + 3] = vStart + 1;
                    meshData.Triangles[tStart + 4] = vStart + 2;
                    meshData.Triangles[tStart + 5] = vStart + 3;
                } else {
                    meshData.Triangles[tStart] = vStart;
                    meshData.Triangles[tStart + 1] = vStart + 2;
                    meshData.Triangles[tStart + 2] = vStart + 3;
                    meshData.Triangles[tStart + 3] = vStart;
                    meshData.Triangles[tStart + 4] = vStart + 1;
                    meshData.Triangles[tStart + 5] = vStart + 2;
                }
            }
            meshData.VertexCount += 4;
            meshData.TriangleCount += 6;
            Debug.Log($"[GreedyMeshBuilder] EmitQuad: dir={direction}, pos=({u},{v},{w}), size=({quadWidth},{quadHeight}), blockType={blockType}");
        }
        
        /// <summary>
        /// Adiciona UVs com tiling para não esticar a textura em quads grandes.
        /// </summary>
        private static void AddTiledUVs(MeshData meshData, int vStart, byte blockType, int width, int height) {
            int row = blockType / AtlasSize;
            int col = blockType % AtlasSize;

            float u0 = col * TileSize + UVPadding;
            float v0 = row * TileSize + UVPadding;
            float u1 = u0 + TileSize - UVPadding * 2;
            float v1 = v0 + TileSize - UVPadding * 2;

            // Mapeia o quad inteiro para o tile do atlas (sem tiling)
            meshData.UVs[vStart]     = new Vector2(u0, v0);
            meshData.UVs[vStart + 1] = new Vector2(u1, v0);
            meshData.UVs[vStart + 2] = new Vector2(u1, v1);
            meshData.UVs[vStart + 3] = new Vector2(u0, v1);
        }
        
        /// <summary>
        /// Retorna cor de debug baseada no tipo de bloco.
        /// </summary>
        private static Color32 GetBlockColor(byte blockType) {
            // Gera cor determinística baseada no tipo
            UnityEngine.Random.InitState(blockType * 12345);
            return new Color32(
                (byte)UnityEngine.Random.Range(100, 255),
                (byte)UnityEngine.Random.Range(100, 255),
                (byte)UnityEngine.Random.Range(100, 255),
                255
            );
        }
        
        #endregion
        
        #region Métodos de Região (Flood Fill)
        
        /// <summary>
        /// Encontra uma região conectada de voxels do mesmo tipo usando Flood Fill.
        /// Útil para operações de seleção e análise.
        /// </summary>
        public static List<Vector3Int> FloodFillRegion(
            byte[,,] voxels,
            Vector3Int start,
            int width, int height, int depth,
            bool horizontalOnly = true) {
            
            var region = new List<Vector3Int>();
            var visited = new HashSet<Vector3Int>();
            var stack = new Stack<Vector3Int>();
            
            byte targetType = voxels[start.x, start.y, start.z];
            if (targetType == 0) return region;
            
            stack.Push(start);
            
            while (stack.Count > 0) {
                var pos = stack.Pop();
                
                if (!visited.Add(pos)) continue;
                
                if (pos.x < 0 || pos.x >= width ||
                    pos.y < 0 || pos.y >= height ||
                    pos.z < 0 || pos.z >= depth) continue;
                
                if (voxels[pos.x, pos.y, pos.z] != targetType) continue;
                
                region.Add(pos);
                
                // Adiciona vizinhos
                if (horizontalOnly) {
                    // Apenas 4 direções horizontais
                    stack.Push(new Vector3Int(pos.x + 1, pos.y, pos.z));
                    stack.Push(new Vector3Int(pos.x - 1, pos.y, pos.z));
                    stack.Push(new Vector3Int(pos.x, pos.y, pos.z + 1));
                    stack.Push(new Vector3Int(pos.x, pos.y, pos.z - 1));
                }
                else {
                    // 6 direções
                    foreach (var offset in DirectionOffsets) {
                        stack.Push(pos + offset);
                    }
                }
            }
            
            return region;
        }
        
        /// <summary>
        /// Calcula o bounding box de uma região.
        /// </summary>
        public static (Vector3Int min, Vector3Int max) ComputeRegionBounds(List<Vector3Int> region) {
            if (region.Count == 0) {
                return (Vector3Int.zero, Vector3Int.zero);
            }
            
            Vector3Int min = region[0];
            Vector3Int max = region[0];
            
            foreach (var pos in region) {
                min = Vector3Int.Min(min, pos);
                max = Vector3Int.Max(max, pos);
            }
            
            return (min, max);
        }
        
        #endregion
        
        #region Estatísticas
        
        /// <summary>
        /// Calcula estatísticas de otimização do Greedy Meshing.
        /// </summary>
        public static (int originalFaces, int mergedQuads, float reduction) CalculateOptimizationStats(
            byte[,,] voxels,
            int width, int height, int depth) {
            
            int originalFaces = 0;
            int mergedQuads = 0;
            
            // Conta faces originais (sem meshing)
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    for (int z = 0; z < depth; z++) {
                        if (voxels[x, y, z] == 0) continue;
                        
                        // Verifica cada face
                        foreach (var offset in DirectionOffsets) {
                            int nx = x + offset.x;
                            int ny = y + offset.y;
                            int nz = z + offset.z;
                            
                            bool exposed = false;
                            if (nx < 0 || nx >= width || ny < 0 || ny >= height || nz < 0 || nz >= depth) {
                                exposed = true;
                            }
                            else {
                                exposed = (voxels[nx, ny, nz] == 0);
                            }
                            
                            if (exposed) originalFaces++;
                        }
                    }
                }
            }
            
            // Estima quads mesclados (simplificado)
            // Uma estimativa mais precisa requer rodar o algoritmo completo
            mergedQuads = Mathf.Max(1, originalFaces / 4);
            
            float reduction = originalFaces > 0 ? 
                1.0f - ((float)mergedQuads / originalFaces) : 0f;
            
            return (originalFaces, mergedQuads, reduction);
        }
        
        #endregion
    }
}

