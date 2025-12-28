// ChunkLODManager.cs - Level of Detail manager for voxel chunks
// Implements lazy loading and dynamic LOD based on camera distance

using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Voxel.Data;
using Voxel.Jobs;

namespace Voxel
{
    /// <summary>
    /// Manages chunk loading, unloading, and LOD transitions.
    /// 
    /// Features:
    /// - Lazy loading: Only load chunks when camera approaches
    /// - Distance-based LOD: Full detail near, simplified far
    /// - Async job scheduling for mesh generation
    /// - Frustum culling integration
    /// </summary>
    public class ChunkLODManager : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChunkPool chunkPool;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private Camera mainCamera;
        
        [Header("Load Distances")]
        [SerializeField] private float loadDistance = 100f;
        [SerializeField] private float unloadDistance = 150f;
        [SerializeField] private float lod0Distance = 50f;
        [SerializeField] private float lod1Distance = 100f;
        [SerializeField] private float lod2Distance = 200f;
        
        [Header("Performance")]
        [SerializeField] private int maxChunksPerFrame = 4;
        [SerializeField] private int maxMeshJobsPerFrame = 2;
        [SerializeField] private bool enableFrustumCulling = true;
        
        // Chunk loading state
        private ChunkCoord _lastCameraChunk;
        private Queue<ChunkCoord> _loadQueue;
        private Queue<ChunkCoord> _meshQueue;
        private HashSet<ChunkCoord> _pendingLoad;
        private HashSet<ChunkCoord> _pendingMesh;
        
        // Active mesh generation jobs
        private struct MeshJobHandle
        {
            public ChunkCoord coord;
            public JobHandle jobHandle;
            public ChunkMeshData meshData;
            public NativeList<FaceData> faces;
            public List<NativeArray<VoxelData>> disposableNeighbors; // Add this field
        }
        private List<MeshJobHandle> _activeMeshJobs;
        
        // Data source (to be replaced with actual loader)
        private IChunkDataProvider _dataProvider;
        
        // Frustum planes cache
        private Plane[] _frustumPlanes = new Plane[6];
        
        // Statistics
        public int LoadedChunks => chunkPool?.ActiveCount ?? 0;
        public int PendingLoads => _loadQueue?.Count ?? 0;
        public int PendingMeshes => _meshQueue?.Count ?? 0;
        public int ActiveJobs => _activeMeshJobs?.Count ?? 0;
        
        private void Awake()
        {
            _loadQueue = new Queue<ChunkCoord>(256);
            _meshQueue = new Queue<ChunkCoord>(256);
            _pendingLoad = new HashSet<ChunkCoord>();
            _pendingMesh = new HashSet<ChunkCoord>();
            _activeMeshJobs = new List<MeshJobHandle>(maxMeshJobsPerFrame * 2);
            
            if (cameraTransform == null)
                cameraTransform = Camera.main?.transform;
            if (mainCamera == null)
                mainCamera = Camera.main;
        }
        
        private void Start()
        {
            // Initialize with default data provider
            if (_dataProvider == null)
            {
                _dataProvider = new HeightmapDataProvider();
            }
            
            // Initial load around camera
            if (cameraTransform != null)
            {
                _lastCameraChunk = ChunkCoord.FromWorldPosition(
                    cameraTransform.position,
                    VoxelConstants.ChunkSizeMeters
                );
                UpdateChunkLoading();
            }
        }
        
        private void Update()
        {
            if (cameraTransform == null)
                return;
            
            // Update frustum planes for culling
            if (enableFrustumCulling && mainCamera != null)
            {
                GeometryUtility.CalculateFrustumPlanes(mainCamera, _frustumPlanes);
            }
            
            // Check if camera moved to new chunk
            ChunkCoord currentChunk = ChunkCoord.FromWorldPosition(
                cameraTransform.position,
                VoxelConstants.ChunkSizeMeters
            );
            
            if (currentChunk != _lastCameraChunk)
            {
                _lastCameraChunk = currentChunk;
                UpdateChunkLoading();
            }
            
            // Process load queue
            ProcessLoadQueue();
            
            // Process mesh generation jobs
            ProcessMeshJobs();
            
            // Check completed jobs
            CheckCompletedJobs();
            // Debug: mostrar contadores de faces
            // Debug.Log($"[Voxel Debug] Faces visíveis: {Voxel.Jobs.VoxelFaceCullingJob.debugVisibleFaceCount}");
            // Para mostrar quads gerados por chunk, use: meshData.indices.Length / 6 após job concluído
        }
        
