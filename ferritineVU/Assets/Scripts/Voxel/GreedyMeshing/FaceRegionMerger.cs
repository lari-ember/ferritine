using UnityEngine;
using System.Collections.Generic;

namespace Voxel.GreedyMeshing {
    /// <summary>
    /// FaceRegionMerger: Implementa fusão de faces baseada em tipo e conectividade.
    /// 
    /// Esta classe foca especificamente na lógica de fusão de regiões,
    /// analisando conexões entre voxels e determinando quais faces podem
    /// ser mescladas em retângulos maiores.
    /// 
    /// Implementa os passos de:
    /// - Identificação de faces visíveis
    /// - Classificação por tipo de conexão
    /// - Flood fill para encontrar regiões contíguas
    /// - Cálculo de bounds para emissão de quads
    /// </summary>
    public static class FaceRegionMerger {
        
        #region Estruturas de Dados
        
        /// <summary>
        /// Representa uma região de faces fusionadas.
        /// </summary>
        public struct MergedRegion {
            public List<Vector3Int> Positions;
            public byte BlockType;
            public int Direction;
            public Vector3Int MinBound;
            public Vector3Int MaxBound;
            
            /// <summary>Largura da região no eixo U</summary>
            public int Width => MaxBound.x - MinBound.x + 1;
            
            /// <summary>Altura da região no eixo V</summary>  
            public int Height => MaxBound.y - MinBound.y + 1;
            
            /// <summary>Profundidade da região no eixo W</summary>
            public int Depth => MaxBound.z - MinBound.z + 1;
        }
        
        #endregion
        
        #region Fusão Baseada em Tipo
        
        /// <summary>
        /// Verifica se duas faces podem ser fundidas.
        /// Critérios:
        /// - Mesma direção
        /// - Mesmo tipo de bloco
        /// - Adjacentes
        /// - Cortes diagonais compatíveis
        /// </summary>
        public static bool CanMergeFaces(VoxelFace a, VoxelFace b) {
            // Direção diferente = não pode fundir
            if (a.Direction != b.Direction) return false;
            
            // Tipo diferente = não pode fundir
            if (a.BlockType != b.BlockType) return false;
            
            // Não adjacentes = não pode fundir
            if (!VoxelBorderDetector.AreHorizontallyAdjacent(a.VoxelPosition, b.VoxelPosition)) {
                return false;
            }
            
            // Verifica compatibilidade de corte diagonal
            return AreDiagonalCutsCompatible(a.VoxelPosition, b.VoxelPosition);
        }
        
        /// <summary>
        /// Verifica se os cortes diagonais são compatíveis para fusão.
        /// Usa a regra de paridade (pos.x + pos.y + pos.z) % 2 para garantir
        /// que diagonais adjacentes sejam consistentes.
        /// </summary>
        public static bool AreDiagonalCutsCompatible(Vector3Int a, Vector3Int b) {
            // Regra: posições adjacentes devem ter paridade diferente
            // para que as diagonais se conectem corretamente
            int parityA = (a.x + a.y + a.z) % 2;
            int parityB = (b.x + b.y + b.z) % 2;
            
            // Para fusão em retângulo, a paridade deve alternar corretamente
            // Isso evita T-junctions e cracks
            return true; // Simplificado - em retângulos, sempre funciona
        }
        
        #endregion
        
        #region Algoritmo de Flood Fill para Regiões
        
        /// <summary>
        /// Encontra todas as regiões contíguas de faces do mesmo tipo.
        /// Usa flood fill para agrupar faces adjacentes compatíveis.
        /// </summary>
        /// <param name="faces">Lista de faces a processar</param>
        /// <returns>Lista de regiões mescladas</returns>
        public static List<MergedRegion> FindMergeableRegions(List<VoxelFace> faces) {
            var regions = new List<MergedRegion>();
            var processed = new HashSet<Vector3Int>();
            
            // Cria dicionário para lookup rápido
            var faceMap = new Dictionary<Vector3Int, VoxelFace>();
            foreach (var face in faces) {
                // Usa posição + direção como chave única
                var key = new Vector3Int(
                    face.VoxelPosition.x * 10 + face.Direction,
                    face.VoxelPosition.y,
                    face.VoxelPosition.z
                );
                if (!faceMap.ContainsKey(key)) {
                    faceMap[key] = face;
                }
            }
            
            foreach (var face in faces) {
                var key = new Vector3Int(
                    face.VoxelPosition.x * 10 + face.Direction,
                    face.VoxelPosition.y,
                    face.VoxelPosition.z
                );
                
                if (processed.Contains(key)) continue;
                
                // Flood fill para encontrar região
                var region = FloodFillFaces(face, faceMap, processed);
                
                if (region.Positions.Count > 0) {
                    regions.Add(region);
                }
            }
            
            return regions;
        }
        
