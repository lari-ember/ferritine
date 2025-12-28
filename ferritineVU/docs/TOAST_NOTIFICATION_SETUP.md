# 🔔 Configuração do Sistema de Toast Notifications

## 📋 Problema Identificado
Os toast notifications (mensagens de sucesso, erro, aviso, info) não estavam sendo exibidos porque o sistema não estava configurado na cena.

## ✅ Solução Implementada
O **UIManager** agora inicializa automaticamente o **ToastNotificationManager** quando o prefab de toast é configurado.

## 🛠️ Como Configurar

### Passo 1: Abrir a Cena Principal
1. Abra `Assets/Scenes/MainSimulation.unity` no Unity

### Passo 2: Selecionar o UIManager
1. Na Hierarchy, encontre o GameObject que contém o componente **UIManager**
   - Geralmente está no Canvas principal ou em um GameObject dedicado "UI"
2. Selecione esse GameObject

### Passo 3: Configurar o Toast Notification Prefab
1. No Inspector, localize o componente **UIManager**
2. No campo **"Toast Notification Prefab"** (na seção "UI Prefabs"):
   - Arraste o prefab `Assets/Prefabs/UI/ToastNotification.prefab`
   - Ou clique no círculo ao lado do campo e selecione "ToastNotification"

### Passo 4: Verificar os Logs
1. Entre em Play Mode
2. No Console, você deve ver:
   ```
   [UIManager] ✓ ToastNotificationManager initialized successfully
   [ToastNotificationManager] ✓ Initialized with 5 toasts
   ```

## 🎨 Estilos de Toast

O sistema vem com 4 estilos pré-configurados:

| Tipo | Cor | Uso |
|------|-----|-----|
| **Success** | Verde | Operações bem-sucedidas |
| **Warning** | Laranja | Avisos importantes |
| **Error** | Vermelho | Erros e falhas |
| **Info** | Azul | Informações gerais |

## 📝 Como Usar no Código

```csharp
// Mostrar mensagem de sucesso
ToastNotificationManager.ShowSuccess("Agente teleportado com sucesso!");

// Mostrar aviso
ToastNotificationManager.ShowWarning("Destino muito distante");

// Mostrar erro
ToastNotificationManager.ShowError("Falha na conexão com backend");

// Mostrar informação
ToastNotificationManager.ShowInfo("Simulação pausada");

// Método genérico com duração customizada
ToastNotificationManager.Show("Mensagem", ToastNotificationManager.ToastType.Info, 5f);
```

## 🏗️ Estrutura do Prefab ToastNotification

O prefab deve ter a seguinte estrutura:
```
ToastNotification (GameObject com Image)
├── Icon (GameObject com Image)
└── MessageText (GameObject com TextMeshProUGUI)
```

**Componentes necessários:**
- `Image` - No objeto raiz (para o background colorido)
- `Image` - No objeto filho "Icon" (para o ícone)
- `TextMeshProUGUI` - No objeto filho "MessageText" (para o texto)
- `RectTransform` - Em todos os objetos

## 🔧 Troubleshooting

### Toast não aparece
1. Verifique se o prefab está configurado no UIManager
2. Verifique o Console para mensagens de erro
3. Certifique-se que o UIManager está ativo na cena

### Toast aparece mas sem texto
1. Verifique se o prefab tem um objeto filho chamado **exatamente** "MessageText"
2. Verifique se esse objeto tem o componente `TextMeshProUGUI`

### Toast aparece mas sem cor/ícone
1. Os estilos são configurados automaticamente pelo UIManager
2. Se quiser customizar, edite o método `SetupToastStyles()` no UIManager

## 📁 Arquivos Relacionados
- `Assets/Scripts/UI/UIManager.cs` - Gerenciador principal de UI
- `Assets/Scripts/UI/ToastNotificationManager.cs` - Sistema de toast
- `Assets/Prefabs/UI/ToastNotification.prefab` - Prefab do toast