        private void OnDestroy()
        {
            if (_activeMeshJobs != null)
            {
                foreach (var job in _activeMeshJobs)
                {
                    job.jobHandle.Complete();
                    job.meshData.Dispose();
                    if (job.faces.IsCreated) job.faces.Dispose();
                }
                _activeMeshJobs.Clear();
            }
        }
        
        /// <summary>
        /// Set the data provider for chunk loading.
        /// </summary>
        public void SetDataProvider(IChunkDataProvider provider)
        {
            _dataProvider = provider;
        }
        
        /// <summary>
        /// Force reload of all chunks around camera.
        /// </summary>
        public void ForceReload()
        {
            // Clear all
            _loadQueue.Clear();
            _meshQueue.Clear();
            _pendingLoad.Clear();
            _pendingMesh.Clear();
            
            // Unload all
            chunkPool.ReturnChunksOutsideRadius(_lastCameraChunk, 0);
            
            // Reload
            UpdateChunkLoading();
        }
        
        private void UpdateChunkLoading()
        {
            float3 cameraPos = cameraTransform.position;
            int loadRadius = Mathf.CeilToInt(loadDistance / VoxelConstants.ChunkSizeMeters);
            int unloadRadius = Mathf.CeilToInt(unloadDistance / VoxelConstants.ChunkSizeMeters);
            
            // Unload distant chunks
            chunkPool.ReturnChunksOutsideRadius(_lastCameraChunk, unloadRadius);
            
            // Queue chunks to load (spiral out from camera)
            QueueChunksInRadius(_lastCameraChunk, loadRadius);
        }
        
        private void QueueChunksInRadius(ChunkCoord center, int radius)
        {
            // Spiral out from center for prioritized loading
            for (int r = 0; r <= radius; r++)
            {
                for (int x = -r; x <= r; x++)
                {
                    for (int y = -r; y <= r; y++)
                    {
                        for (int z = -r; z <= r; z++)
                        {
                            // Only process shell at distance r
                            if (Mathf.Abs(x) != r && Mathf.Abs(y) != r && Mathf.Abs(z) != r)
                                continue;
                            
                            var coord = new ChunkCoord(center.x + x, center.y + y, center.z + z);
                            
                            // Skip if already loaded or pending
                            if (chunkPool.IsChunkLoaded(coord) || _pendingLoad.Contains(coord))
                                continue;
                            
                            // Frustum culling
                            if (enableFrustumCulling && !IsChunkInFrustum(coord))
                                continue;
                            
                            // Add to load queue
                            _loadQueue.Enqueue(coord);
                            _pendingLoad.Add(coord);
                        }
                    }
                }
            }
        }
        
        private void ProcessLoadQueue()
        {
            // Ensure all jobs are completed before modifying chunk data
            CheckCompletedJobs();
            int processed = 0;
            
            while (_loadQueue.Count > 0 && processed < maxChunksPerFrame)
            {
                var coord = _loadQueue.Dequeue();
                _pendingLoad.Remove(coord);
                
                // Skip if camera moved too far
                float dist = GetChunkDistance(coord);
                if (dist > unloadDistance)
                    continue;
                
                // Load chunk data
                var chunk = chunkPool.GetChunk(coord);
                
                if (_dataProvider != null)
                {
                    var voxelData = chunkPool.GetVoxelArray();
                    _dataProvider.LoadChunkData(coord, voxelData);
                    chunk.SetVoxelData(voxelData);
                    
                    // Queue for mesh generation
                    if (!_pendingMesh.Contains(coord))
                    {
                        _meshQueue.Enqueue(coord);
                        _pendingMesh.Add(coord);
                    }
                }
                
                processed++;
            }
        }
        
        private void ProcessMeshJobs()
        {
            // Don't start new jobs if at limit
            if (_activeMeshJobs.Count >= maxMeshJobsPerFrame)
                return;
            
            while (_meshQueue.Count > 0 && _activeMeshJobs.Count < maxMeshJobsPerFrame)
            {
                var coord = _meshQueue.Dequeue();
                _pendingMesh.Remove(coord);
                
                var chunk = chunkPool.TryGetChunk(coord);
                if (chunk == null || !chunk.HasVoxelData)
                    continue;
                
                // Determine LOD level
                float dist = GetChunkDistance(coord);
                byte lodLevel = GetLODLevel(dist);
                
                // Start mesh generation job
                StartMeshJob(coord, chunk, lodLevel);
            }
        }
        
