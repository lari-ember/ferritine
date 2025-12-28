# 🚀 Toast Notifications - Quick Reference Card

## ⚡ Setup Rápido (30 segundos)

```
1. Abra: MainSimulation.unity
2. Selecione: UIManager na Hierarchy
3. Arraste: ToastNotification.prefab → Toast Notification Prefab field
4. Play ✓
```

## 📋 Eventos Disponíveis

### Teleporte
```csharp
GameEventManager.OnTeleportStarted?.Invoke("AgentName");
GameEventManager.OnTeleportSuccess?.Invoke("AgentName", "DestName");
GameEventManager.OnTeleportFailed?.Invoke("AgentName", "Reason");
```
**Toasts:** 🚀 Info → ✅ Success ou ❌ Error

### Backend
```csharp
GameEventManager.OnBackendOnline?.Invoke();
GameEventManager.OnBackendOffline?.Invoke();
GameEventManager.OnBackendError?.Invoke(statusCode, "ErrorMsg");
```
**Toasts:** ✅ Success, ⚠️ Warning, 🔴 Error

### Validação
```csharp
GameEventManager.OnInvalidAction?.Invoke("Reason");
GameEventManager.OnWarningAction?.Invoke("Message");
```
**Toasts:** ⛔ Error, ⚠️ Warning

### Operações
```csharp
GameEventManager.OnOperationSuccess?.Invoke("Message");
GameEventManager.OnOperationFailed?.Invoke("Message");
```
**Toasts:** ✅ Success, ❌ Error

## 🎨 Toast Types

```csharp
ToastNotificationManager.ShowSuccess("message");   // Verde ✅
ToastNotificationManager.ShowError("message");     // Vermelho ❌
ToastNotificationManager.ShowWarning("message");   // Laranja ⚠️
ToastNotificationManager.ShowInfo("message");      // Azul 🔵
ToastNotificationManager.Show("msg", ToastType.Success, duration); // Custom
```

## 📍 Onde Editar

| Funcionalidade | Arquivo |
|---|---|
| Novos eventos | `GameEventManager.cs` |
| Configuração prefab | `UIManager.cs` → `SetupToastStyles()` |
| Cores/Estilos | `UIManager.cs` → `SetupToastStyles()` |
| Pool size | `ToastNotificationManager.cs` |
| Duração padrão | `ToastNotificationManager.cs` |

## ✨ Exemplos Prontos

### Teleporte bem-sucedido
```csharp
GameEventManager.OnTeleportSuccess?.Invoke("Agent-1", "Station A");
// Toast: ✅ Agent-1 teleportado para Station A!
```

### Erro de validação
```csharp
GameEventManager.OnInvalidAction?.Invoke("Apenas agentes podem teleportar");
// Toast: ⛔ Ação inválida: Apenas agentes podem teleportar
```

### Operação concluída
```csharp
GameEventManager.OnOperationSuccess?.Invoke("Fila modificada para 5");
// Toast: ✅ Fila modificada para 5
```

## 🎯 Casos de Uso Reais

### Teleporte
```csharp
// Em BackendTeleportManager
if (sucesso) GameEventManager.OnTeleportSuccess?.Invoke(agentId, locationId);
else GameEventManager.OnTeleportFailed?.Invoke(agentId, errorMsg);
```

### Pausa de Veículo
```csharp
// Em InspectorPanelController
if (request.result == Success) {
    GameEventManager.OnOperationSuccess?.Invoke("Veículo pausado");
}
```

### Validação
```csharp
// Em qualquer controller
if (!IsValid(action)) {
    GameEventManager.OnInvalidAction?.Invoke("Razão da invalidação");
    return;
}
```

## 🧪 Debug

### Ver todos os toasts sendo disparados
```csharp
// Em GameEventManager.cs, cada handler tem Debug.Log()
// Console mostra: [GameEventManager] Mensagem
```

### Testar manualmente
1. Crie um script vazio
2. Adicione: `GameEventManager.OnTeleportSuccess?.Invoke("Test", "Test");`
3. Chame do botão ou Start()
4. Veja o toast aparecer

## 📐 Estrutura do Prefab

```
ToastNotification (Image - background colorido)
├── Icon (Image - ícone)
└── MessageText (TextMeshProUGUI - texto da mensagem)
```

✅ O UIManager encontra "Icon" e "MessageText" automaticamente!

## ⚙️ Customizações Comuns

### Mudar cor de sucesso
```csharp
// Em UIManager.SetupToastStyles()
toastManager.toastStyles[0].backgroundColor = new Color(0.2f, 1f, 0.2f, 0.95f);
```

### Mudar duração padrão
```csharp
// Em ToastNotificationManager
public float displayDuration = 5f; // em segundos
```

### Mudar tamanho do pool
```csharp
// Em ToastNotificationManager
public int poolSize = 20;
public int prewarmCount = 10;
```

## ❌ Troubleshooting

| Problema | Solução |
|----------|---------|
| Toast não aparece | Verificar se prefab foi configurado no UIManager |
| Sem texto | Prefab precisa ter filho "MessageText" com TextMeshProUGUI |
| Sem cor | UIManager.SetupToastStyles() não foi chamado |
| Performance baixa | Aumentar `poolSize` se muitos toasts simultâneos |

## 📞 Arquivos Importantes

- `GameEventManager.cs` - Eventos centralizados
- `ToastNotificationManager.cs` - Sistema de notificações
- `UIManager.cs` - Inicialização automática
- `GAME_EVENTS_SYSTEM.md` - Documentação completa
- `IMPLEMENTATION_SUMMARY.md` - Visão geral do projeto

---

**Dica:** Sempre use `GameEventManager.OnEvent?.Invoke()` para disparar eventos, nunca chame `ToastNotificationManager` diretamente!

