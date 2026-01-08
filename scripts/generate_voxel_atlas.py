#!/usr/bin/env python3
"""
Gerador de Texture Atlas para Voxels - Ferritine
Gera um atlas 8x8 (512x512) com texturas procedurais para os materiais geológicos.
"""

from PIL import Image, ImageDraw, ImageFont
import random
import os

# Configuração
ATLAS_SIZE = 512
GRID_SIZE = 8
TILE_SIZE = ATLAS_SIZE // GRID_SIZE  # 64px

# Definição de materiais (BlockType: (R, G, B, Nome))
MATERIALS = {
    0: (0, 0, 0, 0, "Ar"),              # Transparente
    1: (124, 252, 0, 255, "Grama"),      # Verde limão
    2: (139, 69, 19, 255, "Terra"),      # Marrom
    3: (205, 92, 92, 255, "Argila"),     # Vermelho indiano
    4: (244, 164, 96, 255, "Areia"),     # Areia clara
    5: (169, 169, 169, 255, "Cascalho"), # Cinza
    6: (178, 34, 34, 255, "Laterita"),   # Vermelho tijolo
    10: (105, 105, 105, 255, "Granito"), # Cinza escuro
    11: (220, 220, 220, 255, "Diorito"), # Cinza claro
    12: (128, 128, 128, 255, "Andesito"), # Cinza médio
    13: (47, 79, 79, 255, "Basalto"),    # Cinza ardósia
    14: (119, 136, 153, 255, "Gneiss"),  # Cinza azulado
    15: (176, 196, 222, 255, "Migmatito"), # Azul aço
    20: (222, 184, 135, 255, "Arenito"), # Bege
    21: (245, 245, 220, 255, "Calcário"), # Bege claro
    30: (192, 192, 192, 255, "Concreto"), # Prata
    31: (28, 28, 28, 255, "Asfalto"),    # Preto
    40: (70, 130, 180, 200, "Água"),     # Azul (semi-transparente)
    41: (34, 139, 34, 255, "Vegetação"), # Verde floresta
    50: (105, 105, 105, 255, "Rocha"),   # Cinza
}

def add_noise(draw, x0, y0, x1, y1, base_color, intensity=20, points=200):
    """Adiciona ruído de textura para simular imperfeições naturais."""
    r, g, b, a = base_color
    
    for _ in range(points):
        px = random.randint(x0, x1-1)
        py = random.randint(y0, y1-1)
        
        noise = random.randint(-intensity, intensity)
        nr = max(0, min(255, r + noise))
        ng = max(0, min(255, g + noise))
        nb = max(0, min(255, b + noise))
        
        draw.point((px, py), fill=(nr, ng, nb, a))

def add_spots(draw, x0, y0, x1, y1, spot_color, count=10, size=3):
    """Adiciona manchas (para simular minerais, granulação, etc)."""
    for _ in range(count):
        cx = random.randint(x0 + size, x1 - size)
        cy = random.randint(y0 + size, y1 - size)
        draw.ellipse([cx-size, cy-size, cx+size, cy+size], fill=spot_color)

def add_lines(draw, x0, y0, x1, y1, line_color, count=5):
    """Adiciona linhas (para simular estratificação em rochas sedimentares)."""
    for _ in range(count):
        y = random.randint(y0, y1)
        thickness = random.randint(1, 2)
        draw.line([(x0, y), (x1, y)], fill=line_color, width=thickness)

def add_grid_pattern(draw, x0, y0, x1, y1, line_color):
    """Adiciona padrão de grade (para concreto/asfalto)."""
    spacing = 8
    for x in range(x0, x1, spacing):
        draw.line([(x, y0), (x, y1)], fill=line_color, width=1)
    for y in range(y0, y1, spacing):
        draw.line([(x0, y), (x1, y)], fill=line_color, width=1)

