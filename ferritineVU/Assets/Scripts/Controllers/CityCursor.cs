using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Voxel;

namespace Controllers {
    /// <summary>
    /// Sistema de cursor estilo Cities: Skylines.
    /// 
    /// Funcionalidades:
    /// - Luz suave (spot light) que segue o terreno sob o mouse
    /// - Highlight em GameObjects selecionáveis (prédios, veículos, agentes)
    /// - Modo FPS: crosshair no centro da tela com raycast de 1 metro
    /// 
    /// Este NÃO é um sistema de mineração/colocação de blocos.
    /// </summary>
    public class CityCursor : MonoBehaviour {
        
        [Header("Referências")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private TerrainWorld terrainWorld;
        [SerializeField] private CameraController cameraController;
        
        [Header("Luz do Cursor")]
        [Tooltip("Intensidade base da luz")]
        [SerializeField] private float lightIntensity = 3f;
        
        [Tooltip("Alcance da luz")]
        [SerializeField] private float lightRange = 8f;
        
        [Tooltip("Ângulo do cone de luz")]
        [SerializeField] private float lightSpotAngle = 45f;
        
        [Tooltip("Cor da luz normal")]
        [SerializeField] private Color normalColor = new Color(1f, 0.95f, 0.8f);
        
        [Tooltip("Cor quando sobre objeto selecionável")]
        [SerializeField] private Color hoverColor = new Color(1f, 0.8f, 0.3f);
        
        [Header("Raycast")]
        [Tooltip("Distância máxima do raycast (modo normal)")]
        [SerializeField] private float maxRaycastDistance = 500f;
        
        [Header("Modo Primeira Pessoa")]
        [Tooltip("Distância máxima de interação em FPS (1 metro ≈ 1 jarda)")]
        [SerializeField] private float fpsInteractionDistance = 1f;
        
        [Tooltip("Cor do crosshair")]
        [SerializeField] private Color crosshairColor = Color.white;
        
        [Tooltip("Tamanho do crosshair")]
        [SerializeField] private float crosshairSize = 20f;
        
        [Tooltip("Espessura das linhas do crosshair")]
        [SerializeField] private float crosshairThickness = 2f;
        
        [Tooltip("Espaço no centro do crosshair")]
        [SerializeField] private float crosshairGap = 4f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugLogs;
        
        // Componentes
        private Light _cursorLight;
        private GameObject _lightObject;
        
        // Crosshair UI
        private Canvas _crosshairCanvas;
        private RectTransform _crosshairContainer;
        private Image[] _crosshairLines;
        
        // Estado
        private Vector3 _hitPoint;
        private bool _hasHit;
        private GameObject _currentHoveredObject;
        private SelectableEntity _currentHoveredEntity;
        private bool _isFirstPersonMode;
        
        void Start() {
            // Encontrar câmera
            if (mainCamera == null) {
                mainCamera = Camera.main;
                if (mainCamera == null) {
                    Debug.LogError("[CityCursor] Câmera não encontrada!");
                    enabled = false;
                    return;
                }
            }
            
            // Encontrar CameraController
            if (cameraController == null) {
                cameraController = mainCamera.GetComponent<CameraController>();
            }
            
            // Encontrar TerrainWorld (opcional)
            if (terrainWorld == null) {
                terrainWorld = FindFirstObjectByType<TerrainWorld>();
            }
            
            // Criar luz
            CreateCursorLight();
            
            // Criar crosshair para modo FPS
            CreateCrosshair();
            
            Debug.Log("[CityCursor] ✓ Inicializado com sucesso");
        }
        
        void CreateCrosshair() {
            // Criar Canvas para o crosshair
            GameObject canvasObj = new GameObject("CrosshairCanvas");
            _crosshairCanvas = canvasObj.AddComponent<Canvas>();
            _crosshairCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _crosshairCanvas.sortingOrder = 100;
            
            // Adicionar CanvasScaler para responsividade
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Container do crosshair
            GameObject containerObj = new GameObject("CrosshairContainer");
            containerObj.transform.SetParent(canvasObj.transform, false);
            _crosshairContainer = containerObj.AddComponent<RectTransform>();
            _crosshairContainer.anchoredPosition = Vector2.zero;
            
            // Criar as 4 linhas do crosshair
            _crosshairLines = new Image[4];
            
            // Linha superior
            _crosshairLines[0] = CreateCrosshairLine("Top", 
                new Vector2(0, crosshairGap + crosshairSize / 2), 
                new Vector2(crosshairThickness, crosshairSize));
            
            // Linha inferior
            _crosshairLines[1] = CreateCrosshairLine("Bottom", 
                new Vector2(0, -(crosshairGap + crosshairSize / 2)), 
                new Vector2(crosshairThickness, crosshairSize));
            
            // Linha esquerda
            _crosshairLines[2] = CreateCrosshairLine("Left", 
                new Vector2(-(crosshairGap + crosshairSize / 2), 0), 
                new Vector2(crosshairSize, crosshairThickness));
            
            // Linha direita
            _crosshairLines[3] = CreateCrosshairLine("Right", 
                new Vector2(crosshairGap + crosshairSize / 2, 0), 
                new Vector2(crosshairSize, crosshairThickness));
            
            // Inicialmente escondido
            _crosshairCanvas.gameObject.SetActive(false);
            
            Debug.Log("[CityCursor] ✓ Crosshair criado");
        }
        
        Image CreateCrosshairLine(string lineName, Vector2 position, Vector2 size) {
            GameObject lineObj = new GameObject(lineName);
            lineObj.transform.SetParent(_crosshairContainer, false);
            
            RectTransform rect = lineObj.AddComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            
            Image img = lineObj.AddComponent<Image>();
            img.color = crosshairColor;
            
            return img;
        }
        
        void CreateCursorLight() {
            _lightObject = new GameObject("CursorLight");
            // NÃO fazer filho da câmera - a luz precisa de posição mundial
            // _lightObject.transform.SetParent(transform);
            
            _cursorLight = _lightObject.AddComponent<Light>();
            _cursorLight.type = LightType.Spot;
            _cursorLight.intensity = lightIntensity;
            _cursorLight.range = lightRange;
            _cursorLight.spotAngle = lightSpotAngle;
            _cursorLight.color = normalColor;
            _cursorLight.shadows = LightShadows.None; // Mais leve
            
            // Rotação apontando para baixo
            _lightObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            
            Debug.Log("[CityCursor] ✓ Luz criada (posição mundial)");
        }
        
        void Update() {
            if (mainCamera == null || _cursorLight == null) return;
            
            // Verificar se está em modo FPS
            bool wasFirstPerson = _isFirstPersonMode;
            _isFirstPersonMode = cameraController != null && cameraController.IsFirstPerson;
            
            // Mudou de modo?
            if (wasFirstPerson != _isFirstPersonMode) {
                OnModeChanged();
            }
            
            if (_isFirstPersonMode) {
                // Modo FPS: raycast do centro da tela com distância limitada
                PerformFPSRaycast();
            } else {
                // Modo normal: raycast do mouse
                PerformRaycast();
                UpdateLight();
            }
        }
        
        void OnModeChanged() {
            if (_isFirstPersonMode) {
                // Entrando em FPS: esconder luz, mostrar crosshair
                _cursorLight.enabled = false;
                if (_crosshairCanvas != null) {
                    _crosshairCanvas.gameObject.SetActive(true);
                }
                Debug.Log("[CityCursor] Modo FPS ativado - crosshair visível");
            } else {
                // Saindo de FPS: esconder crosshair
                if (_crosshairCanvas != null) {
                    _crosshairCanvas.gameObject.SetActive(false);
                }
                Debug.Log("[CityCursor] Modo normal ativado - luz do cursor");
            }
            
            // Limpar hover ao trocar de modo
            ClearHover();
        }
        
        void ClearHover() {
            if (_currentHoveredEntity != null) {
                _currentHoveredEntity.Unhighlight();
                _currentHoveredEntity = null;
            }
            _currentHoveredObject = null;
            _hasHit = false;
        }
        
        void PerformFPSRaycast() {
            // Ray do centro da tela (onde está a mira)
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            
            // Raycast com distância limitada a 1 metro
            int layerMask = ~(1 << 2); // Ignora Ignore Raycast
            
            if (Physics.Raycast(ray, out RaycastHit hit, fpsInteractionDistance, layerMask)) {
                _hasHit = true;
                _hitPoint = hit.point;
                
                GameObject hitObj = hit.collider.gameObject;
                
                // Mudou de objeto?
                if (hitObj != _currentHoveredObject) {
                    // Remove highlight anterior
                    if (_currentHoveredEntity != null) {
                        _currentHoveredEntity.Unhighlight();
                    }
                    
                    _currentHoveredObject = hitObj;
                    _currentHoveredEntity = hitObj.GetComponent<SelectableEntity>();
                    
                    if (_currentHoveredEntity == null) {
                        _currentHoveredEntity = hitObj.GetComponentInParent<SelectableEntity>();
                    }
                    
                    // Highlight no novo (se existir)
                    if (_currentHoveredEntity != null) {
                        _currentHoveredEntity.Highlight();
                        // Mudar cor do crosshair para indicar objeto interativo
                        SetCrosshairColor(hoverColor);
                        
                        if (showDebugLogs) {
                            Debug.Log($"[CityCursor FPS] Hover: {_currentHoveredEntity.gameObject.name}");
                        }
                    } else {
                        SetCrosshairColor(crosshairColor);
                    }
                }
                
                if (showDebugLogs && Time.frameCount % 60 == 0) {
                    Debug.Log($"[CityCursor FPS] Hit: {hit.collider.name} at {hit.distance:F2}m");
                }
                
            } else {
                if (_hasHit || _currentHoveredEntity != null) {
                    ClearHover();
                    SetCrosshairColor(crosshairColor);
                }
                _hasHit = false;
            }
        }
        
        void SetCrosshairColor(Color color) {
            if (_crosshairLines == null) return;
            foreach (var line in _crosshairLines) {
                if (line != null) {
                    line.color = color;
                }
            }
        }
        
        void PerformRaycast() {
            Vector2 mouseScreenPos;
            if (Mouse.current != null) {
                mouseScreenPos = Mouse.current.position.ReadValue();
            } else {
                mouseScreenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPos);
            
            // Raycast em tudo (ignora apenas Ignore Raycast)
            int layerMask = ~(1 << 2); // Ignora layer 2 (Ignore Raycast)
            
            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance, layerMask)) {
                _hasHit = true;
                _hitPoint = hit.point;
                
                // Verificar se é um objeto selecionável
                GameObject hitObj = hit.collider.gameObject;
                
                // Mudou de objeto?
                if (hitObj != _currentHoveredObject) {
                    // Remove highlight do anterior
                    if (_currentHoveredEntity != null) {
                        _currentHoveredEntity.Unhighlight();
                    }
                    
                    _currentHoveredObject = hitObj;
                    _currentHoveredEntity = hitObj.GetComponent<SelectableEntity>();
                    
                    // Se não tem SelectableEntity, procura nos pais
                    if (_currentHoveredEntity == null) {
                        _currentHoveredEntity = hitObj.GetComponentInParent<SelectableEntity>();
                    }
                    
                    // Aplica highlight no novo
                    if (_currentHoveredEntity != null) {
                        _currentHoveredEntity.Highlight();
                        
                        if (showDebugLogs) {
                            Debug.Log($"[CityCursor] Hover: {_currentHoveredEntity.gameObject.name}");
                        }
                    }
                }
                
                if (showDebugLogs && Time.frameCount % 120 == 0) {
                    Debug.Log($"[CityCursor] Hit: {hit.collider.name} at {hit.point:F1}");
                }
                
            } else {
                _hasHit = false;
                
                // Remove highlight
                if (_currentHoveredEntity != null) {
                    _currentHoveredEntity.Unhighlight();
                    _currentHoveredEntity = null;
                }
                _currentHoveredObject = null;
            }
        }
        
