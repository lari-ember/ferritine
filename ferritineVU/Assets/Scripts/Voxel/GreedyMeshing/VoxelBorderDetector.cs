using UnityEngine;
using System.Collections.Generic;

namespace Voxel.GreedyMeshing {
    /// <summary>
    /// VoxelBorderDetector: Identifica quais voxels estão nas bordas.
    /// 
    /// Um voxel está na borda quando pelo menos uma de suas faces LATERAIS
    /// (eixo horizontal: X+, X-, Z+, Z-) está exposta ao ar.
    /// 
    /// Faces verticais (Y+, Y-) são ignoradas conforme especificação.
    /// 
    /// Esta classe implementa o Passo A do algoritmo Greedy Meshing:
    /// identificação de faces visíveis para posterior fusão.
    /// </summary>
    public static class VoxelBorderDetector {
        
        #region Direções e Offsets
        
        /// <summary>
        /// Offsets para as 4 direções horizontais.
        /// Índice corresponde ao enum HorizontalDirection.
        /// </summary>
        private static readonly Vector3Int[] HorizontalOffsets = {
            new Vector3Int(1, 0, 0),   // X+ (Leste)
            new Vector3Int(-1, 0, 0),  // X- (Oeste)
            new Vector3Int(0, 0, 1),   // Z+ (Norte)
            new Vector3Int(0, 0, -1)   // Z- (Sul)
        };
        
        /// <summary>
        /// Offsets para todas as 6 direções (incluindo verticais).
        /// </summary>
        private static readonly Vector3Int[] AllDirections = {
            new Vector3Int(1, 0, 0),   // 0: X+
            new Vector3Int(-1, 0, 0),  // 1: X-
            new Vector3Int(0, 1, 0),   // 2: Y+
            new Vector3Int(0, -1, 0),  // 3: Y-
            new Vector3Int(0, 0, 1),   // 4: Z+
            new Vector3Int(0, 0, -1)   // 5: Z-
        };
        
        /// <summary>
        /// Mapeamento de direção completa (6) para direção horizontal (4).
        /// -1 significa que não é uma direção horizontal.
        /// </summary>
        private static readonly int[] DirectionToHorizontal = { 0, 1, -1, -1, 2, 3 };
        
        #endregion
        
        #region Identificação de Bordas
        
        /// <summary>
        /// Identifica todos os voxels de borda em um chunk.
        /// Retorna uma lista de posições onde pelo menos uma face lateral está exposta.
        /// 
        /// Performance: O(n) onde n = largura * altura * profundidade do chunk.
        /// </summary>
        /// <param name="voxels">Array 3D de tipos de voxel</param>
        /// <param name="width">Largura do chunk</param>
        /// <param name="height">Altura do chunk</param>
        /// <param name="depth">Profundidade do chunk</param>
        /// <returns>Lista de posições de voxels de borda</returns>
        public static List<Vector3Int> IdentifyBorderVoxels(byte[,,] voxels, int width, int height, int depth) {
            var borderVoxels = new List<Vector3Int>();
            
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    for (int z = 0; z < depth; z++) {
                        byte type = voxels[x, y, z];
                        if (type == 0) continue; // Ar, ignora
                        
                        // Verifica se alguma face lateral está exposta
                        if (HasExposedLateralFace(voxels, x, y, z, width, height, depth)) {
                            borderVoxels.Add(new Vector3Int(x, y, z));
                        }
                    }
                }
            }
            
