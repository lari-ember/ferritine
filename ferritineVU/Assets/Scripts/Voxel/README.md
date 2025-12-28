# Voxel Terrain System - Ferritine

Sistema de terreno voxelizado de alta performance para Unity com DOTS.

## Arquitetura

```
┌─────────────────────────────────────────────────────────────────┐
│                        HeightmapVoxelLoader                     │
│                      (Main Entry Point)                         │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐         │
│  │  ChunkPool  │    │LODManager   │    │VoxelRenderer│         │
│  │  (Objects)  │◄───│  (Loading)  │───►│ (Rendering) │         │
│  └─────────────┘    └─────────────┘    └─────────────┘         │
│         │                  │                   │                │
│         ▼                  ▼                   ▼                │
│  ┌─────────────┐    ┌─────────────┐    ┌─────────────┐         │
│  │ChunkObject  │    │   Jobs      │    │  Materials  │         │
│  │(GameObject) │    │(Burst/DOTS) │    │  (Batched)  │         │
│  └─────────────┘    └─────────────┘    └─────────────┘         │
└─────────────────────────────────────────────────────────────────┘
```

## Estrutura de Arquivos

```
Assets/Scripts/Voxel/
├── Data/
│   └── VoxelStructs.cs        # Structs de dados (zero GC)
├── Jobs/
│   ├── VoxelFaceCullingJob.cs # Face culling paralelo
│   └── GreedyMeshingJob.cs    # Greedy meshing otimizado
├── Editor/
│   └── VoxelSystemSetup.cs    # Utilitários do editor
├── ChunkPool.cs               # Object pooling para chunks
├── ChunkLODManager.cs         # LOD e lazy loading
├── VoxelRenderer.cs           # Renderização com instancing
└── HeightmapVoxelLoader.cs    # Controller principal
```

## Configuração Rápida

### Via Menu (Recomendado)
1. **Ferritine > Voxel System > Create Voxel Terrain**
2. Arraste `cwb.png` para o campo "Heightmap Texture"
3. Pressione Play

### Manual
1. Criar GameObject vazio "VoxelTerrain"
2. Adicionar componente `HeightmapVoxelLoader`
3. Configurar heightmap e parâmetros
4. Play

## Requisitos

### Pacotes Unity (Package Manager)
- **Burst** (com.unity.burst)
- **Mathematics** (com.unity.mathematics)
- **Collections** (com.unity.collections)

### Configuração do Heightmap
A textura `cwb.png` precisa de:
- **Read/Write Enabled**: ✓ (OBRIGATÓRIO)
- **sRGB**: ✗ (grayscale)
- **Generate Mip Maps**: ✗
- **Max Size**: 8192+
- **Compression**: None

Use: **Ferritine > Voxel System > Configure Heightmap Texture**

## Escala

| Parâmetro | Valor |
|-----------|-------|
| 1 Voxel | 3.6 cm |
| 1 Chunk | 64³ voxels = ~2.3m³ |
| Mapa Total | ~33km x 33km (~1100km²) |
| Altura Máxima | 200m |

## Técnicas de Performance

1. **Unity Job System + Burst** - Face culling e meshing paralelos
2. **Object Pooling** - Zero GC durante gameplay
3. **Face Culling** - Só renderiza faces expostas
4. **Greedy Meshing** - Mescla faces em quads maiores
5. **LOD Dinâmico** - Detalhe baseado em distância
6. **Lazy Loading** - Carrega chunks sob demanda

## Debug

O sistema mostra estatísticas em runtime:
- Chunks visíveis
- Draw calls
- Jobs ativos
- Triângulos totais

## Troubleshooting

### "Heightmap texture must be readable"
Habilitar "Read/Write Enabled" nas configurações de importação.

### Chunks não carregam
1. Verificar pacotes Burst/Mathematics/Collections
2. Checar console para erros
3. Verificar câmera dentro dos bounds

### Performance baixa
1. Verificar Burst Compilation (Jobs > Burst > Enable)
2. Reduzir loadDistance no LODManager

