using UnityEngine;
using System.Collections.Generic;

namespace Voxel {
    /// <summary>
    /// ZoneVisualizer: Sistema de visualização de zonas no terreno.
    /// 
    /// Fase 2: "O Construtor" - Feedback Imediato
    /// 
    /// Este componente é responsável por colorir os vértices do terreno
    /// baseado nos dados do CityLayer, dando feedback visual imediato
    /// ao jogador quando ele pinta zonas.
    /// 
    /// Performance:
    /// - Usa vertex colors em vez de criar GameObjects (muito mais rápido)
    /// - Atualiza apenas chunks marcados como dirty
    /// - Escuta eventos do CityLayer para atualização reativa
    /// 
    /// Integração com Shader:
    /// - Vertex Color R: Tipo de zona (ZonaTipo enum / 255)
    /// - Vertex Color G: Validação (0 = inválido, 1 = válido)
    /// - Vertex Color B: Está pintado (0 = não, 1 = sim)
    /// - Vertex Color A: Reservado para expansão
    /// </summary>
    [AddComponentMenu("Voxel/Zone Visualizer")]
    public class ZoneVisualizer : MonoBehaviour {
        
        [Header("Referências")]
        [Tooltip("Autoridade lógica do zoneamento")]
        [SerializeField] private CityLayer cityLayer;
        
        [Tooltip("Mundo de voxels para acessar as meshes")]
        [SerializeField] private VoxelWorld voxelWorld;
        
        [Tooltip("Fonte de dados do terreno")]
        [SerializeField] private TerrainWorld terrainWorld;
        
        [Header("Configuração Visual")]
        [Tooltip("Material com shader de zona (VoxelZoneOverlay)")]
        [SerializeField] private Material zoneMaterial;
        
        [Tooltip("Se true, mostra cores das zonas mesmo quando o jogo está em produção")]
        [SerializeField] private bool showZonesInProduction = true;
        
        [Tooltip("Intensidade do overlay de zona (0-1)")]
        [Range(0f, 1f)]
        [SerializeField] private float overlayIntensity = 0.6f;
        
        /// <summary>
        /// Cache de dados de zona por coordenada de célula.
        /// Espelha os dados do CityLayer para acesso rápido durante a geração de mesh.
        /// </summary>
        private Dictionary<Vector2Int, ZoneCellData> _zoneCache = new Dictionary<Vector2Int, ZoneCellData>();
        
        /// <summary>
        /// Estrutura para armazenar dados de zona de uma célula.
        /// </summary>
        private struct ZoneCellData {
            public ZonaTipo tipo;
            public bool isValid;
            public bool isPainted;
        }
        
        void Awake() {
            // Auto-encontrar referências
            if (cityLayer == null) cityLayer = FindFirstObjectByType<CityLayer>();
            if (voxelWorld == null) voxelWorld = FindFirstObjectByType<VoxelWorld>();
            if (terrainWorld == null) terrainWorld = FindFirstObjectByType<TerrainWorld>();
        }
        
        void OnEnable() {
            // Inscreve nos eventos do CityLayer
            CityLayer.OnLoteChanged += OnLoteChanged;
            
            // Inscreve nos eventos de batch do ZoneBrush
            ZoneBrush.OnBatchPainted += OnBatchPainted;
        }
        
        void OnDisable() {
            CityLayer.OnLoteChanged -= OnLoteChanged;
            ZoneBrush.OnBatchPainted -= OnBatchPainted;
        }
        
        /// <summary>
        /// Chamado quando um lote individual é alterado no CityLayer.
        /// </summary>
        void OnLoteChanged(Lote lote) {
            if (lote == null) return;
            
            // Atualiza o cache local
            _zoneCache[lote.pos] = new ZoneCellData {
                tipo = lote.zona,
                isValid = lote.estaValido,
                isPainted = lote.zona != ZonaTipo.Nenhuma
            };
            
            // Nota: A regeneração da mesh é feita pelo ZoneBrush via dirty flags
        }
        
