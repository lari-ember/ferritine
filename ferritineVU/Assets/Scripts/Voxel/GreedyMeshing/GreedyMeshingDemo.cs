using UnityEngine;
using System.Collections.Generic;

namespace Voxel.GreedyMeshing {
    /// <summary>
    /// GreedyMeshingDemo: Componente de demonstração e teste do sistema Greedy Meshing.
    /// 
    /// Adicione este componente a um GameObject na cena para:
    /// - Visualizar bordas de voxels em tempo real
    /// - Ver estatísticas de otimização
    /// - Testar diferentes modos de meshing
    /// 
    /// Uso: Arraste um VoxelWorld e um TerrainWorld para as referências.
    /// </summary>
    [AddComponentMenu("Voxel/Greedy Meshing Demo")]
    public class GreedyMeshingDemo : MonoBehaviour {
        
        #region Referências
        
        [Header("Referências")]
        [Tooltip("Mundo de voxels para teste")]
        public VoxelWorld voxelWorld;
        
        [Tooltip("Terreno para consultas cross-chunk")]
        public TerrainWorld terrainWorld;
        
        #endregion
        
        #region Configuração de Visualização
        
        [Header("Visualização de Bordas")]
        [Tooltip("Mostrar gizmos de bordas no editor")]
        public bool showBorderGizmos = true;
        
        [Tooltip("Cor para voxels isolados")]
        public Color isolatedColor = Color.red;
        
        [Tooltip("Cor para voxels em corner (L)")]
        public Color cornerColor = Color.yellow;
        
        [Tooltip("Cor para voxels em linha (I)")]
        public Color straightColor = Color.green;
        
        [Tooltip("Cor para voxels em T")]
        public Color tShapeColor = Color.cyan;
        
        [Tooltip("Cor para voxels com todas conexões")]
        public Color fullColor = Color.blue;
        
        [Tooltip("Tamanho dos gizmos")]
        [Range(0.1f, 1f)]
        public float gizmoSize = 0.3f;
        
        #endregion
        
        #region Configuração de Teste
        
        [Header("Teste de Chunk")]
        [Tooltip("Posição do chunk para análise")]
        public Vector2Int testChunkPos = Vector2Int.zero;
        
        [Tooltip("Camada Y para visualização 2D")]
        [Range(0, 255)]
        public int visualizationY = 10;
        
        #endregion
        
        #region Estado Interno
        
        private List<VoxelFace> _cachedBorderFaces;
        private ChunkData _cachedChunkData;
        private ChunkMeshGeneratorGreedy.MeshStats _lastStats;
        
        #endregion
        
        #region Métodos Públicos
        
        /// <summary>
        /// Analisa um chunk e cacheia os resultados.
        /// </summary>
        [ContextMenu("Analisar Chunk")]
        public void AnalyzeChunk() {
            if (terrainWorld == null) {
                Debug.LogError("[GreedyMeshingDemo] TerrainWorld não configurado!");
                return;
            }
            
            // Obtém dados do chunk
            _cachedChunkData = terrainWorld.GetGarantirChunk(testChunkPos);
            
            if (_cachedChunkData == null) {
                Debug.LogWarning($"[GreedyMeshingDemo] Chunk {testChunkPos} não encontrado!");
                return;
            }
            
            int chunkGlobalX = testChunkPos.x * ChunkData.Largura;
            int chunkGlobalZ = testChunkPos.y * ChunkData.Largura;
            
            // Identifica bordas
            _cachedBorderFaces = VoxelBorderDetector.IdentifyBorderFaces(
                _cachedChunkData.voxels,
                ChunkData.Largura,
                _cachedChunkData.Altura,
                ChunkData.Largura,
                terrainWorld,
                chunkGlobalX,
                chunkGlobalZ
            );
            
            // Estatísticas
            var borderStats = VoxelBorderDetector.GetBorderStats(_cachedBorderFaces);
            
            Debug.Log($"[GreedyMeshingDemo] Chunk {testChunkPos} analisado:\n" +
                $"  Total de faces de borda: {borderStats.total}\n" +
                $"  Isoladas/Singles: {borderStats.isolated}\n" +
                $"  Corners (L): {borderStats.corners}\n" +
                $"  Straights (I): {borderStats.straights}\n" +
                $"  T-Shapes: {borderStats.tShapes}\n" +
                $"  Full (+): {borderStats.fulls}");
        }
        
