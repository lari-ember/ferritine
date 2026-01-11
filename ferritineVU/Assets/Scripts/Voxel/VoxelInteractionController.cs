using UnityEngine;
using UnityEngine.InputSystem;

namespace Voxel {
    /// <summary>
    /// Controlador de interação com voxels usando o novo Input System.
    /// 
    /// Este componente gerencia:
    /// - Clique esquerdo: Destruir/minerar voxel
    /// - Clique direito: Colocar novo voxel adjacente
    /// - Preview: Mostra qual voxel será afetado
    /// 
    /// SETUP:
    /// 1. Adicione este componente a um GameObject (ex: Player ou Camera)
    /// 2. Configure as referências no Inspector:
    ///    - Arraste o TerrainWorld
    ///    - Arraste a Camera principal
    ///    - Configure as Input Actions (Click, SecondaryClick, MousePosition)
    /// 3. Opcionalmente, configure um prefab de preview para feedback visual
    /// 
    /// IMPORTANTE: Certifique-se de que os chunks têm a Layer "Terrain" configurada
    /// para que o Physics.Raycast funcione corretamente como fallback.
    /// </summary>
    public class VoxelInteractionController : MonoBehaviour {
        
        [Header("Referências")]
        [Tooltip("Referência ao mundo de voxels")]
        [SerializeField] private TerrainWorld terrainWorld;
        
        [Tooltip("Câmera principal para calcular raycasts")]
        [SerializeField] private Camera mainCamera;
        
        [Header("Input Actions (Novo Input System)")]
        [Tooltip("Ação de clique primário (destruir voxel)")]
        [SerializeField] private InputActionReference clickAction;
        
        [Tooltip("Ação de clique secundário (colocar voxel)")]
        [SerializeField] private InputActionReference secondaryClickAction;
        
        [Tooltip("Ação que fornece a posição do mouse")]
        [SerializeField] private InputActionReference mousePositionAction;
        
        [Header("Configurações de Raycast")]
        [Tooltip("Distância máxima do raycast em metros")]
        [SerializeField] private float maxRaycastDistance = 100f;
        
        [Tooltip("Layer mask para raycast de física (fallback)")]
        [SerializeField] private LayerMask terrainLayerMask = -1;
        
        [Header("Configurações de Interação")]
        [Tooltip("Tipo de bloco a ser colocado com clique secundário")]
        [SerializeField] private BlockType blockToPlace = BlockType.Terra;
        
        [Tooltip("Se true, usa raycast de voxel DDA. Se false, usa Physics.Raycast")]
        [SerializeField] private bool useVoxelRaycast = true;
        
        [Header("Preview Visual (Opcional)")]
        [Tooltip("Prefab para mostrar preview do voxel alvo")]
        [SerializeField] private GameObject previewPrefab;
        
        [Tooltip("Cor do preview ao destruir")]
        [SerializeField] private Color destroyPreviewColor = new Color(1f, 0.3f, 0.3f, 0.5f);
        
        [Tooltip("Cor do preview ao colocar")]
        [SerializeField] private Color placePreviewColor = new Color(0.3f, 1f, 0.3f, 0.5f);
        
        // Estado interno
        private GameObject _previewInstance;
        private Renderer _previewRenderer;
        private MaterialPropertyBlock _previewPropertyBlock;
        private VoxelRaycast.VoxelHitInfo _lastHit;
        private bool _isSecondaryHeld;
        
        // Ações criadas dinamicamente (fallback se não configuradas no Inspector)
        private InputAction _internalClickAction;
        private InputAction _internalSecondaryClickAction;
        private InputAction _internalMousePositionAction;
        
        void Awake() {
            // Auto-encontrar referências se não configuradas
            if (mainCamera == null) {
                mainCamera = Camera.main;
                if (mainCamera == null) {
                    Debug.LogError("[VoxelInteractionController] Câmera principal não encontrada!");
                }
            }
            
            if (terrainWorld == null) {
                terrainWorld = FindFirstObjectByType<TerrainWorld>();
                if (terrainWorld == null) {
                    Debug.LogWarning("[VoxelInteractionController] TerrainWorld não encontrado. Configure manualmente.");
                }
            }
            
            // Configurar Input Actions (fallback se não definidas no Inspector)
            SetupInputActions();
            
            // Configurar preview visual
            SetupPreview();
            
            // Configurar layer mask para terrain
            SetupTerrainLayerMask();
        }
        