        /// <summary>
        /// Chamado quando um lote de células é pintado.
        /// Útil para efeitos visuais em batch.
        /// </summary>
        void OnBatchPainted(List<Vector2Int> cells, ZonaTipo tipo) {
            // Pode adicionar efeitos sonoros ou partículas aqui
            Debug.Log($"[ZoneVisualizer] Batch de {cells.Count} células pintadas como {tipo}");
        }
        
        /// <summary>
        /// Aplica cores de zona aos vértices de uma mesh.
        /// Chamado pelo ChunkMeshGenerator após gerar a geometria.
        /// </summary>
        /// <param name="mesh">Mesh do chunk a colorir</param>
        /// <param name="chunkPos">Posição do chunk</param>
        public void ApplyZoneColorsToMesh(Mesh mesh, Vector2Int chunkPos) {
            if (mesh == null) return;
            
            Vector3[] vertices = mesh.vertices;
            Color[] colors = new Color[vertices.Length];
            Vector3[] normals = mesh.normals;
            
            float scale = terrainWorld != null ? terrainWorld.escalaVoxel : 0.5f;
            
            for (int i = 0; i < vertices.Length; i++) {
                // Converte posição do vértice para coordenada de célula
                Vector3 worldPos = vertices[i];
                
                // Adiciona offset do chunk
                worldPos.x += chunkPos.x * ChunkData.Largura * scale;
                worldPos.z += chunkPos.y * ChunkData.Largura * scale;
                
                Vector2Int cellPos = new Vector2Int(
                    Mathf.FloorToInt(worldPos.x / scale),
                    Mathf.FloorToInt(worldPos.z / scale)
                );
                
                // Busca dados da zona no cache
                if (_zoneCache.TryGetValue(cellPos, out ZoneCellData zoneData)) {
                    // Codifica dados de zona na cor do vértice
                    // R = tipo de zona normalizado (0-1, onde 0-255 mapeia para ZonaTipo)
                    // G = validação (0 ou 1)
                    // B = está pintado (0 ou 1)
                    // A = reservado (1 = opaco)
                    colors[i] = new Color(
                        (float)zoneData.tipo / 255f,
                        zoneData.isValid ? 1f : 0f,
                        zoneData.isPainted ? 1f : 0f,
                        1f
                    );
                } else {
                    // Sem zona - cor padrão (transparente no shader)
                    colors[i] = new Color(0, 0, 0, 1);
                }
            }
            
            mesh.colors = colors;
        }
        
        /// <summary>
        /// Obtém a cor de uma zona específica.
        /// Pode ser usado pela UI ou outros sistemas.
        /// </summary>
        public static Color GetZoneColor(ZonaTipo tipo) {
            return ZoneBrush.GetZoneColor(tipo);
        }
        
        /// <summary>
        /// Verifica se uma posição tem zona pintada.
        /// </summary>
        public bool HasZoneAt(Vector2Int cellPos) {
            return _zoneCache.TryGetValue(cellPos, out var data) && data.isPainted;
        }
        
        /// <summary>
        /// Obtém o tipo de zona em uma posição.
        /// </summary>
        public ZonaTipo GetZoneAt(Vector2Int cellPos) {
            if (_zoneCache.TryGetValue(cellPos, out var data)) {
                return data.tipo;
            }
            return ZonaTipo.Nenhuma;
        }
        
        /// <summary>
        /// Limpa o cache de zonas (usado ao carregar um novo mapa).
        /// </summary>
        public void ClearCache() {
            _zoneCache.Clear();
        }
        
        /// <summary>
        /// Configura a intensidade do overlay de zona no material.
        /// </summary>
        public void SetOverlayIntensity(float intensity) {
            overlayIntensity = Mathf.Clamp01(intensity);
            if (zoneMaterial != null) {
                zoneMaterial.SetFloat("_ZoneOverlayStrength", overlayIntensity);
            }
        }
        
        /// <summary>
        /// Ativa/desativa a exibição do grid.
        /// </summary>
        public void SetShowGrid(bool show) {
            if (zoneMaterial != null) {
                zoneMaterial.SetFloat("_ShowGrid", show ? 1f : 0f);
            }
        }
    }
}

