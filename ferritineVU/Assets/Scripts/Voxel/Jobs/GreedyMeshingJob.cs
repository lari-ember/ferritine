// GreedyMeshingJob.cs - Optimized mesh generation using greedy algorithm
// Merges adjacent coplanar faces of same material into larger quads
// Significantly reduces polygon count and draw calls

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Voxel.Data;

namespace Voxel.Jobs
{
    /// <summary>
    /// Greedy meshing algorithm for voxel chunks.
    /// Merges adjacent faces of same material into larger quads.
    /// 
    /// Algorithm:
    /// 1. For each slice (layer) in each axis direction
    /// 2. Build 2D mask of exposed faces
    /// 3. Greedily expand rectangles from each face
    /// 4. Generate quad for merged region
    /// 
    /// Results in 50-90% fewer triangles compared to naive meshing.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    public struct GreedyMeshingJob : IJob
    {
        // Input: face data from culling job
        [ReadOnly] public NativeArray<FaceData> faces;
        public int faceCount;
        
        // Input: voxel data for material lookup
        [ReadOnly] public NativeArray<VoxelData> voxels;
        
        // Chunk offset for world position
        public float3 chunkWorldOffset;
        public float voxelSize;
        
        // Output: mesh data
        public NativeList<float3> vertices;
        public NativeList<int> indices;
        public NativeList<float3> normals;
        public NativeList<float2> uvs;
        public NativeList<int> materialIndices;
        
        // Working memory (pre-allocated)
        [DeallocateOnJobCompletion]
        public NativeArray<bool> mask;
        
        public void Execute()
        {
            vertices.Clear();
            indices.Clear();
            normals.Clear();
            uvs.Clear();
            materialIndices.Clear();
            
            // Process each face direction separately
            ProcessFaceDirection(FaceFlags.PosY, new float3(0, 1, 0));  // Top faces
            ProcessFaceDirection(FaceFlags.NegY, new float3(0, -1, 0)); // Bottom faces
            ProcessFaceDirection(FaceFlags.PosX, new float3(1, 0, 0));  // East faces
            ProcessFaceDirection(FaceFlags.NegX, new float3(-1, 0, 0)); // West faces
            ProcessFaceDirection(FaceFlags.PosZ, new float3(0, 0, 1));  // North faces
            ProcessFaceDirection(FaceFlags.NegZ, new float3(0, 0, -1)); // South faces
        }
        
        private void ProcessFaceDirection(FaceFlags direction, float3 normal)
        {
            int chunkSize = VoxelConstants.ChunkSize;
            
            // Determine slice axis based on direction
            int primaryAxis = GetPrimaryAxis(normal);
            int axis1, axis2;
            GetSecondaryAxes(primaryAxis, out axis1, out axis2);
            
            // Process each slice along primary axis
            for (int slice = 0; slice < chunkSize; slice++)
            {
                // Clear mask
                for (int i = 0; i < chunkSize * chunkSize; i++)
                    mask[i] = false;
                
                // Build mask for this slice
                BuildMaskForSlice(direction, primaryAxis, axis1, axis2, slice);
                
                // Greedy merge faces in mask
                GreedyMerge(direction, normal, primaryAxis, axis1, axis2, slice);
            }
        }
        
        private void BuildMaskForSlice(FaceFlags direction, int primaryAxis, int axis1, int axis2, int slice)
        {
            int chunkSize = VoxelConstants.ChunkSize;
            
            for (int i = 0; i < faceCount; i++)
            {
                FaceData face = faces[i];
                
                // Check if this face has the direction we're processing
                if ((face.faces & direction) == 0)
                    continue;
                
                // Check if face is in this slice
                int faceSlice = GetCoordByAxis(face.position, primaryAxis);
                if (faceSlice != slice)
                    continue;
                
                // Set mask
                int u = GetCoordByAxis(face.position, axis1);
                int v = GetCoordByAxis(face.position, axis2);
                int maskIndex = u + v * chunkSize;
                mask[maskIndex] = true;
            }
        }
        
        private void GreedyMerge(FaceFlags direction, float3 normal, int primaryAxis, int axis1, int axis2, int slice)
        {
            int chunkSize = VoxelConstants.ChunkSize;
            
            for (int v = 0; v < chunkSize; v++)
            {
                for (int u = 0; u < chunkSize; )
                {
                    int maskIndex = u + v * chunkSize;
                    
                    if (!mask[maskIndex])
                    {
                        u++;
                        continue;
                    }
                    
                    // Get voxel type at this position for material
                    int3 pos = BuildPosition(primaryAxis, axis1, axis2, slice, u, v);
                    int voxelIndex = VoxelConstants.CoordToIndex(pos.x, pos.y, pos.z);
                    VoxelType voxelType = voxels[voxelIndex].Type;
                    TerrainMaterial material = voxels[voxelIndex].Material;
                    
                    // Find width (extend in u direction)
                    int width = 1;
                    while (u + width < chunkSize)
                    {
                        int checkIndex = (u + width) + v * chunkSize;
                        if (!mask[checkIndex])
                            break;
                        
                        // Check same material
                        int3 checkPos = BuildPosition(primaryAxis, axis1, axis2, slice, u + width, v);
                        int checkVoxelIndex = VoxelConstants.CoordToIndex(checkPos.x, checkPos.y, checkPos.z);
                        if (voxels[checkVoxelIndex].Type != voxelType)
                            break;
                        
                        width++;
                    }
                    
                    // Find height (extend in v direction)
                    int height = 1;
                    bool done = false;
                    while (v + height < chunkSize && !done)
                    {
                        // Check entire row
                        for (int du = 0; du < width; du++)
                        {
                            int checkIndex = (u + du) + (v + height) * chunkSize;
                            if (!mask[checkIndex])
                            {
                                done = true;
                                break;
                            }
                            
                            // Check same material
                            int3 checkPos = BuildPosition(primaryAxis, axis1, axis2, slice, u + du, v + height);
                            int checkVoxelIndex = VoxelConstants.CoordToIndex(checkPos.x, checkPos.y, checkPos.z);
                            if (voxels[checkVoxelIndex].Type != voxelType)
                            {
                                done = true;
                                break;
                            }
                        }
                        
                        if (!done)
                            height++;
                    }
                    
                    // Generate quad for merged region
                    GenerateQuad(direction, normal, primaryAxis, axis1, axis2, slice, u, v, width, height, material);
                    
                    // Clear mask for merged region
                    for (int dv = 0; dv < height; dv++)
                    {
                        for (int du = 0; du < width; du++)
                        {
                            mask[(u + du) + (v + dv) * chunkSize] = false;
                        }
                    }
                    
                    u += width;
                }
            }
        }
        
        private void GenerateQuad(FaceFlags direction, float3 normal, int primaryAxis, int axis1, int axis2,
                                 int slice, int u, int v, int width, int height, TerrainMaterial material)
        {
            // Calculate world positions of quad corners
            float faceOffset = GetFaceOffset(direction);
            
            // Base position (adjusted by face offset)
            float3 basePos = chunkWorldOffset;
            SetAxisValue(ref basePos, primaryAxis, (slice + faceOffset) * voxelSize);
            SetAxisValue(ref basePos, axis1, u * voxelSize);
            SetAxisValue(ref basePos, axis2, v * voxelSize);
            
            // Quad corners
            float3 p0 = basePos;
            float3 p1 = basePos;
            float3 p2 = basePos;
            float3 p3 = basePos;
            
            SetAxisValue(ref p1, axis1, (u + width) * voxelSize);
            SetAxisValue(ref p2, axis1, (u + width) * voxelSize);
            SetAxisValue(ref p2, axis2, (v + height) * voxelSize);
            SetAxisValue(ref p3, axis2, (v + height) * voxelSize);
            
            // Offset by chunk world position
            p0 += chunkWorldOffset;
            p1 += chunkWorldOffset;
            p2 += chunkWorldOffset;
            p3 += chunkWorldOffset;
            
            // Add vertices
            int baseVertex = vertices.Length;
            vertices.Add(p0);
            vertices.Add(p1);
            vertices.Add(p2);
            vertices.Add(p3);
            
            // Add normals
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            
            // Add UVs (scaled by quad size for texture tiling)
            uvs.Add(new float2(0, 0));
            uvs.Add(new float2(width, 0));
            uvs.Add(new float2(width, height));
            uvs.Add(new float2(0, height));
            
            // Add indices (two triangles, order depends on face direction)
            bool flipWinding = ShouldFlipWinding(direction);
            if (flipWinding)
            {
                indices.Add(baseVertex);
                indices.Add(baseVertex + 2);
                indices.Add(baseVertex + 1);
                indices.Add(baseVertex);
                indices.Add(baseVertex + 3);
                indices.Add(baseVertex + 2);
            }
            else
            {
                indices.Add(baseVertex);
                indices.Add(baseVertex + 1);
                indices.Add(baseVertex + 2);
                indices.Add(baseVertex);
                indices.Add(baseVertex + 2);
                indices.Add(baseVertex + 3);
            }
            
            // Add material index (for material separation)
            materialIndices.Add((int)material);
            materialIndices.Add((int)material);
        }
        
        // Helper methods
        
        private int GetPrimaryAxis(float3 normal)
        {
            if (math.abs(normal.x) > 0.5f) return 0;
            if (math.abs(normal.y) > 0.5f) return 1;
            return 2;
        }
        
        private void GetSecondaryAxes(int primary, out int axis1, out int axis2)
        {
            switch (primary)
            {
                case 0: axis1 = 2; axis2 = 1; break;  // X primary: Z, Y secondary
                case 1: axis1 = 0; axis2 = 2; break;  // Y primary: X, Z secondary
                default: axis1 = 0; axis2 = 1; break; // Z primary: X, Y secondary
            }
        }
        
        private int GetCoordByAxis(int3 pos, int axis)
        {
            switch (axis)
            {
                case 0: return pos.x;
                case 1: return pos.y;
                default: return pos.z;
            }
        }
        
        private int3 BuildPosition(int primaryAxis, int axis1, int axis2, int slice, int u, int v)
        {
            int3 pos = int3.zero;
            SetAxisValue(ref pos, primaryAxis, slice);
            SetAxisValue(ref pos, axis1, u);
            SetAxisValue(ref pos, axis2, v);
            return pos;
        }
        
        private void SetAxisValue(ref int3 pos, int axis, int value)
        {
            switch (axis)
            {
                case 0: pos.x = value; break;
                case 1: pos.y = value; break;
                case 2: pos.z = value; break;
            }
        }
        
        private void SetAxisValue(ref float3 pos, int axis, float value)
        {
            switch (axis)
            {
                case 0: pos.x = value; break;
                case 1: pos.y = value; break;
                case 2: pos.z = value; break;
            }
        }
        
        private float GetFaceOffset(FaceFlags direction)
        {
            // Positive faces need +1 offset, negative faces stay at 0
            switch (direction)
            {
                case FaceFlags.PosX:
                case FaceFlags.PosY:
                case FaceFlags.PosZ:
                    return 1f;
                default:
                    return 0f;
            }
        }
        
        private bool ShouldFlipWinding(FaceFlags direction)
        {
            // Negative faces need flipped winding for correct back-face culling
            return direction == FaceFlags.NegX || 
                   direction == FaceFlags.NegY || 
                   direction == FaceFlags.NegZ;
        }
    }
    
