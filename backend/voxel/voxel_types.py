"""
Voxel Types and Terrain Materials
=================================

Definições de tipos de voxel e materiais de terreno.
Mapeados para valores uint8 para serialização eficiente.

Compatível com Unity - mesmos valores numéricos.
"""

from enum import IntEnum
from dataclasses import dataclass
from typing import Tuple


class VoxelType(IntEnum):
    """
    Tipos de voxel (0-255).
    0 = vazio (ar), outros = sólidos.
    
    Valores sincronizados com Unity VoxelStructs.cs
    """
    AIR = 0           # Vazio - não renderiza
    SOLID = 1         # Sólido genérico
    
    # Terrenos naturais (1-49)
    GRASS = 10
    DIRT = 11
    SAND = 12
    ROCK = 13
    STONE = 14
    GRAVEL = 15
    CLAY = 16
    MUD = 17
    SNOW = 18
    ICE = 19
    
    # Água e fluidos (50-69)
    WATER = 50
    WATER_DEEP = 51
    RIVER = 52
    SWAMP = 53
    
    # Vegetação (70-99)
    FOREST_FLOOR = 70
    FARMLAND = 71
    GARDEN = 72
    PARK = 73
    
    # Urbano (100-149)
    ASPHALT = 100
    CONCRETE = 101
    BRICK = 102
    COBBLESTONE = 103
    SIDEWALK = 104
    RAIL_BED = 105
    
    # Construções (150-199)
    FOUNDATION = 150
    WALL = 151
    ROOF = 152
    FLOOR = 153
    
    # Especiais (200-255)
    BEDROCK = 200     # Não pode ser modificado
    UNDERGROUND = 201 # Subsolo não visível
    DEBUG = 255       # Para testes


class TerrainMaterial(IntEnum):
    """
    Materiais para renderização (agrupam VoxelTypes).
    Usado para batching por material no Unity.
    """
    TERRAIN_NATURAL = 0   # Grass, dirt, rock, etc
    TERRAIN_WATER = 1     # Água e fluidos
    TERRAIN_URBAN = 2     # Asfalto, concreto
    TERRAIN_VEGETATION = 3 # Vegetação
    BUILDING = 4          # Construções
    SPECIAL = 5           # Bedrock, debug


# Mapeamento VoxelType → TerrainMaterial
VOXEL_TO_MATERIAL = {
    VoxelType.AIR: None,
    VoxelType.SOLID: TerrainMaterial.TERRAIN_NATURAL,
    
    # Naturais
    VoxelType.GRASS: TerrainMaterial.TERRAIN_NATURAL,
    VoxelType.DIRT: TerrainMaterial.TERRAIN_NATURAL,
    VoxelType.SAND: TerrainMaterial.TERRAIN_NATURAL,
    VoxelType.ROCK: TerrainMaterial.TERRAIN_NATURAL,
    VoxelType.STONE: TerrainMaterial.TERRAIN_NATURAL,
    VoxelType.GRAVEL: TerrainMaterial.TERRAIN_NATURAL,
    VoxelType.CLAY: TerrainMaterial.TERRAIN_NATURAL,
    VoxelType.MUD: TerrainMaterial.TERRAIN_NATURAL,
    VoxelType.SNOW: TerrainMaterial.TERRAIN_NATURAL,
    VoxelType.ICE: TerrainMaterial.TERRAIN_WATER,
    
    # Água
    VoxelType.WATER: TerrainMaterial.TERRAIN_WATER,
    VoxelType.WATER_DEEP: TerrainMaterial.TERRAIN_WATER,
    VoxelType.RIVER: TerrainMaterial.TERRAIN_WATER,
    VoxelType.SWAMP: TerrainMaterial.TERRAIN_WATER,
    
    # Vegetação
    VoxelType.FOREST_FLOOR: TerrainMaterial.TERRAIN_VEGETATION,
    VoxelType.FARMLAND: TerrainMaterial.TERRAIN_VEGETATION,
    VoxelType.GARDEN: TerrainMaterial.TERRAIN_VEGETATION,
    VoxelType.PARK: TerrainMaterial.TERRAIN_VEGETATION,
    
    # Urbano
    VoxelType.ASPHALT: TerrainMaterial.TERRAIN_URBAN,
    VoxelType.CONCRETE: TerrainMaterial.TERRAIN_URBAN,
    VoxelType.BRICK: TerrainMaterial.TERRAIN_URBAN,
    VoxelType.COBBLESTONE: TerrainMaterial.TERRAIN_URBAN,
    VoxelType.SIDEWALK: TerrainMaterial.TERRAIN_URBAN,
    VoxelType.RAIL_BED: TerrainMaterial.TERRAIN_URBAN,
    
    # Construções
    VoxelType.FOUNDATION: TerrainMaterial.BUILDING,
    VoxelType.WALL: TerrainMaterial.BUILDING,
    VoxelType.ROOF: TerrainMaterial.BUILDING,
    VoxelType.FLOOR: TerrainMaterial.BUILDING,
    
    # Especiais
    VoxelType.BEDROCK: TerrainMaterial.SPECIAL,
    VoxelType.UNDERGROUND: TerrainMaterial.SPECIAL,
    VoxelType.DEBUG: TerrainMaterial.SPECIAL,
}


