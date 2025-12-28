using UnityEngine;
using System;

/// <summary>
/// Centraliza todos os eventos reais do jogo.
/// Conecta eventos de negócio a notificações toast e audio/visual feedback.
/// </summary>
public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }
    
    // ==================== TELEPORT EVENTS ====================
    public static event Action<string> OnTeleportStarted;
    public static event Action<string, string> OnTeleportSuccess;
    public static event Action<string, string> OnTeleportFailed;
    
    // ==================== BACKEND EVENTS ====================
    public static event Action OnBackendOnline;
    public static event Action OnBackendOffline;
    public static event Action<int, string> OnBackendError;
    
    // ==================== VALIDATION EVENTS ====================
    public static event Action<string> OnInvalidAction;
    public static event Action<string> OnWarningAction;
    
    // ==================== OPERATION EVENTS ====================
    public static event Action<string> OnOperationSuccess;
    public static event Action<string> OnOperationFailed;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SubscribeToGameEvents();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    /// <summary>
    /// Inscreve os handlers para converter eventos de negócio em toasts.
    /// </summary>
    void SubscribeToGameEvents()
    {
        OnTeleportStarted += HandleTeleportStarted;
        OnTeleportSuccess += HandleTeleportSuccess;
        OnTeleportFailed += HandleTeleportFailed;
        
        OnBackendOnline += HandleBackendOnline;
        OnBackendOffline += HandleBackendOffline;
        OnBackendError += HandleBackendError;
        
        OnInvalidAction += HandleInvalidAction;
        OnWarningAction += HandleWarningAction;
        
        OnOperationSuccess += HandleOperationSuccess;
        OnOperationFailed += HandleOperationFailed;
    }
    
    // ==================== TELEPORT HANDLERS ====================
    
    void HandleTeleportStarted(string entityName)
    {
        string message = $"🚀 Teleportando {entityName}...";
        ToastNotificationManager.Show(message, ToastNotificationManager.ToastType.Info, 2f);
        Debug.Log($"[GameEventManager] {message}");
    }
    
    void HandleTeleportSuccess(string entityName, string destinationName)
    {
        string message = $"✅ {entityName} teleportado para {destinationName}!";
        ToastNotificationManager.ShowSuccess(message);
        Debug.Log($"[GameEventManager] {message}");
    }
    
    void HandleTeleportFailed(string entityName, string reason)
    {
        string message = $"❌ Teleporte de {entityName} falhou: {reason}";
        ToastNotificationManager.ShowError(message);
        Debug.LogError($"[GameEventManager] {message}");
    }
    
    // ==================== BACKEND HANDLERS ====================
    
    void HandleBackendOnline()
    {
        string message = "✅ Conectado ao servidor";
        ToastNotificationManager.ShowSuccess(message);
        Debug.Log($"[GameEventManager] {message}");
    }
    
    void HandleBackendOffline()
    {
        string message = "⚠️ Conexão com servidor perdida";
        ToastNotificationManager.ShowWarning(message);
        Debug.LogWarning($"[GameEventManager] {message}");
    }
    
    void HandleBackendError(int errorCode, string errorMessage)
    {
        string message = $"🔴 Erro do servidor ({errorCode}): {errorMessage}";
        ToastNotificationManager.ShowError(message);
        Debug.LogError($"[GameEventManager] {message}");
    }
    
    // ==================== VALIDATION HANDLERS ====================
    
    void HandleInvalidAction(string reason)
    {
        string message = $"⛔ Ação inválida: {reason}";
        ToastNotificationManager.ShowError(message);
        Debug.LogWarning($"[GameEventManager] {message}");
    }
    
    void HandleWarningAction(string message)
    {
        string fullMessage = $"⚠️ {message}";
        ToastNotificationManager.ShowWarning(fullMessage);
        Debug.LogWarning($"[GameEventManager] {fullMessage}");
    }
    
    // ==================== OPERATION HANDLERS ====================
    
    void HandleOperationSuccess(string message)
    {
        string fullMessage = $"✅ {message}";
        ToastNotificationManager.ShowSuccess(fullMessage);
        Debug.Log($"[GameEventManager] {fullMessage}");
    }
    
    void HandleOperationFailed(string message)
    {
        string fullMessage = $"❌ {message}";
        ToastNotificationManager.ShowError(fullMessage);
        Debug.LogError($"[GameEventManager] {fullMessage}");
    }
    
    // ==================== PUBLIC DISPATCH METHODS ====================
    
    public static void RaiseTeleportStarted(string entityName)
    {
        OnTeleportStarted?.Invoke(entityName);
    }
    
    public static void RaiseTeleportSuccess(string entityName, string destinationName)
    {
        OnTeleportSuccess?.Invoke(entityName, destinationName);
    }
    
    public static void RaiseTeleportFailed(string entityName, string reason)
    {
        OnTeleportFailed?.Invoke(entityName, reason);
    }
    
    public static void RaiseBackendOnline()
    {
        OnBackendOnline?.Invoke();
    }
    
    public static void RaiseBackendOffline()
    {
        OnBackendOffline?.Invoke();
    }
    
    public static void RaiseBackendError(int errorCode, string errorMessage)
    {
        OnBackendError?.Invoke(errorCode, errorMessage);
    }
    
    public static void RaiseInvalidAction(string reason)
    {
        OnInvalidAction?.Invoke(reason);
    }
    
    public static void RaiseWarningAction(string message)
    {
        OnWarningAction?.Invoke(message);
    }
    
    public static void RaiseOperationSuccess(string message)
    {
        OnOperationSuccess?.Invoke(message);
    }
    
    public static void RaiseOperationFailed(string message)
    {
        OnOperationFailed?.Invoke(message);
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            OnTeleportStarted -= HandleTeleportStarted;
            OnTeleportSuccess -= HandleTeleportSuccess;
            OnTeleportFailed -= HandleTeleportFailed;
            
            OnBackendOnline -= HandleBackendOnline;
            OnBackendOffline -= HandleBackendOffline;
            OnBackendError -= HandleBackendError;
            
            OnInvalidAction -= HandleInvalidAction;
            OnWarningAction -= HandleWarningAction;
            
            OnOperationSuccess -= HandleOperationSuccess;
            OnOperationFailed -= HandleOperationFailed;
        }
    }
}

