using UnityEngine;
using System;

namespace Systems
{

/// <summary>
/// Gerenciador centralizado de tempo simulado.
/// Controla o fluxo de tempo do dia/noite (ciclo de 24h em 12 minutos reais).
/// Fornece eventos para sincronizar animações, skybox e comportamentos de agentes.
/// 
/// Multiplicadores de velocidade:
/// - 1x: 24h = 12 minutos reais (normal)
/// - 2x: 24h = 6 minutos reais (rápido)
/// - 3x: 24h = 4 minutos reais (super rápido)
/// </summary>
public class TimeManager : MonoBehaviour
{
    [Header("Time Configuration")]
    [SerializeField] private float cycleLength = 720f; // 12 minutos em segundos (ciclo completo 24h)
    [SerializeField] private float startTimeOfDay = 6f; // Começar às 6h da manhã
    
    [Header("Speed Multipliers")]
    [SerializeField] private float[] speedMultipliers = { 1f, 2f, 3f };
    
    [Header("Events")]
    public Action<float> OnTimeChanged; // Chamado quando o tempo muda (0-24)
    public Action OnHourChanged; // Chamado quando a hora inteira muda
    public Action OnDayChanged; // Chamado quando o dia muda (24h -> 0h)
    public Action OnPauseChanged; // Chamado quando pause/play muda
    
