using UnityEngine;

/// <summary>
/// Controla as animações do agente baseado no status da simulação.
/// Compatível com FBX animado exportado do Blender.
/// </summary>
[RequireComponent(typeof(Animator))]
public class AgentAnimator : MonoBehaviour
{
    [Header("Animation Settings")]
    [Tooltip("Velocidade mínima para acionar animação de caminhada")]
    public float walkSpeedThreshold = 0.1f;
    
    private Animator _animator;
    private VehicleMover _mover;
    private Vector3 _lastPosition;
    
    // Animation parameter names (must match Animator Controller)
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int IsWorking = Animator.StringToHash("IsWorking");
    private static readonly int Speed = Animator.StringToHash("Speed");
    
    private string _currentStatus = "IDLE";
    
    void Awake()
    {
        _animator = GetComponent<Animator>();
        _mover = GetComponent<VehicleMover>();
        _lastPosition = transform.position;
    }
    
    void Update()
    {
        if (_animator == null) return;
        
        // Calcular velocidade de movimento
        float speed = Vector3.Distance(transform.position, _lastPosition) / Time.deltaTime;
        _lastPosition = transform.position;
        
        // Determinar estado de animação baseado em movimento
        bool isMoving = speed > walkSpeedThreshold;
        
        // Atualizar parâmetros do animator
        _animator.SetBool(IsWalking, isMoving);
        _animator.SetFloat(Speed, speed);
    }
    
    /// <summary>
    /// Atualiza o status do agente baseado nos dados da API.
    /// Chamado pelo WorldController quando AgentData é atualizado.
    /// </summary>
    /// <param name="status">Status do agente: IDLE, WALKING, WORKING, etc.</param>
    public void UpdateStatus(string status)
    {
        if (_currentStatus == status) return;
        
        _currentStatus = status;
        
        if (_animator == null) return;
        
        // Mapear status para parâmetros de animação
        switch (status)
        {
            case "WORKING":
                _animator.SetBool(IsWorking, true);
                break;
            case "IDLE":
            case "WALKING":
            default:
                _animator.SetBool(IsWorking, false);
                break;
        }
    }
    
    /// <summary>
    /// Força uma animação específica (útil para debug)
    /// </summary>
    public void PlayAnimation(string triggerName)
    {
        if (_animator != null)
        {
            _animator.SetTrigger(triggerName);
        }
    }
}

