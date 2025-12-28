// ChunkPool.cs - Object pool for voxel chunks with zero GC allocation
// Manages reusable chunk GameObjects and native arrays

using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Voxel.Data;

namespace Voxel
{
    /// <summary>
    /// Object pool for chunk meshes and data.
    /// Eliminates garbage collection during gameplay.
    /// 
    /// Features:
    /// - Pre-allocated GameObjects with MeshFilter/MeshRenderer
    /// - Reusable NativeArrays for voxel data
    /// - LRU-style reclamation of distant chunks
    /// - Thread-safe access for Job System
    /// </summary>
    public class ChunkPool : MonoBehaviour
    {
        [Header("Pool Configuration")]
        [SerializeField] private int poolSize = 64;
        [SerializeField] private Material[] chunkMaterials;
        [SerializeField] private Transform poolContainer;
        
        // Pool of inactive chunk GameObjects
        private Queue<ChunkObject> _inactivePool;
        
        // Active chunks by coordinate
        private Dictionary<ChunkCoord, ChunkObject> _activeChunks;
        
        // Pool of reusable NativeArrays for voxel data
        private Queue<NativeArray<VoxelData>> _voxelDataPool;
        
        // Pool of reusable mesh data containers
        private Queue<ChunkMeshData> _meshDataPool;
        
        // Statistics
        public int ActiveCount => _activeChunks?.Count ?? 0;
        public int PooledCount => _inactivePool?.Count ?? 0;
        public int TotalCreated { get; private set; }
        
        private void Awake()
        {
            Initialize();
        }
        
        private void OnDestroy()
        {
            Cleanup();
        }
        
        /// <summary>
        /// Initialize pools with prewarm.
        /// </summary>
        public void Initialize()
        {
            _inactivePool = new Queue<ChunkObject>(poolSize);
            _activeChunks = new Dictionary<ChunkCoord, ChunkObject>(poolSize);
            _voxelDataPool = new Queue<NativeArray<VoxelData>>(poolSize);
            _meshDataPool = new Queue<ChunkMeshData>(poolSize);
            
            if (poolContainer == null)
            {
                poolContainer = new GameObject("ChunkPool").transform;
                poolContainer.SetParent(transform);
            }
            
            // Prewarm GameObjects
            for (int i = 0; i < poolSize; i++)
            {
                var chunkObj = CreateChunkObject();
                chunkObj.gameObject.SetActive(false);
                _inactivePool.Enqueue(chunkObj);
            }
            
            // Prewarm NativeArrays
            for (int i = 0; i < poolSize / 2; i++)
            {
                var voxelArray = new NativeArray<VoxelData>(
                    VoxelConstants.ChunkSizeCubed, 
                    Allocator.Persistent,
                    NativeArrayOptions.ClearMemory
                );
                _voxelDataPool.Enqueue(voxelArray);
            }
            
            Debug.Log($"[ChunkPool] Initialized with {poolSize} chunks, {_voxelDataPool.Count} voxel arrays");
        }
        
        /// <summary>
        /// Gets a chunk for the given coordinate, activating from pool or creating new.
        /// </summary>
        public ChunkObject GetChunk(ChunkCoord coord)
        {
            // Check if already active
            if (_activeChunks.TryGetValue(coord, out var existing))
            {
                existing.UpdateAccessTime();
                return existing;
            }
            
            ChunkObject chunk;
            
            // Try get from pool
            if (_inactivePool.Count > 0)
            {
                chunk = _inactivePool.Dequeue();
                chunk.gameObject.SetActive(true);
            }
            else
            {
                // Pool exhausted - create new or reclaim oldest
                if (_activeChunks.Count >= poolSize * 2)
                {
                    chunk = ReclaimOldestChunk();
                }
                else
                {
                    chunk = CreateChunkObject();
                }
            }
            
            // Configure for new coordinate
            chunk.Initialize(coord, this);
            chunk.transform.position = coord.ToWorldPosition(VoxelConstants.ChunkSizeMeters);
            Debug.Log($"Chunk {coord} posicionado em {chunk.transform.position}");
            _activeChunks[coord] = chunk;
            
            return chunk;
        }
        
        /// <summary>
        /// Returns a chunk to the pool.
        /// </summary>
        public void ReturnChunk(ChunkCoord coord)
        {
            if (!_activeChunks.TryGetValue(coord, out var chunk))
                return;
            
            _activeChunks.Remove(coord);
            
            // Clear mesh data
            chunk.ClearMesh();
            chunk.gameObject.SetActive(false);
            chunk.transform.SetParent(poolContainer);
            
            _inactivePool.Enqueue(chunk);
        }
        
