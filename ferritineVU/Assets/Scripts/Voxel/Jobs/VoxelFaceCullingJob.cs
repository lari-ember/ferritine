// VoxelFaceCullingJob.cs - Parallel face culling using Unity Job System
// Determines which faces of each voxel are visible (exposed to air)
// Optimized with Burst compiler for maximum performance

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Voxel.Data;

namespace Voxel.Jobs
{
    /// <summary>
    /// Job that calculates visible faces for each voxel in a chunk.
    /// Face culling: only generate geometry for faces exposed to air/transparent blocks.
    /// 
    /// Uses IJobParallelFor for parallel processing across voxels.
    /// Burst compiled for 10-50x speedup over managed C#.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    public struct VoxelFaceCullingJob : IJobParallelFor
    {
        // Input: chunk voxel data (read-only)
        [ReadOnly] public NativeArray<VoxelData> voxels;
        
        // Input: neighbor chunk data for edge voxels (6 neighbors, read-only)
        // Index: 0=-X, 1=+X, 2=-Y, 3=+Y, 4=-Z, 5=+Z
        // Can be empty if neighbor not loaded (assumes air)
        [ReadOnly] public NativeArray<VoxelData> neighborNegX;
        [ReadOnly] public NativeArray<VoxelData> neighborPosX;
        [ReadOnly] public NativeArray<VoxelData> neighborNegY;
        [ReadOnly] public NativeArray<VoxelData> neighborPosY;
        [ReadOnly] public NativeArray<VoxelData> neighborNegZ;
        [ReadOnly] public NativeArray<VoxelData> neighborPosZ;
        
        // Output: face data for each solid voxel with visible faces
        [WriteOnly] public NativeList<FaceData>.ParallelWriter facesOutput;
        
        public void Execute(int index)
        {
            // Get voxel at this index
            VoxelData voxel = voxels[index];
            
            // Skip air voxels
            if (!voxel.IsSolid)
                return;
            
            // Convert flat index to 3D coordinates
            int3 pos = VoxelConstants.IndexToCoord(index);
            
            // Check each of 6 faces for visibility
            FaceFlags visibleFaces = FaceFlags.None;
            
            // -X face (West)
            if (IsFaceVisible(pos.x - 1, pos.y, pos.z, -1, 0, 0))
                visibleFaces |= FaceFlags.NegX;
            
            // +X face (East)
            if (IsFaceVisible(pos.x + 1, pos.y, pos.z, 1, 0, 0))
                visibleFaces |= FaceFlags.PosX;
            
            // -Y face (Down)
            if (IsFaceVisible(pos.x, pos.y - 1, pos.z, 0, -1, 0))
                visibleFaces |= FaceFlags.NegY;
            
            // +Y face (Up)
            if (IsFaceVisible(pos.x, pos.y + 1, pos.z, 0, 1, 0))
                visibleFaces |= FaceFlags.PosY;
            
            // -Z face (South)
            if (IsFaceVisible(pos.x, pos.y, pos.z - 1, 0, 0, -1))
                visibleFaces |= FaceFlags.NegZ;
            
            // +Z face (North)
            if (IsFaceVisible(pos.x, pos.y, pos.z + 1, 0, 0, 1))
                visibleFaces |= FaceFlags.PosZ;
            
            // Only output if at least one face is visible
            if (visibleFaces != FaceFlags.None)
            {
                FaceData face = new FaceData
                {
                    position = pos,
                    faces = visibleFaces,
                    type = voxel.Type,
                    padding = 0
                };
                facesOutput.AddNoResize(face);
            }
        }
        
        /// <summary>
        /// Checks if a face is visible (neighbor is air/transparent).
        /// Handles edge cases with neighbor chunks.
        /// </summary>
        private bool IsFaceVisible(int nx, int ny, int nz, int dirX, int dirY, int dirZ)
        {
            // Check if neighbor is within this chunk
            if (VoxelConstants.IsInBounds(nx, ny, nz))
            {
                int neighborIndex = VoxelConstants.CoordToIndex(nx, ny, nz);
                return !voxels[neighborIndex].IsSolid || voxels[neighborIndex].IsTransparent;
            }
            
            // Neighbor is in adjacent chunk - check neighbor arrays
            return IsNeighborTransparent(nx, ny, nz, dirX, dirY, dirZ);
        }
        
        /// <summary>
        /// Checks if voxel in neighbor chunk is transparent.
        /// </summary>
        private bool IsNeighborTransparent(int nx, int ny, int nz, int dirX, int dirY, int dirZ)
        {
            // Wrap coordinates to neighbor chunk
            int wrapX = (nx + VoxelConstants.ChunkSize) % VoxelConstants.ChunkSize;
            int wrapY = (ny + VoxelConstants.ChunkSize) % VoxelConstants.ChunkSize;
            int wrapZ = (nz + VoxelConstants.ChunkSize) % VoxelConstants.ChunkSize;
            int neighborIndex = VoxelConstants.CoordToIndex(wrapX, wrapY, wrapZ);
            
            NativeArray<VoxelData> neighborArray;
            
            // Select correct neighbor array based on direction
            if (dirX < 0) neighborArray = neighborNegX;
            else if (dirX > 0) neighborArray = neighborPosX;
            else if (dirY < 0) neighborArray = neighborNegY;
            else if (dirY > 0) neighborArray = neighborPosY;
            else if (dirZ < 0) neighborArray = neighborNegZ;
            else neighborArray = neighborPosZ;
            
            // If neighbor not loaded, assume air (render face)
            if (!neighborArray.IsCreated || neighborArray.Length == 0)
                return true;
            
            VoxelData neighbor = neighborArray[neighborIndex];
            return !neighbor.IsSolid || neighbor.IsTransparent;
        }
    }
    
    /// <summary>
    /// Job to count solid voxels in a chunk (for statistics/optimization).
    /// </summary>
    [BurstCompile]
    public struct CountSolidVoxelsJob : IJob
    {
        [ReadOnly] public NativeArray<VoxelData> voxels;
        public NativeArray<int> count;
        
        public void Execute()
        {
            int total = 0;
            for (int i = 0; i < voxels.Length; i++)
            {
                if (voxels[i].IsSolid)
                    total++;
            }
            count[0] = total;
        }
    }
    
    /// <summary>
    /// Job to determine if chunk is completely empty (all air).
    /// Early exit optimization for meshing.
    /// </summary>
    [BurstCompile]
    public struct IsChunkEmptyJob : IJob
    {
        [ReadOnly] public NativeArray<VoxelData> voxels;
        public NativeArray<bool> isEmpty;
        
        public void Execute()
        {
            for (int i = 0; i < voxels.Length; i++)
            {
                if (voxels[i].IsSolid)
                {
                    isEmpty[0] = false;
                    return;
                }
            }
            isEmpty[0] = true;
        }
    }
    
    /// <summary>
    /// Job to copy chunk data with decompression from byte array.
    /// </summary>
    [BurstCompile]
    public struct DecompressChunkJob : IJob
    {
        [ReadOnly] public NativeArray<byte> compressedData;
        public NativeArray<VoxelData> voxels;
        
        public void Execute()
        {
            // Simple RLE decompression (to be replaced with zlib if needed)
            // For now, assume uncompressed 1:1 mapping
            int dataLength = math.min(compressedData.Length, voxels.Length);
            
            for (int i = 0; i < dataLength; i++)
            {
                voxels[i] = new VoxelData { typeAndFlags = compressedData[i] };
            }
            
            // Fill remaining with air
            for (int i = dataLength; i < voxels.Length; i++)
            {
                voxels[i] = VoxelData.Empty;
            }
        }
    }
}
