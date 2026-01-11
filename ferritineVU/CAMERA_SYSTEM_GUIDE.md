# Sistema de Câmera Híbrida - Ferritine VU

## 🎮 Modos de Câmera

### 1. **Modo Free (Padrão)**
Câmera RTS estilo Cities: Skylines

| Controle | Ação |
|----------|------|
| WASD / Setas | Mover câmera |
| Q / E | Rotacionar |
| R / F | Inclinar (pitch) |
| Scroll | Zoom |
| Page Up/Down | Subir/descer |
| Shift | Sprint (2.5x mais rápido) |
| MMB (arraste) | Pan |
| Shift + RMB | Orbitar |

### 2. **Modo Follow**
Segue uma entidade selecionada

| Controle | Ação |
|----------|------|
| Clique em entidade | Selecionar |
| Duplo clique | Seguir (ou via código) |
| ESC | Parar de seguir |

**Código:**
```csharp
// Seguir entidade selecionada
cameraController.FollowSelectedEntity();

// Seguir qualquer Transform
cameraController.StartFollowing(targetTransform);

// Parar
cameraController.StopFollowing();
```

### 3. **Modo First Person (V)**
Andar pela cidade como pedestre

| Controle | Ação |
|----------|------|
| V | Entrar/Sair do modo FPS |
| WASD | Andar |
| Mouse | Olhar ao redor |
| Shift | Correr |
| ESC | Sair |

**Código:**
```csharp
// Entrar na posição atual
cameraController.EnterFirstPerson();

// Entrar em posição específica
cameraController.EnterFirstPersonAt(worldPosition);

// Sair
cameraController.ExitFirstPerson();

// Verificar estado
if (cameraController.IsFirstPerson) { ... }
```

### 4. **Modo Orbit**
Orbitar ao redor de um ponto

| Controle | Ação |
|----------|------|
| Shift + RMB (arraste) | Orbitar |

### 5. **Modo Preview**
Visualização temporária de uma localização

**Código:**
```csharp
cameraController.PreviewLocation(worldPosition);
cameraController.StopPreview();
```

---

## 🛡️ Colisão com Terreno

A câmera automaticamente:
- Não atravessa o terreno de voxels
- Mantém altura mínima configurável
- Detecta altura via raycast

### Configuração no Inspector:

```
┌─────────────────────────────────────────────────────┐
│ ▼ Collision                                         │
│   Collision Layer        [ Terrain ✓ ]              │
│   Collision Buffer       [ 2 ]                      │
│   Enable Terrain Collision [✓]                      │
│   Use Voxel Terrain Collision [✓]   ← IMPORTANTE!   │
│   Voxel Terrain Layer    [ Terrain ✓ ]              │
└─────────────────────────────────────────────────────┘
```

### No Modo FPS:
- Câmera fica à altura dos olhos (1.7m)
- Colisão com paredes (não atravessa prédios)
- Segue a altura do terreno automaticamente

---

## 📌 Bookmarks (Ctrl+1-9)

| Controle | Ação |
|----------|------|
| Ctrl + 1-9 | Salvar posição |
| 1-9 | Restaurar posição |

---

## 🔧 Setup

1. A câmera já tem o `CameraController` configurado
2. Configure as layers no Inspector:
   - `Selectable Layer` → Layer "Selectable"
   - `Voxel Terrain Layer` → Layer "Terrain"
3. Ative `Use Voxel Terrain Collision`

---

## 📊 API Pública

```csharp
// Obter modo atual
CameraMode mode = cameraController.GetCurrentMode();

// Teleportar
cameraController.TeleportTo(position, yaw: 45f, pitch: 30f);

// Seguir
cameraController.StartFollowing(transform);
cameraController.StopFollowing();

// First Person
cameraController.EnterFirstPerson();
cameraController.EnterFirstPersonAt(position);
cameraController.ExitFirstPerson();

// Preview
cameraController.PreviewLocation(position);
cameraController.StopPreview();

// Eventos
cameraController.OnEntitySelected.AddListener((entity) => { ... });
cameraController.OnCameraModeChanged.AddListener((mode) => { ... });
```

---

## 🎯 Integração com CityCursor

O `CityCursor` mostra uma luz no terreno. O `CameraController` gerencia o movimento.

Ambos trabalham juntos:
- `CityCursor` → feedback visual do mouse
- `CameraController` → controle de câmera e seleção

### Modo Normal (RTS):
- **Luz spot** segue o cursor no terreno
- Cor muda quando sobre objetos selecionáveis

### Modo Primeira Pessoa (FPS):
- **Luz escondida** (não faz sentido em FPS)
- **Crosshair (mira)** aparece no centro da tela
- Raycast sai da mira com **distância máxima de 1 metro**
- Crosshair muda de cor quando sobre objeto interativo

### Configuração do Crosshair no Inspector:

```
▼ Modo Primeira Pessoa
  FPS Interaction Distance: [ 1 ]     ← 1 metro (≈ 1 jarda)
  Crosshair Color:          [████] Branco
  Crosshair Size:           [ 20 ]
  Crosshair Thickness:      [ 2 ]
  Crosshair Gap:            [ 4 ]     ← Espaço no centro
```

