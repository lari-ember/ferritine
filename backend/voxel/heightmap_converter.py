"""
Heightmap to Voxel Converter
============================

Converte imagens heightmap grayscale em dados volumétricos de chunks.
"""

import struct
import zlib
import hashlib
import logging
from pathlib import Path
from dataclasses import dataclass, field
from typing import Tuple, Optional, Generator
import numpy as np
from PIL import Image

from .voxel_types import VoxelType, get_voxel_type_from_height

logger = logging.getLogger(__name__)

# Constantes de escala
VOXEL_SIZE_CM = 3.6
VOXEL_SIZE_M = 0.036
CHUNK_SIZE = 64
CHUNK_SIZE_METERS = CHUNK_SIZE * VOXEL_SIZE_M
MAP_AREA_KM2 = 1100
MAP_SIDE_KM = 33.17
MAP_SIDE_M = MAP_SIDE_KM * 1000
MAX_HEIGHT_M = 200
MAX_HEIGHT_VOXELS = int(MAX_HEIGHT_M / VOXEL_SIZE_M)


@dataclass
class ChunkData:
    """Dados de um chunk de voxels."""
    x: int
    y: int
    z: int
    version: int = 1
    data: np.ndarray = field(default=None)
    
    def __post_init__(self):
        if self.data is None:
            self.data = np.zeros((CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE), dtype=np.uint8)
    
    @property
    def hash(self) -> str:
        return hashlib.md5(self.data.tobytes()).hexdigest()[:16]
    
    @property
    def is_empty(self) -> bool:
        return np.all(self.data == VoxelType.AIR)
    
    @property
    def solid_count(self) -> int:
        return int(np.sum(self.data != VoxelType.AIR))
    
    def serialize(self, compress: bool = True) -> bytes:
        header = struct.pack('<iiiI', self.x, self.y, self.z, self.version)
        raw_data = self.data.tobytes()
        if compress:
            return header + zlib.compress(raw_data, level=6)
        return header + raw_data
    
    @classmethod
    def deserialize(cls, data: bytes, compressed: bool = True) -> 'ChunkData':
        x, y, z, version = struct.unpack('<iiiI', data[:16])
        raw_data = zlib.decompress(data[16:]) if compressed else data[16:]
        voxel_data = np.frombuffer(raw_data, dtype=np.uint8).reshape((CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE))
        return cls(x=x, y=y, z=z, version=version, data=voxel_data.copy())


@dataclass
class HeightmapConfig:
    """Configuração para conversão de heightmap."""
    real_width_m: float = MAP_SIDE_M
    real_height_m: float = MAP_SIDE_M
    min_altitude_m: float = 0.0
    max_altitude_m: float = MAX_HEIGHT_M
    water_level: float = 0.08
    sand_level: float = 0.12
    grass_level: float = 0.55
    rock_level: float = 0.75
    chunk_size: int = CHUNK_SIZE
    skip_empty_chunks: bool = True
    add_bedrock_layer: bool = True