        /// <summary>
        /// Returns all chunks outside the given radius from center.
        /// </summary>
        public void ReturnChunksOutsideRadius(ChunkCoord center, int radius)
        {
            var toReturn = new List<ChunkCoord>();
            
            foreach (var kvp in _activeChunks)
            {
                int dist = Mathf.Max(
                    Mathf.Abs(kvp.Key.x - center.x),
                    Mathf.Abs(kvp.Key.y - center.y),
                    Mathf.Abs(kvp.Key.z - center.z)
                );
                
                if (dist > radius)
                    toReturn.Add(kvp.Key);
            }
            
            foreach (var coord in toReturn)
                ReturnChunk(coord);
        }
        
        /// <summary>
        /// Gets a reusable NativeArray for voxel data.
        /// </summary>
        public NativeArray<VoxelData> GetVoxelArray()
        {
            if (_voxelDataPool.Count > 0)
            {
                return _voxelDataPool.Dequeue();
            }
            
            // Create new
            return new NativeArray<VoxelData>(
                VoxelConstants.ChunkSizeCubed,
                Allocator.Persistent,
                NativeArrayOptions.ClearMemory
            );
        }
        
        /// <summary>
        /// Returns a NativeArray to the pool.
        /// </summary>
        public void ReturnVoxelArray(NativeArray<VoxelData> array)
        {
            if (!array.IsCreated)
                return;
            // Ensure all jobs using this array are completed
            CompleteAndClearDependencies(array);
            // Clear for reuse
            for (int i = 0; i < array.Length; i++)
                array[i] = VoxelData.Empty;
            _voxelDataPool.Enqueue(array);
        }
        
        /// <summary>
        /// Gets a reusable ChunkMeshData container.
        /// </summary>
        public ChunkMeshData GetMeshData()
        {
            if (_meshDataPool.Count > 0)
            {
                var data = _meshDataPool.Dequeue();
                data.Clear();
                return data;
            }
            
            // Create new
            var newData = new ChunkMeshData();
            newData.Allocate(Allocator.Persistent);
            return newData;
        }
        
        /// <summary>
        /// Returns a ChunkMeshData to the pool.
        /// </summary>
        public void ReturnMeshData(ChunkMeshData data)
        {
            if (!data.IsCreated)
                return;
            
            data.Clear();
            _meshDataPool.Enqueue(data);
        }
        
        /// <summary>
        /// Check if chunk at coord is loaded.
        /// </summary>
        public bool IsChunkLoaded(ChunkCoord coord)
        {
            return _activeChunks.ContainsKey(coord);
        }
        
        /// <summary>
        /// Get chunk if loaded, null otherwise.
        /// </summary>
        public ChunkObject TryGetChunk(ChunkCoord coord)
        {
            _activeChunks.TryGetValue(coord, out var chunk);
            return chunk;
        }
        
        /// <summary>
        /// Get all active chunk coordinates.
        /// </summary>
        public IEnumerable<ChunkCoord> GetActiveCoords()
        {
            if (_activeChunks == null)
                yield break;
            foreach (var key in _activeChunks.Keys)
                yield return key;
        }
        
        private ChunkObject CreateChunkObject()
        {
            var go = new GameObject($"Chunk_{TotalCreated}");
            go.transform.SetParent(poolContainer);
            
            // Add required components
            var meshFilter = go.AddComponent<MeshFilter>();
            var meshRenderer = go.AddComponent<MeshRenderer>();
            
            // Create mesh
            meshFilter.mesh = new Mesh();
            meshFilter.mesh.MarkDynamic(); // Optimize for frequent updates
            
            // Set materials
            if (chunkMaterials != null && chunkMaterials.Length > 0)
            {
                meshRenderer.materials = chunkMaterials;
            }
            
            // Add chunk component
            var chunk = go.AddComponent<ChunkObject>();
            
            TotalCreated++;
            
            return chunk;
        }
        
        private ChunkObject ReclaimOldestChunk()
        {
            ChunkCoord oldest = default;
            long oldestTime = long.MaxValue;
            
            foreach (var kvp in _activeChunks)
            {
                if (kvp.Value.LastAccessTicks < oldestTime)
                {
                    oldestTime = kvp.Value.LastAccessTicks;
                    oldest = kvp.Key;
                }
            }
            
            var chunk = _activeChunks[oldest];
            _activeChunks.Remove(oldest);
            
            chunk.ClearMesh();
            
            return chunk;
        }
        
