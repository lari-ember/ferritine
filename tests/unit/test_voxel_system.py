"""
Unit tests for Voxel System
===========================

Tests for heightmap conversion and chunk management.
"""

import pytest
import numpy as np
from pathlib import Path
import sys
import tempfile

# Add project root to path
sys.path.insert(0, str(Path(__file__).parent.parent.parent))


class TestVoxelTypes:
    """Tests for voxel type definitions."""
    
    def test_voxel_type_values(self):
        """Test that voxel types have correct values."""
        from backend.voxel.voxel_types import VoxelType
        
        assert VoxelType.AIR == 0
        assert VoxelType.GRASS == 10
        assert VoxelType.WATER == 50
        assert VoxelType.ASPHALT == 100
        assert VoxelType.BEDROCK == 200
    
    def test_terrain_material_mapping(self):
        """Test voxel to material mapping."""
        from backend.voxel.voxel_types import VoxelType, TerrainMaterial, get_material_for_voxel
        
        assert get_material_for_voxel(VoxelType.GRASS) == TerrainMaterial.TERRAIN_NATURAL
        assert get_material_for_voxel(VoxelType.WATER) == TerrainMaterial.TERRAIN_WATER
        assert get_material_for_voxel(VoxelType.ASPHALT) == TerrainMaterial.TERRAIN_URBAN
    
    def test_voxel_type_from_height(self):
        """Test height-based voxel type selection."""
        from backend.voxel.voxel_types import VoxelType, get_voxel_type_from_height
        
        # Very low = water
        assert get_voxel_type_from_height(0.05) == VoxelType.WATER
        
        # Low = sand
        assert get_voxel_type_from_height(0.12) == VoxelType.SAND
        
        # Medium = grass/dirt
        voxel_type = get_voxel_type_from_height(0.4)
        assert voxel_type in [VoxelType.GRASS, VoxelType.DIRT]
        
        # High = rock
        assert get_voxel_type_from_height(0.7) == VoxelType.ROCK
        
        # Very high = stone
        assert get_voxel_type_from_height(0.9) == VoxelType.STONE


class TestChunkData:
    """Tests for ChunkData class."""
    
    def test_chunk_creation(self):
        """Test creating empty chunk."""
        from backend.voxel.heightmap_converter import ChunkData, CHUNK_SIZE
        
        chunk = ChunkData(x=0, y=0, z=0)
        
        assert chunk.x == 0
        assert chunk.y == 0
        assert chunk.z == 0
        assert chunk.version == 1
        assert chunk.data.shape == (CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE)
        assert chunk.is_empty
        assert chunk.solid_count == 0
    
    def test_chunk_serialization(self):
        """Test chunk serialization and deserialization."""
        from backend.voxel.heightmap_converter import ChunkData
        from backend.voxel.voxel_types import VoxelType
        
        # Create chunk with some data
        chunk = ChunkData(x=5, y=10, z=2)
        chunk.data[0, 0, 0] = VoxelType.GRASS
        chunk.data[1, 1, 1] = VoxelType.STONE
        
        # Serialize
        serialized = chunk.serialize(compress=True)
        assert len(serialized) > 0
        
        # Deserialize
        restored = ChunkData.deserialize(serialized, compressed=True)
        
        assert restored.x == chunk.x
        assert restored.y == chunk.y
        assert restored.z == chunk.z
        assert np.array_equal(restored.data, chunk.data)
    
    def test_chunk_hash(self):
        """Test chunk hash generation."""
        from backend.voxel.heightmap_converter import ChunkData
        from backend.voxel.voxel_types import VoxelType
        
        chunk1 = ChunkData(x=0, y=0, z=0)
        chunk2 = ChunkData(x=0, y=0, z=0)
        
        # Same data = same hash
        assert chunk1.hash == chunk2.hash
        
        # Different data = different hash
        chunk2.data[0, 0, 0] = VoxelType.GRASS
        assert chunk1.hash != chunk2.hash


