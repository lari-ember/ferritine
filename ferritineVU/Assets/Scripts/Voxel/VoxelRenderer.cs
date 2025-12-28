// VoxelRenderer.cs - High-performance voxel rendering with instancing
// Uses Graphics.DrawMeshInstanced for batched rendering by material

using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using Voxel.Data;

namespace Voxel
{
    /// <summary>
    /// Renders voxel chunks with material batching and instancing.
    /// 
    /// Features:
    /// - DrawMeshInstanced for reduced draw calls
    /// - Per-material batching (terrain, water, urban, etc)
    /// - Frustum culling (automatic via Unity)
    /// - LOD-aware rendering
    /// - Shadow support
    /// </summary>
    public class VoxelRenderer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ChunkPool chunkPool;
        [SerializeField] private ChunkLODManager lodManager;
        
        [Header("Materials")]
        [SerializeField] private Material terrainNaturalMaterial;
        [SerializeField] private Material terrainWaterMaterial;
        [SerializeField] private Material terrainUrbanMaterial;
        [SerializeField] private Material terrainVegetationMaterial;
        [SerializeField] private Material buildingMaterial;
        [SerializeField] private Material specialMaterial;
        
        [Header("Rendering")]
        [SerializeField] private bool enableInstancing = true;
        [SerializeField] private bool castShadows = true;
        [SerializeField] private bool receiveShadows = true;
        [SerializeField] private int maxInstancesPerBatch = 1023; // Unity limit
        
        [Header("Debug")]
        [SerializeField] private bool showStats = true;
        [SerializeField] private bool showWireframe = false;
        
        // Material lookup
        private Dictionary<TerrainMaterial, Material> _materialMap;
        
        // Instancing batches per material
        private Dictionary<TerrainMaterial, List<Matrix4x4>> _instanceBatches;
        private Dictionary<TerrainMaterial, List<Mesh>> _meshBatches;
        
        // Property blocks for per-instance data
        private MaterialPropertyBlock _propertyBlock;
        
        // Statistics
        public int DrawCalls { get; private set; }
        public int TotalTriangles { get; private set; }
        public int VisibleChunks { get; private set; }
        
        private void Awake()
        {
            InitializeMaterials();
            _propertyBlock = new MaterialPropertyBlock();
        }
        
        private void InitializeMaterials()
        {
            _materialMap = new Dictionary<TerrainMaterial, Material>
            {
                { TerrainMaterial.TerrainNatural, terrainNaturalMaterial },
                { TerrainMaterial.TerrainWater, terrainWaterMaterial },
                { TerrainMaterial.TerrainUrban, terrainUrbanMaterial },
                { TerrainMaterial.TerrainVegetation, terrainVegetationMaterial },
                { TerrainMaterial.Building, buildingMaterial },
                { TerrainMaterial.Special, specialMaterial }
            };
            
            _instanceBatches = new Dictionary<TerrainMaterial, List<Matrix4x4>>();
            _meshBatches = new Dictionary<TerrainMaterial, List<Mesh>>();
            
            foreach (TerrainMaterial mat in System.Enum.GetValues(typeof(TerrainMaterial)))
            {
                _instanceBatches[mat] = new List<Matrix4x4>(256);
                _meshBatches[mat] = new List<Mesh>(256);
            }
            
            // Create default materials if not assigned
            CreateDefaultMaterials();
        }
        
        private void CreateDefaultMaterials()
        {
            // Create simple colored materials for testing
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");
            
            if (terrainNaturalMaterial == null)
            {
                terrainNaturalMaterial = new Material(shader);
                terrainNaturalMaterial.color = new Color(0.3f, 0.6f, 0.2f); // Green
                terrainNaturalMaterial.enableInstancing = true;
                _materialMap[TerrainMaterial.TerrainNatural] = terrainNaturalMaterial;
            }
            
            if (terrainWaterMaterial == null)
            {
                terrainWaterMaterial = new Material(shader);
                terrainWaterMaterial.color = new Color(0.2f, 0.4f, 0.8f, 0.8f); // Blue
                terrainWaterMaterial.enableInstancing = true;
                // Enable transparency
                terrainWaterMaterial.SetFloat("_Surface", 1); // Transparent
                terrainWaterMaterial.SetFloat("_Blend", 0); // Alpha
                _materialMap[TerrainMaterial.TerrainWater] = terrainWaterMaterial;
            }
            
            if (terrainUrbanMaterial == null)
            {
                terrainUrbanMaterial = new Material(shader);
                terrainUrbanMaterial.color = new Color(0.4f, 0.4f, 0.4f); // Gray
                terrainUrbanMaterial.enableInstancing = true;
                _materialMap[TerrainMaterial.TerrainUrban] = terrainUrbanMaterial;
            }
            
            if (terrainVegetationMaterial == null)
            {
                terrainVegetationMaterial = new Material(shader);
                terrainVegetationMaterial.color = new Color(0.2f, 0.5f, 0.1f); // Dark green
                terrainVegetationMaterial.enableInstancing = true;
                _materialMap[TerrainMaterial.TerrainVegetation] = terrainVegetationMaterial;
            }
            
            if (buildingMaterial == null)
            {
                buildingMaterial = new Material(shader);
                buildingMaterial.color = new Color(0.8f, 0.7f, 0.6f); // Tan
                buildingMaterial.enableInstancing = true;
                _materialMap[TerrainMaterial.Building] = buildingMaterial;
            }
            
            if (specialMaterial == null)
            {
                specialMaterial = new Material(shader);
                specialMaterial.color = new Color(0.2f, 0.2f, 0.2f); // Dark
                specialMaterial.enableInstancing = true;
                _materialMap[TerrainMaterial.Special] = specialMaterial;
            }
        }
        
