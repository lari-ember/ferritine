// HeightmapVoxelLoader.cs - Loads voxel data from heightmap texture
// Main entry point for the voxel system - attach to a GameObject in scene

using UnityEngine;
using Unity.Collections;
using Voxel.Data;

namespace Voxel
{
    /// <summary>
    /// Main controller for the voxel terrain system.
    /// Loads heightmap and manages chunk loading/rendering.
    /// 
    /// Setup:
    /// 1. Attach this script to an empty GameObject
    /// 2. Assign the cwb.png heightmap texture
    /// 3. Configure materials for each terrain type
    /// 4. Press Play
    /// </summary>
    public class HeightmapVoxelLoader : MonoBehaviour
    {
        [Header("Heightmap Source")]
        [Tooltip("Grayscale heightmap texture (cwb.png)")]
        [SerializeField] private Texture2D heightmapTexture;
        
        [Tooltip("Real-world dimensions of the heightmap in kilometers")]
        [SerializeField] private float mapSizeKm = 33.17f;
        
        [Tooltip("Maximum height of terrain in meters")]
        [SerializeField] private float maxHeightM = 200f;
        
        [Header("Terrain Levels")]
        [Range(0, 1)]
        [SerializeField] private float waterLevel = 0.08f;
        
        [Range(0, 1)]
        [SerializeField] private float sandLevel = 0.12f;
        
        [Range(0, 1)]
        [SerializeField] private float grassLevel = 0.55f;
        
        [Range(0, 1)]
        [SerializeField] private float rockLevel = 0.75f;
        
        [Header("Materials")]
        [SerializeField] private Material grassMaterial;
        [SerializeField] private Material dirtMaterial;
        [SerializeField] private Material sandMaterial;
        [SerializeField] private Material rockMaterial;
        [SerializeField] private Material stoneMaterial;
        [SerializeField] private Material waterMaterial;
        
        [Header("Components")]
        [SerializeField] private ChunkPool chunkPool;
        [SerializeField] private ChunkLODManager lodManager;
        [SerializeField] private VoxelRenderer voxelRenderer;
        
        [Header("Debug")]
        [SerializeField] private bool autoInitialize = true;
        [SerializeField] private bool logStats = true;
        
        // Data provider instance
        private HeightmapTextureDataProvider _dataProvider;
        
        // Initialization state
        public bool IsInitialized { get; private set; }
        
        private void Start()
        {
            if (autoInitialize)
            {
                Initialize();
            }
        }
        
        /// <summary>
        /// Initialize the voxel system.
        /// </summary>
        public void Initialize()
        {
            if (IsInitialized)
            {
                Debug.LogWarning("[HeightmapVoxelLoader] Already initialized");
                return;
            }
            
            // Validate heightmap
            if (heightmapTexture == null)
            {
                Debug.LogError("[HeightmapVoxelLoader] Heightmap texture not assigned!");
                return;
            }
            
            // Make heightmap readable
            if (!heightmapTexture.isReadable)
            {
                Debug.LogError("[HeightmapVoxelLoader] Heightmap texture must be readable! " +
                              "Enable 'Read/Write Enabled' in texture import settings.");
                return;
            }
            
            Debug.Log($"[HeightmapVoxelLoader] Initializing with heightmap {heightmapTexture.width}x{heightmapTexture.height}");
            
            // Create data provider
            _dataProvider = new HeightmapTextureDataProvider(
                heightmapTexture,
                mapSizeKm * 1000f,
                maxHeightM,
                waterLevel,
                sandLevel,
                grassLevel,
                rockLevel
            );
            
            // Setup components
            SetupComponents();
            
            // Set data provider
            if (lodManager != null)
            {
                lodManager.SetDataProvider(_dataProvider);
            }
            
            // Setup materials
            SetupMaterials();
            
            IsInitialized = true;
            Debug.Log("[HeightmapVoxelLoader] Initialization complete");
        }
        
