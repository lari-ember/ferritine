// VoxelStructs.cs - Data structures for voxel system using DOTS
// Uses structs instead of classes for stack allocation (zero GC)
// Compatible with Unity Job System and Burst Compiler

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;

namespace Voxel.Data
{
    /// <summary>
    /// Voxel type enumeration (0-255).
    /// Values synchronized with Python backend (voxel_types.py).
    /// </summary>
    public enum VoxelType : byte
    {
        // Empty
        Air = 0,
        Solid = 1,
        
        // Natural terrain (1-49)
        Grass = 10,
        Dirt = 11,
        Sand = 12,
        Rock = 13,
        Stone = 14,
        Gravel = 15,
        Clay = 16,
        Mud = 17,
        Snow = 18,
        Ice = 19,
        
        // Water and fluids (50-69)
        Water = 50,
        WaterDeep = 51,
        River = 52,
        Swamp = 53,
        
        // Vegetation (70-99)
        ForestFloor = 70,
        Farmland = 71,
        Garden = 72,
        Park = 73,
        
        // Urban (100-149)
        Asphalt = 100,
        Concrete = 101,
        Brick = 102,
        Cobblestone = 103,
        Sidewalk = 104,
        RailBed = 105,
        
        // Buildings (150-199)
        Foundation = 150,
        Wall = 151,
        Roof = 152,
        Floor = 153,
        
        // Special (200-255)
        Bedrock = 200,
        Underground = 201,
        Debug = 255
    }
    
    /// <summary>
    /// Material categories for batching (render groups).
    /// </summary>
    public enum TerrainMaterial : byte
    {
        TerrainNatural = 0,
        TerrainWater = 1,
        TerrainUrban = 2,
        TerrainVegetation = 3,
        Building = 4,
        Special = 5
    }
    