@dataclass
class VoxelProperties:
    """Propriedades de um tipo de voxel."""
    voxel_type: VoxelType
    material: TerrainMaterial
    is_solid: bool = True
    is_transparent: bool = False
    is_walkable: bool = True
    is_buildable: bool = True
    hardness: float = 1.0  # 0-10, dificuldade de escavar
    color_hint: Tuple[int, int, int] = (128, 128, 128)  # RGB para debug


# Propriedades detalhadas por tipo
VOXEL_PROPERTIES = {
    VoxelType.AIR: VoxelProperties(
        VoxelType.AIR, None, is_solid=False, is_transparent=True,
        hardness=0, color_hint=(200, 200, 255)
    ),
    VoxelType.GRASS: VoxelProperties(
        VoxelType.GRASS, TerrainMaterial.TERRAIN_NATURAL,
        hardness=0.5, color_hint=(34, 139, 34)
    ),
    VoxelType.DIRT: VoxelProperties(
        VoxelType.DIRT, TerrainMaterial.TERRAIN_NATURAL,
        hardness=0.5, color_hint=(139, 90, 43)
    ),
    VoxelType.ROCK: VoxelProperties(
        VoxelType.ROCK, TerrainMaterial.TERRAIN_NATURAL,
        hardness=3.0, is_buildable=False, color_hint=(128, 128, 128)
    ),
    VoxelType.STONE: VoxelProperties(
        VoxelType.STONE, TerrainMaterial.TERRAIN_NATURAL,
        hardness=5.0, is_buildable=False, color_hint=(90, 90, 90)
    ),
    VoxelType.WATER: VoxelProperties(
        VoxelType.WATER, TerrainMaterial.TERRAIN_WATER,
        is_solid=False, is_transparent=True, is_walkable=False,
        is_buildable=False, hardness=0, color_hint=(30, 144, 255)
    ),
    VoxelType.WATER_DEEP: VoxelProperties(
        VoxelType.WATER_DEEP, TerrainMaterial.TERRAIN_WATER,
        is_solid=False, is_transparent=True, is_walkable=False,
        is_buildable=False, hardness=0, color_hint=(0, 100, 200)
    ),
    VoxelType.ASPHALT: VoxelProperties(
        VoxelType.ASPHALT, TerrainMaterial.TERRAIN_URBAN,
        hardness=2.0, color_hint=(50, 50, 50)
    ),
    VoxelType.CONCRETE: VoxelProperties(
        VoxelType.CONCRETE, TerrainMaterial.TERRAIN_URBAN,
        hardness=4.0, color_hint=(180, 180, 180)
    ),
    VoxelType.BEDROCK: VoxelProperties(
        VoxelType.BEDROCK, TerrainMaterial.SPECIAL,
        is_buildable=False, hardness=10.0, color_hint=(20, 20, 20)
    ),
}


def get_voxel_type_from_height(
    height_normalized: float,
    water_level: float = 0.1,
    sand_level: float = 0.15,
    grass_level: float = 0.6,
    rock_level: float = 0.8
) -> VoxelType:
    """
    Determina tipo de voxel baseado em altura normalizada (0-1).
    
    Args:
        height_normalized: Altura normalizada 0-1
        water_level: Abaixo disso é água
        sand_level: Entre water e sand é areia/praia
        grass_level: Entre sand e grass é grama/terra
        rock_level: Acima disso é rocha
    
    Returns:
        VoxelType apropriado para a altura
    """
    if height_normalized < water_level:
        return VoxelType.WATER_DEEP if height_normalized < water_level * 0.5 else VoxelType.WATER
    elif height_normalized < sand_level:
        return VoxelType.SAND
    elif height_normalized < grass_level:
        return VoxelType.GRASS if height_normalized < (grass_level + sand_level) / 2 else VoxelType.DIRT
    elif height_normalized < rock_level:
        return VoxelType.ROCK
    else:
        return VoxelType.STONE


def get_material_for_voxel(voxel_type: VoxelType) -> TerrainMaterial:
    """Retorna material de renderização para um tipo de voxel."""
    return VOXEL_TO_MATERIAL.get(voxel_type, TerrainMaterial.TERRAIN_NATURAL)

