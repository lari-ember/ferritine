"""
Chunk Manager - Database Operations
====================================

Gerencia chunks de voxels no PostgreSQL com cache LRU.
"""

import struct
import logging
from datetime import datetime
from typing import List, Optional, Tuple, Dict
from pathlib import Path

from .heightmap_converter import ChunkData, CHUNK_SIZE, CHUNK_SIZE_METERS

logger = logging.getLogger(__name__)


class ChunkManager:
    """Gerencia chunks de voxels no banco de dados com cache LRU."""
    
    def __init__(self, session=None, cache_size: int = 256):
        self.session = session
        self.cache_size = cache_size
        self._cache: Dict[Tuple[int, int, int], ChunkData] = {}
        self._cache_order: List[Tuple[int, int, int]] = []
        logger.info(f"ChunkManager initialized (cache_size={cache_size})")
    
    def _make_key(self, x: int, y: int, z: int) -> Tuple[int, int, int]:
        return (x, y, z)
    
    def _cache_put(self, chunk: ChunkData):
        key = self._make_key(chunk.x, chunk.y, chunk.z)
        if key in self._cache:
            self._cache_order.remove(key)
        self._cache[key] = chunk
        self._cache_order.append(key)
        while len(self._cache) > self.cache_size:
            oldest = self._cache_order.pop(0)
            del self._cache[oldest]
    
    def _cache_get(self, x: int, y: int, z: int) -> Optional[ChunkData]:
        key = self._make_key(x, y, z)
        chunk = self._cache.get(key)
        if chunk:
            self._cache_order.remove(key)
            self._cache_order.append(key)
        return chunk
    
    def get_chunk(self, x: int, y: int, z: int, use_cache: bool = True) -> Optional[ChunkData]:
        if use_cache:
            cached = self._cache_get(x, y, z)
            if cached:
                return cached
        if self.session is None:
            return None
        try:
            from backend.database.models import VoxelChunk
            result = self.session.query(VoxelChunk).filter(
                VoxelChunk.chunk_x == x,
                VoxelChunk.chunk_y == y,
                VoxelChunk.chunk_z == z
            ).first()
            if result:
                chunk = ChunkData.deserialize(result.data_compressed, compressed=True)
                if use_cache:
                    self._cache_put(chunk)
                return chunk
        except Exception as e:
            logger.error(f"Error fetching chunk ({x},{y},{z}): {e}")
        return None
    
    def save_chunk(self, chunk: ChunkData, update_cache: bool = True) -> bool:
        if self.session is None:
            return False
        try:
            from backend.database.models import VoxelChunk
            existing = self.session.query(VoxelChunk).filter(
                VoxelChunk.chunk_x == chunk.x,
                VoxelChunk.chunk_y == chunk.y,
                VoxelChunk.chunk_z == chunk.z
            ).first()
            serialized = chunk.serialize(compress=True)
            if existing:
                existing.data_compressed = serialized
                existing.version += 1
                existing.data_hash = chunk.hash
                existing.solid_count = chunk.solid_count
                existing.updated_at = datetime.utcnow()
            else:
                new_chunk = VoxelChunk(
                    chunk_x=chunk.x,
                    chunk_y=chunk.y,
                    chunk_z=chunk.z,
                    data_compressed=serialized,
                    data_hash=chunk.hash,
                    solid_count=chunk.solid_count,
                    version=chunk.version
                )
                self.session.add(new_chunk)
            self.session.commit()
            if update_cache:
                self._cache_put(chunk)
            return True
        except Exception as e:
            logger.error(f"Error saving chunk: {e}")
            self.session.rollback()
            return False
    
    def save_chunks_batch(self, chunks: List[ChunkData], batch_size: int = 100) -> int:
        if self.session is None:
            return 0
        saved = 0
        try:
            from backend.database.models import VoxelChunk
            for i, chunk in enumerate(chunks):
                serialized = chunk.serialize(compress=True)
                new_chunk = VoxelChunk(
                    chunk_x=chunk.x,
                    chunk_y=chunk.y,
                    chunk_z=chunk.z,
                    data_compressed=serialized,
                    data_hash=chunk.hash,
                    solid_count=chunk.solid_count,
                    version=chunk.version
                )
                self.session.merge(new_chunk)
                saved += 1
                if (i + 1) % batch_size == 0:
                    self.session.commit()
            self.session.commit()
        except Exception as e:
            logger.error(f"Error in batch save: {e}")
            self.session.rollback()
        return saved
    
    def get_chunks_in_range(
        self,
        min_x: int, max_x: int,
        min_y: int, max_y: int,
        min_z: int, max_z: int
    ) -> List[ChunkData]:
        chunks = []
        if self.session is None:
            return chunks
        try:
            from backend.database.models import VoxelChunk
            results = self.session.query(VoxelChunk).filter(
                VoxelChunk.chunk_x >= min_x, VoxelChunk.chunk_x <= max_x,
                VoxelChunk.chunk_y >= min_y, VoxelChunk.chunk_y <= max_y,
                VoxelChunk.chunk_z >= min_z, VoxelChunk.chunk_z <= max_z
            ).all()
            for result in results:
                chunk = ChunkData.deserialize(result.data_compressed, compressed=True)
                chunks.append(chunk)
                self._cache_put(chunk)
        except Exception as e:
            logger.error(f"Error fetching chunk range: {e}")
        return chunks
    
    def get_chunks_near_position(
        self,
        world_x: float,
        world_y: float,
        world_z: float,
        radius_chunks: int = 4
    ) -> List[ChunkData]:
        cx = int(world_x / CHUNK_SIZE_METERS)
        cy = int(world_y / CHUNK_SIZE_METERS)
        cz = int(world_z / CHUNK_SIZE_METERS)
        return self.get_chunks_in_range(
            cx - radius_chunks, cx + radius_chunks,
            cy - radius_chunks, cy + radius_chunks,
            cz - radius_chunks, cz + radius_chunks
        )
    
    def delete_chunk(self, x: int, y: int, z: int) -> bool:
        if self.session is None:
            return False
        try:
            from backend.database.models import VoxelChunk
            self.session.query(VoxelChunk).filter(
                VoxelChunk.chunk_x == x,
                VoxelChunk.chunk_y == y,
                VoxelChunk.chunk_z == z
            ).delete()
            self.session.commit()
            key = self._make_key(x, y, z)
            if key in self._cache:
                del self._cache[key]
                self._cache_order.remove(key)
            return True
        except Exception as e:
            logger.error(f"Error deleting chunk: {e}")
            self.session.rollback()
            return False
    
    def get_chunk_count(self) -> int:
        if self.session is None:
            return 0
        try:
            from backend.database.models import VoxelChunk
            return self.session.query(VoxelChunk).count()
        except Exception:
            return 0
    
    def clear_cache(self):
        self._cache.clear()
        self._cache_order.clear()
    
    def get_cache_stats(self) -> dict:
        return {
            'cached_chunks': len(self._cache),
            'max_size': self.cache_size,
            'memory_estimate_mb': len(self._cache) * CHUNK_SIZE ** 3 / 1024 / 1024
        }


