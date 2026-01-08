#!/usr/bin/env python3
"""
Gerador de Texture Atlas para Sistema de Voxels - Geologia de Curitiba
Cria um atlas 8x8 (512x512px) com cores representando materiais geológicos.
"""

from PIL import Image, ImageDraw, ImageFont
import os

# Configurações
ATLAS_SIZE = 8          # Grade 8x8
TILE_SIZE = 64          # Cada tile 64x64px
OUTPUT_SIZE = ATLAS_SIZE * TILE_SIZE  # 512x512px total
OUTPUT_FILE = "voxel_atlas_8x8.png"

# Mapeamento BlockType → Cor (RGBA)
# Baseado em VoxelDataTypes.cs
BLOCK_COLORS = {
    # Básicos
    0: (0, 0, 0, 0),              # Ar (transparente)
    1: (76, 175, 80, 255),        # Grama (verde vibrante)
    2: (139, 69, 19, 255),        # Terra (marrom)
    3: (205, 92, 92, 255),        # Argila (vermelho terroso)
    4: (244, 164, 96, 255),       # Areia (bege claro)
    5: (169, 169, 169, 255),      # Cascalho (cinza médio)
    6: (210, 105, 30, 255),       # Laterita (laranja avermelhado)
    
    # Rochas ígneas e metamórficas (blockType 10-15)
    10: (105, 105, 105, 255),     # Granito (cinza escuro)
    11: (128, 128, 128, 255),     # Diorito (cinza médio)
    12: (192, 192, 192, 255),     # Andesito (cinza claro)
    13: (47, 79, 79, 255),        # Basalto (cinza escuro esverdeado)
    14: (119, 136, 153, 255),     # Gneiss (cinza azulado)
    15: (112, 128, 144, 255),     # Migmatito (cinza ardósia)
    
    # Sedimentares (blockType 20-21)
    20: (245, 222, 179, 255),     # Arenito (bege claro)
    21: (245, 245, 220, 255),     # Calcário (branco sujo)
    
    # Urbanos (blockType 30-31)
    30: (211, 211, 211, 255),     # Concreto (cinza claro)
    31: (28, 28, 28, 255),        # Asfalto (preto)
    
    # Água e vegetação (blockType 40-41)
    40: (30, 144, 255, 255),      # Água (azul royal)
    41: (34, 139, 34, 255),       # Vegetação (verde floresta)
    
    # Genérico (blockType 50)
    50: (128, 128, 128, 255),     # Rocha genérica (cinza)
}

# Nomes dos blocos (para debug/legenda)
BLOCK_NAMES = {
    0: "Ar", 1: "Grama", 2: "Terra", 3: "Argila", 4: "Areia", 5: "Cascalho", 6: "Laterita",
    10: "Granito", 11: "Diorito", 12: "Andesito", 13: "Basalto", 14: "Gneiss", 15: "Migmatito",
    20: "Arenito", 21: "Calcário",
    30: "Concreto", 31: "Asfalto",
    40: "Água", 41: "Vegetação",
    50: "Rocha"
}

def create_voxel_atlas():
    """Cria o texture atlas para voxels."""
    print(f"🎨 Gerando texture atlas {OUTPUT_SIZE}x{OUTPUT_SIZE}...")
    
    # Criar imagem base (com transparência)
    atlas = Image.new('RGBA', (OUTPUT_SIZE, OUTPUT_SIZE), (0, 0, 0, 0))
    draw = ImageDraw.Draw(atlas)
    
    tiles_created = 0
    
    # Preencher tiles
    for block_type, color in BLOCK_COLORS.items():
        # Calcular posição no atlas (row-major order)
        row = block_type // ATLAS_SIZE
        col = block_type % ATLAS_SIZE
        
        # Coordenadas do tile
        x = col * TILE_SIZE
        y = row * TILE_SIZE
        
        # Desenhar retângulo sólido
        draw.rectangle(
            [(x, y), (x + TILE_SIZE - 1, y + TILE_SIZE - 1)],
            fill=color
        )
        
        # Adicionar borda sutil para debug (opcional)
        if block_type != 0:  # Não bordar o ar
            border_color = tuple(max(0, c - 30) for c in color[:3]) + (255,)
            draw.rectangle(
                [(x, y), (x + TILE_SIZE - 1, y + TILE_SIZE - 1)],
                outline=border_color,
                width=1
            )
        
        tiles_created += 1
        block_name = BLOCK_NAMES.get(block_type, f"Unknown_{block_type}")
        print(f"  ✓ Tile [{row},{col}] = BlockType {block_type:2d} ({block_name})")
    
    # Salvar arquivo
    atlas.save(OUTPUT_FILE)
    print(f"\n✅ Atlas criado com sucesso: {OUTPUT_FILE}")
    print(f"📊 Tiles gerados: {tiles_created}")
    print(f"📐 Dimensões: {OUTPUT_SIZE}x{OUTPUT_SIZE}px")
    print(f"🔲 Tamanho do tile: {TILE_SIZE}x{TILE_SIZE}px")
    
    # Criar legenda
    create_legend()

