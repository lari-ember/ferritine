# 🚀 Quick Setup Guide - Agent Animation System

## ⚡ Setup Rápido (5 minutos)

### 1️⃣ Configurar FBX (30 segundos)
```
Selecionar: hm.fbx
Inspector → Model → Apply
Inspector → Animation → Import Animation ✅ → Apply
```

### 2️⃣ Criar Material (30 segundos)
```
Assets/Materials/ → Create → Material → "AgentVertexColor"
Shader: Custom/VertexColor
Arrastar para hm.fbx mesh
```

### 3️⃣ Criar Animator Controller (2 minutos)
```
Assets/ → Create → Animator Controller → "AgentAnimatorController"

Parameters:
  + IsWalking (Bool)
  + IsWorking (Bool)
  + Speed (Float)

States:
  + Idle (default)
  + Walk
  + Work

Transitions:
  Idle ↔ Walk (IsWalking)
  Any → Work (IsWorking)
  Work → Idle (!IsWorking)

Arrastar clips do hm.fbx para states
```

### 4️⃣ Atualizar Prefab (2 minutos)
```
Abrir: agentprefab.prefab
Deletar: capsule (geometria antiga)
Adicionar child: hm.fbx
Components no root:
  ✅ Agent3D (modelRoot = hm)
  ✅ AgentAnimator
  ✅ Animator (Controller = AgentAnimatorController)
  ✅ VehicleMover
  ✅ SelectableEntity
Apply All
```

## ✅ Testar

Play → Verificar:
- [ ] Agentes com cores voxel (não cinza)
- [ ] Animação idle quando parado
- [ ] Animação walk quando movendo
- [ ] Transições suaves

## 🐛 Debug Rápido

| Problema | Solução |
|----------|---------|
| Agente branco/cinza | Material = AgentVertexColor |
| Sem animação | Animator.Controller = AgentAnimatorController |
| Não move | VehicleMover.moveSpeed = 1.2 |
| Animação trava | Verificar transitions no Animator |

## 📝 Animator Parameters

Use estes nomes EXATOS:
- `IsWalking` (Bool)
- `IsWorking` (Bool)
- `Speed` (Float)

Mudá-los quebra o código!

## 🎯 Settings Recomendados

**Material:**
- Smoothness: 0.3-0.5
- Metallic: 0.0

**VehicleMover:**
- Move Speed: 1.2
- Rotate Speed: 180
- Preserve Y: ✅

**AgentAnimator:**
- Walk Speed Threshold: 0.1

**Transitions:**
- Exit Time: 0.25
- Transition Duration: 0.25

---

📖 **Documentação completa:** `AGENT_ANIMATION_IMPLEMENTATION.md`

