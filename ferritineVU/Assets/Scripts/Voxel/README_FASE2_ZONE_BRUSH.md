# Fase 2: O Construtor (Modificação em Tempo Real)

## 📋 Resumo

Esta fase implementa o sistema de pintura de zonas para o CityLayer, permitindo que o jogador "pinte" o mapa com diferentes tipos de zoneamento urbano (Residencial, Comercial, Industrial, etc.).

## 🎯 Objetivos Alcançados

### 2.1 Escrita de Dados (Pintura de Zonas)

O CityLayer agora não é apenas visual - ele armazena o que cada célula representa:

- **0 - Nenhuma**: Área não zoneada
- **1-3 - Residencial**: Baixa, Média e Alta densidade (🏠🏢🏙️)
- **4-5 - Comercial**: Local e Central (🏪🏬)
- **6-7 - Industrial**: Leve e Pesada (🏭⚙️)
- **8+ - Especiais**: Misto, Rural, Parque, Via, etc.

### 2.2 O Zone Brush

Sistema de pintura implementado em `ZoneBrush.cs`:

```
Assets/Scripts/Voxel/
├── ZoneBrush.cs           # Sistema principal de pintura
├── ZoneBrushUI.cs         # Interface e atalhos de teclado
├── ZoneVisualizer.cs      # Colorização visual via vertex colors
└── ZonaHelper.cs          # Utilitários (expandido com densidade, compatibilidade, etc.)
```

### 2.3 Feedback Imediato

Shader `VoxelZoneOverlay.shader` que pinta o topo dos voxels com cores das zonas:

- ✅ Muito mais performático que criar GameObjects de "chão colorido"
- ✅ Cores representativas por tipo de zona
- ✅ Grid de orientação
- ✅ Pulso visual para zonas inválidas

## 🔧 Dirty Flags (HashSet)

A técnica mais importante desta fase:

```csharp
// Em vez de regenerar a mesh a cada célula pintada:
private HashSet<Vector2Int> _chunksToUpdate = new HashSet<Vector2Int>();

void PaintArea(Vector2Int center, ZonaTipo tipo) {
    // 1. Atualiza os DADOS primeiro
    cityLayer.PintarZona(cellPos, tipo);
    
    // 2. MARCA o chunk como dirty (não regenera ainda!)
    _chunksToUpdate.Add(chunkPos);
}

void LateUpdate() {
    // 3. Só no fim do frame, regenera cada chunk UMA VEZ
    foreach (var chunk in _chunksToUpdate) {
        RegenerateChunkMesh(chunk);
    }
    _chunksToUpdate.Clear();
}
```

### Por que HashSet?

Se o jogador pintar 100 células em 5 chunks diferentes no mesmo frame:
- ❌ **Sem HashSet**: 100 regenerações de mesh = LAG
- ✅ **Com HashSet**: 5 regenerações (uma por chunk) = SUAVE 60 FPS

O HashSet garante unicidade: mesmo chunk tocado 10 vezes = adicionado 1 vez.

## ⌨️ Atalhos de Teclado

| Tecla | Ação |
|-------|------|
| 1-9 | Seleciona zona correspondente |
| 0 / Q | Apagar (ZonaTipo.Nenhuma) |
| [ ] | Diminui/Aumenta tamanho do pincel |
| Tab | Próxima zona |
| Ctrl+Tab | Zona anterior |
| H | Toggle painel de ajuda |
| LMB | Pintar |
| RMB | Apagar |
| Ctrl+Scroll | Ajustar tamanho do pincel |

## 🎨 Cores das Zonas

| Zona | Cor | Hex Aproximado |
|------|-----|----------------|
| Residencial Baixa | Verde claro | #66CC66 |
| Residencial Média | Verde | #33B333 |
| Residencial Alta | Verde escuro | #1A801A |
| Comercial Local | Azul claro | #6699E6 |
| Comercial Central | Azul | #3366CC |
| Industrial Leve | Amarelo | #E6E666 |
| Industrial Pesada | Laranja | #E6B333 |
| Misto | Roxo | #B380CC |
| Parque | Verde-água | #33E680 |
| Via | Cinza | #808080 |

## 📁 Arquivos Criados

1. **ZoneBrush.cs** - Sistema principal de pintura
   - Raycast para detectar posição do mouse
   - Pintura com suporte a arraste
   - Dirty flags via HashSet
   - Preview visual do pincel

2. **ZoneBrushUI.cs** - Interface do usuário
   - Atalhos de teclado
   - Painel de ajuda OnGUI
   - Ajuste de tamanho via scroll