def generate_tile(draw, block_id, x0, y0, x1, y1):
    """Gera uma tile específica com textura procedural."""
    
    if block_id not in MATERIALS:
        # Tile vazio (rosa para debug)
        draw.rectangle([x0, y0, x1, y1], fill=(255, 0, 255, 128))
        return
    
    r, g, b, a, name = MATERIALS[block_id]
    base_color = (r, g, b, a)
    
    # Preencher com cor base
    draw.rectangle([x0, y0, x1, y1], fill=base_color)
    
    # Adicionar texturas específicas por tipo
    
    # GRAMA (1)
    if block_id == 1:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=30, points=300)
        # Adicionar pontos verdes escuros (folhas)
        for _ in range(50):
            px = random.randint(x0, x1)
            py = random.randint(y0, y1)
            draw.point((px, py), fill=(34, 139, 34, 255))
    
    # TERRA (2)
    elif block_id == 2:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=25, points=250)
        add_spots(draw, x0, y0, x1, y1, (101, 67, 33, 255), count=15, size=2)
    
    # ARGILA (3)
    elif block_id == 3:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=20, points=200)
        # Linhas de estratificação
        add_lines(draw, x0, y0, x1, y1, (178, 34, 34, 255), count=3)
    
    # AREIA (4)
    elif block_id == 4:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=15, points=400)
    
    # CASCALHO (5)
    elif block_id == 5:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=30, points=200)
        # Pedrinhas
        add_spots(draw, x0, y0, x1, y1, (128, 128, 128, 255), count=30, size=2)
        add_spots(draw, x0, y0, x1, y1, (64, 64, 64, 255), count=20, size=1)
    
    # LATERITA (6)
    elif block_id == 6:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=25, points=250)
    
    # GRANITO (10)
    elif block_id == 10:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=20, points=150)
        # Manchas de quartzo (branco) e mica (preto)
        add_spots(draw, x0, y0, x1, y1, (220, 220, 220, 255), count=20, size=2)
        add_spots(draw, x0, y0, x1, y1, (40, 40, 40, 255), count=15, size=1)
    
    # DIORITO (11)
    elif block_id == 11:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=25, points=150)
        add_spots(draw, x0, y0, x1, y1, (100, 100, 100, 255), count=25, size=2)
    
    # BASALTO (13)
    elif block_id == 13:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=15, points=100)
        # Aparência mais lisa (rocha vulcânica)
    
    # ARENITO (20)
    elif block_id == 20:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=20, points=300)
        add_lines(draw, x0, y0, x1, y1, (210, 180, 140, 255), count=4)
    
    # CALCÁRIO (21)
    elif block_id == 21:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=10, points=150)
    
    # CONCRETO (30)
    elif block_id == 30:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=15, points=100)
        add_grid_pattern(draw, x0, y0, x1, y1, (160, 160, 160, 255))
    
    # ASFALTO (31)
    elif block_id == 31:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=10, points=200)
        # Marcas de desgaste
        for _ in range(20):
            px = random.randint(x0, x1)
            py = random.randint(y0, y1)
            draw.point((px, py), fill=(50, 50, 50, 255))
    
    # ÁGUA (40)
    elif block_id == 40:
        # Efeito de ondulação (linhas horizontais variadas)
        for y in range(y0, y1, 4):
            offset = random.randint(-2, 2)
            alpha_var = random.randint(-20, 20)
            draw.line([(x0, y+offset), (x1, y+offset)], 
                     fill=(70+offset*3, 130, 180, max(100, min(255, 200+alpha_var))), 
                     width=2)
    
    # VEGETAÇÃO (41)
    elif block_id == 41:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=30, points=300)
        # Folhagem
        for _ in range(40):
            px = random.randint(x0, x1)
            py = random.randint(y0, y1)
            shade = random.randint(20, 60)
            draw.point((px, py), fill=(34+shade, 139, 34, 255))
    
    # Outros materiais
    else:
        add_noise(draw, x0, y0, x1, y1, base_color, intensity=20, points=200)

