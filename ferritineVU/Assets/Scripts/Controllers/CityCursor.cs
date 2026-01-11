using UnityEngine;
using UnityEngine.InputSystem;
using Voxel;

namespace Controllers {
    /// <summary>
    /// Sistema de cursor estilo Cities: Skylines.
    /// 
    /// Funcionalidades:
    /// - Luz suave (spot light) que segue o terreno sob o mouse
    /// - Highlight em GameObjects selecionáveis (prédios, veículos, agentes)
    /// 
    /// Este NÃO é um sistema de mineração/colocação de blocos.
    /// </summary>
    public class CityCursor : MonoBehaviour {
        
        [Header("Referências")]
        [SerializeField] private Camera mainCamera;
        [SerializeField] private TerrainWorld terrainWorld;
        
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
        [Tooltip("Distância máxima do raycast")]
        [SerializeField] private float maxRaycastDistance = 500f;
        
        [Header("Debug")]
        [SerializeField] private bool showDebugLogs;
        
        // Componentes
        private Light _cursorLight;
        private GameObject _lightObject;
        
        // Estado
        private Vector3 _hitPoint;
        private bool _hasHit;
        private GameObject _currentHoveredObject;
        private SelectableEntity _currentHoveredEntity;
        
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
            
            // Encontrar TerrainWorld (opcional)
            if (terrainWorld == null) {
                terrainWorld = FindFirstObjectByType<TerrainWorld>();
            }
            
            // Criar luz
            CreateCursorLight();
            
            Debug.Log("[CityCursor] ✓ Inicializado com sucesso");
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
            
            // Raycast simples - todas as layers exceto UI
            PerformRaycast();
            
            // Atualizar luz
            UpdateLight();
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
        
        #endregion
    }
}