    /// <summary>
    /// Chunk coordinate (stack allocated, zero GC).
    /// Uses int3 for SIMD compatibility with Burst.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ChunkCoord : IEquatable<ChunkCoord>
    {
        public int x;
        public int y;
        public int z;
        
        public ChunkCoord(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        
        public ChunkCoord(int3 coord)
        {
            this.x = coord.x;
            this.y = coord.y;
            this.z = coord.z;
        }
        
        public int3 ToInt3() => new int3(x, y, z);
        
        public static ChunkCoord FromWorldPosition(float3 worldPos, float chunkSizeMeters)
        {
            return new ChunkCoord(
                (int)math.floor(worldPos.x / chunkSizeMeters),
                (int)math.floor(worldPos.y / chunkSizeMeters),
                (int)math.floor(worldPos.z / chunkSizeMeters)
            );
        }
        
        public float3 ToWorldPosition(float chunkSizeMeters)
        {
            return new float3(x, y, z) * chunkSizeMeters;
        }
        
        public bool Equals(ChunkCoord other) => x == other.x && y == other.y && z == other.z;
        public override bool Equals(object obj) => obj is ChunkCoord other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(x, y, z);
        public override string ToString() => $"({x}, {y}, {z})";
        
        public static bool operator ==(ChunkCoord a, ChunkCoord b) => a.Equals(b);
        public static bool operator !=(ChunkCoord a, ChunkCoord b) => !a.Equals(b);
        
        // Neighbor offsets for chunk loading
        public static readonly ChunkCoord[] NeighborOffsets = new ChunkCoord[]
        {
            new ChunkCoord(-1, 0, 0), new ChunkCoord(1, 0, 0),
            new ChunkCoord(0, -1, 0), new ChunkCoord(0, 1, 0),
            new ChunkCoord(0, 0, -1), new ChunkCoord(0, 0, 1)
        };
    }
    
    /// <summary>
    /// Single voxel data (1 byte for type + flags).
    /// Packed struct for memory efficiency.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VoxelData
    {
        public byte typeAndFlags; // Lower 7 bits = type, highest bit = flags
        
        public VoxelType Type
        {
            get => (VoxelType)(typeAndFlags & 0x7F);
            set => typeAndFlags = (byte)((typeAndFlags & 0x80) | ((byte)value & 0x7F));
        }
        
        public bool IsSolid
        {
            get => Type != VoxelType.Air && Type != VoxelType.Water && Type != VoxelType.WaterDeep;
        }
        
        public bool IsTransparent
        {
            get => Type == VoxelType.Air || Type == VoxelType.Water || Type == VoxelType.WaterDeep || Type == VoxelType.Ice;
        }
        
        public TerrainMaterial Material
        {
            get
            {
                byte t = (byte)Type;
                if (t == 0) return TerrainMaterial.TerrainNatural;
                if (t >= 10 && t < 50) return TerrainMaterial.TerrainNatural;
                if (t >= 50 && t < 70) return TerrainMaterial.TerrainWater;
                if (t >= 70 && t < 100) return TerrainMaterial.TerrainVegetation;
                if (t >= 100 && t < 150) return TerrainMaterial.TerrainUrban;
                if (t >= 150 && t < 200) return TerrainMaterial.Building;
                return TerrainMaterial.Special;
            }
        }
        
        public static VoxelData Empty => new VoxelData { typeAndFlags = 0 };
        public static VoxelData Solid => new VoxelData { typeAndFlags = (byte)VoxelType.Stone };
    }
    
    /// <summary>
    /// Face visibility flags (6 faces of a cube).
    /// Uses bit flags for compact storage.
    /// </summary>
    [Flags]
    public enum FaceFlags : byte
    {
        None = 0,
        NegX = 1 << 0,  // -X (West)
        PosX = 1 << 1,  // +X (East)
        NegY = 1 << 2,  // -Y (Down)
        PosY = 1 << 3,  // +Y (Up)
        NegZ = 1 << 4,  // -Z (South)
        PosZ = 1 << 5,  // +Z (North)
        All = NegX | PosX | NegY | PosY | NegZ | PosZ
    }
    
    /// <summary>
    /// Face data for mesh generation (12 bytes).
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FaceData
    {
        public int3 position;        // Local position in chunk
        public FaceFlags faces;      // Which faces are visible
        public VoxelType type;       // Type for material
        public byte padding;         // Alignment
        
        public int VisibleFaceCount
        {
            get
            {
                int count = 0;
                if ((faces & FaceFlags.NegX) != 0) count++;
                if ((faces & FaceFlags.PosX) != 0) count++;
                if ((faces & FaceFlags.NegY) != 0) count++;
                if ((faces & FaceFlags.PosY) != 0) count++;
                if ((faces & FaceFlags.NegZ) != 0) count++;
                if ((faces & FaceFlags.PosZ) != 0) count++;
                return count;
            }
        }
    }
    
    /// <summary>
    /// Chunk metadata for cache management.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ChunkMetadata
    {
        public ChunkCoord coord;
        public int version;
        public int solidCount;
        public bool isEmpty;
        public byte lodLevel;        // 0 = full, 1 = half, 2 = quarter
        public long lastAccessTicks; // For LRU cache
        
        public bool IsLoaded => version > 0;
    }
    
    /// <summary>
    /// Mesh data output from meshing job.
    /// </summary>
    public struct ChunkMeshData : IDisposable
    {
        public NativeList<float3> vertices;
        public NativeList<int> indices;
        public NativeList<float3> normals;
        public NativeList<float2> uvs;
        public NativeList<int> materialIndices; // Per-triangle material ID
        
        public bool IsCreated => vertices.IsCreated;
        
        public void Allocate(Allocator allocator)
        {
            vertices = new NativeList<float3>(4096, allocator);
            indices = new NativeList<int>(6144, allocator);
            normals = new NativeList<float3>(4096, allocator);
            uvs = new NativeList<float2>(4096, allocator);
            materialIndices = new NativeList<int>(2048, allocator);
        }
        
        public void Clear()
        {
            if (IsCreated)
            {
                vertices.Clear();
                indices.Clear();
                normals.Clear();
                uvs.Clear();
                materialIndices.Clear();
            }
        }
        
        public void Dispose()
        {
            if (vertices.IsCreated) vertices.Dispose();
            if (indices.IsCreated) indices.Dispose();
            if (normals.IsCreated) normals.Dispose();
            if (uvs.IsCreated) uvs.Dispose();
            if (materialIndices.IsCreated) materialIndices.Dispose();
        }
    }
    
    /// <summary>
    /// Constants for voxel system (synchronized with Python backend).
    /// </summary>
    public static class VoxelConstants
    {
        // Scale
        public const float VoxelSizeCm = 3.6f;
        public const float VoxelSizeM = 0.036f;
        
        // Chunk dimensions
        public const int ChunkSize = 64;
        public const int ChunkSizeSquared = ChunkSize * ChunkSize;
        public const int ChunkSizeCubed = ChunkSize * ChunkSize * ChunkSize;
        public const float ChunkSizeMeters = ChunkSize * VoxelSizeM; // ~2.3m
        
        // Map dimensions (~33km x 33km)
        public const float MapSideKm = 33.17f;
        public const float MapSideM = MapSideKm * 1000f;
        public const float MaxHeightM = 200f;
        public const int MaxHeightVoxels = (int)(MaxHeightM / VoxelSizeM);
        
        // LOD distances
        public const float LOD0Distance = 50f;   // Full detail
        public const float LOD1Distance = 150f;  // 50% detail
        public const float LOD2Distance = 300f;  // 25% detail
        
        // Pool sizes
        public const int MaxLoadedChunks = 256;
        public const int ChunkPoolSize = 64;
        public const int MeshPoolSize = 64;
        
        /// <summary>
        /// Convert 3D local coord to flat index.
        /// </summary>
        public static int CoordToIndex(int x, int y, int z)
        {
            return x + y * ChunkSize + z * ChunkSizeSquared;
        }
        
        /// <summary>
        /// Convert flat index to 3D local coord.
        /// </summary>
        public static int3 IndexToCoord(int index)
        {
            int z = index / ChunkSizeSquared;
            int rem = index % ChunkSizeSquared;
            int y = rem / ChunkSize;
            int x = rem % ChunkSize;
            return new int3(x, y, z);
        }
        
        /// <summary>
        /// Check if local coord is within chunk bounds.
        /// </summary>
        public static bool IsInBounds(int x, int y, int z)
        {
            return x >= 0 && x < ChunkSize && 
                   y >= 0 && y < ChunkSize && 
                   z >= 0 && z < ChunkSize;
        }
        
        /// <summary>
        /// Check if local coord is on chunk edge (needs neighbor check).
        /// </summary>
        public static bool IsOnEdge(int x, int y, int z)
        {
            return x == 0 || x == ChunkSize - 1 ||
                   y == 0 || y == ChunkSize - 1 ||
                   z == 0 || z == ChunkSize - 1;
        }
    }
}