    /// <summary>
    /// Simple mesh generation job (no greedy merging).
    /// Faster but produces more triangles. Useful for LOD or debugging.
    /// </summary>
    [BurstCompile]
    public struct SimpleMeshingJob : IJob
    {
        [ReadOnly] public NativeArray<FaceData> faces;
        public int faceCount;
        
        public float3 chunkWorldOffset;
        public float voxelSize;
        
        public NativeList<float3> vertices;
        public NativeList<int> indices;
        public NativeList<float3> normals;
        
        private static readonly float3[] FaceNormals = new float3[]
        {
            new float3(-1, 0, 0), // NegX
            new float3(1, 0, 0),  // PosX
            new float3(0, -1, 0), // NegY
            new float3(0, 1, 0),  // PosY
            new float3(0, 0, -1), // NegZ
            new float3(0, 0, 1)   // PosZ
        };
        
        public void Execute()
        {
            vertices.Clear();
            indices.Clear();
            normals.Clear();
            
            for (int i = 0; i < faceCount; i++)
            {
                FaceData face = faces[i];
                float3 pos = (float3)face.position * voxelSize + chunkWorldOffset;
                
                // Generate each visible face
                if ((face.faces & FaceFlags.NegX) != 0) AddFace(pos, 0);
                if ((face.faces & FaceFlags.PosX) != 0) AddFace(pos, 1);
                if ((face.faces & FaceFlags.NegY) != 0) AddFace(pos, 2);
                if ((face.faces & FaceFlags.PosY) != 0) AddFace(pos, 3);
                if ((face.faces & FaceFlags.NegZ) != 0) AddFace(pos, 4);
                if ((face.faces & FaceFlags.PosZ) != 0) AddFace(pos, 5);
            }
        }
        