    // Singleton
    private static TimeManager _instance;
    public static TimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<TimeManager>();
                if (_instance == null)
                {
                    Debug.LogError("[TimeManager] No TimeManager found in scene!");
                }
            }
            return _instance;
        }
    }
    
    // Estado privado
    private float _currentTimeOfDay; // 0-24 (horas)
    private bool _isPaused;
    private int _currentSpeedMultiplierIndex;
    
    // Propriedades públicas
    public float CurrentTimeOfDay => _currentTimeOfDay;
    public int CurrentHour => Mathf.FloorToInt(_currentTimeOfDay);
    public int CurrentMinute => Mathf.FloorToInt((_currentTimeOfDay % 1f) * 60f);
    public bool IsPaused => _isPaused;
    public float CurrentSpeedMultiplier => speedMultipliers[_currentSpeedMultiplierIndex];
    public string TimeString => $"{CurrentHour:D2}:{CurrentMinute:D2}";
    
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else if (_instance != this)
        {
            Debug.LogWarning("[TimeManager] Duplicate TimeManager found, destroying this one.");
            Destroy(gameObject);
            return;
        }
        
        _currentTimeOfDay = startTimeOfDay;
    }
    
    void Start()
    {
        Debug.Log($"[TimeManager] Iniciado às {TimeString} (hora {CurrentHour})");
        OnTimeChanged?.Invoke(_currentTimeOfDay);
    }
    
    void Update()
    {
        if (_isPaused) return;
        
        // Calcular delta time ajustado pela velocidade
        float adjustedDeltaTime = Time.deltaTime * CurrentSpeedMultiplier;
        
        // Quanto de tempo (em horas) passou
        float hoursElapsed = (adjustedDeltaTime / cycleLength) * 24f;
        
        int hourBefore = CurrentHour;
        
        _currentTimeOfDay += hoursElapsed;
        
        // Wrap around 24h
        if (_currentTimeOfDay >= 24f)
        {
            _currentTimeOfDay -= 24f;
            OnDayChanged?.Invoke();
            Debug.Log("[TimeManager] Novo dia iniciado!");
        }
        
        // Verificar mudança de hora
        if (CurrentHour != hourBefore)
        {
            OnHourChanged?.Invoke();
            Debug.Log($"[TimeManager] Hora mudou para {TimeString}");
        }
        
        // Invocar evento de tempo contínuo
        OnTimeChanged?.Invoke(_currentTimeOfDay);
    }
    
    /// <summary>
    /// Alterna pausa/play da simulação.
    /// </summary>
    public void TogglePause()
    {
        SetPaused(!_isPaused);
    }
    
    /// <summary>
    /// Define se a simulação está pausada.
    /// </summary>
    public void SetPaused(bool paused)
    {
        if (_isPaused == paused) return;
        
        _isPaused = paused;
        OnPauseChanged?.Invoke();
        Debug.Log($"[TimeManager] Simulação {(_isPaused ? "PAUSADA" : "RETOMADA")}");
    }
    
    /// <summary>
    /// Aumenta a velocidade de simulação (cicla entre 1x, 2x, 3x).
    /// </summary>
    public void IncreaseSpeed()
    {
        _currentSpeedMultiplierIndex = (_currentSpeedMultiplierIndex + 1) % speedMultipliers.Length;
        Debug.Log($"[TimeManager] Velocidade: {CurrentSpeedMultiplier}x");
    }
    
    /// <summary>
    /// Diminui a velocidade de simulação.
    /// </summary>
    public void DecreaseSpeed()
    {
        _currentSpeedMultiplierIndex--;
        if (_currentSpeedMultiplierIndex < 0)
            _currentSpeedMultiplierIndex = speedMultipliers.Length - 1;
        Debug.Log($"[TimeManager] Velocidade: {CurrentSpeedMultiplier}x");
    }
    
    /// <summary>
    /// Define um multiplicador de velocidade específico (0=1x, 1=2x, 2=3x).
    /// </summary>
    public void SetSpeedMultiplier(int index)
    {
        if (index >= 0 && index < speedMultipliers.Length)
        {
            _currentSpeedMultiplierIndex = index;
            Debug.Log($"[TimeManager] Velocidade: {CurrentSpeedMultiplier}x");
        }
    }
    
    /// <summary>
    /// Define o tempo para uma hora específica (0-24).
    /// Útil para testes ou pular para diferentes períodos do dia.
    /// </summary>
    public void SetTimeOfDay(float hour)
    {
        if (hour >= 0 && hour < 24)
        {
            _currentTimeOfDay = hour;
            OnTimeChanged?.Invoke(_currentTimeOfDay);
            Debug.Log($"[TimeManager] Tempo definido para {TimeString}");
        }
    }
    
    /// <summary>
    /// Pula X horas para frente.
    /// </summary>
    public void SkipHours(float hours)
    {
        SetTimeOfDay(_currentTimeOfDay + hours);
    }
    
    /// <summary>
    /// Retorna uma cor lerp entre dia e noite baseada no tempo.
    /// Usado pelo SkyboxController para transições suaves.
    /// </summary>
    public Color GetDayNightColor()
    {
        // Sunrise: 6h, Noon: 12h, Sunset: 18h, Night: 0h-5h, 19h-24h
        
        if (_currentTimeOfDay >= 5f && _currentTimeOfDay < 6f)
        {
            // Sunrise: 5h-6h (transição noite -> dia)
            float t = (_currentTimeOfDay - 5f) / 1f;
            return Color.Lerp(Color.black, Color.white, t);
        }
        else if (_currentTimeOfDay >= 6f && _currentTimeOfDay < 18f)
        {
            // Dia: 6h-18h
            return Color.white;
        }
        else if (_currentTimeOfDay >= 18f && _currentTimeOfDay < 20f)
        {
            // Sunset: 18h-20h (transição dia -> noite)
            float t = (_currentTimeOfDay - 18f) / 2f;
            return Color.Lerp(Color.white, Color.black, t);
        }
        else
        {
            // Noite: 20h-5h
            return Color.black;
        }
    }
    
    /// <summary>
    /// Calcula a intensidade da luz ambiente baseada no tempo.
    /// 0 = noite (sem luz), 1 = dia (luz máxima)
    /// </summary>
    public float GetLightIntensity()
    {
        if (_currentTimeOfDay >= 5f && _currentTimeOfDay < 6f)
        {
            // Sunrise
            return (_currentTimeOfDay - 5f) / 1f;
        }
        else if (_currentTimeOfDay >= 6f && _currentTimeOfDay < 18f)
        {
            // Dia
            return 1f;
        }
        else if (_currentTimeOfDay >= 18f && _currentTimeOfDay < 20f)
        {
            // Sunset
            return 1f - ((_currentTimeOfDay - 18f) / 2f);
        }
        else
        {
            // Noite
            return 0.2f; // Luz mínima para evitar total escuridão
        }
    }
    
    /// <summary>
    /// Retorna se é período diurno (6h-18h).
    /// </summary>
    public bool IsDaytime()
    {
        return _currentTimeOfDay >= 6f && _currentTimeOfDay < 18f;
    }
    
    /// <summary>
    /// Retorna se é período noturno (18h-6h).
    /// </summary>
    public bool IsNighttime()
    {
        return !IsDaytime();
    }
}

}  // namespace Systems