def create_legend():
    """Cria uma imagem de legenda para referência."""
    print("\n📋 Gerando legenda...")
    
    # Legenda: lista vertical de blocos com nomes
    legend_width = 300
    tile_height = 40
    legend_height = len(BLOCK_COLORS) * tile_height
    
    legend = Image.new('RGBA', (legend_width, legend_height), (255, 255, 255, 255))
    draw = ImageDraw.Draw(legend)
    
    # Tentar carregar fonte (fallback para fonte padrão se não encontrar)
    try:
        font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 16)
    except:
        font = ImageFont.load_default()
    
    y = 0
    for block_type in sorted(BLOCK_COLORS.keys()):
        color = BLOCK_COLORS[block_type]
        name = BLOCK_NAMES.get(block_type, f"Block {block_type}")
        
        # Desenhar amostra de cor
        sample_size = 30
        draw.rectangle(
            [(5, y + 5), (5 + sample_size, y + 5 + sample_size)],
            fill=color,
            outline=(0, 0, 0, 255),
            width=1
        )
        
        # Desenhar texto
        text = f"[{block_type:2d}] {name}"
        text_color = (0, 0, 0, 255) if color[3] > 0 else (128, 128, 128, 255)
        draw.text((45, y + 10), text, fill=text_color, font=font)
        
        y += tile_height
    
    legend_file = "voxel_atlas_legend.png"
    legend.save(legend_file)
    print(f"✅ Legenda criada: {legend_file}")

def create_readme():
    """Cria um README com instruções de uso."""
    readme_content = f"""# Voxel Texture Atlas - Geologia de Curitiba

## Arquivos Gerados
- `{OUTPUT_FILE}` - Atlas principal (use no Unity)
- `voxel_atlas_legend.png` - Referência visual (não usar no jogo)

## Configuração no Unity

1. **Importar o atlas**:
   - Arraste `{OUTPUT_FILE}` para `Assets/Textures/Voxel/`
   - No Inspector:
     * Texture Type: Default
     * Filter Mode: Point (no-blur) ou Bilinear
     * Compression: None
     * Max Size: 1024
     * Apply

2. **Criar material**:
   - Create → Material → Nome: `VoxelTerrain`
   - Shader: Standard ou URP/Lit
   - Albedo: Arraste o atlas aqui

3. **Atribuir ao TerrainWorld**:
   - No Inspector do TerrainWorld, atribua o material criado

## Especificações Técnicas
- Atlas: {ATLAS_SIZE}x{ATLAS_SIZE} tiles
- Resolução total: {OUTPUT_SIZE}x{OUTPUT_SIZE}px
- Tamanho do tile: {TILE_SIZE}x{TILE_SIZE}px
- Blocos mapeados: {len(BLOCK_COLORS)}

## Mapeamento UV
O código em `ChunkMeshGenerator.cs` calcula automaticamente:
```
tileIndex = blockType
row = tileIndex / {ATLAS_SIZE}
col = tileIndex % {ATLAS_SIZE}
u = col / {ATLAS_SIZE}
v = row / {ATLAS_SIZE}
```

Exemplo: BlockType.Granito (10)
- row = 10 / 8 = 1
- col = 10 % 8 = 2
- Posição: linha 1, coluna 2

## Customização
Para adicionar texturas realistas:
1. Edite o atlas em um editor de imagem (GIMP/Photoshop)
2. Substitua as cores sólidas por texturas 64x64px seamless
3. Mantenha a posição dos blocos no grid 8x8
"""
    
    readme_file = "README_ATLAS.txt"
    with open(readme_file, 'w', encoding='utf-8') as f:
        f.write(readme_content)
    
    print(f"📄 README criado: {readme_file}")

if __name__ == "__main__":
    print("=" * 60)
    print("  GERADOR DE TEXTURE ATLAS - FERRITINE VOXEL SYSTEM")
    print("  Geologia de Curitiba - Primeiro Planalto Paranaense")
    print("=" * 60)
    print()
    
    create_voxel_atlas()
    create_readme()
    
    print("\n" + "=" * 60)
    print("🎉 CONCLUÍDO!")
    print(f"Importe '{OUTPUT_FILE}' no Unity e configure o material.")
    print("=" * 60)