class HeightmapConverter:
    """Converte heightmap PNG grayscale em chunks de voxels."""
    
    def __init__(self, heightmap_path: str, config: HeightmapConfig = None):
        self.heightmap_path = Path(heightmap_path)
        self.config = config or HeightmapConfig()
        self._image: Optional[np.ndarray] = None
        self._image_width: int = 0
        self._image_height: int = 0
        self._meters_per_pixel_x: float = 0
        self._meters_per_pixel_y: float = 0
        self.stats = {
            'chunks_generated': 0,
            'chunks_empty': 0,
            'voxels_solid': 0,
            'bytes_raw': 0,
            'bytes_compressed': 0
        }
        logger.info(f"HeightmapConverter initialized: {heightmap_path}")
    
    def load_image(self) -> np.ndarray:
        if self._image is not None:
            return self._image
        if not self.heightmap_path.exists():
            raise FileNotFoundError(f"Heightmap not found: {self.heightmap_path}")
        img = Image.open(self.heightmap_path).convert('L')
        self._image_width, self._image_height = img.size
        self._image = np.array(img, dtype=np.float32) / 255.0
        self._meters_per_pixel_x = self.config.real_width_m / self._image_width
        self._meters_per_pixel_y = self.config.real_height_m / self._image_height
        logger.info(f"Heightmap loaded: {self._image_width}x{self._image_height}")
        return self._image
    
    def get_height_at_world_pos(self, world_x: float, world_y: float) -> float:
        if self._image is None:
            self.load_image()
        px = max(0, min(int(world_x / self._meters_per_pixel_x), self._image_width - 1))
        py = max(0, min(int(world_y / self._meters_per_pixel_y), self._image_height - 1))
        return float(self._image[py, px])
    
    def get_terrain_height_voxels(self, world_x: float, world_y: float) -> int:
        h = self.get_height_at_world_pos(world_x, world_y)
        height_m = self.config.min_altitude_m + h * (self.config.max_altitude_m - self.config.min_altitude_m)
        return int(height_m / VOXEL_SIZE_M)
    
    def _generate_chunk(self, chunk_x: int, chunk_y: int, chunk_z: int) -> ChunkData:
        chunk = ChunkData(x=chunk_x, y=chunk_y, z=chunk_z)
        chunk_world_x = chunk_x * CHUNK_SIZE * VOXEL_SIZE_M
        chunk_world_y = chunk_y * CHUNK_SIZE * VOXEL_SIZE_M
        chunk_world_z = chunk_z * CHUNK_SIZE * VOXEL_SIZE_M
        
        for local_x in range(CHUNK_SIZE):
            for local_y in range(CHUNK_SIZE):
                world_x = chunk_world_x + local_x * VOXEL_SIZE_M
                world_y = chunk_world_y + local_y * VOXEL_SIZE_M
                terrain_height = self.get_terrain_height_voxels(world_x, world_y)
                height_norm = self.get_height_at_world_pos(world_x, world_y)
                
                for local_z in range(CHUNK_SIZE):
                    voxel_height = int(chunk_world_z / VOXEL_SIZE_M) + local_z
                    
                    if voxel_height < terrain_height:
                        if voxel_height == 0 and self.config.add_bedrock_layer:
                            vtype = VoxelType.BEDROCK
                        elif voxel_height < terrain_height - 3:
                            vtype = VoxelType.STONE
                        elif voxel_height < terrain_height - 1:
                            vtype = VoxelType.DIRT
                        else:
                            vtype = get_voxel_type_from_height(
                                height_norm,
                                self.config.water_level,
                                self.config.sand_level,
                                self.config.grass_level,
                                self.config.rock_level
                            )
                    elif voxel_height == terrain_height:
                        vtype = get_voxel_type_from_height(
                            height_norm,
                            self.config.water_level,
                            self.config.sand_level,
                            self.config.grass_level,
                            self.config.rock_level
                        )
                    else:
                        water_voxels = int(
                            self.config.water_level *
                            (self.config.max_altitude_m - self.config.min_altitude_m) /
                            VOXEL_SIZE_M
                        )
                        if voxel_height <= water_voxels and terrain_height < water_voxels:
                            vtype = VoxelType.WATER
                        else:
                            vtype = VoxelType.AIR
                    
                    chunk.data[local_x, local_y, local_z] = vtype
        return chunk
    
    def calculate_chunk_bounds(self) -> Tuple[int, int, int, int, int, int]:
        if self._image is None:
            self.load_image()
        cx = int(np.ceil(self.config.real_width_m / CHUNK_SIZE_METERS))
        cy = int(np.ceil(self.config.real_height_m / CHUNK_SIZE_METERS))
        cz = int(np.ceil(MAX_HEIGHT_VOXELS / CHUNK_SIZE))
        return (0, cx, 0, cy, 0, cz)
    
    def generate_chunks(self, progress_callback=None) -> Generator[ChunkData, None, None]:
        if self._image is None:
            self.load_image()
        min_x, max_x, min_y, max_y, min_z, max_z = self.calculate_chunk_bounds()
        total = (max_x - min_x) * (max_y - min_y) * (max_z - min_z)
        current = 0
        
        for cx in range(min_x, max_x):
            for cy in range(min_y, max_y):
                for cz in range(min_z, max_z):
                    current += 1
                    chunk = self._generate_chunk(cx, cy, cz)
                    
                    if self.config.skip_empty_chunks and chunk.is_empty:
                        self.stats['chunks_empty'] += 1
                        continue
                    
                    self.stats['chunks_generated'] += 1
                    self.stats['voxels_solid'] += chunk.solid_count
                    
                    raw = chunk.serialize(compress=False)
                    compressed = chunk.serialize(compress=True)
                    self.stats['bytes_raw'] += len(raw)
                    self.stats['bytes_compressed'] += len(compressed)
                    
                    if progress_callback and current % 100 == 0:
                        progress_callback(current, total)
                    
                    yield chunk
    
    def generate_chunk_at(self, chunk_x: int, chunk_y: int, chunk_z: int) -> ChunkData:
        if self._image is None:
            self.load_image()
        return self._generate_chunk(chunk_x, chunk_y, chunk_z)
    
    def export_to_file(self, output_path: str, progress_callback=None) -> dict:
        output_path = Path(output_path)
        output_path.parent.mkdir(parents=True, exist_ok=True)
        
        chunks_data = []
        for chunk in self.generate_chunks(progress_callback):
            chunks_data.append({
                'x': chunk.x,
                'y': chunk.y,
                'z': chunk.z,
                'data': chunk.serialize(compress=True)
            })
        
        with open(output_path, 'wb') as f:
            f.write(b'FVOX')
            f.write(struct.pack('<III', 1, len(chunks_data), CHUNK_SIZE))
            f.write(b'\x00' * 16)
            
            index_size = len(chunks_data) * 20
            data_offset = 32 + index_size
            current_offset = data_offset
            
            for ci in chunks_data:
                f.write(struct.pack('<iii', ci['x'], ci['y'], ci['z']))
                f.write(struct.pack('<Q', current_offset))
                current_offset += len(ci['data'])
            
            for ci in chunks_data:
                f.write(ci['data'])
        
        return {
            'file_path': str(output_path),
            'file_size_bytes': output_path.stat().st_size,
            'chunk_count': len(chunks_data),
            **self.stats
        }


def convert_heightmap(
    input_path: str,
    output_path: str,
    config: HeightmapConfig = None,
    progress_callback=None
) -> dict:
    """Função de conveniência para converter heightmap em arquivo .fvox."""
    converter = HeightmapConverter(input_path, config)
    return converter.export_to_file(output_path, progress_callback)

