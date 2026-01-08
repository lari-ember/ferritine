# 🎨 Guia de Criação do Texture Atlas para Voxels

## 📐 Especificações Técnicas

### Dimensões
- **Resolução**: 512x512 pixels (ou 1024x1024 para HD)
- **Grid**: 8 linhas × 8 colunas = 64 células
- **Tamanho de cada célula**: 64x64 pixels (ou 128x128 para HD)
- **Formato**: PNG com transparência (se necessário)

### Mapeamento de BlockTypes

```
Grid Layout (8x8):

ROW 0:  [0:Ar]  [1:Grama]  [2:Terra]  [3:Argila]  [4:Areia]  [5:Cascalho]  [6:Laterita]  [7:---]
ROW 1:  [8:---]  [9:---]  [10:Granito]  [11:Diorito]  [12:Andesito]  [13:Basalto]  [14:Gneiss]  [15:Migmatito]
ROW 2:  [16:---]  [17:---]  [18:---]  [19:---]  [20:Arenito]  [21:Calcario]  [22:---]  [23:---]
ROW 3:  [24:---]  [25:---]  [26:---]  [27:---]  [28:---]  [29:---]  [30:Concreto]  [31:Asfalto]
ROW 4:  [32:---]  [33:---]  [34:---]  [35:---]  [36:---]  [37:---]  [38:---]  [39:---]
ROW 5:  [40:Agua]  [41:Vegetacao]  [42:---]  [43:---]  [44:---]  [45:---]  [46:---]  [47:---]
ROW 6:  [48:---]  [49:---]  [50:Rocha]  [51:---]  [52:---]  [53:---]  [54:---]  [55:---]
ROW 7:  [56:---]  [57:---]  [58:---]  [59:---]  [60:---]  [61:---]  [62:---]  [63:---]
```

## 🎨 Paleta de Cores Recomendada

### Materiais Essenciais (Implementados)

| BlockType | Index | Hex Color | RGB | Descrição Visual |
|-----------|-------|-----------|-----|------------------|
| **Ar** | 0 | #00000000 | - | Transparente/vazio |
| **Grama** | 1 | #7CFC00 | (124, 252, 0) | Verde limão vibrante |
| **Terra** | 2 | #8B4513 | (139, 69, 19) | Marrom saddle |
| **Argila** | 3 | #CD5C5C | (205, 92, 92) | Vermelho indiano |
| **Areia** | 4 | #F4A460 | (244, 164, 96) | Areia clara |
| **Cascalho** | 5 | #A9A9A9 | (169, 169, 169) | Cinza escuro |
| **Laterita** | 6 | #B22222 | (178, 34, 34) | Vermelho tijolo |
| **Granito** | 10 | #696969 | (105, 105, 105) | Cinza dim |
| **Diorito** | 11 | #DCDCDC | (220, 220, 220) | Cinza gainsboro |
| **Andesito** | 12 | #808080 | (128, 128, 128) | Cinza puro |
| **Basalto** | 13 | #2F4F4F | (47, 79, 79) | Cinza ardósia escuro |
| **Gneiss** | 14 | #778899 | (119, 136, 153) | Cinza ardósia claro |
| **Migmatito** | 15 | #B0C4DE | (176, 196, 222) | Azul aço claro |
| **Arenito** | 20 | #DEB887 | (222, 184, 135) | Madeira marrom |
| **Calcário** | 21 | #F5F5DC | (245, 245, 220) | Bege |
| **Concreto** | 30 | #C0C0C0 | (192, 192, 192) | Prata |
| **Asfalto** | 31 | #1C1C1C | (28, 28, 28) | Preto escuro |
| **Água** | 40 | #4682B4 | (70, 130, 180) | Azul aço |
| **Vegetação** | 41 | #228B22 | (34, 139, 34) | Verde floresta |
| **Rocha** | 50 | #696969 | (105, 105, 105) | Cinza dim |

## 🛠️ Método 1: Criar no GIMP (Recomendado)

### Passo a Passo

1. **Criar novo arquivo**
   ```
   Arquivo → Novo
   - Largura: 512px
   - Altura: 512px
   - Tipo: RGB color
   - Preenchimento: Transparência
   ```

2. **Ativar grade**
   ```
   Ver → Mostrar Grade
   Imagem → Configurar Grade
   - Largura: 64px
   - Altura: 64px
   - Estilo: Linhas sólidas
   ```

