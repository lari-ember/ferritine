# ✅ Agent Animation Setup - Checklist

## 📋 Checklist de Implementação

### Parte 1: Código (CONCLUÍDO ✅)
- [x] VertexColor.shader criado
- [x] AgentAnimator.cs criado
- [x] EntityAgent3D.cs atualizado
- [x] WorldController.cs atualizado
- [x] Documentação criada

### Parte 2: Unity Editor (FAZER AGORA)

#### 🎨 Import Settings
- [ ] Selecionar `hm.fbx` no Project
- [ ] Model tab:
  - [ ] Import Blend Shapes ✅
  - [ ] Import Visibility ✅
  - [ ] Import Cameras ❌
  - [ ] Import Lights ❌
  - [ ] Apply
- [ ] Animation tab:
  - [ ] Import Animation ✅
  - [ ] Animation Type: Generic (ou Humanoid)
  - [ ] Apply

#### 🎨 Material
- [ ] Create → Material → "AgentVertexColor"
- [ ] Shader: Custom/VertexColor
- [ ] Smoothness: 0.4
- [ ] Metallic: 0.0
- [ ] Aplicar no mesh do hm.fbx

#### 🎬 Animator Controller
- [ ] Create → Animator Controller → "AgentAnimatorController"
- [ ] Abrir Animator window
- [ ] Parameters:
  - [ ] IsWalking (Bool)
  - [ ] IsWorking (Bool)
  - [ ] Speed (Float)
- [ ] States:
  - [ ] Idle (Make Default)
  - [ ] Walk
  - [ ] Work
- [ ] Transitions:
  - [ ] Idle → Walk (Condition: IsWalking = true)
  - [ ] Walk → Idle (Condition: IsWalking = false)
  - [ ] Any State → Work (Condition: IsWorking = true)
  - [ ] Work → Idle (Condition: IsWorking = false)
- [ ] Atribuir clips:
  - [ ] Idle clip → Idle state
  - [ ] Walk clip → Walk state
  - [ ] Work clip → Work state

#### 🎮 Prefab Update
- [ ] Abrir agentprefab.prefab
- [ ] Deletar geometria antiga (capsule)
- [ ] Adicionar hm.fbx como child
- [ ] Root components:
  - [ ] Agent3D presente
  - [ ] AgentAnimator presente
  - [ ] Animator presente
  - [ ] VehicleMover presente
  - [ ] SelectableEntity presente
- [ ] Agent3D:
  - [ ] Model Root = hm (arrastar)
- [ ] Animator:
  - [ ] Controller = AgentAnimatorController
  - [ ] Apply Root Motion = false
- [ ] VehicleMover:
  - [ ] Move Speed = 1.2
  - [ ] Rotate Speed = 180
  - [ ] Preserve Y = true
- [ ] AgentAnimator:
  - [ ] Walk Speed Threshold = 0.1
- [ ] Apply All

#### 🧪 Teste
- [ ] Play mode
- [ ] Agentes aparecem com cores voxel
- [ ] Animação idle quando parados
- [ ] Animação walk quando em movimento
- [ ] Transições suaves
- [ ] Sem erros no Console

### Parte 3: Validação Final

#### Visual
- [ ] Cores do MagicaVoxel preservadas
- [ ] Modelo não aparece branco/cinza
- [ ] Proporções corretas

#### Animação
- [ ] Idle loop funciona
- [ ] Walk loop funciona
- [ ] Work animation funciona
- [ ] Transições suaves (sem pulos)

#### Performance
- [ ] 50 agentes sem lag
- [ ] FPS > 60
- [ ] Sem warnings excessivos

#### Integração
- [ ] Status da API atualiza animações
- [ ] Movimento sincroniza com walk animation
- [ ] Seleção de agente funciona
- [ ] Inspector mostra dados corretos

## 🎯 Valores de Referência

```
Material AgentVertexColor:
  Shader: Custom/VertexColor
  Smoothness: 0.4
  Metallic: 0.0

Animator Controller Parameters:
  IsWalking: Bool
  IsWorking: Bool
  Speed: Float

Transition Settings:
  Exit Time: 0.25
  Transition Duration: 0.25
  Has Exit Time: true (exceto Any State)

VehicleMover Settings:
  moveSpeed: 1.2
  rotateSpeed: 180
  preserveY: true

AgentAnimator Settings:
  walkSpeedThreshold: 0.1
```

## 📊 Status

**Código:** ✅ 100% Completo  
**Unity Setup:** ⏳ Pendente  
**Testes:** ⏳ Pendente

---

**Última atualização:** 2025-12-07  
**Próximo passo:** Configurar no Unity Editor seguindo checklist acima