        private void SetupComponents()
        {
            // Create ChunkPool if not assigned
            if (chunkPool == null)
            {
                var poolGO = new GameObject("ChunkPool");
                poolGO.transform.SetParent(transform);
                chunkPool = poolGO.AddComponent<ChunkPool>();
                chunkPool.Initialize();
            }
            
            // Create LODManager if not assigned
            if (lodManager == null)
            {
                var lodGO = new GameObject("ChunkLODManager");
                lodGO.transform.SetParent(transform);
                lodManager = lodGO.AddComponent<ChunkLODManager>();
                
                // Set references via reflection (normally would use SerializeField)
                var chunkPoolField = typeof(ChunkLODManager).GetField("chunkPool", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                chunkPoolField?.SetValue(lodManager, chunkPool);
            }
            
            // Create VoxelRenderer if not assigned
            if (voxelRenderer == null)
            {
                var rendererGO = new GameObject("VoxelRenderer");
                rendererGO.transform.SetParent(transform);
                voxelRenderer = rendererGO.AddComponent<VoxelRenderer>();
            }
        }
        
        private void SetupMaterials()
        {
            if (voxelRenderer == null)
                return;
            
            // Create default materials if not assigned
            if (grassMaterial == null)
            {
                grassMaterial = CreateDefaultMaterial(new Color(0.3f, 0.6f, 0.2f), "Grass");
            }
            if (dirtMaterial == null)
            {
                dirtMaterial = CreateDefaultMaterial(new Color(0.5f, 0.35f, 0.2f), "Dirt");
            }
            if (sandMaterial == null)
            {
                sandMaterial = CreateDefaultMaterial(new Color(0.9f, 0.85f, 0.6f), "Sand");
            }
            if (rockMaterial == null)
            {
                rockMaterial = CreateDefaultMaterial(new Color(0.5f, 0.5f, 0.5f), "Rock");
            }
            if (stoneMaterial == null)
            {
                stoneMaterial = CreateDefaultMaterial(new Color(0.4f, 0.4f, 0.4f), "Stone");
            }
            if (waterMaterial == null)
            {
                waterMaterial = CreateDefaultMaterial(new Color(0.2f, 0.4f, 0.8f, 0.7f), "Water");
            }
            
            // Apply to renderer
            voxelRenderer.SetMaterial(TerrainMaterial.TerrainNatural, grassMaterial);
            voxelRenderer.SetMaterial(TerrainMaterial.TerrainWater, waterMaterial);
            voxelRenderer.SetMaterial(TerrainMaterial.TerrainUrban, rockMaterial);
            voxelRenderer.SetMaterial(TerrainMaterial.TerrainVegetation, grassMaterial);
        }
        
        private Material CreateDefaultMaterial(Color color, string name)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
                shader = Shader.Find("Standard");
            
            Material mat = new Material(shader);
            mat.name = $"VoxelMaterial_{name}";
            mat.color = color;
            mat.enableInstancing = true;
            
            // Setup transparency for water
            if (color.a < 1f)
            {
                mat.SetFloat("_Surface", 1); // Transparent
                mat.SetFloat("_Blend", 0);   // Alpha
                mat.SetOverrideTag("RenderType", "Transparent");
                mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt("_ZWrite", 0);
                mat.renderQueue = 3000;
            }
            
            return mat;
        }
        
        private void Update()
        {
            if (!IsInitialized)
                return;
            
            // Log stats periodically
            if (logStats && Time.frameCount % 300 == 0)
            {
                LogStats();
            }
        }
        
        /// <summary>
        /// Log current statistics.
        /// </summary>
        public void LogStats()
        {
            if (chunkPool != null)
            {
                Debug.Log($"[VoxelStats] Active: {chunkPool.ActiveCount}, Pooled: {chunkPool.PooledCount}, Total: {chunkPool.TotalCreated}");
            }
            if (lodManager != null)
            {
                Debug.Log($"[VoxelStats] Loaded: {lodManager.LoadedChunks}, Pending: {lodManager.PendingLoads}, Jobs: {lodManager.ActiveJobs}");
            }
        }
        
        /// <summary>
        /// Force reload all chunks.
        /// </summary>
        [ContextMenu("Force Reload")]
        public void ForceReload()
        {
            if (lodManager != null)
            {
                lodManager.ForceReload();
            }
        }
        
        /// <summary>
        /// Get height at world position.
        /// </summary>
        public float GetHeightAt(float worldX, float worldY)
        {
            if (_dataProvider == null)
                return 0f;
            
            return _dataProvider.GetHeightAtPosition(worldX, worldY);
        }
        
        private void OnDestroy()
        {
            // Cleanup
            _dataProvider = null;
        }
    }
    
    /// <summary>
    /// Data provider that reads from Unity Texture2D.
    /// </summary>
    public class HeightmapTextureDataProvider : IChunkDataProvider
    {
        private readonly Texture2D _heightmap;
        private readonly float _mapSizeM;
        private readonly float _maxHeightM;
        private readonly float _waterLevel;
        private readonly float _sandLevel;
        private readonly float _grassLevel;
        private readonly float _rockLevel;
        
        // Cached pixel data for performance
        private Color[] _pixels;
        private int _width;
        private int _height;
        
        public HeightmapTextureDataProvider(
            Texture2D heightmap,
            float mapSizeM,
            float maxHeightM,
            float waterLevel,
            float sandLevel,
            float grassLevel,
            float rockLevel)
        {
            _heightmap = heightmap;
            _mapSizeM = mapSizeM;
            _maxHeightM = maxHeightM;
            _waterLevel = waterLevel;
            _sandLevel = sandLevel;
            _grassLevel = grassLevel;
            _rockLevel = rockLevel;
            
            // Cache pixels
            _width = heightmap.width;
            _height = heightmap.height;
            _pixels = heightmap.GetPixels();
            
            Debug.Log($"[HeightmapTextureDataProvider] Loaded {_width}x{_height} pixels");
        }
        