            return borderVoxels;
        }
        
        /// <summary>
        /// Identifica bordas com informação completa de conexões.
        /// Retorna VoxelFace com dados de conectividade para cada face exposta.
        /// </summary>
        public static List<VoxelFace> IdentifyBorderFaces(
            byte[,,] voxels, 
            int width, int height, int depth,
            TerrainWorld world = null,
            int chunkGlobalX = 0, int chunkGlobalZ = 0) {
            
            var faces = new List<VoxelFace>();
            
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    for (int z = 0; z < depth; z++) {
                        byte type = voxels[x, y, z];
                        if (type == 0) continue;
                        
                        // Verifica cada uma das 4 direções horizontais
                        for (int dir = 0; dir < 4; dir++) {
                            Vector3Int offset = HorizontalOffsets[dir];
                            int nx = x + offset.x;
                            int ny = y; // Sempre mesmo Y (horizontal)
                            int nz = z + offset.z;
                            
                            bool isExposed;
                            bool isChunkBorder = false;
                            
                            // Verifica se vizinho está dentro do chunk
                            if (nx >= 0 && nx < width && nz >= 0 && nz < depth) {
                                isExposed = (voxels[nx, ny, nz] == 0);
                            }
                            else {
                                isChunkBorder = true;
                                // Consulta cross-chunk se TerrainWorld disponível
                                if (world != null) {
                                    int gX = chunkGlobalX + nx;
                                    int gZ = chunkGlobalZ + nz;
                                    isExposed = (world.GetVoxelAt(gX, ny, gZ) == 0);
                                }
                                else {
                                    isExposed = true; // Assume exposto se não temos dados
                                }
                            }
                            
                            if (isExposed) {
                                var face = new VoxelFace(
                                    new Vector3Int(x, y, z),
                                    dir, // Direção horizontal (0-3)
                                    type
                                );
                                face.IsChunkBorder = isChunkBorder;
                                
                                // Calcula conexões com vizinhos do mesmo tipo
                                face.Connections = GetFaceConnections(voxels, x, y, z, type, width, height, depth);
                                face.ConnectionType = ClassifyConnection(face.Connections);
                                
                                faces.Add(face);
                            }
                        }
                    }
                }
            }
            
            return faces;
        }
        
        /// <summary>
        /// Verifica se um voxel específico está na borda (tem face lateral exposta).
        /// </summary>
        public static bool IsVoxelBorder(byte[,,] voxels, int x, int y, int z, int width, int height, int depth) {
            if (voxels[x, y, z] == 0) return false;
            return HasExposedLateralFace(voxels, x, y, z, width, height, depth);
        }
        
        /// <summary>
        /// Verifica se pelo menos uma face lateral está exposta ao ar.
        /// Nota: parâmetro height não é usado pois só verificamos faces laterais (horizontais).
        /// </summary>
        private static bool HasExposedLateralFace(byte[,,] voxels, int x, int y, int z, int w, int height, int d) {
            // height é mantido para consistência da API, mas faces laterais não dependem dele
            foreach (var offset in HorizontalOffsets) {
                int nx = x + offset.x;
                int nz = z + offset.z;
                
                // Borda do chunk = face exposta
                if (nx < 0 || nx >= w || nz < 0 || nz >= d) {
                    return true;
                }
                
                // Vizinho é ar = face exposta
                if (voxels[nx, y, nz] == 0) {
                    return true;
                }
            }
            
            return false;
        }
        
        #endregion
        
        #region Análise de Conexões
        
        /// <summary>
        /// Obtém as conexões laterais de um voxel com vizinhos do mesmo tipo.
        /// 
        /// Retorna array de 4 booleanos:
        /// [0] = X+ (Leste)
        /// [1] = X- (Oeste)
        /// [2] = Z+ (Norte)
        /// [3] = Z- (Sul)
        /// </summary>
        public static bool[] GetFaceConnections(
            byte[,,] voxels, 
            int x, int y, int z, 
            byte targetType,
            int width, int height, int depth) {
            
            bool[] connections = new bool[4];
            
            for (int i = 0; i < 4; i++) {
                Vector3Int offset = HorizontalOffsets[i];
                int nx = x + offset.x;
                int nz = z + offset.z;
                
                if (nx >= 0 && nx < width && nz >= 0 && nz < depth) {
                    connections[i] = (voxels[nx, y, nz] == targetType);
                }
                // Fora do chunk = não conectado (para propósitos de meshing interno)
            }
            
            return connections;
        }
        
        /// <summary>
        /// Classifica o tipo de conexão baseado nas 4 conexões laterais.
        /// 
        /// Tipos (conforme especificação):
        /// 1. Isolated: 0 conexões (bloco sozinho)
        /// 2. Single: 1 conexão (pode formar par)
        /// 3. Corner: 2 conexões em L (perpendiculares)
        /// 4. Straight: 2 conexões em I (opostas)
        /// 5. TShape: 3 conexões
        /// 6. Full: 4 conexões
        /// </summary>
        public static VoxelConnectionType ClassifyConnection(bool[] connections) {
            int count = 0;
            foreach (bool c in connections) {
                if (c) count++;
            }
            
            switch (count) {
                case 0:
                    return VoxelConnectionType.Isolated;
                    
                case 1:
                    return VoxelConnectionType.Single;
                    
                case 2:
                    // Verifica se são opostas (Straight/I) ou adjacentes (Corner/L)
                    // Opostas: [0,1] (X+,X-) ou [2,3] (Z+,Z-)
                    if ((connections[0] && connections[1]) || (connections[2] && connections[3])) {
                        return VoxelConnectionType.Straight;
                    }
                    return VoxelConnectionType.Corner;
                    
                case 3:
                    return VoxelConnectionType.TShape;
                    
                case 4:
                    return VoxelConnectionType.Full;
                    
                default:
                    return VoxelConnectionType.Isolated;
            }
        }
        
        /// <summary>
        /// Retorna as direções de expansão possíveis para um tipo de conexão.
        /// Útil para o algoritmo de Greedy Meshing.
        /// </summary>
        public static List<Vector3Int> GetExpansionDirections(VoxelConnectionType type, bool[] connections) {
            var directions = new List<Vector3Int>();
            
            switch (type) {
                case VoxelConnectionType.Isolated:
                    // Não expande
                    break;
                    
                case VoxelConnectionType.Single:
                    // Expande na direção da conexão
                    for (int i = 0; i < 4; i++) {
                        if (connections[i]) {
                            directions.Add(HorizontalOffsets[i]);
                            break;
                        }
                    }
                    break;
                    
                case VoxelConnectionType.Corner:
                case VoxelConnectionType.Straight:
                    // Expande em todas as direções conectadas
                    for (int i = 0; i < 4; i++) {
                        if (connections[i]) {
                            directions.Add(HorizontalOffsets[i]);
                        }
                    }
                    break;
                    
                case VoxelConnectionType.TShape:
                    // Expande em 3 direções conectadas
                    for (int i = 0; i < 4; i++) {
                        if (connections[i]) {
                            directions.Add(HorizontalOffsets[i]);
                        }
                    }
                    break;
                    
                case VoxelConnectionType.Full:
                    // Pode expandir em todas as 4 direções
                    for (int i = 0; i < 4; i++) {
                        directions.Add(HorizontalOffsets[i]);
                    }
                    break;
            }
            
            return directions;
        }
        
        #endregion
        
        #region Utilitários de Vizinhança
        
        /// <summary>
        /// Conta quantas faces laterais de um voxel estão expostas.
        /// </summary>
        public static int CountExposedFaces(byte[,,] voxels, int x, int y, int z, int w, int h, int d) {
            if (voxels[x, y, z] == 0) return 0;
            
            int count = 0;
            foreach (var offset in HorizontalOffsets) {
                int nx = x + offset.x;
                int nz = z + offset.z;
                
                if (nx < 0 || nx >= w || nz < 0 || nz >= d || voxels[nx, y, nz] == 0) {
                    count++;
                }
            }
            
            return count;
        }
        
        /// <summary>
        /// Obtém todos os vizinhos do mesmo tipo (lateral apenas).
        /// </summary>
        public static List<Vector3Int> GetSameTypeNeighbors(
            byte[,,] voxels, 
            int x, int y, int z, 
            byte targetType,
            int width, int height, int depth) {
            
            var neighbors = new List<Vector3Int>();
            
            for (int i = 0; i < 4; i++) {
                Vector3Int offset = HorizontalOffsets[i];
                int nx = x + offset.x;
                int nz = z + offset.z;
                
                if (nx >= 0 && nx < width && nz >= 0 && nz < depth) {
                    if (voxels[nx, y, nz] == targetType) {
                        neighbors.Add(new Vector3Int(nx, y, nz));
                    }
                }
            }
            
            return neighbors;
        }
        
        /// <summary>
        /// Verifica adjacência entre duas posições (Manhattan distance = 1).
        /// </summary>
        public static bool AreAdjacent(Vector3Int a, Vector3Int b) {
            Vector3Int diff = a - b;
            return Mathf.Abs(diff.x) + Mathf.Abs(diff.y) + Mathf.Abs(diff.z) == 1;
        }
        
        /// <summary>
        /// Verifica adjacência horizontal apenas (ignora Y).
        /// </summary>
        public static bool AreHorizontallyAdjacent(Vector3Int a, Vector3Int b) {
            if (a.y != b.y) return false;
            Vector3Int diff = a - b;
            return Mathf.Abs(diff.x) + Mathf.Abs(diff.z) == 1;
        }
        
        #endregion
        
        #region Debug/Visualização
        
        /// <summary>
        /// Gera uma representação visual das conexões (para debug).
        /// </summary>
        public static string ConnectionToString(bool[] connections) {
            // Representa visualmente: E=Leste, W=Oeste, N=Norte, S=Sul
            string result = "";
            if (connections[0]) result += "E";
            if (connections[1]) result += "W";
            if (connections[2]) result += "N";
            if (connections[3]) result += "S";
            return result.Length > 0 ? result : "Isolated";
        }
        
        /// <summary>
        /// Retorna estatísticas de bordas do chunk.
        /// </summary>
        public static (int total, int isolated, int corners, int straights, int tShapes, int fulls) 
            GetBorderStats(List<VoxelFace> faces) {
            
            int isolated = 0, corners = 0, straights = 0, tShapes = 0, fulls = 0, singles = 0;
            
            foreach (var face in faces) {
                switch (face.ConnectionType) {
                    case VoxelConnectionType.Isolated: isolated++; break;
                    case VoxelConnectionType.Single: singles++; break;
                    case VoxelConnectionType.Corner: corners++; break;
                    case VoxelConnectionType.Straight: straights++; break;
                    case VoxelConnectionType.TShape: tShapes++; break;
                    case VoxelConnectionType.Full: fulls++; break;
                }
            }
            
            return (faces.Count, isolated + singles, corners, straights, tShapes, fulls);
        }
        
        #endregion
    }
}

