using UnityEngine;

/// <summary>
/// Controla as animações do agente baseado no status da simulação.
/// Sincroniza com o backend AgentData.status e movimento físico.
/// Compatível com FBX animado exportado do Blender.
/// 
/// Estados de animação suportados:
/// - IDLE: Parado (sem atividade)
/// - MOVING: Caminhando/deslocando-se
/// - WORKING: Trabalhando/ocupado
/// - SLEEPING: Dormindo
/// - TRAVELING: Em transporte/viagem
/// </summary>
[RequireComponent(typeof(Animator))]
public class AgentAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField]
    [Tooltip("Velocidade mínima para acionar animação de caminhada")]
    public float walkSpeedThreshold = 0.1f;
    
    [SerializeField]
    [Tooltip("Suavidade na transição de velocidade da animação")]
    public float speedDamping = 0.1f;
    
    [SerializeField]
    [Tooltip("Velocidade máxima de caminhada para o parâmetro Speed")]
    public float maxWalkSpeed = 2f;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    private Animator _animator;
    private VehicleMover _mover;
    private Vector3 _lastPosition;
    private float _smoothedSpeed = 0f;
    
    // Animation parameter hashes (performante que usar strings)
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsWorking = Animator.StringToHash("IsWorking");
    private static readonly int IsIdle = Animator.StringToHash("IsIdle");
    private static readonly int IsSleeping = Animator.StringToHash("IsSleeping");
    private static readonly int Speed = Animator.StringToHash("Speed");
    
    private string _currentStatus = "IDLE";
    private float _statusChangeTime = 0f;
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError($"[AgentAnimator] {gameObject.name} não tem componente Animator!");
        }
        
        _mover = GetComponent<VehicleMover>();
        _lastPosition = transform.position;
    }
    
    void Update()
    {
        if (_animator == null) return;
        
        // Calcular velocidade de movimento
        float rawSpeed = Vector3.Distance(transform.position, _lastPosition) / Time.deltaTime;
        _lastPosition = transform.position;
        
        // Suavizar a velocidade para evitar picos
        _smoothedSpeed = Mathf.Lerp(_smoothedSpeed, rawSpeed, speedDamping);
        
        // Normalizar velocidade para 0-1 (relativa ao maxWalkSpeed)
        float normalizedSpeed = Mathf.Min(_smoothedSpeed / maxWalkSpeed, 1f);
        
        // Atualizar parâmetro Speed no animator (para blend trees)
        _animator.SetFloat(Speed, normalizedSpeed);
        
        // Determinar animação baseado em movimento e status
        UpdateAnimationState(normalizedSpeed);
        
        if (showDebugInfo)
        {
            Debug.Log($"[AgentAnimator] {gameObject.name} | Status: {_currentStatus} | Speed: {_smoothedSpeed:F2} | Normalized: {normalizedSpeed:F2}");
        }
    }
    
    /// <summary>
    /// Atualiza o estado de animação baseado em movimento e status do backend.
    /// </summary>
    private void UpdateAnimationState(float normalizedSpeed)
    {
        bool isMoving = normalizedSpeed > (walkSpeedThreshold / maxWalkSpeed);
        
        // Mapear o status para parâmetros de animação
        switch (_currentStatus.ToUpper())
        {
            case "MOVING":
            case "TRAVELING":
            case "COMMUTING":
                _animator.SetBool(IsWalking, true);
                _animator.SetBool(IsWorking, false);
                _animator.SetBool(IsSleeping, false);
                _animator.SetBool(IsIdle, false);
                break;
            
            case "WORKING":
            case "CONSTRUCTING":
            case "REPAIRING":
            case "MAINTENANCE":
                _animator.SetBool(IsWorking, true);
                _animator.SetBool(IsWalking, false);
                _animator.SetBool(IsSleeping, false);
                _animator.SetBool(IsIdle, false);
                break;
            
            case "SLEEPING":
                _animator.SetBool(IsSleeping, true);
                _animator.SetBool(IsWorking, false);
                _animator.SetBool(IsWalking, false);
                _animator.SetBool(IsIdle, false);
                break;
            
            case "IDLE":
            case "AT_HOME":
            case "AT_WORK":
            default:
                // Se está se movendo fisicamente mas status é idle, mostrar walk
                if (isMoving)
                {
                    _animator.SetBool(IsWalking, true);
                }
                else
                {
                    _animator.SetBool(IsIdle, true);
                }
                _animator.SetBool(IsWorking, false);
                _animator.SetBool(IsSleeping, false);
                break;
        }
    }
    
    /// <summary>
    /// Atualiza o status do agente baseado nos dados da API.
    /// Chamado pelo WorldController quando AgentData.status é atualizado.
    /// </summary>
    /// <param name="status">Status do agente: idle, moving, working, sleeping, etc.</param>
    public void UpdateStatus(string status)
    {
        if (string.IsNullOrEmpty(status)) status = "IDLE";
        
        if (_currentStatus.Equals(status, System.StringComparison.OrdinalIgnoreCase))
            return;
        
        _currentStatus = status;
        _statusChangeTime = Time.time;
        
        Debug.Log($"[AgentAnimator] {gameObject.name} status mudou para: {_currentStatus}");
    }
    
    /// <summary>
    /// Força uma animação específica como trigger (útil para ações especiais como teleporte).
    /// </summary>
    public void PlayAnimation(string triggerName)
    {
        if (_animator != null && !string.IsNullOrEmpty(triggerName))
        {
            _animator.SetTrigger(triggerName);
            Debug.Log($"[AgentAnimator] {gameObject.name} trigger: {triggerName}");
        }
    }
    
    /// <summary>
    /// Retorna o status atual do agente.
    /// </summary>
    public string GetCurrentStatus() => _currentStatus;
    
    /// <summary>
    /// Retorna a velocidade suavizada do agente.
    /// </summary>
    public float GetSmoothedSpeed() => _smoothedSpeed;
}