        private void StartMeshJob(ChunkCoord coord, ChunkObject chunk, byte lodLevel)
        {
            // Allocate job data
            var faces = new NativeList<FaceData>(VoxelConstants.ChunkSizeCubed / 4, Allocator.TempJob);
            var meshData = chunkPool.GetMeshData();
            // Get neighbor data for face culling
            var neighbors = GetNeighborData(coord);
            // Track which neighbor arrays need disposal (only those created as empty)
            var disposableNeighbors = new List<NativeArray<VoxelData>>();
            for (int i = 0; i < 6; i++)
            {
                if (neighbors[i].IsCreated && neighbors[i].Length == 0)
                    disposableNeighbors.Add(neighbors[i]);
            }
            // Create face culling job
            var cullJob = new VoxelFaceCullingJob
            {
                voxels = chunk.VoxelData,
                neighborNegX = neighbors[0],
                neighborPosX = neighbors[1],
                neighborNegY = neighbors[2],
                neighborPosY = neighbors[3],
                neighborNegZ = neighbors[4],
                neighborPosZ = neighbors[5],
                facesOutput = faces.AsParallelWriter()
            };
            // Schedule face culling
            var cullHandle = cullJob.Schedule(VoxelConstants.ChunkSizeCubed, 64);
            // Register job dependencies for all arrays used in culling
            ChunkPool.RegisterJobDependency(chunk.VoxelData, cullHandle);
            for (int i = 0; i < 6; i++)
            {
                if (neighbors[i].IsCreated && neighbors[i].Length > 0)
                    ChunkPool.RegisterJobDependency(neighbors[i], cullHandle);
            }
            // Create meshing job (depends on culling)
            float3 worldPos = coord.ToWorldPosition(VoxelConstants.ChunkSizeMeters);
            var meshJob = new GreedyMeshingJob
            {
                faces = faces.AsDeferredJobArray(),
                faceCount = 0, // Will be set after culling completes
                voxels = chunk.VoxelData,
                chunkWorldOffset = worldPos,
                voxelSize = VoxelConstants.VoxelSizeM,
                vertices = meshData.vertices,
                indices = meshData.indices,
                normals = meshData.normals,
                uvs = meshData.uvs,
                materialIndices = meshData.materialIndices,
                mask = new NativeArray<bool>(VoxelConstants.ChunkSizeSquared, Allocator.TempJob)
            };
            var meshHandle = meshJob.Schedule(cullHandle);
            // Register job dependency for meshing job
            ChunkPool.RegisterJobDependency(chunk.VoxelData, meshHandle);
            // Store job handle, now with disposableNeighbors
            _activeMeshJobs.Add(new MeshJobHandle
            {
                coord = coord,
                jobHandle = meshHandle,
                meshData = meshData,
                faces = faces,
                disposableNeighbors = disposableNeighbors
            });
        }
        
        private void CheckCompletedJobs()
        {
            for (int i = _activeMeshJobs.Count - 1; i >= 0; i--)
            {
                var job = _activeMeshJobs[i];
                
                if (job.jobHandle.IsCompleted)
                {
                    // Complete job
                    job.jobHandle.Complete();
                    
                    // Apply mesh to chunk
                    var chunk = chunkPool.TryGetChunk(job.coord);
                    if (chunk != null)
                    {
                        chunk.ApplyMesh(job.meshData);
                    }
                    
                    // Cleanup
                    job.faces.Dispose();
                    chunkPool.ReturnMeshData(job.meshData);
                    
                    // Dispose any empty neighbor arrays created for this job
                    if (job.disposableNeighbors != null)
                    {
                        foreach (var arr in job.disposableNeighbors)
                        {
                            if (arr.IsCreated) arr.Dispose();
                        }
                    }
                    
                    _activeMeshJobs.RemoveAt(i);
                }
            }
        }
        
        private NativeArray<VoxelData>[] GetNeighborData(ChunkCoord coord)
        {
            var neighbors = new NativeArray<VoxelData>[6];
            for (int i = 0; i < 6; i++)
            {
                var offset = ChunkCoord.NeighborOffsets[i];
                var neighborCoord = new ChunkCoord(
                    coord.x + offset.x,
                    coord.y + offset.y,
                    coord.z + offset.z
                );
                var neighbor = chunkPool.TryGetChunk(neighborCoord);
                if (neighbor != null && neighbor.HasVoxelData)
                {
                    neighbors[i] = neighbor.VoxelData;
                }
                else
                {
                    // Always assign a valid empty NativeArray if neighbor is missing
                    neighbors[i] = new NativeArray<VoxelData>(0, Allocator.TempJob);
                }
            }
            return neighbors;
        }
        