class TestHeightmapConverter:
    """Tests for HeightmapConverter."""
    
    @pytest.fixture
    def simple_heightmap(self, tmp_path):
        """Create a simple test heightmap."""
        from PIL import Image
        
        # Create a simple gradient image
        img = Image.new('L', (64, 64))
        pixels = img.load()
        for y in range(64):
            for x in range(64):
                # Gradient from 0 to 255
                pixels[x, y] = int((x + y) / 126 * 255)
        
        path = tmp_path / "test_heightmap.png"
        img.save(path)
        return str(path)
    
    def test_load_image(self, simple_heightmap):
        """Test loading heightmap image."""
        from backend.voxel.heightmap_converter import HeightmapConverter, HeightmapConfig
        
        config = HeightmapConfig(
            real_width_m=100,
            real_height_m=100
        )
        converter = HeightmapConverter(simple_heightmap, config)
        
        image = converter.load_image()
        
        assert image is not None
        assert image.shape == (64, 64)
        assert image.min() >= 0
        assert image.max() <= 1
    
    def test_height_at_world_pos(self, simple_heightmap):
        """Test getting height at world position."""
        from backend.voxel.heightmap_converter import HeightmapConverter, HeightmapConfig
        
        config = HeightmapConfig(
            real_width_m=100,
            real_height_m=100
        )
        converter = HeightmapConverter(simple_heightmap, config)
        converter.load_image()
        
        # Test corners
        h00 = converter.get_height_at_world_pos(0, 0)
        h11 = converter.get_height_at_world_pos(100, 100)
        
        # Gradient image: corner (0,0) should be darker than (max, max)
        assert h00 < h11
    
    def test_generate_single_chunk(self, simple_heightmap):
        """Test generating a single chunk."""
        from backend.voxel.heightmap_converter import HeightmapConverter, HeightmapConfig
        
        config = HeightmapConfig(
            real_width_m=100,
            real_height_m=100,
            max_altitude_m=50
        )
        converter = HeightmapConverter(simple_heightmap, config)
        
        chunk = converter.generate_chunk_at(0, 0, 0)
        
        assert chunk is not None
        assert chunk.x == 0
        assert chunk.y == 0
        assert chunk.z == 0


class TestChunkManager:
    """Tests for ChunkManager (without database)."""
    
    def test_cache_operations(self):
        """Test cache put and get."""
        from backend.voxel.chunk_manager import ChunkManager
        from backend.voxel.heightmap_converter import ChunkData
        
        manager = ChunkManager(session=None, cache_size=10)
        
        # Create and cache chunk
        chunk = ChunkData(x=1, y=2, z=3)
        manager._cache_put(chunk)
        
        # Retrieve from cache
        cached = manager._cache_get(1, 2, 3)
        assert cached is not None
        assert cached.x == 1
        assert cached.y == 2
        assert cached.z == 3
    
    def test_cache_lru_eviction(self):
        """Test LRU eviction."""
        from backend.voxel.chunk_manager import ChunkManager
        from backend.voxel.heightmap_converter import ChunkData
        
        manager = ChunkManager(session=None, cache_size=3)
        
        # Fill cache beyond capacity
        for i in range(5):
            chunk = ChunkData(x=i, y=0, z=0)
            manager._cache_put(chunk)
        
        # Should have evicted oldest
        assert manager._cache_get(0, 0, 0) is None
        assert manager._cache_get(1, 0, 0) is None
        assert manager._cache_get(2, 0, 0) is not None
        assert manager._cache_get(3, 0, 0) is not None
        assert manager._cache_get(4, 0, 0) is not None
    
    def test_cache_stats(self):
        """Test cache statistics."""
        from backend.voxel.chunk_manager import ChunkManager
        from backend.voxel.heightmap_converter import ChunkData
        
        manager = ChunkManager(session=None, cache_size=10)
        
        for i in range(5):
            chunk = ChunkData(x=i, y=0, z=0)
            manager._cache_put(chunk)
        
        stats = manager.get_cache_stats()
        
        assert stats['cached_chunks'] == 5
        assert stats['max_size'] == 10
        assert stats['memory_estimate_mb'] > 0


if __name__ == "__main__":
    pytest.main([__file__, "-v"])

