using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Voxel {
    /// <summary>
    /// Monitor de memória para o sistema de voxels.
    /// Exibe estatísticas em tempo real no canto da tela.
    /// </summary>
    public class VoxelMemoryMonitor : MonoBehaviour {
        [Header("Referências")]
        public VoxelWorld voxelWorld;
        public TextMeshProUGUI statsText;
        
        [Header("Configuração")]
        [Tooltip("Intervalo de atualização em segundos")]
        public float updateInterval = 1f;
        
        [Tooltip("Mostrar/ocultar com tecla F3")]
        public KeyCode toggleKey = KeyCode.F3;
        
        private float _lastUpdateTime;
        private bool _isVisible = true;
        private CanvasGroup _canvasGroup;
        
        void Start() {
            // Auto-encontrar VoxelWorld se não atribuído
            if (voxelWorld == null) {
                voxelWorld = FindFirstObjectByType<VoxelWorld>();
            }
            
            // Criar UI se não existir
            if (statsText == null) {
                CreateUI();
            }
            
            // Obter ou adicionar CanvasGroup para fade
            _canvasGroup = GetComponent<CanvasGroup>();
            if (_canvasGroup == null) {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            UpdateVisibility();
        }
        
        void Update() {
            // Toggle visibilidade
            if (Input.GetKeyDown(toggleKey)) {
                _isVisible = !_isVisible;
                UpdateVisibility();
            }
            
            // Atualizar estatísticas
            if (_isVisible && voxelWorld != null && Time.time - _lastUpdateTime >= updateInterval) {
                _lastUpdateTime = Time.time;
                UpdateStats();
            }
        }
        
        void UpdateVisibility() {
            if (_canvasGroup != null) {
                _canvasGroup.alpha = _isVisible ? 1f : 0f;
                _canvasGroup.interactable = _isVisible;
                _canvasGroup.blocksRaycasts = _isVisible;
            }
        }
        
        void UpdateStats() {
            if (statsText == null) return;
            
            string stats = voxelWorld.GetMemoryStats();
            statsText.text = stats + $"\n\n[F3] para ocultar";
        }
        
        void CreateUI() {
            // Criar Canvas se não existir
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null) {
                GameObject canvasObj = new GameObject("VoxelMemoryMonitor_Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvasObj.AddComponent<CanvasScaler>();
                canvasObj.AddComponent<GraphicRaycaster>();
                transform.SetParent(canvasObj.transform);
            }
            
            // Criar painel de fundo
            GameObject panel = new GameObject("StatsPanel");
            panel.transform.SetParent(transform);
            
            RectTransform panelRect = panel.AddComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0, 1);
            panelRect.anchorMax = new Vector2(0, 1);
            panelRect.pivot = new Vector2(0, 1);
            panelRect.anchoredPosition = new Vector2(10, -10);
            panelRect.sizeDelta = new Vector2(300, 200);
            
            Image panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.7f);
            
            // Criar texto
            GameObject textObj = new GameObject("StatsText");
            textObj.transform.SetParent(panel.transform);
            
            RectTransform textRect = textObj.AddComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.offsetMin = new Vector2(10, 10);
            textRect.offsetMax = new Vector2(-10, -10);
            
            statsText = textObj.AddComponent<TextMeshProUGUI>();
            statsText.fontSize = 14;
            statsText.color = Color.white;
            statsText.alignment = TextAlignmentOptions.TopLeft;
            statsText.text = "Inicializando...";
            
            Debug.Log("[VoxelMemoryMonitor] UI criada automaticamente.");
        }
    }
}