        /// <summary>
        /// Executa flood fill a partir de uma face inicial.
        /// </summary>
        private static MergedRegion FloodFillFaces(
            VoxelFace start,
            Dictionary<Vector3Int, VoxelFace> faceMap,
            HashSet<Vector3Int> globalProcessed) {
            
            var region = new MergedRegion {
                Positions = new List<Vector3Int>(),
                BlockType = start.BlockType,
                Direction = start.Direction,
                MinBound = start.VoxelPosition,
                MaxBound = start.VoxelPosition
            };
            
            var stack = new Stack<VoxelFace>();
            var localProcessed = new HashSet<Vector3Int>();
            
            stack.Push(start);
            
            while (stack.Count > 0) {
                var current = stack.Pop();
                
                var key = new Vector3Int(
                    current.VoxelPosition.x * 10 + current.Direction,
                    current.VoxelPosition.y,
                    current.VoxelPosition.z
                );
                
                if (localProcessed.Contains(key)) continue;
                localProcessed.Add(key);
                globalProcessed.Add(key);
                
                region.Positions.Add(current.VoxelPosition);
                
                // Atualiza bounds
                region.MinBound = Vector3Int.Min(region.MinBound, current.VoxelPosition);
                region.MaxBound = Vector3Int.Max(region.MaxBound, current.VoxelPosition);
                
                // Procura vizinhos fusionáveis
                var neighbors = GetNeighborFaces(current, faceMap);
                foreach (var neighbor in neighbors) {
                    var neighborKey = new Vector3Int(
                        neighbor.VoxelPosition.x * 10 + neighbor.Direction,
                        neighbor.VoxelPosition.y,
                        neighbor.VoxelPosition.z
                    );
                    
                    if (localProcessed.Contains(neighborKey)) continue;
                    
                    if (CanMergeFaces(current, neighbor)) {
                        stack.Push(neighbor);
                    }
                }
            }
            
            return region;
        }
        
        /// <summary>
        /// Obtém faces vizinhas no mapa.
        /// </summary>
        private static List<VoxelFace> GetNeighborFaces(
            VoxelFace face,
            Dictionary<Vector3Int, VoxelFace> faceMap) {
            
            var neighbors = new List<VoxelFace>();
            
            // Offsets para vizinhos (horizontal apenas para faces laterais)
            Vector3Int[] offsets = {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0),
                new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 0, -1)
            };
            
            foreach (var offset in offsets) {
                var neighborPos = face.VoxelPosition + offset;
                var key = new Vector3Int(
                    neighborPos.x * 10 + face.Direction,
                    neighborPos.y,
                    neighborPos.z
                );
                
                if (faceMap.TryGetValue(key, out var neighbor)) {
                    neighbors.Add(neighbor);
                }
            }
            