class ChunkFileReader:
    """Leitor de arquivo .fvox para chunks exportados."""
    
    def __init__(self, file_path: str):
        self.file_path = Path(file_path)
        self._file = None
        self._index: Dict[Tuple[int, int, int], int] = {}
        self._chunk_count = 0
        self._chunk_size = CHUNK_SIZE
    
    def open(self):
        if not self.file_path.exists():
            raise FileNotFoundError(f"File not found: {self.file_path}")
        self._file = open(self.file_path, 'rb')
        magic = self._file.read(4)
        if magic != b'FVOX':
            raise ValueError(f"Invalid file format: {magic}")
        version = struct.unpack('<I', self._file.read(4))[0]
        self._chunk_count = struct.unpack('<I', self._file.read(4))[0]
        self._chunk_size = struct.unpack('<I', self._file.read(4))[0]
        self._file.read(16)
        for _ in range(self._chunk_count):
            x, y, z = struct.unpack('<iii', self._file.read(12))
            offset = struct.unpack('<Q', self._file.read(8))[0]
            self._index[(x, y, z)] = offset
        logger.info(f"Opened {self.file_path}: {self._chunk_count} chunks")
    
    def close(self):
        if self._file:
            self._file.close()
            self._file = None
    
    def __enter__(self):
        self.open()
        return self
    
    def __exit__(self, *args):
        self.close()
    
    def get_chunk(self, x: int, y: int, z: int) -> Optional[ChunkData]:
        if self._file is None:
            raise RuntimeError("File not open")
        key = (x, y, z)
        if key not in self._index:
            return None
        offset = self._index[key]
        self._file.seek(offset)
        next_offset = min(
            (o for o in self._index.values() if o > offset),
            default=self.file_path.stat().st_size
        )
        data_size = next_offset - offset
        self._file.seek(offset)
        chunk_data = self._file.read(data_size)
        return ChunkData.deserialize(chunk_data, compressed=True)
    
    def get_all_coords(self) -> List[Tuple[int, int, int]]:
        return list(self._index.keys())
    
    @property
    def chunk_count(self) -> int:
        return self._chunk_count

