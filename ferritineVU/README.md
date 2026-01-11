# Ferritine VU - Documentação Técnica

## 📁 Estrutura do Projeto

```
Assets/Scripts/
├── API/                    # Comunicação com backend
├── Audio/                  # Sistema de áudio
├── City/                   # Lógica de cidade
├── Controllers/            # Controladores principais
│   ├── CameraController.cs     # Câmera híbrida (RTS/FPS/Follow)
│   ├── CityCursor.cs           # Cursor visual estilo Cities Skylines
│   └── SelectableEntity.cs     # Entidades selecionáveis
├── Core/                   # Núcleo do sistema
├── Entities/               # Agentes e veículos
├── Systems/                # Sistemas globais (tempo, skybox)
├── UI/                     # Interface do usuário
├── Utils/                  # Utilitários e pools
└── Voxel/                  # Sistema de voxels
    ├── TerrainWorld.cs         # Dados de altura do terreno
    ├── VoxelWorld.cs           # Gerenciador de chunks visuais
    ├── ChunkMeshGenerator.cs   # Geração de meshes otimizada
    ├── ChunkData.cs            # Estrutura de dados do chunk
    └── VoxelRaycast.cs         # Raycast DDA para voxels
```

---

## 🎮 Sistema de Câmera

### Modos Disponíveis

| Modo | Tecla | Descrição |
|------|-------|-----------|
| Free | Padrão | Câmera RTS estilo Cities Skylines |
| Follow | Clique duplo | Segue entidade selecionada |
| First Person | V | Andar pela cidade em 1ª pessoa |
| Orbit | Shift+RMB | Orbitar ao redor de ponto |

### Controles

| Tecla | Ação |
|-------|------|
| WASD | Mover câmera |
| Q/E | Rotacionar |
| R/F | Inclinar |
| Scroll | Zoom suave |
| V | Modo primeira pessoa |
| Shift | Sprint |
| Ctrl+1-9 | Salvar bookmark |
| 1-9 | Ir para bookmark |

---

## 🔦 CityCursor

Sistema de feedback visual estilo city builder:

- **Modo Normal**: Luz spot segue o cursor no terreno
- **Modo FPS**: Crosshair no centro + raycast de 1 metro

### Configuração

```
▼ Luz do Cursor
  Light Intensity: 3
  Light Range: 8

▼ Modo Primeira Pessoa
  FPS Interaction Distance: 1 (metro)
  Crosshair Size: 20
```

---

## 🧱 Sistema de Voxels

### Arquitetura

```
TerrainWorld (dados)
    ↓ GetGarantirChunk()
ChunkData (byte[,,])
    ↓ BuildMesh()
VoxelWorld (visuais)
    ↓ GameObject + Mesh
Cena do Unity
```

### Otimizações Implementadas

1. **Pool de GameObjects**: Reusa objetos de chunk
2. **Descarte Progressivo**: Evita picos de GC
3. **Arrays Estáticos**: Direções e vértices pré-calculados
4. **Capacidade Inicial**: Listas com tamanho estimado

### Configuração de Performance

```csharp
// PreloadProfile (recomendado)
preloadProfile.qualityLevel = QualityLevel.Medium;

// Ou manual:
raioPreload = 200f;           // metros
dadosRetencaoRadius = 2;       // chunks
dadosRetencaoBatchPerFrame = 32;
```

---

## 📊 Monitoramento de Memória

```csharp
// No console ou via código:
Debug.Log(voxelWorld.GetMemoryStats());
```

Saída:
```
[VoxelWorld Memory Stats]
Chunks Visuais: 120
Chunks Dados (RAM): 45
Pool Size: 32/128
Fila Descarte: 0
Memória Total: ~256 MB
```

---

## 🔧 Configuração de Layers

Crie estas layers em `Edit > Project Settings > Tags and Layers`:

| Layer | Uso |
|-------|-----|
| Terrain | Chunks de voxel (raycast) |
| Selectable | Entidades selecionáveis |

---

## 🚀 Performance Tips

1. **Escala de Voxel**: Maior escala = menos voxels = mais rápido
2. **PreloadProfile**: Use perfis de qualidade pré-configurados
3. **Pool Max Size**: Ajuste conforme RAM disponível
4. **GC Interval**: 5s é bom equilíbrio

---

## 📝 Convenções de Código

- Campos privados: `_nomeCampo`
- Constantes: `NomeConstante`
- Regiões: `#region NomeDaRegiao`
- Documentação: XML comments em métodos públicos

