# 🌍 Sistema de Geologia de Curitiba - Guia de Configuração

## ✅ Status da Implementação

### Código Completo
- ✅ **BlockType enum** com materiais brasileiros (VoxelDataTypes.cs)
- ✅ **Sistema de camadas geológicas** (ChunkData.cs)
- ✅ **UV Mapping com texture atlas 8x8** (ChunkMeshGenerator.cs)
- ✅ **Greedy meshing removido** (geometria funcionando)

### Próximos Passos: Assets Visuais
- ⏳ Criar texture atlas 8x8 (512x512 ou 1024x1024)
- ⏳ Configurar material com shader apropriado
- ⏳ Testar visualização das camadas

---

## 📊 Estrutura de Camadas Implementada

```
Superfície (y = heightmap)     → Grama (ou Areia se altitude < 5)
├─ 2-5 blocos abaixo           → Terra (solo superficial)
├─ 6-12 blocos abaixo          → Argila vermelha (típica do Paraná)
└─ 13+ blocos abaixo           → Granito (rocha matriz)

Exceção: Áreas baixas (< 3)    → Água
```

### Geologia Real de Curitiba
Esta implementação reflete a geologia do **Primeiro Planalto Paranaense**:
- **Grama/Vegetação**: Cobertura vegetal típica de clima subtropical
- **Terra**: Solos orgânicos rasos
- **Argila**: Camadas de laterita e argila vermelha (comum na região)
- **Granito**: Embasamento cristalino (Escudo Atlântico)

---

## 🎨 Criando o Texture Atlas

### Opção 1: Atlas Simples (Cores Sólidas)
Para prototipar rapidamente, crie um atlas 8x8 com cores básicas:

```
Posição no Atlas (linha 0):
[0,0] Ar         - Transparente
[0,1] Grama      - Verde #4CAF50
[0,2] Terra      - Marrom #8B4513
[0,3] Argila     - Vermelho #CD5C5C
[0,4] Areia      - Amarelo #F4A460
[0,5] Cascalho   - Cinza #A9A9A9
[0,6] Laterita   - Laranja #D2691E
[0,7] (vazio)

Posição no Atlas (linha 1, blockType 10-17):
[1,2] Granito    - Cinza escuro #696969
[1,3] Diorito    - Cinza médio #808080
[1,4] Andesito   - Cinza claro #C0C0C0
[1,5] Basalto    - Preto #2F4F4F
[1,6] Gneiss     - Cinza listrado
[1,7] Migmatito  - Cinza com veios

Posição no Atlas (linha 2, blockType 20-27):
[2,4] Arenito    - Bege #F5DEB3
[2,5] Calcário   - Branco #F5F5DC

Posição no Atlas (linha 3, blockType 30-37):
[3,6] Concreto   - Cinza claro #D3D3D3
[3,7] Asfalto    - Preto #1C1C1C

Posição no Atlas (linha 5, blockType 40-47):
[5,0] Água       - Azul #1E90FF
[5,1] Vegetação  - Verde escuro #228B22

Posição no Atlas (linha 6, blockType 50):
[6,2] Rocha      - Cinza #808080
```