        private void AddFace(float3 pos, int faceIndex)
        {
            int baseVertex = vertices.Length;
            float3 normal = FaceNormals[faceIndex];
            float s = voxelSize;
            
            // Generate 4 vertices for quad based on face direction
            switch (faceIndex)
            {
                case 0: // -X
                    vertices.Add(pos + new float3(0, 0, 0));
                    vertices.Add(pos + new float3(0, 0, s));
                    vertices.Add(pos + new float3(0, s, s));
                    vertices.Add(pos + new float3(0, s, 0));
                    break;
                case 1: // +X
                    vertices.Add(pos + new float3(s, 0, 0));
                    vertices.Add(pos + new float3(s, s, 0));
                    vertices.Add(pos + new float3(s, s, s));
                    vertices.Add(pos + new float3(s, 0, s));
                    break;
                case 2: // -Y
                    vertices.Add(pos + new float3(0, 0, 0));
                    vertices.Add(pos + new float3(s, 0, 0));
                    vertices.Add(pos + new float3(s, 0, s));
                    vertices.Add(pos + new float3(0, 0, s));
                    break;
                case 3: // +Y
                    vertices.Add(pos + new float3(0, s, 0));
                    vertices.Add(pos + new float3(0, s, s));
                    vertices.Add(pos + new float3(s, s, s));
                    vertices.Add(pos + new float3(s, s, 0));
                    break;
                case 4: // -Z
                    vertices.Add(pos + new float3(0, 0, 0));
                    vertices.Add(pos + new float3(0, s, 0));
                    vertices.Add(pos + new float3(s, s, 0));
                    vertices.Add(pos + new float3(s, 0, 0));
                    break;
                case 5: // +Z
                    vertices.Add(pos + new float3(0, 0, s));
                    vertices.Add(pos + new float3(s, 0, s));
                    vertices.Add(pos + new float3(s, s, s));
                    vertices.Add(pos + new float3(0, s, s));
                    break;
            }
            
            // Add normals
            for (int i = 0; i < 4; i++)
                normals.Add(normal);
            
            // Add indices (two triangles)
            indices.Add(baseVertex);
            indices.Add(baseVertex + 1);
            indices.Add(baseVertex + 2);
            indices.Add(baseVertex);
            indices.Add(baseVertex + 2);
            indices.Add(baseVertex + 3);
        }
    }
}