        /// <summary>
        /// Gera mesh otimizada e mostra estatísticas.
        /// </summary>
        [ContextMenu("Gerar Mesh Greedy")]
        public void GenerateGreedyMesh() {
            if (terrainWorld == null || _cachedChunkData == null) {
                AnalyzeChunk();
                if (_cachedChunkData == null) return;
            }
            
            var (mesh, stats) = ChunkMeshGeneratorGreedy.BuildMeshWithStats(
                terrainWorld,
                _cachedChunkData,
                terrainWorld.escalaVoxel
            );
            
            _lastStats = stats;
            
            Debug.Log($"[GreedyMeshingDemo] Mesh gerada:\n" +
                $"  Faces originais: {stats.OriginalFaceCount}\n" +
                $"  Faces otimizadas: {stats.OptimizedFaceCount}\n" +
                $"  Redução: {stats.ReductionPercent:F1}%\n" +
                $"  Vértices: {stats.OptimizedVertexCount}\n" +
                $"  Triângulos: {stats.OptimizedTriangleCount}");
            
            // Limpa mesh temporária
            if (mesh != null) {
                DestroyImmediate(mesh);
            }
        }
        
        /// <summary>
        /// Compara performance entre método padrão e greedy.
        /// </summary>
        [ContextMenu("Comparar Performance")]
        public void ComparePerformance() {
            if (terrainWorld == null) {
                Debug.LogError("[GreedyMeshingDemo] TerrainWorld não configurado!");
                return;
            }
            
            _cachedChunkData = terrainWorld.GetGarantirChunk(testChunkPos);
            if (_cachedChunkData == null) {
                Debug.LogWarning($"[GreedyMeshingDemo] Chunk {testChunkPos} não encontrado!");
                return;
            }
            
            float scale = terrainWorld.escalaVoxel;
            
            // Teste do método padrão
            var sw1 = System.Diagnostics.Stopwatch.StartNew();
            var meshStandard = ChunkMeshGenerator.BuildMesh(terrainWorld, _cachedChunkData, scale);
            sw1.Stop();
            int standardVerts = meshStandard.vertexCount;
            int standardTris = meshStandard.triangles.Length / 3;
            DestroyImmediate(meshStandard);
            
            // Teste do método greedy
            var sw2 = System.Diagnostics.Stopwatch.StartNew();
            var meshGreedy = ChunkMeshGeneratorGreedy.BuildMesh(terrainWorld, _cachedChunkData, scale);
            sw2.Stop();
            int greedyVerts = meshGreedy.vertexCount;
            int greedyTris = meshGreedy.triangles.Length / 3;
            DestroyImmediate(meshGreedy);
            
            float vertReduction = 100f * (1f - (float)greedyVerts / standardVerts);
            float triReduction = 100f * (1f - (float)greedyTris / standardTris);
            
            Debug.Log($"[GreedyMeshingDemo] Comparação de Performance:\n" +
                $"=== Método Padrão ===\n" +
                $"  Tempo: {sw1.ElapsedMilliseconds}ms\n" +
                $"  Vértices: {standardVerts}\n" +
                $"  Triângulos: {standardTris}\n" +
                $"=== Greedy Meshing ===\n" +
                $"  Tempo: {sw2.ElapsedMilliseconds}ms\n" +
                $"  Vértices: {greedyVerts} ({vertReduction:F1}% redução)\n" +
                $"  Triângulos: {greedyTris} ({triReduction:F1}% redução)");
        }
        
