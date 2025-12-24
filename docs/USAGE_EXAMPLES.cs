// EXEMPLO DE USO - Sistema Dia/Noite e Animações
// Este arquivo demonstra como usar os componentes implementados

using UnityEngine;
using Systems;

/// <summary>
/// Exemplo prático de uso do sistema de dia/noite
/// </summary>
public class DayNightExample : MonoBehaviour
{
    void Start()
    {
        // Exemplo 1: Acessar TimeManager (Singleton)
        AccessTimeManager();
        
        // Exemplo 2: Inscrever em eventos de tempo
        SubscribeToTimeEvents();
        
        // Exemplo 3: Controlar tempo via código
        ControlTimeFromCode();
        
        // Exemplo 4: Teleportar agente via código
        TeleportAgentFromCode();
    }
    
    /// <summary>
    /// Exemplo 1: Acessar TimeManager via singleton
    /// </summary>
    void AccessTimeManager()
    {
        TimeManager tm = TimeManager.Instance;
        
        // Obter tempo atual
        float currentTime = tm.CurrentTimeOfDay;      // 0-24 horas
        int hour = tm.CurrentHour;                     // 0-23
        int minute = tm.CurrentMinute;                 // 0-59
        string timeString = tm.TimeString;             // "14:30"
        
        // Obter estado
        bool isPaused = tm.IsPaused;
        bool isDaytime = tm.IsDaytime();               // 6h-18h
        bool isNighttime = tm.IsNighttime();           // 18h-6h
        
        // Obter velocidade atual
        float speedMultiplier = tm.CurrentSpeedMultiplier; // 1x, 2x ou 3x
        
        Debug.Log($"Hora: {timeString} | Multiplicador: {speedMultiplier}x | Pausa: {isPaused}");
    }
    
    /// <summary>
    /// Exemplo 2: Inscrever em eventos de tempo
    /// </summary>
    void SubscribeToTimeEvents()
    {
        TimeManager tm = TimeManager.Instance;
        
        // Quando o tempo muda continuamente (útil para UI)
        tm.OnTimeChanged += OnTimeUpdated;
        
        // Quando a hora inteira muda (útil para lógica baseada em hora)
        tm.OnHourChanged += OnHourChanged;
        
        // Quando o dia muda (24h → 0h)
        tm.OnDayChanged += OnDayChanged;
        
        // Quando pause/play muda
        tm.OnPauseChanged += OnPauseStatusChanged;
    }
    
    void OnTimeUpdated(float newTime)
    {
        // Chamado ~60 vezes por segundo
        // Ideal para atualizar UI (relógio)
        // Debug.Log($"Tempo: {newTime}");
    }
    
    void OnHourChanged()
    {
        // Chamado quando hora muda (ex: 14:59 → 15:00)
        // Ideal para triggers de hora específica
        TimeManager tm = TimeManager.Instance;
        Debug.Log($"[EVENTO] Hora mudou para: {tm.TimeString}");
        
        // Exemplo: Almoço às 12h
        if (tm.CurrentHour == 12)
        {
            Debug.Log("🍽️ Hora do almoço!");
        }
        
        // Exemplo: Saída de trabalho às 18h
        if (tm.CurrentHour == 18)
        {
            Debug.Log("🚗 Saída do trabalho!");
        }
    }
    
    void OnDayChanged()
    {
        // Chamado quando vira meia-noite (24h → 0h)
        Debug.Log("[EVENTO] Novo dia iniciado!");
    }
    
    void OnPauseStatusChanged()
    {
        // Chamado quando simulação é pausada ou retomada
        TimeManager tm = TimeManager.Instance;
        Debug.Log(tm.IsPaused ? "⏸️ PAUSADO" : "▶️ RODANDO");
    }
    
    /// <summary>
    /// Exemplo 3: Controlar tempo via código
    /// </summary>
    void ControlTimeFromCode()
    {
        TimeManager tm = TimeManager.Instance;
        
        // Pausar simulação
        tm.SetPaused(true);
        
        // Retomar simulação
        tm.SetPaused(false);
        
        // Alternar pause/play
        tm.TogglePause();
        
        // Definir velocidade (0=1x, 1=2x, 2=3x)
        tm.SetSpeedMultiplier(2); // 2x speed
        
        // Aumentar/diminuir velocidade (cicla)
        tm.IncreaseSpeed();  // 1x → 2x → 3x → 1x
        tm.DecreaseSpeed();  // Oposto
        
        // Pular para uma hora específica (útil para testes)
        tm.SetTimeOfDay(12.0f); // Ir para 12:00 (meio-dia)
        tm.SetTimeOfDay(6.0f);  // Ir para 06:00 (nascer do sol)
        tm.SetTimeOfDay(18.0f); // Ir para 18:00 (pôr do sol)
        
        // Pular X horas para frente
        tm.SkipHours(3.0f); // Avança 3 horas
    }
    