        void UpdateLight() {
            if (_hasHit) {
                _cursorLight.enabled = true;
                
                // Posiciona a luz acima do ponto de hit (1 metro acima)
                float heightAbove = 1f;
                Vector3 lightPosition = new Vector3(_hitPoint.x, _hitPoint.y + heightAbove, _hitPoint.z);
                _lightObject.transform.position = lightPosition;
                
                // Debug para verificar posições
                if (showDebugLogs && Time.frameCount % 120 == 0) {
                    Debug.Log($"[CityCursor] Light pos: {lightPosition:F1}, Hit: {_hitPoint:F1}");
                }
                
                // Cor baseada se está sobre selecionável
                Color targetColor = (_currentHoveredEntity != null) ? hoverColor : normalColor;
                _cursorLight.color = Color.Lerp(_cursorLight.color, targetColor, Time.deltaTime * 8f);
                
            } else {
                _cursorLight.enabled = false;
            }
        }
        
        void OnDisable() {
            // Limpar estado
            if (_currentHoveredEntity != null) {
                _currentHoveredEntity.Unhighlight();
                _currentHoveredEntity = null;
            }
            _currentHoveredObject = null;
            
            if (_cursorLight != null) {
                _cursorLight.enabled = false;
            }
        }
        
        void OnDestroy() {
            if (_lightObject != null) {
                Destroy(_lightObject);
            }
            if (_crosshairCanvas != null) {
                Destroy(_crosshairCanvas.gameObject);
            }
        }
        
        #region API Pública
        
        /// <summary>
        /// Posição atual do cursor no mundo.
        /// </summary>
        public Vector3 HitPoint => _hitPoint;
        
        /// <summary>
        /// Se o raycast atingiu algo.
        /// </summary>
        public bool HasHit => _hasHit;
        
        /// <summary>
        /// Entidade atualmente sob o cursor (ou null).
        /// </summary>
        public SelectableEntity HoveredEntity => _currentHoveredEntity;
        
        /// <summary>
        /// Se está em modo primeira pessoa.
        /// </summary>
        public bool IsFirstPersonMode => _isFirstPersonMode;
        
        /// <summary>
        /// Distância máxima de interação no modo FPS.
        /// </summary>
        public float FPSInteractionDistance => fpsInteractionDistance;
        
        #endregion
    }
}