        /// <summary>
        /// Analisa conectividade de um voxel específico.
        /// </summary>
        public void AnalyzeVoxel(int x, int y, int z) {
            if (_cachedChunkData == null) {
                AnalyzeChunk();
                if (_cachedChunkData == null) return;
            }
            
            var (type, exposed, dirs) = FaceRegionMerger.AnalyzeVoxelConnectivity(
                _cachedChunkData.voxels,
                x, y, z,
                ChunkData.Largura,
                _cachedChunkData.Altura,
                ChunkData.Largura
            );
            
            string dirsStr = dirs.Length > 0 ? 
                string.Join(", ", System.Array.ConvertAll(dirs, d => d.ToString())) : 
                "Nenhuma";
            
            Debug.Log($"[GreedyMeshingDemo] Voxel ({x},{y},{z}):\n" +
                $"  Tipo de conexão: {type}\n" +
                $"  Faces expostas: {exposed}\n" +
                $"  Direções conectadas: {dirsStr}");
        }
        
        #endregion
        
        #region Gizmos
        
#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            if (!showBorderGizmos || _cachedBorderFaces == null || _cachedChunkData == null) {
                return;
            }
            
            float scale = terrainWorld != null ? terrainWorld.escalaVoxel : 1f;
            Vector3 chunkOffset = new Vector3(
                testChunkPos.x * ChunkData.Largura * scale,
                0,
                testChunkPos.y * ChunkData.Largura * scale
            );
            
            foreach (var face in _cachedBorderFaces) {
                // Filtra por camada Y se configurado
                if (visualizationY >= 0 && face.VoxelPosition.y != visualizationY) {
                    continue;
                }
                
                // Cor baseada no tipo de conexão
                Gizmos.color = GetConnectionColor(face.ConnectionType);
                
                Vector3 worldPos = chunkOffset + new Vector3(
                    (face.VoxelPosition.x + 0.5f) * scale,
                    (face.VoxelPosition.y + 0.5f) * scale,
                    (face.VoxelPosition.z + 0.5f) * scale
                );
                
                Gizmos.DrawWireCube(worldPos, Vector3.one * gizmoSize * scale);
                
                // Desenha seta na direção da face
                Vector3 faceDir = GetFaceDirection(face.Direction) * scale * 0.5f;
                Gizmos.DrawLine(worldPos, worldPos + faceDir);
            }
        }
        
        private Color GetConnectionColor(VoxelConnectionType type) {
            switch (type) {
                case VoxelConnectionType.Isolated:
                case VoxelConnectionType.Single:
                    return isolatedColor;
                case VoxelConnectionType.Corner:
                    return cornerColor;
                case VoxelConnectionType.Straight:
                    return straightColor;
                case VoxelConnectionType.TShape:
                    return tShapeColor;
                case VoxelConnectionType.Full:
                    return fullColor;
                default:
                    return Color.white;
            }
        }
        
        private Vector3 GetFaceDirection(int dir) {
            switch (dir) {
                case 0: return Vector3.right;
                case 1: return Vector3.left;
                case 2: return Vector3.forward;
                case 3: return Vector3.back;
                default: return Vector3.zero;
            }
        }
#endif
        
        #endregion
        
        #region GUI de Debug
        
        void OnGUI() {
            if (!Application.isPlaying) return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label("=== Greedy Meshing Demo ===");
            GUILayout.Label($"Chunk: {testChunkPos}");
            
            if (_cachedBorderFaces != null) {
                GUILayout.Label($"Faces de borda: {_cachedBorderFaces.Count}");
            }
            
            if (_lastStats.OriginalFaceCount > 0) {
                GUILayout.Label($"Redução: {_lastStats.ReductionPercent:F1}%");
            }
            
            if (GUILayout.Button("Analisar Chunk")) {
                AnalyzeChunk();
            }
            
            if (GUILayout.Button("Gerar Mesh Greedy")) {
                GenerateGreedyMesh();
            }
            
            if (GUILayout.Button("Comparar Performance")) {
                ComparePerformance();
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        #endregion
    }
}

