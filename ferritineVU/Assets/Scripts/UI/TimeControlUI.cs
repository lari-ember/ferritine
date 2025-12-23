using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI
{

/// <summary>
/// Painel de controle de tempo - permite play/pause, ajustar velocidade (1x/2x/3x)
/// e exibe o tempo atual em formato HH:MM.
/// 
/// Estrutura esperada na hierarquia:
/// - TimeControlPanel (Canvas)
///   - ClockDisplay (TextMeshProUGUI) - mostra HH:MM
///   - PlayPauseButton (Button)
///   - SpeedButtons (LayoutGroup)
///     - Speed1x (Button)
///     - Speed2x (Button)
///     - Speed3x (Button)
/// </summary>
public class TimeControlUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI clockDisplay;
    [SerializeField] private Button playPauseButton;
    [SerializeField] private TextMeshProUGUI playPauseText;
    
    [Header("Speed Buttons")]
    [SerializeField] private Button speed1XButton;
    [SerializeField] private Button speed2XButton;
    [SerializeField] private Button speed3XButton;
    
    [Header("Visual Feedback")]
    [SerializeField] private Color selectedButtonColor = Color.green;
    [SerializeField] private Color deselectedButtonColor = Color.white;
    [SerializeField] private Image speedButtonsContainer; // Para highlight ao selecionar velocidade
    
    private Systems.TimeManager _timeManager;
    private int _currentSpeedIndex;
    
    void Awake()
    {
        // Conectar botões se estiverem atribuídos
        if (playPauseButton != null)
            playPauseButton.onClick.AddListener(OnPlayPauseClicked);
        
        if (speed1XButton != null)
            speed1XButton.onClick.AddListener(() => OnSpeedSelected(0));
        
        if (speed2XButton != null)
            speed2XButton.onClick.AddListener(() => OnSpeedSelected(1));
        
        if (speed3XButton != null)
            speed3XButton.onClick.AddListener(() => OnSpeedSelected(2));
    }
    
    void Start()
    {
        _timeManager = Systems.TimeManager.Instance;
        
        if (_timeManager == null)
        {
            Debug.LogError("[TimeControlUI] TimeManager não encontrado!");
            return;
        }
        
        // Inscrever nos eventos de tempo
        _timeManager.OnTimeChanged += OnTimeChanged;
        _timeManager.OnPauseChanged += OnPauseChanged;
        
        // Atualizar UI inicial
        UpdateClockDisplay();
        UpdatePlayPauseButton();
        UpdateSpeedButtonVisuals();
        
        Debug.Log("[TimeControlUI] Inicializado com sucesso");
    }
    
    /// <summary>
    /// Chamado quando o tempo muda - atualiza o relógio.
    /// </summary>
    void OnTimeChanged(float timeOfDay)
    {
        UpdateClockDisplay();
    }
    
    /// <summary>
    /// Chamado quando pause/play muda.
    /// </summary>
    void OnPauseChanged()
    {
        UpdatePlayPauseButton();
    }
    
    /// <summary>
    /// Atualiza a exibição do relógio (HH:MM).
    /// </summary>
    void UpdateClockDisplay()
    {
        if (clockDisplay == null) return;
        
        int hour = _timeManager.CurrentHour;
        int minute = _timeManager.CurrentMinute;
        
        clockDisplay.text = $"{hour:D2}:{minute:D2}";
    }
    
    /// <summary>
    /// Atualiza o botão de play/pause baseado no estado.
    /// </summary>
    void UpdatePlayPauseButton()
    {
        if (playPauseText == null) return;
        
        if (_timeManager.IsPaused)
        {
            playPauseText.text = "▶ Play"; // Triângulo para play
            if (playPauseButton != null)
            {
                ColorBlock colors = playPauseButton.colors;
                colors.normalColor = new Color(0.7f, 1f, 0.7f); // Verde claro
                playPauseButton.colors = colors;
            }
        }
        else
        {
            playPauseText.text = "⏸ Pause"; // Dois retângulos para pause
            if (playPauseButton != null)
            {
                ColorBlock colors = playPauseButton.colors;
                colors.normalColor = new Color(1f, 0.8f, 0.7f); // Laranja claro
                playPauseButton.colors = colors;
            }
        }
    }
    
    /// <summary>
    /// Atualiza os visuais dos botões de velocidade.
    /// Destaca o botão de velocidade atual.
    /// </summary>
    void UpdateSpeedButtonVisuals()
    {
        // Resetar todas as cores
        ResetButtonColor(speed1XButton);
        ResetButtonColor(speed2XButton);
        ResetButtonColor(speed3XButton);
        
        // Destacar o botão ativo
        switch (_currentSpeedIndex)
        {
            case 0:
                HighlightButton(speed1XButton);
                break;
            case 1:
                HighlightButton(speed2XButton);
                break;
            case 2:
                HighlightButton(speed3XButton);
                break;
        }
    }
    
    /// <summary>
    /// Destaca um botão (muda cor).
    /// </summary>
    void HighlightButton(Button button)
    {
        if (button == null) return;
        
        ColorBlock colors = button.colors;
        colors.normalColor = selectedButtonColor;
        button.colors = colors;
    }
    
    /// <summary>
    /// Reseta a cor de um botão para deselected.
    /// </summary>
    void ResetButtonColor(Button button)
    {
        if (button == null) return;
        
        ColorBlock colors = button.colors;
        colors.normalColor = deselectedButtonColor;
        button.colors = colors;
    }
    
    // ==================== BUTTON CALLBACKS ====================
    
    /// <summary>
    /// Chamado quando play/pause é clicado.
    /// </summary>
    void OnPlayPauseClicked()
    {
        _timeManager.TogglePause();
        AudioManager.PlayUISound("button_click");
    }
    
    /// <summary>
    /// Chamado quando um botão de velocidade é clicado.
    /// </summary>
    void OnSpeedSelected(int speedIndex)
    {
        _currentSpeedIndex = speedIndex;
        _timeManager.SetSpeedMultiplier(speedIndex);
        UpdateSpeedButtonVisuals();
        AudioManager.PlayUISound("button_click");
        
        Debug.Log($"[TimeControlUI] Velocidade alterada para {speedIndex + 1}x");
    }
    
    void OnDestroy()
    {
        if (_timeManager != null)
        {
            _timeManager.OnTimeChanged -= OnTimeChanged;
            _timeManager.OnPauseChanged -= OnPauseChanged;
        }
    }
}

}  // namespace UI

