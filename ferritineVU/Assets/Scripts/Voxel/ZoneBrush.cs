using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Voxel {
    /// <summary>
    /// ZoneBrush: Sistema de pintura de zonas para o CityLayer.
    /// 
    /// Fase 2: "O Construtor" - Modificação em Tempo Real
    /// 
    /// Conceitos implementados:
    /// - Pintura de zonas (Residencial, Comercial, Industrial, etc.)
    /// - Dirty Flags: usa HashSet para marcar chunks modificados
    /// - Atualização em lote: todos os dados são atualizados primeiro,
    ///   e só no final do frame os chunks são regenerados
    /// - Feedback visual imediato através do shader de zona
    /// 
    /// Analogia: O CityLayer é um livro de colorir, o mouse é o lápis,
    /// e este script decide qual cor aplicar em cada posição.
    /// 
    /// Por que HashSet?
    /// Se o jogador arrastar o mouse muito rápido e tocar o mesmo chunk
    /// 10 vezes no mesmo frame, o HashSet garante que ele só será atualizado
    /// uma vez. Isso é fundamental para manter os 60 FPS.
    /// </summary>
    [AddComponentMenu("Voxel/Zone Brush")]
    public class ZoneBrush : MonoBehaviour {
        
        #region Referências
        
        [Header("Referências Obrigatórias")]
        [Tooltip("Autoridade lógica do zoneamento urbano")]
        [SerializeField] private CityLayer cityLayer;
        
        [Tooltip("Fonte de dados do terreno (para altura e validação)")]
        [SerializeField] private TerrainWorld terrainWorld;
        
        [Tooltip("Mundo de voxels (para atualizar meshes dos chunks)")]
        [SerializeField] private VoxelWorld voxelWorld;
        
        [Tooltip("Câmera principal para raycasts")]
        [SerializeField] private Camera mainCamera;
        
        #endregion
        
        #region Configuração de Pintura
        
        [Header("Configuração de Pintura")]
        [Tooltip("Zona atualmente selecionada para pintar")]
        [SerializeField] private ZonaTipo zonaSelecionada = ZonaTipo.ResidencialBaixaDensidade;
        
        [Tooltip("Tamanho do pincel em células (1 = 1x1, 3 = 3x3, etc.)")]
        [Range(1, 10)]
        [SerializeField] private int tamanhoPincel = 1;
        
        [Tooltip("Se true, permite arrastar para pintar múltiplas células")]
        [SerializeField] private bool permitirArraste = true;
        
        [Tooltip("Distância máxima do raycast em metros")]
        [SerializeField] private float maxRaycastDistance = 500f;
        
        #endregion
        
        #region Input Actions
        
        [Header("Input Actions")]
        [Tooltip("Ação de pintura (clique esquerdo)")]
        [SerializeField] private InputActionReference paintAction;
        
        [Tooltip("Ação de apagar (clique direito)")]
        [SerializeField] private InputActionReference eraseAction;
        
        [Tooltip("Posição do mouse")]
        [SerializeField] private InputActionReference mousePositionAction;
        
        // Ações internas (fallback)
        private InputAction _internalPaintAction;
        private InputAction _internalEraseAction;
        private InputAction _internalMousePositionAction;
        
        #endregion
        
        #region Preview Visual
        
        [Header("Preview Visual")]
        [Tooltip("Prefab para mostrar preview da área de pintura")]
        [SerializeField] private GameObject previewPrefab;
        
        [Tooltip("Cor do preview ao pintar")]
        [SerializeField] private Color paintPreviewColor = new Color(0.3f, 1f, 0.3f, 0.5f);
        
        [Tooltip("Cor do preview ao apagar")]
        [SerializeField] private Color erasePreviewColor = new Color(1f, 0.3f, 0.3f, 0.5f);
        
        private GameObject _previewInstance;
        private Renderer _previewRenderer;
        private MaterialPropertyBlock _previewPropertyBlock;
        
        #endregion
        
        #region Estado Interno - Dirty Flags
        
        /// <summary>
        /// HashSet de chunks que foram modificados neste frame.
        /// Usando HashSet garante que cada chunk só aparece uma vez,
        /// mesmo se o jogador tocar o mesmo chunk múltiplas vezes no arraste.
        /// 
        /// VANTAGEM DO HASHSET:
        /// - Se o mesmo chunk for tocado 10 vezes no mesmo frame,
        ///   ele só será atualizado UMA vez no LateUpdate.
        /// - Isso é O(1) para adicionar e verificar, vs O(n) de uma lista.
        /// </summary>
        private HashSet<Vector2Int> _chunksToUpdate = new HashSet<Vector2Int>();
        
        /// <summary>
        /// Posições de células que foram pintadas neste frame.
        /// Usado para o evento de lote completo.
        /// </summary>
        private List<Vector2Int> _cellsPaintedThisFrame = new List<Vector2Int>();
        
        /// <summary>
        /// Última posição de célula pintada (para evitar pintura duplicada durante arraste)
        /// </summary>
        private Vector2Int _lastPaintedCell = new Vector2Int(int.MinValue, int.MinValue);
        
        /// <summary>
        /// ID da propriedade _Color do shader (cache para performance)
        /// </summary>
        private static readonly int ShaderColorPropertyId = Shader.PropertyToID("_Color");
        
        /// <summary>
        /// Se está atualmente arrastando para pintar
        /// </summary>
        private bool _isPainting;
        
        /// <summary>
        /// Se está atualmente arrastando para apagar
        /// </summary>
        private bool _isErasing;
        
        /// <summary>
        /// Hit do raycast atual
        /// </summary>
        private VoxelRaycast.VoxelHitInfo _currentHit;
        
        #endregion
        
        #region Eventos
        
        /// <summary>
        /// Evento disparado quando um lote de células é pintado.
        /// Útil para sistemas de som, partículas, ou analytics.
        /// </summary>
        public static event System.Action<List<Vector2Int>, ZonaTipo> OnBatchPainted;
        
        /// <summary>
        /// Evento disparado quando a zona selecionada muda.
        /// </summary>
        public static event System.Action<ZonaTipo> OnZoneSelectionChanged;
        
        #endregion
        
        #region Lifecycle
        
        void Awake() {
            // Auto-encontrar referências
            if (mainCamera == null) mainCamera = Camera.main;
            if (terrainWorld == null) terrainWorld = FindFirstObjectByType<TerrainWorld>();
            if (voxelWorld == null) voxelWorld = FindFirstObjectByType<VoxelWorld>();
            if (cityLayer == null) cityLayer = FindFirstObjectByType<CityLayer>();
            
            // Validar referências críticas
            if (cityLayer == null) {
                Debug.LogError("[ZoneBrush] CityLayer não encontrado! O sistema de pintura não funcionará.");
            }
            if (terrainWorld == null) {
                Debug.LogError("[ZoneBrush] TerrainWorld não encontrado! O sistema de pintura não funcionará.");
            }
            
            SetupInputActions();
            SetupPreview();
        }
        
        void SetupInputActions() {
            // Paint (clique esquerdo)
            if (paintAction == null || paintAction.action == null) {
                _internalPaintAction = new InputAction("Paint", InputActionType.Button, "<Mouse>/leftButton");
            }
            
            // Erase (clique direito)
            if (eraseAction == null || eraseAction.action == null) {
                _internalEraseAction = new InputAction("Erase", InputActionType.Button, "<Mouse>/rightButton");
            }
            
            // Mouse position
            if (mousePositionAction == null || mousePositionAction.action == null) {
                _internalMousePositionAction = new InputAction("MousePos", InputActionType.Value, "<Mouse>/position");
            }
        }
        
        void SetupPreview() {
            if (previewPrefab != null) {
                _previewInstance = Instantiate(previewPrefab);
            } else {
                // Cria preview automaticamente se não houver prefab
                _previewInstance = CreateDefaultPreview();
            }
            
            if (_previewInstance != null) {
                _previewInstance.name = "ZoneBrushPreview";
                _previewInstance.SetActive(false);
                
                _previewRenderer = _previewInstance.GetComponent<Renderer>();
                if (_previewRenderer != null) {
                    _previewPropertyBlock = new MaterialPropertyBlock();
                }
            }
        }
        
        /// <summary>
        /// Cria um preview padrão (quad horizontal) se não houver prefab atribuído.
        /// </summary>
        GameObject CreateDefaultPreview() {
            GameObject preview = GameObject.CreatePrimitive(PrimitiveType.Quad);
            
            // Rotaciona para ficar horizontal
            preview.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            
            // Remove o collider
            var collider = preview.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
            
            // Cria material transparente
            Renderer renderer = preview.GetComponent<Renderer>();
            if (renderer != null) {
                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.color = paintPreviewColor;
                mat.renderQueue = 3000;
                renderer.material = mat;
            }
            
            return preview;
        }
        
        void OnEnable() {
            GetPaintAction()?.Enable();
            GetEraseAction()?.Enable();
            GetMousePositionAction()?.Enable();
        }
        
        void OnDisable() {
            GetPaintAction()?.Disable();
            GetEraseAction()?.Disable();
            GetMousePositionAction()?.Disable();
            
            if (_previewInstance != null) _previewInstance.SetActive(false);
        }
        
        void OnDestroy() {
            _internalPaintAction?.Dispose();
            _internalEraseAction?.Dispose();
            _internalMousePositionAction?.Dispose();
            
            if (_previewInstance != null) Destroy(_previewInstance);
        }
        
        #endregion
        
        #region Update Loop
        
        void Update() {
            if (cityLayer == null || terrainWorld == null || mainCamera == null) return;
            
            // 1. Atualizar raycast
            UpdateRaycast();
            
            // 2. Processar inputs de pintura
            HandlePaintInput();
            HandleEraseInput();
            
            // 3. Atualizar preview visual
            UpdatePreview();
        }
        
        /// <summary>
        /// LateUpdate: Momento perfeito para processar os Dirty Flags.
        /// Todos os dados já foram atualizados durante o frame,
        /// agora regeneramos as meshes apenas UMA VEZ por chunk.
        /// 
        /// Esta é a "mágica" para manter os 60 FPS:
        /// Em vez de regenerar a mesh a cada célula pintada,
        /// esperamos o fim do frame e processamos tudo em lote.
        /// </summary>
        void LateUpdate() {
            ProcessDirtyChunks();
        }
        
        #endregion
        
        #region Raycast
        
        void UpdateRaycast() {
            var mouseAction = GetMousePositionAction();
            if (mouseAction != null && mouseAction.enabled) {
                _currentHit = VoxelRaycast.RaycastFromInputAction(
                    mainCamera, 
                    mouseAction, 
                    terrainWorld, 
                    maxRaycastDistance
                );
            } else {
                _currentHit = VoxelRaycast.RaycastFromMouse(mainCamera, terrainWorld, maxRaycastDistance);
            }
        }
        
        #endregion
        
        #region Input Handling
        
        void HandlePaintInput() {
            var paintAct = GetPaintAction();
            if (paintAct == null) return;
            
            // Início do clique
            if (paintAct.WasPressedThisFrame()) {
                _isPainting = true;
                _lastPaintedCell = new Vector2Int(int.MinValue, int.MinValue);
            }
            
            // Durante o arraste (ou clique único)
            if (_isPainting && paintAct.IsPressed()) {
                if (_currentHit.Hit) {
                    Vector2Int cellPos = WorldToCell(_currentHit.WorldHitPoint);
                    
                    // Evita pintar a mesma célula múltiplas vezes no arraste
                    if (cellPos != _lastPaintedCell || !permitirArraste) {
                        PaintArea(cellPos, zonaSelecionada);
                        _lastPaintedCell = cellPos;
                    }
                }
            }
            
            // Fim do clique
            if (paintAct.WasReleasedThisFrame()) {
                _isPainting = false;
                FinalizeBatch();
            }
        }
        
        void HandleEraseInput() {
            var eraseAct = GetEraseAction();
            if (eraseAct == null) return;
            
            // Início do clique
            if (eraseAct.WasPressedThisFrame()) {
                _isErasing = true;
                _lastPaintedCell = new Vector2Int(int.MinValue, int.MinValue);
            }
            
            // Durante o arraste
            if (_isErasing && eraseAct.IsPressed()) {
                if (_currentHit.Hit) {
                    Vector2Int cellPos = WorldToCell(_currentHit.WorldHitPoint);
                    
                    if (cellPos != _lastPaintedCell || !permitirArraste) {
                        PaintArea(cellPos, ZonaTipo.Nenhuma); // Apagar = pintar com Nenhuma
                        _lastPaintedCell = cellPos;
                    }
                }
            }
            
            // Fim do clique
            if (eraseAct.WasReleasedThisFrame()) {
                _isErasing = false;
                FinalizeBatch();
            }
        }
        
        #endregion
        
        #region Pintura de Zonas
        
        /// <summary>
        /// Pinta uma área baseada no tamanho do pincel.
        /// Todos os dados são atualizados no CityLayer primeiro,
        /// e os chunks são apenas MARCADOS como dirty (não regenerados ainda).
        /// </summary>
        void PaintArea(Vector2Int center, ZonaTipo tipo) {
            int halfSize = tamanhoPincel / 2;
            
            for (int dx = -halfSize; dx <= halfSize; dx++) {
                for (int dz = -halfSize; dz <= halfSize; dz++) {
                    Vector2Int cellPos = new Vector2Int(center.x + dx, center.y + dz);
                    
                    // 1. ATUALIZAR DADOS (CityLayer é a autoridade)
                    cityLayer.PintarZona(cellPos, tipo);
                    
                    // 2. MARCAR CHUNK COMO DIRTY (HashSet garante unicidade)
                    Vector2Int chunkPos = CellToChunk(cellPos);
                    _chunksToUpdate.Add(chunkPos);
                    
                    // Também marca chunks vizinhos se a célula estiver na borda
                    // Isso resolve o problema de arrastar rápido entre chunks
                    MarkNeighborChunksIfOnBorder(cellPos, chunkPos);
                    
                    // 3. Registrar célula pintada para o evento de lote
                    _cellsPaintedThisFrame.Add(cellPos);
                }
            }
        }
        
        /// <summary>
        /// Marca chunks vizinhos como dirty se a célula estiver na borda.
        /// Isso garante que quando o jogador arrasta rápido e cruza a fronteira
        /// entre dois chunks, ambos serão atualizados.
        /// </summary>
        void MarkNeighborChunksIfOnBorder(Vector2Int cellPos, Vector2Int currentChunk) {
            // Calcula posição local dentro do chunk
            int localX = cellPos.x - (currentChunk.x * ChunkData.Largura);
            int localZ = cellPos.y - (currentChunk.y * ChunkData.Largura);
            
            // Se está na borda esquerda
            if (localX == 0) {
                _chunksToUpdate.Add(new Vector2Int(currentChunk.x - 1, currentChunk.y));
            }
            // Se está na borda direita
            if (localX == ChunkData.Largura - 1) {
                _chunksToUpdate.Add(new Vector2Int(currentChunk.x + 1, currentChunk.y));
            }
            // Se está na borda inferior
            if (localZ == 0) {
                _chunksToUpdate.Add(new Vector2Int(currentChunk.x, currentChunk.y - 1));
            }
            // Se está na borda superior
            if (localZ == ChunkData.Largura - 1) {
                _chunksToUpdate.Add(new Vector2Int(currentChunk.x, currentChunk.y + 1));
            }
        }
        
        /// <summary>
        /// Processa todos os chunks marcados como dirty.
        /// Chamado no LateUpdate para garantir que todos os dados
        /// já foram atualizados durante o frame.
        /// 
        /// PERFORMANCE:
        /// - Se o jogador pintou 100 células em 5 chunks diferentes,
        ///   regeneramos apenas 5 meshes (uma por chunk).
        /// - Sem este sistema, regeneraríamos 100 meshes!
        /// </summary>
        void ProcessDirtyChunks() {
            if (_chunksToUpdate.Count == 0) return;
            
            foreach (Vector2Int chunkPos in _chunksToUpdate) {
                // Notifica o VoxelWorld que este chunk precisa ter sua mesh regenerada
                // O VoxelWorld já tem sistema de dirty flags interno, então isso é eficiente
                RegenerateChunkMesh(chunkPos);
            }
            
            // Limpa o conjunto para o próximo frame
            _chunksToUpdate.Clear();
        }
        
        /// <summary>
        /// Solicita regeneração da mesh de um chunk específico.
        /// Esta função pode ser expandida para usar jobs ou coroutines
        /// para distribuir o trabalho ao longo de múltiplos frames.
        /// </summary>
        void RegenerateChunkMesh(Vector2Int chunkPos) {
            // O TerrainWorld já marca chunks como IsDirty internamente
            // O VoxelWorld verifica isso e regenera as meshes no seu Update
            var chunkData = terrainWorld.GetGarantirChunk(chunkPos);
            if (chunkData != null) {
                chunkData.IsDirty = true;
            }
        }
        
        /// <summary>
        /// Finaliza um lote de pintura, disparando o evento.
        /// Chamado quando o jogador solta o botão do mouse.
        /// </summary>
        void FinalizeBatch() {
            if (_cellsPaintedThisFrame.Count > 0) {
                // Dispara evento para outros sistemas (som, partículas, analytics)
                OnBatchPainted?.Invoke(new List<Vector2Int>(_cellsPaintedThisFrame), zonaSelecionada);
                _cellsPaintedThisFrame.Clear();
            }
        }
        
        #endregion
        
        #region Conversões de Coordenadas
        
        /// <summary>
        /// Converte posição do mundo para coordenada de célula do grid.
        /// </summary>
        Vector2Int WorldToCell(Vector3 worldPos) {
            float scale = terrainWorld.escalaVoxel;
            int cellX = Mathf.FloorToInt(worldPos.x / scale);
            int cellZ = Mathf.FloorToInt(worldPos.z / scale);
            return new Vector2Int(cellX, cellZ);
        }
        
        /// <summary>
        /// Converte coordenada de célula para coordenada de chunk.
        /// </summary>
        Vector2Int CellToChunk(Vector2Int cellPos) {
            int chunkX = Mathf.FloorToInt((float)cellPos.x / ChunkData.Largura);
            int chunkZ = Mathf.FloorToInt((float)cellPos.y / ChunkData.Largura);
            return new Vector2Int(chunkX, chunkZ);
        }
        
        /// <summary>
        /// Converte coordenada de célula para posição do mundo.
        /// </summary>
        Vector3 CellToWorld(Vector2Int cellPos) {
            float scale = terrainWorld.escalaVoxel;
            return new Vector3(cellPos.x * scale, 0, cellPos.y * scale);
        }
        
        #endregion
        
        #region Preview Visual
        
        void UpdatePreview() {
            if (_previewInstance == null) return;
            
            if (_currentHit.Hit) {
                _previewInstance.SetActive(true);
                
                // Posiciona o preview na célula atual
                Vector2Int cellPos = WorldToCell(_currentHit.WorldHitPoint);
                Vector3 previewPos = CellToWorld(cellPos);
                
                // Ajusta altura para ficar acima do terreno
                float height = terrainWorld.GetHeight(cellPos.x, cellPos.y);
                previewPos.y = height + 0.1f;
                
                _previewInstance.transform.position = previewPos;
                
                // Ajusta escala baseado no tamanho do pincel
                float scale = tamanhoPincel * terrainWorld.escalaVoxel;
                _previewInstance.transform.localScale = new Vector3(scale, 0.1f, scale);
                
                // Muda cor baseado na ação (pintar/apagar)
                if (_previewRenderer != null && _previewPropertyBlock != null) {
                    Color previewColor = _isErasing ? erasePreviewColor : paintPreviewColor;
                    
                    // Adiciona cor da zona selecionada se estiver pintando
                    if (!_isErasing) {
                        previewColor = GetZoneColor(zonaSelecionada);
                        previewColor.a = 0.5f;
                    }
                    
                    _previewPropertyBlock.SetColor(ShaderColorPropertyId, previewColor);
                    _previewRenderer.SetPropertyBlock(_previewPropertyBlock);
                }
            } else {
                _previewInstance.SetActive(false);
            }
        }
        
        #endregion
        
        #region API Pública
        
        /// <summary>
        /// Define a zona a ser pintada.
        /// </summary>
        public void SetZona(ZonaTipo tipo) {
            if (zonaSelecionada != tipo) {
                zonaSelecionada = tipo;
                OnZoneSelectionChanged?.Invoke(tipo);
            }
        }
        
        /// <summary>
        /// Obtém a zona atualmente selecionada.
        /// </summary>
        public ZonaTipo GetZonaSelecionada() => zonaSelecionada;
        
        /// <summary>
        /// Define o tamanho do pincel.
        /// </summary>
        public void SetTamanhoPincel(int tamanho) {
            tamanhoPincel = Mathf.Clamp(tamanho, 1, 10);
        }
        
        /// <summary>
        /// Obtém o tamanho atual do pincel.
        /// </summary>
        public int GetTamanhoPincel() => tamanhoPincel;
        
        /// <summary>
        /// Pinta uma célula específica programaticamente.
        /// Útil para sistemas automáticos ou IA.
        /// </summary>
        public void PaintCell(Vector2Int cellPos, ZonaTipo tipo) {
            cityLayer.PintarZona(cellPos, tipo);
            _chunksToUpdate.Add(CellToChunk(cellPos));
        }
        
        /// <summary>
        /// Pinta uma área retangular programaticamente.
        /// </summary>
        public void PaintRect(Vector2Int min, Vector2Int max, ZonaTipo tipo) {
            for (int x = min.x; x <= max.x; x++) {
                for (int z = min.y; z <= max.y; z++) {
                    PaintCell(new Vector2Int(x, z), tipo);
                }
            }
            // Processa imediatamente se chamado fora do Update loop
            ProcessDirtyChunks();
        }
        
        #endregion
        
        #region Helpers
        
        InputAction GetPaintAction() => paintAction?.action ?? _internalPaintAction;
        InputAction GetEraseAction() => eraseAction?.action ?? _internalEraseAction;
        InputAction GetMousePositionAction() => mousePositionAction?.action ?? _internalMousePositionAction;
        
        /// <summary>
        /// Retorna uma cor representativa para cada tipo de zona.
        /// Usado para preview visual e pode ser usado pelo shader.
        /// </summary>
        public static Color GetZoneColor(ZonaTipo tipo) {
            switch (tipo) {
                // Residenciais - tons de verde
                case ZonaTipo.ResidencialBaixaDensidade:
                    return new Color(0.4f, 0.8f, 0.4f, 1f); // Verde claro
                case ZonaTipo.ResidencialMediaDensidade:
                    return new Color(0.2f, 0.7f, 0.2f, 1f); // Verde médio
                case ZonaTipo.ResidencialAltaDensidade:
                    return new Color(0.1f, 0.5f, 0.1f, 1f); // Verde escuro
                
                // Comerciais - tons de azul
                case ZonaTipo.ComercialLocal:
                    return new Color(0.4f, 0.6f, 0.9f, 1f); // Azul claro
                case ZonaTipo.ComercialCentral:
                    return new Color(0.2f, 0.4f, 0.8f, 1f); // Azul médio
                
                // Industriais - tons de amarelo/laranja
                case ZonaTipo.IndustrialLeve:
                    return new Color(0.9f, 0.9f, 0.4f, 1f); // Amarelo
                case ZonaTipo.IndustrialPesada:
                    return new Color(0.9f, 0.7f, 0.2f, 1f); // Laranja
                
                // Especiais
                case ZonaTipo.Misto:
                    return new Color(0.7f, 0.5f, 0.8f, 1f); // Roxo
                case ZonaTipo.Rural:
                    return new Color(0.6f, 0.5f, 0.3f, 1f); // Marrom
                case ZonaTipo.Agricultura:
                    return new Color(0.8f, 0.9f, 0.3f, 1f); // Verde-amarelo
                case ZonaTipo.Parque:
                    return new Color(0.2f, 0.9f, 0.5f, 1f); // Verde-água
                case ZonaTipo.Via:
                    return new Color(0.5f, 0.5f, 0.5f, 1f); // Cinza
                case ZonaTipo.Infraestrutura:
                    return new Color(0.7f, 0.7f, 0.7f, 1f); // Cinza claro
                case ZonaTipo.Institucional:
                    return new Color(0.8f, 0.6f, 0.6f, 1f); // Rosa
                case ZonaTipo.Historico:
                    return new Color(0.9f, 0.8f, 0.6f, 1f); // Bege
                case ZonaTipo.Turismo:
                    return new Color(0.4f, 0.8f, 0.9f, 1f); // Cyan
                
                default:
                    return new Color(0.5f, 0.5f, 0.5f, 0.3f); // Cinza transparente
            }
        }
        
        #endregion
    }
}

