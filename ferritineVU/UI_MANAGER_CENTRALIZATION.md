# ✅ Centralização de UI no UIManager

## 📋 Resumo

Expandimos o **UIManager** para gerenciar TODOS os painéis de UI de forma centralizada, ao invés de ter scripts individuais espalhados pela cena.

## 🎯 Objetivo

- **Antes**: Cada painel tinha que ser encontrado na cena com `FindObjectOfType<>()`
- **Depois**: UIManager instancia dinamicamente prefabs e gerencia todo o ciclo de vida

## 📦 Painéis Gerenciados

### 1. EntityInspectorPanel ✅
- **Prefab**: `entityInspectorPrefab`
- **Métodos**:
  - `ShowInspector(SelectableEntity)` - Abre o painel para uma entidade
  - `HideInspector()` - Fecha e destrói o painel
  - `IsInspectorVisible()` - Verifica se está visível

### 2. TeleportSelectorUI ✅
- **Prefab**: `teleportSelectorPrefab`
- **Métodos**:
  - `ShowTeleportSelector(AgentData)` - Abre o seletor para um agente
  - `HideTeleportSelector()` - Fecha e destrói o seletor
  - `IsTeleportSelectorVisible()` - Verifica se está visível

### 3. Método Global ✅
- `CloseAllPanels()` - Fecha todos os painéis abertos (útil para ESC ou reset)

## 🔧 Alterações Realizadas

### UIManager.cs
```csharp
[Header("UI Prefabs")]
[SerializeField] private GameObject entityInspectorPrefab;
[SerializeField] private GameObject teleportSelectorPrefab;  // 🆕 NOVO

// Entity Inspector
private GameObject currentInspectorPanel;
private EntityInspectorPanel currentInspector;

// Teleport Selector 🆕 NOVO
private GameObject currentTeleportPanel;
private TeleportSelectorUI currentTeleportSelector;
```

**Métodos adicionados**:
- `ShowTeleportSelector(AgentData agent)`
- `HideTeleportSelector()`
- `IsTeleportSelectorVisible()`
- `CloseAllPanels()`
- Validação do prefab `teleportSelectorPrefab` em `ValidatePrefabs()`
- Cleanup no `OnDestroy()`

### TeleportSelectorUI.cs
```csharp
/// <summary>
/// Alias for Open() - usado pelo UIManager.
/// </summary>
public void ShowForAgent(AgentData agent)
{
    Open(agent);
}
```

**Método `Close()` atualizado**:
- Agora chama `UIManager.Instance?.HideTeleportSelector()` para notificar o UIManager
- Mesclou lógica de cleanup (unhighlight, preview particle, camera preview)

### EntityInspectorPanel.cs
**Método `OnTeleportClicked()` atualizado**:
```csharp
void OnTeleportClicked()
{
    if (currentEntity == null || currentEntity.entityType != SelectableEntity.EntityType.Agent)
        return;
    
    // 🆕 NOVO: Usa UIManager ao invés de FindObjectOfType
    if (UIManager.Instance != null)
    {
        UIManager.Instance.ShowTeleportSelector(currentEntity.agentData);
        AudioManager.PlayUISound("panel_open");
    }
    else
    {
        ToastNotificationManager.ShowError("UIManager não encontrado!");
    }
}
```

## 🚀 Vantagens da Centralização

### ✅ Performance
- Não usa `FindObjectOfType<>()` (muito lento)
- Instancia só quando necessário
- Destrói quando não está em uso

### ✅ Arquitetura
- **Single Responsibility**: UIManager é o único responsável por painéis
- **Dependency Injection**: Scripts não precisam procurar uns aos outros
- **Singleton Pattern**: Acesso global via `UIManager.Instance`

### ✅ Manutenibilidade
- Fácil adicionar novos painéis (só adicionar ao UIManager)
- Validação automática de prefabs no Awake
- Logs detalhados para debug

### ✅ Escalabilidade
- Pronto para adicionar:
  - NotificationPanel
  - SettingsPanel
  - ConfirmationDialog
  - BuildingInspector
  - RouteEditor
  - etc.

## 🔮 Próximos Passos

Para adicionar um novo painel:

1. **Criar o script do painel** (ex: `NotificationPanel.cs`)
2. **Adicionar prefab field no UIManager**:
   ```csharp
   [SerializeField] private GameObject notificationPanelPrefab;
   private GameObject currentNotificationPanel;
   private NotificationPanel currentNotification;
   ```

3. **Adicionar métodos públicos**:
   ```csharp
   public void ShowNotification(string message) { ... }
   public void HideNotification() { ... }
   ```

4. **Adicionar validação em `ValidatePrefabs()`**

5. **Adicionar cleanup em `OnDestroy()` e `CloseAllPanels()`**

## 📝 Setup na Unity

1. **Abrir cena** (ex: MainScene)
2. **Selecionar GameObject com UIManager**
3. **No Inspector**, atribuir os prefabs:
   - `Entity Inspector Prefab` → Arraste `Assets/Prefabs/UI/EntityInspectorPanel.prefab`
   - `Teleport Selector Prefab` → Arraste `Assets/Prefabs/UI/TeleportSelectorPanel.prefab`

4. **Verificar logs** ao dar Play:
   ```
   [UIManager] ✓ entityInspectorPrefab assigned: EntityInspectorPanel
   [UIManager] ✓ Prefab tem componente EntityInspectorPanel
   [UIManager] ✓ teleportSelectorPrefab assigned: TeleportSelectorPanel
   [UIManager] ✓ Prefab tem componente TeleportSelectorUI
   ```

## ⚠️ Avisos Importantes

### Não usar mais `FindObjectOfType<>`
❌ **Antes (ERRADO)**:
```csharp
TeleportSelectorUI teleportUI = FindObjectOfType<TeleportSelectorUI>();
teleportUI.Open(agent);
```

✅ **Depois (CORRETO)**:
```csharp
UIManager.Instance.ShowTeleportSelector(agent);
```

### Painéis não devem estar na cena
- EntityInspectorPanel → Remover da cena, deve ser prefab
- TeleportSelectorUI → Remover da cena, deve ser prefab
- UIManager instancia dinamicamente quando necessário

### Cache do IDE
Se o Rider/Visual Studio mostrar erros mas o código está correto:
```bash
cd /home/larisssa/Documentos/codigos/ferritine/ferritineVU
find Assets/Scripts/UI -name "*.cs" -exec touch {} \;
```

## 🎨 Diagrama de Arquitetura

```
┌─────────────────────────────────────┐
│          UIManager                  │
│         (Singleton)                 │
│                                     │
│  + ShowInspector(entity)            │
│  + HideInspector()                  │
│  + ShowTeleportSelector(agent)      │
│  + HideTeleportSelector()           │
│  + CloseAllPanels()                 │
└──────────┬──────────────────────────┘
           │ instancia/destrói
           │
    ┌──────┴───────┬──────────────────┐
    │              │                  │
    ▼              ▼                  ▼
┌─────────┐  ┌──────────┐      ┌──────────┐
│Inspector│  │Teleport  │  ... │Future    │
│  Panel  │  │ Selector │      │ Panels   │
└─────────┘  └──────────┘      └──────────┘
```

## ✅ Status

- [x] UIManager expandido
- [x] TeleportSelectorUI integrado
- [x] EntityInspectorPanel atualizado
- [x] Métodos de validação
- [x] Logs de debug
- [x] Cleanup automático
- [x] Documentação completa

---

**Data**: 2025-12-10  
**Autor**: GitHub Copilot  
**Contexto**: Issue - Centralização de UI para melhor arquitetura