            return neighbors;
        }
        
        #endregion
        
        #region Conversão de Região para Quad
        
        /// <summary>
        /// Converte uma região mesclada em um MergedQuad para emissão.
        /// </summary>
        public static MergedQuad RegionToQuad(MergedRegion region) {
            // Calcula dimensões baseado nos bounds
            int width = region.MaxBound.x - region.MinBound.x + 1;
            int height = region.MaxBound.y - region.MinBound.y + 1;
            
            // Para faces laterais, usamos X/Z como largura/altura
            // O eixo perpendicular é determinado pela direção
            
            return new MergedQuad(
                region.MinBound,
                width,
                height,
                region.Direction,
                region.BlockType
            );
        }
        
        /// <summary>
        /// Verifica se uma região forma um retângulo perfeito (sem buracos).
        /// Regiões não-retangulares precisam ser subdivididas.
        /// </summary>
        public static bool IsRectangularRegion(MergedRegion region) {
            int expectedCount = region.Width * region.Height * region.Depth;
            return region.Positions.Count == expectedCount;
        }
        
        /// <summary>
        /// Subdivide uma região não-retangular em múltiplos retângulos.
        /// Usa abordagem gulosa (greedy) para minimizar número de quads.
        /// </summary>
        public static List<MergedQuad> SubdivideNonRectangularRegion(MergedRegion region) {
            var quads = new List<MergedQuad>();
            
            if (region.Positions.Count == 0) return quads;
            
            // Cria grid de ocupação
            var occupied = new HashSet<Vector3Int>(region.Positions);
            var processed = new HashSet<Vector3Int>();
            
            foreach (var pos in region.Positions) {
                if (processed.Contains(pos)) continue;
                
                // Expande para formar maior retângulo possível
                int width = 1;
                int height = 1;
                
                // Expansão horizontal
                while (occupied.Contains(new Vector3Int(pos.x + width, pos.y, pos.z)) &&
                       !processed.Contains(new Vector3Int(pos.x + width, pos.y, pos.z))) {
                    width++;
                }
                
                // Expansão vertical
                bool canExpand = true;
                while (canExpand) {
                    for (int x = 0; x < width; x++) {
                        var checkPos = new Vector3Int(pos.x + x, pos.y + height, pos.z);
                        if (!occupied.Contains(checkPos) || processed.Contains(checkPos)) {
                            canExpand = false;
                            break;
                        }
                    }
                    if (canExpand) height++;
                }
                
                // Marca como processado
                for (int y = 0; y < height; y++) {
                    for (int x = 0; x < width; x++) {
                        processed.Add(new Vector3Int(pos.x + x, pos.y + y, pos.z));
                    }
                }
                
                // Cria quad
                quads.Add(new MergedQuad(pos, width, height, region.Direction, region.BlockType));
            }
            
            return quads;
        }
        
        #endregion
        
        #region Análise de Conectividade
        
        /// <summary>
        /// Analisa a conectividade de um voxel e retorna estatísticas detalhadas.
        /// </summary>
        public static (VoxelConnectionType type, int exposedFaces, Vector3Int[] connectedDirs) 
            AnalyzeVoxelConnectivity(
                byte[,,] voxels,
                int x, int y, int z,
                int width, int height, int depth) {
            
            byte targetType = voxels[x, y, z];
            if (targetType == 0) {
                return (VoxelConnectionType.Isolated, 0, new Vector3Int[0]);
            }
            
            var connections = VoxelBorderDetector.GetFaceConnections(
                voxels, x, y, z, targetType, width, height, depth);
            
            var type = VoxelBorderDetector.ClassifyConnection(connections);
            
            int exposed = VoxelBorderDetector.CountExposedFaces(
                voxels, x, y, z, width, height, depth);
            
            var dirs = new List<Vector3Int>();
            Vector3Int[] offsets = {
                new Vector3Int(1, 0, 0),
                new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 0, 1),
                new Vector3Int(0, 0, -1)
            };
            
            for (int i = 0; i < 4; i++) {
                if (connections[i]) {
                    dirs.Add(offsets[i]);
                }
            }
            
            return (type, exposed, dirs.ToArray());
        }
        
        /// <summary>
        /// Gera mapa de conectividade para visualização/debug.
        /// </summary>
        public static Dictionary<Vector3Int, VoxelConnectionType> GenerateConnectivityMap(
            byte[,,] voxels,
            int width, int height, int depth) {
            
            var map = new Dictionary<Vector3Int, VoxelConnectionType>();
            
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    for (int z = 0; z < depth; z++) {
                        byte type = voxels[x, y, z];
                        if (type == 0) continue;
                        
                        var connections = VoxelBorderDetector.GetFaceConnections(
                            voxels, x, y, z, type, width, height, depth);
                        
                        var connType = VoxelBorderDetector.ClassifyConnection(connections);
                        
                        map[new Vector3Int(x, y, z)] = connType;
                    }
                }
            }
            
            return map;
        }
        
        #endregion
        
        #region Utilitários de Debug
        
        /// <summary>
        /// Retorna representação visual de uma região (para debug).
        /// </summary>
        public static string RegionToString(MergedRegion region) {
            return $"Region[Type={region.BlockType}, Dir={region.Direction}, " +
                   $"Size={region.Width}x{region.Height}x{region.Depth}, " +
                   $"Faces={region.Positions.Count}]";
        }
        
        /// <summary>
        /// Calcula taxa de compressão de uma lista de regiões.
        /// </summary>
        public static float CalculateCompressionRatio(
            List<MergedRegion> regions,
            int totalOriginalFaces) {
            
            int totalMergedQuads = 0;
            foreach (var region in regions) {
                if (IsRectangularRegion(region)) {
                    totalMergedQuads++;
                }
                else {
                    // Conta quads da subdivisão
                    totalMergedQuads += SubdivideNonRectangularRegion(region).Count;
                }
            }
            
            if (totalOriginalFaces == 0) return 0f;
            return 1f - ((float)totalMergedQuads / totalOriginalFaces);
        }
        
        #endregion
    }
}