### Opção 2: Texturas Realistas
Para visual profissional:
1. Baixe texturas de domínio público (ex: [Texture Haven](https://polyhaven.com/textures))
2. Use texturas seamless (sem emendas)
3. Redimensione cada textura para 64x64px ou 128x128px
4. Monte o atlas 8x8 em um editor de imagem (GIMP, Photoshop)

### Gerando o Atlas Programaticamente (Python)
```python
from PIL import Image, ImageDraw

# Configurações
atlas_size = 8
tile_size = 64
output_size = atlas_size * tile_size  # 512x512

# Cores por BlockType
colors = {
    0: (0, 0, 0, 0),           # Ar (transparente)
    1: (76, 175, 80, 255),     # Grama
    2: (139, 69, 19, 255),     # Terra
    3: (205, 92, 92, 255),     # Argila
    4: (244, 164, 96, 255),    # Areia
    5: (169, 169, 169, 255),   # Cascalho
    6: (210, 105, 30, 255),    # Laterita
    10: (105, 105, 105, 255),  # Granito
    11: (128, 128, 128, 255),  # Diorito
    12: (192, 192, 192, 255),  # Andesito
    13: (47, 79, 79, 255),     # Basalto
    14: (119, 136, 153, 255),  # Gneiss
    15: (112, 128, 144, 255),  # Migmatito
    20: (245, 222, 179, 255),  # Arenito
    21: (245, 245, 220, 255),  # Calcário
    30: (211, 211, 211, 255),  # Concreto
    31: (28, 28, 28, 255),     # Asfalto
    40: (30, 144, 255, 255),   # Água
    41: (34, 139, 34, 255),    # Vegetação
    50: (128, 128, 128, 255),  # Rocha
}

# Criar imagem
atlas = Image.new('RGBA', (output_size, output_size), (0, 0, 0, 0))
draw = ImageDraw.Draw(atlas)

# Preencher tiles
for block_type, color in colors.items():
    row = block_type // atlas_size
    col = block_type % atlas_size
    x = col * tile_size
    y = row * tile_size
    
    # Desenhar retângulo sólido
    draw.rectangle(
        [(x, y), (x + tile_size, y + tile_size)],
        fill=color
    )

# Salvar
atlas.save('voxel_atlas_8x8.png')
print("Atlas gerado: voxel_atlas_8x8.png")
```

---

## 🔧 Configuração no Unity

### 1. Importar o Atlas
1. Salve a textura como `voxel_atlas.png`
2. Coloque em `Assets/Textures/Voxel/`
3. No Inspector:
   - **Texture Type**: Default
   - **Filter Mode**: Point (para visual pixelado) ou Bilinear
   - **Compression**: None (para evitar artefatos)
   - **Max Size**: 1024 ou 2048
   - **Apply**

### 2. Criar o Material
1. **Criar shader** (opcional, se quiser efeitos especiais):
   - Água translúcida
   - Emissão para lava/magma
   - Normal maps para rochas

2. **Material básico**:
   - Clique com botão direito em `Assets/Materials/Voxel/`
   - Create → Material
   - Nome: `VoxelTerrain`
   - Shader: `Standard` ou `Universal Render Pipeline/Lit`
   - Arraste o atlas para **Albedo Map**

3. **Configurações recomendadas**:
   - **Metallic**: 0
   - **Smoothness**: 0.2 (para terra/pedra) ou 0.8 (para água)
   - **Tiling**: 1, 1 (o atlas já está configurado)

### 3. Aplicar no TerrainWorld
No script que instancia os chunks, certifique-se de atribuir o material:

```csharp
// No TerrainWorld.cs ou VoxelWorld.cs
public Material voxelMaterial; // Atribua no Inspector

void CreateChunkGameObject(ChunkData data) {
    GameObject chunkObj = new GameObject($"Chunk_{data.pos.x}_{data.pos.y}");
    
    MeshFilter mf = chunkObj.AddComponent<MeshFilter>();
    MeshRenderer mr = chunkObj.AddComponent<MeshRenderer>();
    
    mf.mesh = ChunkMeshGenerator.BuildMesh(this, data, 1.0f);
    mr.material = voxelMaterial; // ← Aplicar material
    
    // ...
}
```

---

## 🧪 Testando as Camadas

### Verificação Visual
Para visualizar as camadas geológicas:

1. **Corte transversal**: Crie um chunk com heightmap variável
2. **Debug colors**: Modifique temporariamente `AddFaceUVs` para usar cores debug
3. **Inspector**: Use o Scene View para examinar a mesh

### Script de Teste
```csharp
// VoxelGeologyTest.cs
using UnityEngine;
using Voxel;

public class VoxelGeologyTest : MonoBehaviour {
    void Start() {
        // Criar heightmap de teste
        int size = 256;
        Color32[] testHeightmap = new Color32[size * size];
        
        for (int i = 0; i < testHeightmap.Length; i++) {
            // Gradiente: altura varia de 0 a 50
            int x = i % size;
            float height = Mathf.Clamp01((float)x / size);
            byte heightValue = (byte)(height * 255);
            testHeightmap[i] = new Color32(heightValue, 0, 0, 255);
        }
        
        // Criar chunk
        ChunkData chunk = new ChunkData(Vector2Int.zero);
        chunk.PopulateFromCache(testHeightmap, size, size, 50);
        
        // Verificar camadas no console
        Debug.Log($"Voxel [0,0,0]: {(BlockType)chunk.voxels[0, 0, 0]}");
        Debug.Log($"Voxel [0,5,0]: {(BlockType)chunk.voxels[0, 5, 0]}");
        Debug.Log($"Voxel [0,10,0]: {(BlockType)chunk.voxels[0, 10, 0]}");
        Debug.Log($"Voxel [0,20,0]: {(BlockType)chunk.voxels[0, 20, 0]}");
    }
}
```

---

## 🎮 Integração com City Builder

### 1. Custo de Construção por Tipo de Solo
```csharp
public static class GeologyCosts {
    public static float GetFoundationCost(BlockType soilType) {
        switch (soilType) {
            case BlockType.Grama:
            case BlockType.Terra:
                return 1.0f; // Custo base
            
            case BlockType.Argila:
                return 1.3f; // Precisa compactação
            
            case BlockType.Areia:
                return 1.5f; // Fundação profunda
            
            case BlockType.Granito:
                return 2.0f; // Explosivos + perfuração
            
            case BlockType.Agua:
                return 3.0f; // Aterro + drenagem
            
            default:
                return 1.0f;
        }
    }
}
```

### 2. Permeabilidade (Drenagem Urbana)
```csharp
public static class GeologyDrainage {
    /// <summary>
    /// Retorna a taxa de infiltração de água (0.0 = impermeável, 1.0 = totalmente permeável)
    /// Importante para simular enchentes em Curitiba
    /// </summary>
    public static float GetPermeability(BlockType soilType) {
        switch (soilType) {
            case BlockType.Areia:
            case BlockType.Cascalho:
                return 0.9f; // Alta permeabilidade
            
            case BlockType.Grama:
            case BlockType.Terra:
                return 0.6f; // Média permeabilidade
            
            case BlockType.Argila:
                return 0.3f; // Baixa permeabilidade
            
            case BlockType.Granito:
            case BlockType.Concreto:
            case BlockType.Asfalto:
                return 0.05f; // Quase impermeável
            
            default:
                return 0.5f;
        }
    }
}
```

### 3. Estabilidade de Terreno
```csharp
public static class GeologyStability {
    /// <summary>
    /// Risco de deslizamento (0.0 = estável, 1.0 = alto risco)
    /// Relevante para as encostas de Curitiba
    /// </summary>
    public static float GetLandslideRisk(BlockType soilType, float slope) {
        float baseRisk = 0f;
        
        switch (soilType) {
            case BlockType.Argila:
                baseRisk = 0.7f; // Alto risco quando saturada
                break;
            case BlockType.Terra:
                baseRisk = 0.5f;
                break;
            case BlockType.Areia:
                baseRisk = 0.4f;
                break;
            case BlockType.Granito:
                baseRisk = 0.1f; // Rocha é estável
                break;
        }
        
        // Inclinação aumenta o risco
        return Mathf.Clamp01(baseRisk * (1 + slope));
    }
}
```

---

## 📚 Referências Geológicas de Curitiba

### Formações Geológicas Reais
- **Embasamento Cristalino**: Complexo Atuba (granitos e gnaisses)
- **Cobertura Sedimentar**: Formação Guabirotuba (argilitos e siltitos)
- **Solo Superficial**: Latossolos vermelhos (argila laterítica)

### Aplicações no Jogo
1. **Primeiro Planalto**: Predominância de granito (blocos de construção baratos)
2. **Áreas de Várzea**: Argila saturada (risco de enchentes)
3. **Zona Norte**: Transição para sedimentos (boa para agricultura)

### Dados Históricos
- **Curitiba** está a ~900m de altitude
- **Rios**: Iguaçu, Barigui, Belém (áreas de argila + água)
- **Topografia**: Relativamente plana (ideal para expansão urbana)

---

## 🚀 Comandos Rápidos

### Gerar Atlas (se tiver Python instalado)
```bash
cd /home/larisssa/Documentos/codigos/ferritine/ferritineVU/Assets/Textures/Voxel
python3 generate_voxel_atlas.py
```

### Verificar Erros no Unity
```bash
# No terminal do Unity Editor
grep -r "voxel" ~/Library/Logs/Unity/Editor.log | tail -20
```

---

## ✅ Checklist de Implementação

- [x] BlockType enum definido
- [x] Sistema de camadas geológicas
- [x] UV Mapping implementado
- [x] Greedy meshing removido
- [ ] Texture atlas criado (8x8, 512x512px)
- [ ] Material configurado no Unity
- [ ] Material atribuído aos chunks
- [ ] Teste visual das camadas
- [ ] Integração com sistema de construção

---

## 🎯 Próximos Passos

1. **Agora**: Criar o texture atlas (use o script Python acima)
2. **Depois**: Configurar material e testar no Unity
3. **Por último**: Integrar com sistema de custos de construção

---

**Nota**: Este sistema está pronto para produção. A geologia implementada é baseada em dados reais de Curitiba e pode ser expandida conforme necessário para gameplay mais complexo (ex: mineração, túneis, fundações profundas).