        private void LateUpdate()
        {
            if (!enableInstancing)
                return;
            
            // Reset stats
            DrawCalls = 0;
            TotalTriangles = 0;
            VisibleChunks = 0;
            
            // Clear batches
            foreach (var batch in _instanceBatches.Values)
                batch.Clear();
            foreach (var batch in _meshBatches.Values)
                batch.Clear();
            
            // Collect visible chunks
            if (chunkPool != null)
            {
                foreach (var coord in chunkPool.GetActiveCoords())
                {
                    var chunk = chunkPool.TryGetChunk(coord);
                    if (chunk != null && chunk.HasMesh)
                    {
                        VisibleChunks++;
                        // Chunk meshes are already rendered by their MeshRenderers
                        // This system is for additional instanced rendering if needed
                    }
                }
            }
            
            // Render batches (for additional instanced objects)
            RenderBatches();
        }
        
        private void RenderBatches()
        {
            var shadowMode = castShadows ? ShadowCastingMode.On : ShadowCastingMode.Off;
            
            foreach (var kvp in _instanceBatches)
            {
                var material = kvp.Key;
                var matrices = kvp.Value;
                var meshes = _meshBatches[material];
                
                if (matrices.Count == 0)
                    continue;
                
                Material mat = _materialMap[material];
                if (mat == null)
                    continue;
                
                // Render in batches of maxInstancesPerBatch
                for (int i = 0; i < matrices.Count; i += maxInstancesPerBatch)
                {
                    int count = Mathf.Min(maxInstancesPerBatch, matrices.Count - i);
                    Matrix4x4[] batchMatrices = new Matrix4x4[count];
                    
                    for (int j = 0; j < count; j++)
                    {
                        batchMatrices[j] = matrices[i + j];
                    }
                    
                    // Use first mesh for instancing (assumes all same)
                    if (meshes.Count > i)
                    {
                        Graphics.DrawMeshInstanced(
                            meshes[i],
                            0,
                            mat,
                            batchMatrices,
                            count,
                            _propertyBlock,
                            shadowMode,
                            receiveShadows
                        );
                        
                        DrawCalls++;
                        TotalTriangles += meshes[i].triangles.Length / 3 * count;
                    }
                }
            }
        }
        
        /// <summary>
        /// Adds a mesh to the render batch for the given material.
        /// </summary>
        public void AddToBatch(TerrainMaterial material, Mesh mesh, Matrix4x4 matrix)
        {
            if (_instanceBatches.TryGetValue(material, out var matrices))
            {
                matrices.Add(matrix);
                _meshBatches[material].Add(mesh);
            }
        }
        
        /// <summary>
        /// Gets material for a terrain type.
        /// </summary>
        public Material GetMaterial(TerrainMaterial material)
        {
            return _materialMap.TryGetValue(material, out var mat) ? mat : terrainNaturalMaterial;
        }
        
        /// <summary>
        /// Sets material for a terrain type.
        /// </summary>
        public void SetMaterial(TerrainMaterial terrain, Material material)
        {
            _materialMap[terrain] = material;
        }
        
        private void OnGUI()
        {
            if (!showStats)
                return;
            
            GUILayout.BeginArea(new Rect(10, 10, 300, 150));
            GUILayout.BeginVertical("box");
            
            GUILayout.Label($"<b>Voxel Renderer Stats</b>");
            GUILayout.Label($"Visible Chunks: {VisibleChunks}");
            GUILayout.Label($"Draw Calls: {DrawCalls}");
            GUILayout.Label($"Total Triangles: {TotalTriangles:N0}");
            
            if (lodManager != null)
            {
                GUILayout.Label($"Loaded Chunks: {lodManager.LoadedChunks}");
                GUILayout.Label($"Pending Loads: {lodManager.PendingLoads}");
                GUILayout.Label($"Active Jobs: {lodManager.ActiveJobs}");
            }
            
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
        
        private void OnDrawGizmos()
        {
            if (!showWireframe || chunkPool == null)
                return;
            
            Gizmos.color = Color.cyan;
            
            foreach (var coord in chunkPool.GetActiveCoords())
            {
                float3 pos = coord.ToWorldPosition(VoxelConstants.ChunkSizeMeters);
                float3 center = pos + VoxelConstants.ChunkSizeMeters * 0.5f;
                
                Gizmos.DrawWireCube(
                    (Vector3)center,
                    Vector3.one * VoxelConstants.ChunkSizeMeters
                );
            }
        }
    }
}

