# Greedy Meshing - Sistema de Otimização de Malhas de Voxel

## 📋 Visão Geral

Este módulo implementa o algoritmo de **Greedy Meshing** para otimização de malhas de voxel.
A ideia central é simples: "Se tenho vários quadrados adjacentes da mesma cor/tipo, 
por que não fazer um retângulo único que cubra todos?"

### Benefícios
- **50-80% de redução** no número de triângulos
- Melhor performance de renderização
- Menor uso de memória de GPU
- Suporte a meshing incremental

---

## 🧩 Arquitetura

```
┌─────────────────────────────────────────────────────────────────┐
│                    ChunkMeshGeneratorGreedy                      │
│            (Drop-in replacement para ChunkMeshGenerator)         │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────────┐
│                      GreedyMeshBuilder                           │
│              (Algoritmo principal de meshing)                    │
│                                                                  │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐           │
│  │ Passo A:     │  │ Passo B:     │  │ Passo C:     │           │
│  │ Slice        │→ │ Scanning     │→ │ Masking      │           │
│  │ (Fatiamento) │  │ (Varredura)  │  │ (Máscara)    │           │
│  └──────────────┘  └──────────────┘  └──────────────┘           │
└─────────────────────────────────────────────────────────────────┘
                              │
        ┌─────────────────────┼─────────────────────┐
        ▼                     ▼                     ▼
┌───────────────┐   ┌───────────────────┐   ┌───────────────┐
│VoxelBorder    │   │FaceRegionMerger   │   │VoxelFaceTypes │
│Detector       │   │                   │   │(Estruturas)   │
│               │   │- Flood Fill       │   │               │
│- Identificação│   │- Fusão de Faces   │   │- VoxelFace    │
│  de Bordas    │   │- Subdivisão       │   │- MergedQuad   │
│- Conexões     │   │                   │   │- MeshData     │
└───────────────┘   └───────────────────┘   └───────────────┘
```

---

## 📐 O Algoritmo em 3 Passos

### Passo A: Slice (Fatiamento)
Você não olha para o volume 3D de uma vez. Passa uma "lâmina" pelo chunk, 
camada por camada em cada eixo (X, Y, Z).

```
Chunk 3D          →    Fatias 2D
  ┌───┐                 ┌─┐ ┌─┐ ┌─┐
 /   /│                 │░│ │▓│ │█│
┌───┐ │          →      └─┘ └─┘ └─┘
│   │/                  w=0  w=1  w=2
└───┘
```

### Passo B: Scanning (Varredura)
Para cada fatia 2D:
1. Encontra uma face que precisa ser desenhada
2. **Expansão Horizontal**: Verifica vizinho ao lado. Mesmo tipo? Expande largura.
3. **Expansão Vertical**: Tenta expandir a linha inteira para cima.

```
Antes:                    Depois:
┌─┬─┬─┬─┐                ┌───────┐
│1│1│1│1│                │       │
├─┼─┼─┼─┤                │  1×4  │  → 1 quad em vez de 4
│2│2│3│3│         →      ├───┬───┤
├─┼─┼─┼─┤                │2×2│3×2│  → 2 quads em vez de 4
│2│2│3│3│                │   │   │
└─┴─┴─┴─┘                └───┴───┘
```

### Passo C: Masking (Máscara)
Para não processar o mesmo voxel duas vezes, usa uma máscara booleana.
Quando um voxel é incluído num retângulo, marca como `true` e ignora.

---

## 🔗 Tipos de Conexão de Voxels

O sistema classifica cada voxel por como ele se conecta aos vizinhos:

```
┌─────────────────────────────────────────────────────────────────┐
│  Tipo 1: Isolated (0 conexões)     Tipo 2: Single (1 conexão)   │
│                                                                  │
│       ░░░░░                              ░░░░░                   │
│       ░░█░░  (bloco sozinho)             ░░█━━█ (pode formar par)│
│       ░░░░░                              ░░░░░                   │
├─────────────────────────────────────────────────────────────────┤
│  Tipo 3: Corner (2 conexões em L)  Tipo 4: Straight (2 opostas) │
│                                                                  │
│       ░░█░░                              ░░░░░                   │
│       ░░┃░░  (formato L)              █━━█━━█ (linha reta)       │
│       ░░█━━█                             ░░░░░                   │
├─────────────────────────────────────────────────────────────────┤
│  Tipo 5: TShape (3 conexões)       Tipo 6: Full (4 conexões)    │
│                                                                  │
│       ░░█░░                              ░░█░░                   │
│       █━━█━━█  (formato T)            █━━█━━█ (cruz completa)    │
│       ░░░░░                              ░░█░░                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🎯 Corte Diagonal Consistente

Para evitar cracks (rachaduras) entre quads, usamos a regra de paridade:

```csharp
bool UseMainDiagonal(Vector3Int pos) {
    return (pos.x + pos.y + pos.z) % 2 == 0;
}
```

Isso garante que diagonais adjacentes se conectem corretamente:

```
  Sem consistência:        Com consistência:
  ┌───┐┌───┐               ┌───┐┌───┐
  │╲  ││  ╲│  ← Crack!     │╲  ││╱  │  ← OK!
  │ ╲ ││ ╲ │               │ ╲ ││ ╱ │
  └───┘└───┘               └───┘└───┘