        void SetupInputActions() {
            // Click primário
            if (clickAction == null || clickAction.action == null) {
                _internalClickAction = new InputAction("Click", InputActionType.Button, "<Mouse>/leftButton");
                Debug.Log("[VoxelInteractionController] ✓ Click action criada internamente");
            }
            
            // Click secundário
            if (secondaryClickAction == null || secondaryClickAction.action == null) {
                _internalSecondaryClickAction = new InputAction("SecondaryClick", InputActionType.Button, "<Mouse>/rightButton");
                Debug.Log("[VoxelInteractionController] ✓ SecondaryClick action criada internamente");
            }
            
            // Posição do mouse
            if (mousePositionAction == null || mousePositionAction.action == null) {
                _internalMousePositionAction = new InputAction("MousePosition", InputActionType.Value, "<Mouse>/position");
                Debug.Log("[VoxelInteractionController] ✓ MousePosition action criada internamente");
            }
        }
        
        void SetupPreview() {
            if (previewPrefab != null) {
                _previewInstance = Instantiate(previewPrefab);
                _previewInstance.name = "VoxelPreview";
                _previewInstance.SetActive(false);
                
                _previewRenderer = _previewInstance.GetComponent<Renderer>();
                if (_previewRenderer != null) {
                    _previewPropertyBlock = new MaterialPropertyBlock();
                }
            }
        }
        
        void SetupTerrainLayerMask() {
            // Se a layer mask não foi configurada, tenta encontrar a layer "Terrain"
            if (terrainLayerMask == -1) {
                int terrainLayer = LayerMask.NameToLayer("Terrain");
                if (terrainLayer != -1) {
                    terrainLayerMask = 1 << terrainLayer;
                    Debug.Log($"[VoxelInteractionController] ✓ Layer 'Terrain' configurada automaticamente");
                } else {
                    Debug.LogWarning("[VoxelInteractionController] Layer 'Terrain' não encontrada. Crie-a em Edit > Project Settings > Tags and Layers");
                }
            }
        }
        
        void OnEnable() {
            // Habilitar Input Actions
            GetClickAction()?.Enable();
            GetSecondaryClickAction()?.Enable();
            GetMousePositionAction()?.Enable();
            
            Debug.Log("[VoxelInteractionController] ✓ Input Actions habilitadas");
        }
        
        void OnDisable() {
            // Desabilitar Input Actions
            GetClickAction()?.Disable();
            GetSecondaryClickAction()?.Disable();
            GetMousePositionAction()?.Disable();
            
            // Esconder preview
            if (_previewInstance != null) {
                _previewInstance.SetActive(false);
            }
        }
        
        void OnDestroy() {
            // Limpar ações criadas internamente
            _internalClickAction?.Dispose();
            _internalSecondaryClickAction?.Dispose();
            _internalMousePositionAction?.Dispose();
            
            // Destruir preview
            if (_previewInstance != null) {
                Destroy(_previewInstance);
            }
        }
        
        void Update() {
            if (terrainWorld == null || mainCamera == null) return;
            
            // Atualizar estado do clique secundário
            var secondaryAction = GetSecondaryClickAction();
            _isSecondaryHeld = secondaryAction != null && secondaryAction.IsPressed();
            
            // Executar raycast continuamente para preview
            UpdateRaycast();
            
            // Processar inputs
            HandlePrimaryClick();
            HandleSecondaryClick();
            
            // Atualizar preview visual
            UpdatePreview();
        }
        