        private void Cleanup()
        {
            // Dispose all NativeArrays
            while (_voxelDataPool.Count > 0)
            {
                var array = _voxelDataPool.Dequeue();
                if (array.IsCreated)
                    array.Dispose();
            }
            // Dispose mesh data
            while (_meshDataPool.Count > 0)
            {
                var data = _meshDataPool.Dequeue();
                data.Dispose();
            }
            // Dispose active chunk data
            if (_activeChunks != null)
            {
                foreach (var chunk in _activeChunks.Values)
                {
                    chunk.Dispose();
                }
            }
            Debug.Log("[ChunkPool] Cleaned up all native resources");
        }
        
        /// <summary>
        /// Log pool statistics.
        /// </summary>
        public void LogStats()
        {
            Debug.Log($"[ChunkPool] Stats: Active={ActiveCount}, Pooled={PooledCount}, " +
                     $"TotalCreated={TotalCreated}, VoxelArrays={_voxelDataPool.Count}, " +
                     $"MeshData={_meshDataPool.Count}");
        }
        
        // Job dependency tracking for voxel arrays
        private static Dictionary<NativeArray<VoxelData>, List<JobHandle>> _arrayJobHandles = new Dictionary<NativeArray<VoxelData>, List<JobHandle>>();
        public static void RegisterJobDependency(NativeArray<VoxelData> array, JobHandle handle)
        {
            if (!array.IsCreated) return;
            if (!_arrayJobHandles.TryGetValue(array, out var list))
            {
                list = new List<JobHandle>();
                _arrayJobHandles[array] = list;
            }
            list.Add(handle);
        }
        private static void CompleteAndClearDependencies(NativeArray<VoxelData> array)
        {
            if (!array.IsCreated) return;
            if (_arrayJobHandles.TryGetValue(array, out var list))
            {
                foreach (var handle in list)
                {
                    handle.Complete();
                }
                list.Clear();
            }
        }
    }
    
    /// <summary>
    /// Component attached to each chunk GameObject.
    /// </summary>
    public class ChunkObject : MonoBehaviour
    {
        public ChunkCoord Coord { get; private set; }
        public ChunkMetadata Metadata { get; private set; }
        public long LastAccessTicks { get; private set; }
        public bool HasMesh { get; private set; }
        
        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private ChunkPool _pool;
        
        // Voxel data (may be null if not loaded)
        private NativeArray<VoxelData> _voxelData;
        public NativeArray<VoxelData> VoxelData => _voxelData;
        public bool HasVoxelData => _voxelData.IsCreated;
        
        public void Initialize(ChunkCoord coord, ChunkPool pool)
        {
            Coord = coord;
            _pool = pool;
            HasMesh = false;
            UpdateAccessTime();
            
            Metadata = new ChunkMetadata
            {
                coord = coord,
                version = 0,
                solidCount = 0,
                isEmpty = true,
                lodLevel = 0,
                lastAccessTicks = LastAccessTicks
            };
            
            if (_meshFilter == null)
                _meshFilter = GetComponent<MeshFilter>();
            if (_meshRenderer == null)
                _meshRenderer = GetComponent<MeshRenderer>();
        }
        
        public void SetVoxelData(NativeArray<VoxelData> data)
        {
            // Return old data if exists
            if (_voxelData.IsCreated)
            {
                _pool.ReturnVoxelArray(_voxelData);
            }
            _voxelData = data;
        }
        
        public void UpdateAccessTime()
        {
            LastAccessTicks = System.DateTime.UtcNow.Ticks;
        }
        
        public void ApplyMesh(ChunkMeshData meshData)
        {
            if (_meshFilter == null || _meshFilter.mesh == null)
                return;
            Debug.Log($"ApplyMesh: {meshData.vertices.Length} vértices para chunk {Coord}");
            var mesh = _meshFilter.mesh;
            mesh.Clear();
            
            if (meshData.vertices.Length == 0)
            {
                HasMesh = false;
                return;
            }
            
            // Apply mesh data
            mesh.SetVertices(meshData.vertices.AsArray());
            mesh.SetIndices(meshData.indices.AsArray(), MeshTopology.Triangles, 0);
            mesh.SetNormals(meshData.normals.AsArray());
            
            if (meshData.uvs.Length > 0)
                mesh.SetUVs(0, meshData.uvs.AsArray());
            
            mesh.RecalculateBounds();
            
            HasMesh = true;
            UpdateAccessTime();
        }
        
        public void ClearMesh()
        {
            if (_meshFilter != null && _meshFilter.mesh != null)
            {
                _meshFilter.mesh.Clear();
            }
            HasMesh = false;
        }
        
        public void Dispose()
        {
            if (_voxelData.IsCreated)
            {
                _pool?.ReturnVoxelArray(_voxelData);
                _voxelData = default;
            }
        }
        
        private void OnDestroy()
        {
            Dispose();
        }
    }
}