3. **Criar guias**
   ```
   Imagem → Guias → Novo guia
   Adicionar guias em: 0, 64, 128, 192, 256, 320, 384, 448, 512
   (tanto horizontal quanto vertical)
   ```

4. **Preencher células**
   ```
   Para cada BlockType:
   - Selecionar ferramenta de seleção retangular
   - Selecionar célula 64x64
   - Preencher com cor (Balde de tinta)
   - Opcional: Adicionar textura/ruído
   ```

5. **Adicionar detalhes** (opcional)
   ```
   - Filtros → Render → Nuvens → Ruído sólido
     - Tamanho X/Y: 3-5 (para textura sutil)
     - Turbulência: 1-3
   - Filtros → Distorções → Embaralhar
     - Para textura orgânica
   ```

6. **Exportar**
   ```
   Arquivo → Exportar Como
   - Nome: VoxelAtlas.png
   - Tipo: PNG
   - Compressão: 9
   ```

## 🛠️ Método 2: Usar Texturas Existentes

### Fontes de Texturas Gratuitas

1. **OpenGameArt.org**
   - Procurar por: "voxel textures", "minecraft textures"
   - Licença: CC0 ou CC-BY

2. **Textures.com** (FreePBR)
   - Categoria: Ground, Rock, Terrain
   - 15 texturas grátis por dia

3. **PolyHaven.com**
   - Texturas PBR em alta resolução
   - 100% grátis (CC0)

### Adaptar Texturas

```bash
# Redimensionar para 64x64 (ImageMagick)
convert input.jpg -resize 64x64^ -gravity center -extent 64x64 output.png

# Criar grid de texturas (ImageMagick)
montage tile_*.png -tile 8x8 -geometry 64x64+0+0 VoxelAtlas.png
```

## 🛠️ Método 3: Criar Proceduralmente (Python)

```python
from PIL import Image, ImageDraw
import random

# Configuração
ATLAS_SIZE = 512
GRID_SIZE = 8
TILE_SIZE = ATLAS_SIZE // GRID_SIZE  # 64px

# Cores dos materiais (RGB)
COLORS = {
    0: (0, 0, 0, 0),        # Ar (transparente)
    1: (124, 252, 0),       # Grama
    2: (139, 69, 19),       # Terra
    3: (205, 92, 92),       # Argila
    4: (244, 164, 96),      # Areia
    10: (105, 105, 105),    # Granito
    40: (70, 130, 180),     # Água
    # ... adicione mais
}

# Criar imagem
atlas = Image.new('RGBA', (ATLAS_SIZE, ATLAS_SIZE), (0, 0, 0, 0))
draw = ImageDraw.Draw(atlas)

# Preencher tiles
for block_id, color in COLORS.items():
    row = block_id // GRID_SIZE
    col = block_id % GRID_SIZE
    
    x0 = col * TILE_SIZE
    y0 = row * TILE_SIZE
    x1 = x0 + TILE_SIZE
    y1 = y0 + TILE_SIZE
    
    # Preencher com cor base
    draw.rectangle([x0, y0, x1, y1], fill=color)
    
    # Adicionar ruído (opcional)
    for _ in range(100):  # 100 pontos aleatórios
        px = random.randint(x0, x1-1)
        py = random.randint(y0, y1-1)
        noise = random.randint(-20, 20)
        r = max(0, min(255, color[0] + noise))
        g = max(0, min(255, color[1] + noise))
        b = max(0, min(255, color[2] + noise))
        draw.point((px, py), fill=(r, g, b, 255))

# Salvar
atlas.save('VoxelAtlas.png')
print("Atlas criado: VoxelAtlas.png")
```

## 📥 Importar no Unity

### 1. Copiar arquivo
```
Copiar VoxelAtlas.png para:
Assets/Textures/VoxelAtlas.png
```

### 2. Configurar Import Settings

```
Selecionar VoxelAtlas.png no Project
No Inspector:

Texture Type: Default
  - sRGB (Color Texture): ✅
  
Advanced:
  - Non-Power of 2: None
  - Read/Write Enabled: ❌
  - Generate Mip Maps: ❌ (para voxel art)

Filter Mode: Point (no filter)
  OU Bilinear (se preferir suavizado)

Max Size: 512 (ou 1024 se criou em HD)

Format: 
  - PC: RGBA 32 bit
  - Android: ASTC 8x8
  - iOS: PVRTC 4 bits

APLICAR
```

### 3. Criar Material