3. **ZoneVisualizer.cs** - Feedback visual
   - Cache de dados de zona
   - Aplicação de vertex colors nas meshes
   - Integração com eventos do CityLayer

4. **VoxelZoneOverlay.shader** - Shader de visualização
   - Overlay de cores por zona
   - Grid de orientação
   - Pulso para zonas inválidas

5. **ZonaHelper.cs** - Expandido com:
   - `GetDensityLevel()` - Nível de densidade (0-3)
   - `AreCompatible()` - Compatibilidade entre zonas vizinhas
   - `GetMaxFloors()` - Altura máxima permitida
   - `GetPollutionLevel()` / `GetNoiseLevel()` - Impactos ambientais
   - `GetZoneName()` / `GetZoneDescription()` - Textos localizados
   - `GetZoneIcon()` - Ícones Unicode
   - `GetZoneHotkey()` - Atalhos de teclado

## 🔗 Como Usar

### Onde Adicionar ZoneBrush + ZoneBrushUI?

Recomenda-se adicionar estes componentes a um **GameObject dedicado** na hierarquia:

```
📂 Hierarquia Recomendada:
├── GameManager (ou Main)
│   ├── CityLayer           ← Autoridade de zoneamento
│   └── ZoneBrushController ← CRIE ESTE OBJETO!
│       └── Componentes:
│           ├── ZoneBrush
│           └── ZoneBrushUI
├── VoxelWorld
│   └── TerrainHolder (chunks)
└── Main Camera
```

**Passo a Passo:**

1. Crie um GameObject vazio: `GameObject > Create Empty`
2. Renomeie para "ZoneBrushController"
3. Posicione como filho do GameManager (opcional, mas organizado)
4. Adicione os componentes:
   - `Add Component > Voxel > Zone Brush`
   - `Add Component > Voxel > Zone Brush UI`
5. Configure as referências no Inspector:
   - **ZoneBrush**:
     - CityLayer → arraste o objeto com CityLayer
     - TerrainWorld → arraste o TerrainWorld
     - VoxelWorld → arraste o VoxelWorld
     - MainCamera → arraste a Main Camera (ou deixe vazio para auto-detectar)
   - **ZoneBrushUI**:
     - ZoneBrush → será preenchido automaticamente se estiver no mesmo objeto

### No Unity:

1. Adicione o componente `ZoneBrush` a um GameObject (ex: GameManager)
2. Arraste as referências:
   - CityLayer
   - TerrainWorld
   - VoxelWorld
3. Adicione `ZoneBrushUI` ao mesmo objeto para ter atalhos de teclado
4. (Opcional) Adicione `ZoneVisualizer` para colorização automática

### Programaticamente:

```csharp
// Pinta uma célula específica
zoneBrush.PaintCell(new Vector2Int(10, 20), ZonaTipo.ResidencialMediaDensidade);

// Pinta uma área retangular
zoneBrush.PaintRect(
    new Vector2Int(0, 0), 
    new Vector2Int(10, 10), 
    ZonaTipo.ComercialLocal
);

// Muda a zona selecionada
zoneBrush.SetZona(ZonaTipo.IndustrialLeve);

// Muda tamanho do pincel
zoneBrush.SetTamanhoPincel(3); // 3x3
```

## 🧠 Perguntas de Design (Respostas)

### P: O que é mais eficiente ao pintar 10×10 blocos de uma vez?

**R**: Pintar todos os blocos nos dados primeiro e, no final do frame, reconstruir a malha do Chunk apenas uma vez. Implementado via `_chunksToUpdate` HashSet no LateUpdate.

### P: Qual a vantagem do HashSet quando o mouse cruza dois chunks?

**R**: O HashSet garante que ambos os chunks serão atualizados, mas cada um apenas UMA VEZ, mesmo se o mouse passou por células de ambos os chunks múltiplas vezes durante o arraste.

### P: Como dar feedback visual imediato?

**R**: Usando vertex colors + shader. O shader `VoxelZoneOverlay` pinta o topo dos voxels baseado no tipo de zona codificado na cor do vértice. Isso é O(1) para renderização vs O(n) de criar GameObjects.

## 🚀 Próximos Passos (Fase 3)

- [ ] Sistema de construção automática (agentes construtores)
- [ ] Crescimento orgânico de edifícios baseado na demanda
- [ ] Validação de zoneamento em tempo real
- [ ] Efeitos sonoros e partículas ao pintar

