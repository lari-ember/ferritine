using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace Voxel {
    /// <summary>
    /// ZoneBrushUI: Interface do usuário para o sistema de pintura de zonas.
    /// 
    /// Fase 2: "O Construtor" - Modificação em Tempo Real
    /// 
    /// Funcionalidades:
    /// - Seleção de zona com atalhos de teclado (1-9, 0)
    /// - Ajuste do tamanho do pincel com scroll
    /// - Exibição do painel de zonas disponíveis
    /// - Feedback visual da zona selecionada
    /// </summary>
    [AddComponentMenu("Voxel/Zone Brush UI")]
    public class ZoneBrushUI : MonoBehaviour {
        
        [Header("Referências")]
        [Tooltip("Referência ao ZoneBrush para controlar a pintura")]
        [SerializeField] private ZoneBrush zoneBrush;
        
        [Header("Configuração de Input")]
        [Tooltip("Input Action para scroll (ajuste de tamanho)")]
        [SerializeField] private InputActionReference scrollAction;
        
        [Tooltip("Sensibilidade do scroll para ajuste de tamanho")]
        [SerializeField] private float scrollSensitivity = 0.1f;
        
        [Header("UI Visual")]
        [Tooltip("Texto para mostrar a zona selecionada (TextMeshPro)")]
        [SerializeField] private TMPro.TextMeshProUGUI selectedZoneText;
        
        [Tooltip("Texto para mostrar o tamanho do pincel")]
        [SerializeField] private TMPro.TextMeshProUGUI brushSizeText;
        
        [Tooltip("Imagem para cor da zona selecionada")]
        [SerializeField] private UnityEngine.UI.Image zoneColorIndicator;
        
        [Header("On-Screen Help")]
        [Tooltip("Mostra painel de ajuda com atalhos")]
        [SerializeField] private bool showHelp = true;
        
        [Tooltip("Canvas para o painel de ajuda (será criado se não existir)")]
        [SerializeField] private RectTransform helpPanel;
        
        // Input interno
        private InputAction _internalScrollAction;
        private float _scrollAccumulator;
        
        // Zonas que podem ser selecionadas pelo teclado
        private readonly ZonaTipo[] _selectableZones = {
            ZonaTipo.ResidencialBaixaDensidade,
            ZonaTipo.ResidencialMediaDensidade,
            ZonaTipo.ResidencialAltaDensidade,
            ZonaTipo.ComercialLocal,
            ZonaTipo.ComercialCentral,
            ZonaTipo.IndustrialLeve,
            ZonaTipo.IndustrialPesada,
            ZonaTipo.Parque,
            ZonaTipo.Via,
            ZonaTipo.Nenhuma // 0 = Apagar
        };
        
        void Awake() {
            if (zoneBrush == null) {
                zoneBrush = FindFirstObjectByType<ZoneBrush>();
            }
            
            SetupInputActions();
        }
        
        void SetupInputActions() {
            if (scrollAction == null || scrollAction.action == null) {
                _internalScrollAction = new InputAction("Scroll", InputActionType.Value, "<Mouse>/scroll/y");
            }
        }
        
        void OnEnable() {
            GetScrollAction()?.Enable();
            ZoneBrush.OnZoneSelectionChanged += OnZoneSelectionChanged;
        }
        
        void OnDisable() {
            GetScrollAction()?.Disable();
            ZoneBrush.OnZoneSelectionChanged -= OnZoneSelectionChanged;
        }
        
        void OnDestroy() {
            _internalScrollAction?.Dispose();
        }
        
        void Update() {
            HandleKeyboardShortcuts();
            HandleScrollWheel();
            UpdateUI();
        }
        
        #region Keyboard Shortcuts
        
        void HandleKeyboardShortcuts() {
            // Teclas numéricas 1-9 para zonas
            for (int i = 0; i < 9; i++) {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                    SelectZoneByIndex(i);
                    return;
                }
            }
            
            // 0 = Apagar/Nenhuma
            if (Input.GetKeyDown(KeyCode.Alpha0)) {
                SelectZoneByIndex(9); // Nenhuma está na posição 9
                return;
            }
            
            // Teclas adicionais
            if (Input.GetKeyDown(KeyCode.Q)) {
                // Modo de apagar rápido
                zoneBrush?.SetZona(ZonaTipo.Nenhuma);
            }
            
            if (Input.GetKeyDown(KeyCode.Tab)) {
                // Cicla para próxima zona
                CycleToNextZone();
            }
            
            // Ctrl+Tab = zona anterior
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Tab)) {
                CycleToPreviousZone();
            }
            
            // [ e ] para ajustar tamanho do pincel
            if (Input.GetKeyDown(KeyCode.LeftBracket)) {
                AdjustBrushSize(-1);
            }
            if (Input.GetKeyDown(KeyCode.RightBracket)) {
                AdjustBrushSize(1);
            }
            
            // H = Toggle help
            if (Input.GetKeyDown(KeyCode.H)) {
                showHelp = !showHelp;
            }
        }
        
        void SelectZoneByIndex(int index) {
            if (zoneBrush == null) return;
            if (index < 0 || index >= _selectableZones.Length) return;
            
            zoneBrush.SetZona(_selectableZones[index]);
        }
        
        void CycleToNextZone() {
            if (zoneBrush == null) return;
            
            ZonaTipo current = zoneBrush.GetZonaSelecionada();
            int currentIndex = Array.IndexOf(_selectableZones, current);
            int nextIndex = (currentIndex + 1) % _selectableZones.Length;
            zoneBrush.SetZona(_selectableZones[nextIndex]);
        }
        
        void CycleToPreviousZone() {
            if (zoneBrush == null) return;
            
            ZonaTipo current = zoneBrush.GetZonaSelecionada();
            int currentIndex = Array.IndexOf(_selectableZones, current);
            int prevIndex = currentIndex - 1;
            if (prevIndex < 0) prevIndex = _selectableZones.Length - 1;
            zoneBrush.SetZona(_selectableZones[prevIndex]);
        }
        
        #endregion
        
        #region Scroll Wheel
        
        void HandleScrollWheel() {
            if (zoneBrush == null) return;
            
            var scrollAct = GetScrollAction();
            if (scrollAct == null || !scrollAct.enabled) return;
            
            float scroll = scrollAct.ReadValue<float>();
            
            // Requer Ctrl pressionado para ajustar tamanho
            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
                _scrollAccumulator += scroll * scrollSensitivity;
                
                if (Mathf.Abs(_scrollAccumulator) >= 1f) {
                    int delta = Mathf.FloorToInt(_scrollAccumulator);
                    AdjustBrushSize(delta);
                    _scrollAccumulator -= delta;
                }
            }
        }
        
        void AdjustBrushSize(int delta) {
            if (zoneBrush == null) return;
            
            int currentSize = zoneBrush.GetTamanhoPincel();
            zoneBrush.SetTamanhoPincel(currentSize + delta);
        }
        
        #endregion
        
        #region UI Updates
        
        void OnZoneSelectionChanged(ZonaTipo newZone) {
            UpdateUI();
        }
        
        void UpdateUI() {
            if (zoneBrush == null) return;
            
            ZonaTipo selectedZone = zoneBrush.GetZonaSelecionada();
            int brushSize = zoneBrush.GetTamanhoPincel();
            
            // Atualiza texto da zona
            if (selectedZoneText != null) {
                string icon = ZonaHelper.GetZoneIcon(selectedZone);
                string name = ZonaHelper.GetZoneName(selectedZone);
                selectedZoneText.text = $"{icon} {name}";
            }
            
            // Atualiza texto do tamanho
            if (brushSizeText != null) {
                brushSizeText.text = $"Pincel: {brushSize}x{brushSize}";
            }
            
            // Atualiza cor do indicador
            if (zoneColorIndicator != null) {
                zoneColorIndicator.color = ZonaHelper.GetZoneColor(selectedZone);
            }
        }
        
        #endregion
        
        #region On-Screen Help (OnGUI - Debug)
        
        void OnGUI() {
            if (!showHelp) return;
            
            // Estilo da caixa
            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.fontSize = 12;
            boxStyle.padding = new RectOffset(10, 10, 10, 10);
            
            // Estilo do texto
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontSize = 11;
            labelStyle.richText = true;
            
            // Posição do painel
            float panelWidth = 280;
            float panelHeight = 320;
            float margin = 10;
            Rect panelRect = new Rect(margin, Screen.height - panelHeight - margin, panelWidth, panelHeight);
            
            GUI.Box(panelRect, "🎨 Zone Brush - Atalhos", boxStyle);
            
            GUILayout.BeginArea(new Rect(panelRect.x + 10, panelRect.y + 25, panelRect.width - 20, panelRect.height - 35));
            
            // Zona atual
            if (zoneBrush != null) {
                ZonaTipo current = zoneBrush.GetZonaSelecionada();
                Color zoneColor = ZonaHelper.GetZoneColor(current);
                GUILayout.Label($"<b>Zona:</b> {ZonaHelper.GetZoneIcon(current)} {ZonaHelper.GetZoneName(current)}", labelStyle);
                GUILayout.Label($"<b>Pincel:</b> {zoneBrush.GetTamanhoPincel()}x{zoneBrush.GetTamanhoPincel()}", labelStyle);
                GUILayout.Space(5);
            }
            
            // Atalhos de zona
            GUILayout.Label("<b>Seleção de Zona:</b>", labelStyle);
            GUILayout.Label("  1 - Residencial Baixa", labelStyle);
            GUILayout.Label("  2 - Residencial Média", labelStyle);
            GUILayout.Label("  3 - Residencial Alta", labelStyle);
            GUILayout.Label("  4 - Comercial Local", labelStyle);
            GUILayout.Label("  5 - Comercial Central", labelStyle);
            GUILayout.Label("  6 - Industrial Leve", labelStyle);
            GUILayout.Label("  7 - Industrial Pesada", labelStyle);
            GUILayout.Label("  8 - Parque", labelStyle);
            GUILayout.Label("  9 - Via", labelStyle);
            GUILayout.Label("  0 / Q - Apagar", labelStyle);
            
            GUILayout.Space(5);
            GUILayout.Label("<b>Outros:</b>", labelStyle);
            GUILayout.Label("  [ ] - Ajustar tamanho pincel", labelStyle);
            GUILayout.Label("  Tab - Próxima zona", labelStyle);
            GUILayout.Label("  H - Toggle ajuda", labelStyle);
            GUILayout.Label("  LMB - Pintar", labelStyle);
            GUILayout.Label("  RMB - Apagar", labelStyle);
            
            GUILayout.EndArea();
        }
        
        #endregion
        
        #region Helpers
        
        InputAction GetScrollAction() => scrollAction?.action ?? _internalScrollAction;
        
        #endregion
    }
}