        /// <summary>
        /// Atualiza o raycast para obter o voxel sob o cursor.
        /// </summary>
        void UpdateRaycast() {
            if (useVoxelRaycast) {
                // Método 1: Raycast DDA direto no grid de voxels (mais preciso)
                var mouseAction = GetMousePositionAction();
                if (mouseAction != null && mouseAction.enabled) {
                    _lastHit = VoxelRaycast.RaycastFromInputAction(
                        mainCamera, 
                        mouseAction, 
                        terrainWorld, 
                        maxRaycastDistance
                    );
                } else {
                    // Fallback para Mouse.current
                    _lastHit = VoxelRaycast.RaycastFromMouse(mainCamera, terrainWorld, maxRaycastDistance);
                }
            } else {
                // Método 2: Physics.Raycast (requer colliders e layer configurada)
                _lastHit = RaycastWithPhysics();
            }
        }
        
        /// <summary>
        /// Executa raycast usando o sistema de física do Unity.
        /// Útil quando você tem colliders nos chunks.
        /// </summary>
        VoxelRaycast.VoxelHitInfo RaycastWithPhysics() {
            var result = new VoxelRaycast.VoxelHitInfo { Hit = false };
            
            Vector2 mousePos = GetMousePosition();
            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            
            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance, terrainLayerMask)) {
                result.Hit = true;
                result.WorldHitPoint = hit.point;
                result.Normal = hit.normal;
                result.Distance = hit.distance;
                
                // Converte hit de física para posição de voxel
                // IMPORTANTE: hit.normal é usado para garantir que pegamos o voxel correto
                result.VoxelPosition = VoxelRaycast.GetVoxelFromPhysicsHit(hit, terrainWorld.escalaVoxel);
                
                // Obtém o tipo de bloco
                result.BlockType = (BlockType)terrainWorld.GetVoxelAt(
                    result.VoxelPosition.x, 
                    result.VoxelPosition.y, 
                    result.VoxelPosition.z
                );
                
                // Calcula posição do chunk
                result.ChunkPosition = new Vector2Int(
                    Mathf.FloorToInt((float)result.VoxelPosition.x / ChunkData.Largura),
                    Mathf.FloorToInt((float)result.VoxelPosition.z / ChunkData.Largura)
                );
            }
            