        public void LoadChunkData(ChunkCoord coord, NativeArray<VoxelData> target)
        {
            for (int x = 0; x < VoxelConstants.ChunkSize; x++)
            {
                for (int y = 0; y < VoxelConstants.ChunkSize; y++)
                {
                    // World position (horizontal)
                    float worldX = (coord.x * VoxelConstants.ChunkSize + x) * VoxelConstants.VoxelSizeM;
                    float worldY = (coord.y * VoxelConstants.ChunkSize + y) * VoxelConstants.VoxelSizeM;
                    
                    // Sample height
                    float heightNormalized = SampleHeight(worldX, worldY);
                    float heightM = heightNormalized * _maxHeightM;
                    int heightVoxels = Mathf.FloorToInt(heightM / VoxelConstants.VoxelSizeM);
                    
                    for (int z = 0; z < VoxelConstants.ChunkSize; z++)
                    {
                        int worldZ = coord.z * VoxelConstants.ChunkSize + z;
                        int index = VoxelConstants.CoordToIndex(x, y, z);
                        
                        VoxelType type;
                        
                        if (worldZ < heightVoxels - 3)
                        {
                            // Deep underground - stone
                            type = VoxelType.Stone;
                        }
                        else if (worldZ < heightVoxels - 1)
                        {
                            // Subsurface - dirt
                            type = VoxelType.Dirt;
                        }
                        else if (worldZ <= heightVoxels)
                        {
                            // Surface - type based on height
                            type = GetSurfaceType(heightNormalized);
                        }
                        else
                        {
                            // Above surface
                            int waterHeightVoxels = Mathf.FloorToInt(_waterLevel * _maxHeightM / VoxelConstants.VoxelSizeM);
                            
                            if (worldZ <= waterHeightVoxels && heightVoxels < waterHeightVoxels)
                            {
                                // Water
                                type = VoxelType.Water;
                            }
                            else
                            {
                                // Air
                                type = VoxelType.Air;
                            }
                        }
                        
                        target[index] = new VoxelData { typeAndFlags = (byte)type };
                    }
                }
            }
        }
        
        public bool IsChunkAvailable(ChunkCoord coord)
        {
            // Check if chunk is within map bounds
            float chunkMinX = coord.x * VoxelConstants.ChunkSizeMeters;
            float chunkMinY = coord.y * VoxelConstants.ChunkSizeMeters;
            
            return chunkMinX >= 0 && chunkMinX < _mapSizeM &&
                   chunkMinY >= 0 && chunkMinY < _mapSizeM &&
                   coord.z >= 0 && coord.z < Mathf.CeilToInt(_maxHeightM / VoxelConstants.ChunkSizeMeters);
        }
        
        public float GetHeightAtPosition(float worldX, float worldY)
        {
            return SampleHeight(worldX, worldY) * _maxHeightM;
        }
        
        private float SampleHeight(float worldX, float worldY)
        {
            // Convert world coords to UV
            float u = Mathf.Clamp01(worldX / _mapSizeM);
            float v = Mathf.Clamp01(worldY / _mapSizeM);
            
            // Convert to pixel coords
            float px = u * (_width - 1);
            float py = v * (_height - 1);
            
            // Bilinear interpolation
            int x0 = Mathf.FloorToInt(px);
            int y0 = Mathf.FloorToInt(py);
            int x1 = Mathf.Min(x0 + 1, _width - 1);
            int y1 = Mathf.Min(y0 + 1, _height - 1);
            
            float fx = px - x0;
            float fy = py - y0;
            
            float h00 = _pixels[y0 * _width + x0].grayscale;
            float h10 = _pixels[y0 * _width + x1].grayscale;
            float h01 = _pixels[y1 * _width + x0].grayscale;
            float h11 = _pixels[y1 * _width + x1].grayscale;
            
            float h0 = Mathf.Lerp(h00, h10, fx);
            float h1 = Mathf.Lerp(h01, h11, fx);
            
            return Mathf.Lerp(h0, h1, fy);
        }
        
        private VoxelType GetSurfaceType(float heightNormalized)
        {
            if (heightNormalized < _waterLevel)
            {
                return heightNormalized < _waterLevel * 0.5f ? VoxelType.WaterDeep : VoxelType.Water;
            }
            if (heightNormalized < _sandLevel)
            {
                return VoxelType.Sand;
            }
            if (heightNormalized < _grassLevel)
            {
                return heightNormalized < (_grassLevel + _sandLevel) / 2 ? VoxelType.Grass : VoxelType.Dirt;
            }
            if (heightNormalized < _rockLevel)
            {
                return VoxelType.Rock;
            }
            return VoxelType.Stone;
        }
    }
}