```

---

## 🚀 Como Usar

### Substituir o gerador padrão:

```csharp
// Antes (ChunkMeshGenerator padrão):
Mesh mesh = ChunkMeshGenerator.BuildMesh(world, data, scale);

// Depois (com Greedy Meshing):
using Voxel.GreedyMeshing;
Mesh mesh = ChunkMeshGeneratorGreedy.BuildMesh(world, data, scale);
```

### Com estatísticas:

```csharp
var (mesh, stats) = ChunkMeshGeneratorGreedy.BuildMeshWithStats(world, data, scale);
Debug.Log($"Redução: {stats.ReductionPercent:F1}%");
```

### Apenas faces horizontais (bordas):

```csharp
Mesh mesh = GreedyMeshBuilder.BuildGreedyMeshHorizontalOnly(
    voxels, width, height, depth, scale
);
```

### Analisar bordas e conectividade:

```csharp
var borderFaces = VoxelBorderDetector.IdentifyBorderFaces(voxels, w, h, d);

foreach (var face in borderFaces) {
    Debug.Log($"Voxel {face.VoxelPosition}: {face.ConnectionType}");
}
```

---

## 📊 Classes Principais

### `VoxelBorderDetector`
Identifica voxels na borda (faces laterais expostas ao ar).

| Método | Descrição |
|--------|-----------|
| `IdentifyBorderVoxels()` | Lista posições de voxels de borda |
| `IdentifyBorderFaces()` | Lista faces com dados de conectividade |
| `GetFaceConnections()` | Obtém conexões de um voxel específico |
| `ClassifyConnection()` | Classifica tipo de conexão (Isolated, Corner, etc.) |

### `GreedyMeshBuilder`
Implementa o algoritmo de Greedy Meshing.

| Método | Descrição |
|--------|-----------|
| `BuildGreedyMesh()` | Gera mesh completa otimizada |
| `BuildGreedyMeshHorizontalOnly()` | Apenas faces X±, Z± |
| `FloodFillRegion()` | Encontra região conectada |

### `FaceRegionMerger`
Lógica de fusão de faces baseada em tipo.

| Método | Descrição |
|--------|-----------|
| `CanMergeFaces()` | Verifica se duas faces podem fundir |
| `FindMergeableRegions()` | Agrupa faces em regiões |
| `SubdivideNonRectangularRegion()` | Divide regiões complexas |

### `ChunkMeshGeneratorGreedy`
Drop-in replacement para `ChunkMeshGenerator`.

| Método | Descrição |
|--------|-----------|
| `BuildMesh()` | API compatível com original |
| `BuildMeshWithStats()` | Inclui estatísticas de otimização |
| `UpdateMeshRegion()` | Atualização incremental |

---

## 💡 Dicas de Performance

1. **Dirty Flags**: Marca chunks modificados e regenera só no final do frame
2. **Batch Updates**: Agrupe múltiplas modificações antes de regenerar
3. **LOD**: Para chunks distantes, use meshing menos detalhado
4. **Jobs System**: O algoritmo é paralelizável (veja seção avançada)

---

## 🔧 Configuração

```csharp
// Habilitar logging de estatísticas
ChunkMeshGeneratorGreedy.EnableStats = true;
```

---

## 📈 Exemplo de Resultados

```
Chunk (0,0): Faces 2048 → 412 (79.9% redução)
Chunk (0,1): Faces 1856 → 398 (78.6% redução)
Chunk (1,0): Faces 2304 → 521 (77.4% redução)
Agregado: 6208 → 1331 (78.6% redução)
```

---

## 🎓 Referências

- [0fps - Meshing in a Minecraft Game](https://0fps.net/2012/06/30/meshing-in-a-minecraft-game/)
- [Greedy Meshing Voxels](https://eddieabbondanz.io/post/voxel/greedy-mesh/)
- [Voxel Engine Tutorial](https://github.com/roboleary/VoxelEngine)