    /// <summary>
    /// Exemplo 4: Teleportar agente via código
    /// </summary>
    void TeleportAgentFromCode()
    {
        // Obter manager
        BackendTeleportManager teleportManager = BackendTeleportManager.Instance;
        
        // Exemplo: Teleportar agente para estação
        teleportManager.TeleportAgent(
            agentId: "550e8400-e29b-41d4-a716-446655440000", // UUID do agente
            locationType: "station",
            locationId: "550e8400-e29b-41d4-a716-446655440001", // UUID da estação
            onComplete: (success, message) => {
                if (success)
                {
                    Debug.Log("✅ Teleporte realizado com sucesso!");
                }
                else
                {
                    Debug.LogError($"❌ Erro ao teleportar: {message}");
                }
            }
        );
        
        // Exemplo: Teleportar para edifício
        teleportManager.TeleportAgent(
            agentId: "550e8400-e29b-41d4-a716-446655440002",
            locationType: "building",
            locationId: "550e8400-e29b-41d4-a716-446655440003",
            onComplete: (success, message) => {
                Debug.Log($"Teleporte para edifício: {message}");
            }
        );
    }
}

/// <summary>
/// Exemplo de script para controlar agentes com base em tempo
/// </summary>
public class AgentScheduleExample : MonoBehaviour
{
    void Start()
    {
        // Inscrever no evento de mudança de hora
        TimeManager.Instance.OnHourChanged += ExecuteSchedule;
    }
    
    void ExecuteSchedule()
    {
        TimeManager tm = TimeManager.Instance;
        
        switch (tm.CurrentHour)
        {
            case 6:
                Debug.Log("🌅 Nascer do sol - agentes acordam");
                break;
            
            case 8:
                Debug.Log("🚗 Agentes começam a se deslocar");
                break;
            
            case 9:
                Debug.Log("💼 Agentes começam a trabalhar");
                break;
            
            case 12:
                Debug.Log("🍽️ Pausa para almoço");
                break;
            
            case 13:
                Debug.Log("💼 Retomam trabalho");
                break;
            
            case 18:
                Debug.Log("🚗 Saída de trabalho - agentes retornam");
                break;
            
            case 19:
                Debug.Log("🏠 Agentes chegam em casa");
                break;
            
            case 23:
                Debug.Log("😴 Hora de dormir");
                break;
        }
    }
    
    void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnHourChanged -= ExecuteSchedule;
        }
    }
}

/// <summary>
/// Exemplo de script para reagir a dia/noite
/// </summary>
public class DayNightEffectsExample : MonoBehaviour
{
    [SerializeField] private Light streetLights;
    [SerializeField] private GameObject sunFlare;
    
    void Start()
    {
        TimeManager.Instance.OnTimeChanged += UpdateEffects;
    }
    
    void UpdateEffects(float timeOfDay)
    {
        // Acender luzes de rua à noite (19h-5h)
        if (timeOfDay >= 19f || timeOfDay < 5f)
        {
            if (streetLights != null)
                streetLights.intensity = 1.0f;
        }
        else if (timeOfDay >= 5f && timeOfDay < 6f)
        {
            // Transição suave no nascer do sol
            float t = (timeOfDay - 5f) / 1f;
            if (streetLights != null)
                streetLights.intensity = Mathf.Lerp(1.0f, 0.0f, t);
        }
        else
        {
            // Desligar durante o dia
            if (streetLights != null)
                streetLights.intensity = 0.0f;
        }
        
        // Mostrar sol durante o dia
        if (sunFlare != null)
        {
            bool showSun = timeOfDay >= 5f && timeOfDay < 20f;
            sunFlare.SetActive(showSun);
        }
    }
    
    void OnDestroy()
    {
        if (TimeManager.Instance != null)
        {
            TimeManager.Instance.OnTimeChanged -= UpdateEffects;
        }
    }
}

/// <summary>
/// Exemplo de script para animar agente via código
/// </summary>
public class AgentAnimationExample : MonoBehaviour
{
    void Start()
    {
        // Assumindo que este GameObject é um agente com AgentAnimator
        AgentAnimator animator = GetComponent<AgentAnimator>();
        
        if (animator != null)
        {
            // Atualizar status manualmente
            animator.UpdateStatus("idle");      // Agente parado
            animator.UpdateStatus("moving");    // Agente caminhando
            animator.UpdateStatus("working");   // Agente trabalhando
            animator.UpdateStatus("sleeping");  // Agente dormindo
            
            // Executar animação especial
            animator.PlayAnimation("wave");     // Acena
            animator.PlayAnimation("dance");    // Dança
        }
    }
}

// ============================================================
// TESTES RÁPIDOS - Cole no Console durante Play
// ============================================================

/*

// Teste 1: Pular para pôr do sol
TimeManager.Instance.SetTimeOfDay(18.0f);
TimeManager.Instance.SetSpeedMultiplier(0); // Pausar

// Teste 2: 2x speed
TimeManager.Instance.SetSpeedMultiplier(2);
TimeManager.Instance.SetPaused(false);

// Teste 3: Teleportar agente (substitua UUIDs reais)
var bt = BackendTeleportManager.Instance;
bt.TeleportAgent("seu-agent-id", "station", "sua-station-id");

// Teste 4: Log de evento
TimeManager.Instance.OnHourChanged += () => {
    Debug.Log("HORA MUDOU: " + TimeManager.Instance.TimeString);
};

*/