        private float GetChunkDistance(ChunkCoord coord)
        {
            float3 chunkCenter = coord.ToWorldPosition(VoxelConstants.ChunkSizeMeters) + 
                                VoxelConstants.ChunkSizeMeters * 0.5f;
            return math.distance(chunkCenter, (float3)cameraTransform.position);
        }
        
        private byte GetLODLevel(float distance)
        {
            if (distance < lod0Distance) return 0;
            if (distance < lod1Distance) return 1;
            if (distance < lod2Distance) return 2;
            return 3;
        }
        
        private bool IsChunkInFrustum(ChunkCoord coord)
        {
            float3 min = coord.ToWorldPosition(VoxelConstants.ChunkSizeMeters);
            float3 max = min + VoxelConstants.ChunkSizeMeters;
            
            var bounds = new Bounds(
                (min + max) * 0.5f,
                (Vector3)(max - min)
            );
            
            return GeometryUtility.TestPlanesAABB(_frustumPlanes, bounds);
        }
        
        /// <summary>
        /// Debug visualization.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || chunkPool == null)
                return;
            
            // Draw loaded chunk bounds
            Gizmos.color = new Color(0, 1, 0, 0.2f);
            foreach (var coord in chunkPool.GetActiveCoords())
            {
                float3 pos = coord.ToWorldPosition(VoxelConstants.ChunkSizeMeters);
                Gizmos.DrawWireCube(
                    (Vector3)(pos + VoxelConstants.ChunkSizeMeters * 0.5f),
                    Vector3.one * VoxelConstants.ChunkSizeMeters
                );
            }
            
            // Draw load radius
            if (cameraTransform != null)
            {
                Gizmos.color = new Color(1, 1, 0, 0.3f);
                Gizmos.DrawWireSphere(cameraTransform.position, loadDistance);
            }
        }
    }
    
    /// <summary>
    /// Interface for chunk data providers.
    /// </summary>
    public interface IChunkDataProvider
    {
        void LoadChunkData(ChunkCoord coord, NativeArray<VoxelData> target);
        bool IsChunkAvailable(ChunkCoord coord);
    }
    
    /// <summary>
    /// Simple heightmap-based data provider for testing.
    /// </summary>
    public class HeightmapDataProvider : IChunkDataProvider
    {
        private Texture2D _heightmap;
        private float _maxHeight = 200f;
        
        public HeightmapDataProvider(Texture2D heightmap = null)
        {
            _heightmap = heightmap;
        }
        
        public void LoadChunkData(ChunkCoord coord, NativeArray<VoxelData> target)
        {
            int solids = 0;
            int stones = 0;
            int airs = 0;
            int waters = 0;
            for (int i = 0; i < target.Length; i++)
            {
                target[i] = new VoxelData { typeAndFlags = (byte)VoxelType.Stone };
                if (target[i].IsSolid) solids++;
                if (target[i].Type == VoxelType.Stone) stones++;
                if (target[i].Type == VoxelType.Air) airs++;
                if (target[i].Type == VoxelType.Water) waters++;
            }
            Debug.Log($"Chunk {coord}: {solids} sólidos, {stones} Stones, {airs} Airs, {waters} Waters");
        }
        
        public bool IsChunkAvailable(ChunkCoord coord)
        {
            // Always available for procedural generation
            return true;
        }
        
        private float GetHeightAtPosition(float x, float y)
        {
            if (_heightmap != null)
            {
                // Sample from texture
                float u = x / (VoxelConstants.MapSideM);
                float v = y / (VoxelConstants.MapSideM);
                Color pixel = _heightmap.GetPixelBilinear(u, v);
                return pixel.grayscale * _maxHeight;
            }
            
            // Procedural noise (Perlin)
            float scale = 0.001f;
            float noise = Mathf.PerlinNoise(x * scale, y * scale);
            noise += 0.5f * Mathf.PerlinNoise(x * scale * 2, y * scale * 2);
            noise += 0.25f * Mathf.PerlinNoise(x * scale * 4, y * scale * 4);
            return noise * 0.5f * _maxHeight;
        }
        
        private VoxelType GetSurfaceType(float normalizedHeight)
        {
            if (normalizedHeight < 0.08f) return VoxelType.Water;
            if (normalizedHeight < 0.12f) return VoxelType.Sand;
            if (normalizedHeight < 0.55f) return VoxelType.Grass;
            if (normalizedHeight < 0.75f) return VoxelType.Rock;
            return VoxelType.Stone;
        }
    }
}