```
Botão direito em Assets/Materials/ → Create → Material
Nome: VoxelTerrainMaterial

Inspector:
  - Shader: Standard
    (ou Universal Render Pipeline/Lit se usar URP)
  
  - Albedo: Arrastar VoxelAtlas.png
  
  - Metallic: 0
  - Smoothness: 0.2-0.4
  
  - Normal Map: Nenhum (ou criar um)
  - Height Map: Nenhum
  
  - Tiling: 1, 1
  - Offset: 0, 0
```

### 4. Aplicar no Chunk

No script que cria chunks (ex: `ChunkRenderer.cs`):

```csharp
[Header("Rendering")]
public Material voxelMaterial; // Arrastar VoxelTerrainMaterial aqui

void GenerateChunk(Vector2Int chunkPos)
{
    ChunkData data = terrainWorld.GetGarantirChunk(chunkPos);
    Mesh mesh = ChunkMeshGenerator.BuildMesh(terrainWorld, data, terrainWorld.escalaVoxel);
    
    // Atribuir mesh e material
    MeshFilter mf = GetComponent<MeshFilter>();
    MeshRenderer mr = GetComponent<MeshRenderer>();
    
    mf.mesh = mesh;
    mr.material = voxelMaterial; // Aplica o atlas
}
```

## 🔍 Verificação

### Teste Visual

1. **Executar o jogo**
2. **Observar o terreno**:
   - Superfície deve ser **verde** (grama)
   - Escavar deve revelar **marrom** (terra)
   - Mais fundo: **vermelho** (argila)
   - Base: **cinza** (granito)

### Teste de UV Coordinates

```csharp
// Debug script para visualizar UVs
void OnDrawGizmos()
{
    MeshFilter mf = GetComponent<MeshFilter>();
    if (mf == null || mf.sharedMesh == null) return;
    
    Vector2[] uvs = mf.sharedMesh.uv;
    Debug.Log($"Total UVs: {uvs.Length}");
    
    // Mostrar primeiras 10 UVs
    for (int i = 0; i < Mathf.Min(10, uvs.Length); i++)
    {
        Debug.Log($"UV[{i}]: {uvs[i]}");
    }
}
```

## 🎨 Melhorias Avançadas

### 1. Normal Map (para relevo)

```python
# Gerar normal map a partir do atlas
from PIL import Image
import numpy as np

atlas = Image.open('VoxelAtlas.png')
# ... processar para gerar normal map
normal_map.save('VoxelAtlas_Normal.png')
```

No Unity:
```
- Normal Map: VoxelAtlas_Normal.png
- Bump: 0.5-1.0
```

### 2. Ambient Occlusion (para sombras)

```csharp
// Adicionar AO em ChunkMeshGenerator.cs
private static void AddFaceWithAO(...)
{
    // Calcular AO nos vértices
    float[] aoValues = CalculateAO(pos, dir);
    
    // Armazenar em vertex colors
    for (int i = 0; i < 4; i++)
    {
        colors.Add(new Color(1, 1, 1, aoValues[i]));
    }
}
```

### 3. Variações de Textura

```csharp
// Adicionar variação baseada em posição
private static void AddFaceUVs(List<Vector2> uvs, byte blockType, Vector3Int pos)
{
    int atlasSize = 8;
    float tileSize = 1.0f / atlasSize;
    
    // Variação baseada em hash da posição
    int variation = (pos.x * 73 + pos.y * 179 + pos.z * 283) % 4;
    int tileIndex = blockType + (variation * 64); // 4 variações por tile
    
    // ... resto do código
}
```

## ❓ Troubleshooting

**Problema**: Texturas aparecem borradas  
**Solução**: Mudar Filter Mode para `Point (no filter)`

**Problema**: UVs errados (texturas misturadas)  
**Solução**: Verificar se atlas é exatamente 512x512 e células são 64x64

**Problema**: Transparência não funciona  
**Solução**: 
1. Material deve usar shader com alpha (ex: Standard com Rendering Mode = Transparent)
2. Atlas PNG deve ter canal alpha

**Problema**: Cores muito escuras  
**Solução**: Adicionar luz direcional na cena, ou ajustar Emission no material

## 📚 Referências

- [Unity: Texture Import Settings](https://docs.unity3d.com/Manual/class-TextureImporter.html)
- [OpenGameArt: Voxel Textures](https://opengameart.org/art-search-advanced?keys=voxel)
- [GIMP: Create Texture Atlas](https://docs.gimp.org/en/)
- [ImageMagick: Montage](https://imagemagick.org/script/montage.php)

---

**Próximo passo**: Criar o atlas e testar visualmente no Unity!