            return result;
        }
        
        /// <summary>
        /// Processa clique primário (destruir voxel).
        /// </summary>
        void HandlePrimaryClick() {
            var clickActionRef = GetClickAction();
            if (clickActionRef == null) return;
            
            if (clickActionRef.WasPressedThisFrame()) {
                if (_lastHit.Hit) {
                    DestroyVoxel(_lastHit.VoxelPosition);
                }
            }
        }
        
        /// <summary>
        /// Processa clique secundário (colocar voxel).
        /// </summary>
        void HandleSecondaryClick() {
            var secondaryAction = GetSecondaryClickAction();
            if (secondaryAction == null) return;
            
            if (secondaryAction.WasPressedThisFrame()) {
                if (_lastHit.Hit) {
                    // Coloca o bloco na posição adjacente (usando a normal)
                    Vector3Int adjacentPos = _lastHit.GetAdjacentVoxel();
                    PlaceVoxel(adjacentPos, blockToPlace);
                }
            }
        }
        
        /// <summary>
        /// Destrói (remove) um voxel na posição especificada.
        /// </summary>
        public void DestroyVoxel(Vector3Int position) {
            if (terrainWorld == null) return;
            
            // Log detalhado para debug
            Vector3 worldPos = new Vector3(
                position.x * terrainWorld.escalaVoxel,
                position.y * terrainWorld.escalaVoxel,
                position.z * terrainWorld.escalaVoxel
            );
            Debug.Log($"[VoxelInteractionController] Voxel destruído em {position} " +
                      $"(Mundo: {worldPos}, Escala: {terrainWorld.escalaVoxel}m, " +
                      $"Chunk: ({position.x / ChunkData.Largura}, {position.z / ChunkData.Largura}))");
            
            // Define o voxel como ar (vazio)
            terrainWorld.SetVoxelAt(position.x, position.y, position.z, (byte)BlockType.Ar);
            
            // Emitir evento (opcional, para sistemas de áudio, partículas, etc.)
            OnVoxelDestroyed?.Invoke(position, _lastHit.BlockType);
        }
        
        /// <summary>
        /// Coloca um novo voxel na posição especificada.
        /// </summary>
        public void PlaceVoxel(Vector3Int position, BlockType blockType) {
            if (terrainWorld == null) return;
            
            // Verifica se a posição está vazia
            byte existingVoxel = terrainWorld.GetVoxelAt(position.x, position.y, position.z);
            if (existingVoxel != (byte)BlockType.Ar) {
                Debug.LogWarning($"[VoxelInteractionController] Posição {position} já está ocupada!");
                return;
            }
            
            // Define o novo voxel
            terrainWorld.SetVoxelAt(position.x, position.y, position.z, (byte)blockType);
            
            Debug.Log($"[VoxelInteractionController] Voxel {blockType} colocado em {position}");
            
            // Emitir evento
            OnVoxelPlaced?.Invoke(position, blockType);
        }
        
        /// <summary>
        /// Atualiza o preview visual do voxel alvo.
        /// </summary>
        void UpdatePreview() {
            if (_previewInstance == null) return;
            
            if (_lastHit.Hit) {
                _previewInstance.SetActive(true);
                
                // Posição do preview
                Vector3Int targetPos;
                Color previewColor;
                
                if (_isSecondaryHeld) {
                    // Preview de colocação (posição adjacente)
                    targetPos = _lastHit.GetAdjacentVoxel();
                    previewColor = placePreviewColor;
                } else {
                    // Preview de destruição
                    targetPos = _lastHit.VoxelPosition;
                    previewColor = destroyPreviewColor;
                }
                
                // Converte para coordenadas de mundo
                Vector3 worldPos = VoxelRaycast.VoxelToWorld(targetPos, terrainWorld.escalaVoxel);
                // Centraliza no voxel
                worldPos += Vector3.one * (terrainWorld.escalaVoxel * 0.5f);
                
                _previewInstance.transform.position = worldPos;
                _previewInstance.transform.localScale = Vector3.one * (terrainWorld.escalaVoxel * 1.01f);
                
                // Atualiza cor
                if (_previewRenderer != null && _previewPropertyBlock != null) {
                    _previewPropertyBlock.SetColor("_Color", previewColor);
                    _previewPropertyBlock.SetColor("_BaseColor", previewColor); // URP
                    _previewRenderer.SetPropertyBlock(_previewPropertyBlock);
                }
            } else {
                _previewInstance.SetActive(false);
            }
        }
        
        #region Helpers
        
        /// <summary>
        /// Obtém a posição do mouse usando o novo Input System.
        /// </summary>
        Vector2 GetMousePosition() {
            var mouseAction = GetMousePositionAction();
            if (mouseAction != null && mouseAction.enabled) {
                return mouseAction.ReadValue<Vector2>();
            }
            
            // Fallback para Mouse.current
            if (Mouse.current != null) {
                return Mouse.current.position.ReadValue();
            }
            
            return Vector2.zero;
        }
        
        InputAction GetClickAction() {
            return clickAction?.action ?? _internalClickAction;
        }
        
        InputAction GetSecondaryClickAction() {
            return secondaryClickAction?.action ?? _internalSecondaryClickAction;
        }
        
        InputAction GetMousePositionAction() {
            return mousePositionAction?.action ?? _internalMousePositionAction;
        }
        
        #endregion
        
        #region Eventos
        
        /// <summary>
        /// Evento disparado quando um voxel é destruído.
        /// </summary>
        public event System.Action<Vector3Int, BlockType> OnVoxelDestroyed;
        
        /// <summary>
        /// Evento disparado quando um voxel é colocado.
        /// </summary>
        public event System.Action<Vector3Int, BlockType> OnVoxelPlaced;
        
        #endregion
        
        #region API Pública
        
        /// <summary>
        /// Obtém informações do último hit do raycast.
        /// </summary>
        public VoxelRaycast.VoxelHitInfo GetLastHit() => _lastHit;
        
        /// <summary>
        /// Define o tipo de bloco a ser colocado.
        /// </summary>
        public void SetBlockToPlace(BlockType blockType) {
            blockToPlace = blockType;
        }
        
        /// <summary>
        /// Alterna entre raycast de voxel e Physics.Raycast.
        /// </summary>
        public void SetUseVoxelRaycast(bool useVoxel) {
            useVoxelRaycast = useVoxel;
        }
        
        #endregion
    }
}