def create_atlas(output_path="VoxelAtlas.png", show_labels=False):
    """Cria o texture atlas completo."""
    
    print(f"Criando texture atlas {ATLAS_SIZE}x{ATLAS_SIZE}...")
    
    # Criar imagem com alpha
    atlas = Image.new('RGBA', (ATLAS_SIZE, ATLAS_SIZE), (0, 0, 0, 0))
    draw = ImageDraw.Draw(atlas)
    
    # Gerar cada tile
    for block_id in MATERIALS.keys():
        row = block_id // GRID_SIZE
        col = block_id % GRID_SIZE
        
        x0 = col * TILE_SIZE
        y0 = row * TILE_SIZE
        x1 = x0 + TILE_SIZE
        y1 = y0 + TILE_SIZE
        
        print(f"  Gerando tile {block_id}: {MATERIALS[block_id][4]} ({row},{col})")
        generate_tile(draw, block_id, x0, y0, x1, y1)
        
        # Adicionar labels (debug)
        if show_labels:
            try:
                font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 10)
            except:
                font = ImageFont.load_default()
            
            text = f"{block_id}\n{MATERIALS[block_id][4]}"
            draw.text((x0+2, y0+2), text, fill=(255, 255, 255, 255), font=font)
    
    # Salvar
    atlas.save(output_path)
    print(f"✅ Atlas salvo: {output_path}")
    print(f"   Resolução: {ATLAS_SIZE}x{ATLAS_SIZE}")
    print(f"   Grid: {GRID_SIZE}x{GRID_SIZE}")
    print(f"   Tile size: {TILE_SIZE}x{TILE_SIZE}")
    print(f"   Total materiais: {len(MATERIALS)}")

def create_reference_image(output_path="VoxelAtlas_Reference.png"):
    """Cria imagem de referência com labels para documentação."""
    print("\nCriando imagem de referência...")
    
    # Tamanho maior para incluir labels
    ref_width = ATLAS_SIZE + 200
    ref_height = ATLAS_SIZE + 50
    
    ref = Image.new('RGB', (ref_width, ref_height), (255, 255, 255))
    draw = ImageDraw.Draw(ref)
    
    # Copiar atlas
    atlas = Image.open("VoxelAtlas.png").convert('RGB')
    ref.paste(atlas, (0, 0))
    
    # Adicionar grid
    for i in range(GRID_SIZE + 1):
        x = i * TILE_SIZE
        draw.line([(x, 0), (x, ATLAS_SIZE)], fill=(0, 0, 0), width=2)
        y = i * TILE_SIZE
        draw.line([(0, y), (ATLAS_SIZE, y)], fill=(0, 0, 0), width=2)
    
    # Adicionar legenda
    try:
        font = ImageFont.truetype("/usr/share/fonts/truetype/dejavu/DejaVuSans.ttf", 12)
    except:
        font = ImageFont.load_default()
    
    y_offset = 10
    draw.text((ATLAS_SIZE + 10, y_offset), "MATERIAIS:", fill=(0, 0, 0), font=font)
    y_offset += 20
    
    for block_id in sorted(MATERIALS.keys()):
        r, g, b, a, name = MATERIALS[block_id]
        # Quadradinho de cor
        draw.rectangle([ATLAS_SIZE + 10, y_offset, ATLAS_SIZE + 25, y_offset + 10], 
                      fill=(r, g, b))
        # Nome
        text = f"{block_id:2d}: {name}"
        draw.text((ATLAS_SIZE + 30, y_offset), text, fill=(0, 0, 0), font=font)
        y_offset += 15
    
    ref.save(output_path)
    print(f"✅ Referência salva: {output_path}")

if __name__ == "__main__":
    import sys
    
    # Opções de linha de comando
    show_labels = "--labels" in sys.argv
    
    # Criar atlas
    create_atlas("VoxelAtlas.png", show_labels=show_labels)
    
    # Criar referência
    create_reference_image("VoxelAtlas_Reference.png")
    
    print("\n🎨 Texture Atlas criado com sucesso!")
    print("\n📋 Próximos passos:")
    print("   1. Copiar VoxelAtlas.png para: Assets/Textures/")
    print("   2. Configurar import settings no Unity (Point filter, 512x512)")
    print("   3. Criar material e aplicar o atlas")
    print("   4. Testar no terreno")

