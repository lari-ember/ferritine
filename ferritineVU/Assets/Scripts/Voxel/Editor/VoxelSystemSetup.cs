// VoxelSystemSetup.cs - Editor utility for setting up the voxel system
// Provides menu items and wizards for easy configuration

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Voxel.Editor
{
    /// <summary>
    /// Editor menu and setup utilities for the Voxel System.
    /// </summary>
    public static class VoxelSystemSetup
    {
        private const string MENU_PATH = "Ferritine/Voxel System/";
        
        /// <summary>
        /// Creates a complete voxel terrain setup in the scene.
        /// </summary>
        [MenuItem(MENU_PATH + "Create Voxel Terrain", false, 0)]
        public static void CreateVoxelTerrain()
        {
            // Check if already exists
            var existing = Object.FindFirstObjectByType<HeightmapVoxelLoader>();
            if (existing != null)
            {
                if (!EditorUtility.DisplayDialog(
                    "Voxel Terrain Exists",
                    "A voxel terrain already exists in the scene. Create another?",
                    "Yes", "No"))
                {
                    Selection.activeGameObject = existing.gameObject;
                    return;
                }
            }
            
            // Create root object
            var root = new GameObject("VoxelTerrain");
            Undo.RegisterCreatedObjectUndo(root, "Create Voxel Terrain");
            
            // Add main component
            var loader = root.AddComponent<HeightmapVoxelLoader>();
            
            // Try to find heightmap texture
            var heightmapPath = "Assets/Sprites/nap/cwb.png";
            var heightmap = AssetDatabase.LoadAssetAtPath<Texture2D>(heightmapPath);
            
            if (heightmap != null)
            {
                // Use reflection to set private field (since it's serialized)
                var field = typeof(HeightmapVoxelLoader).GetField("heightmapTexture", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                field?.SetValue(loader, heightmap);
                
                Debug.Log($"[VoxelSetup] Found heightmap at {heightmapPath}");
            }
            else
            {
                Debug.LogWarning($"[VoxelSetup] Heightmap not found at {heightmapPath}. Please assign manually.");
            }
            
            // Select the new object
            Selection.activeGameObject = root;
            
            EditorUtility.DisplayDialog(
                "Voxel Terrain Created",
                "Voxel terrain has been created.\n\n" +
                "1. Assign the heightmap texture (cwb.png) if not auto-detected\n" +
                "2. Configure terrain levels (water, sand, grass, rock)\n" +
                "3. Press Play to see the terrain generate",
                "OK");
        }
        
        /// <summary>
        /// Checks if required packages are installed.
        /// </summary>
        [MenuItem(MENU_PATH + "Check Dependencies", false, 100)]
        public static void CheckDependencies()
        {
            var missingPackages = new System.Collections.Generic.List<string>();
            
            // Check for Burst
            if (!IsPackageInstalled("com.unity.burst"))
                missingPackages.Add("Burst (com.unity.burst)");
            
            // Check for Mathematics
            if (!IsPackageInstalled("com.unity.mathematics"))
                missingPackages.Add("Mathematics (com.unity.mathematics)");
            
            // Check for Collections
            if (!IsPackageInstalled("com.unity.collections"))
                missingPackages.Add("Collections (com.unity.collections)");
            
            if (missingPackages.Count > 0)
            {
                EditorUtility.DisplayDialog(
                    "Missing Packages",
                    "The following packages are required:\n\n" +
                    string.Join("\n", missingPackages) +
                    "\n\nInstall via Package Manager (Window > Package Manager)",
                    "OK");
            }
            else
            {
                EditorUtility.DisplayDialog(
                    "All Dependencies OK",
                    "All required packages are installed!\n\n" +
                    "✓ Burst\n" +
                    "✓ Mathematics\n" +
                    "✓ Collections",
                    "OK");
            }
        }
        
        /// <summary>
        /// Opens the voxel system documentation.
        /// </summary>
        [MenuItem(MENU_PATH + "Open Documentation", false, 200)]
        public static void OpenDocumentation()
        {
            var readmePath = "Assets/Scripts/Voxel/README.md";
            var asset = AssetDatabase.LoadAssetAtPath<TextAsset>(readmePath);
            
            if (asset != null)
            {
                AssetDatabase.OpenAsset(asset);
            }
            else
            {
                Debug.LogWarning($"Documentation not found at {readmePath}");
            }
        }
        
        /// <summary>
        /// Configures the heightmap texture import settings.
        /// </summary>
        [MenuItem(MENU_PATH + "Configure Heightmap Texture", false, 101)]
        public static void ConfigureHeightmapTexture()
        {
            var path = EditorUtility.OpenFilePanel("Select Heightmap", "Assets", "png");
            
            if (string.IsNullOrEmpty(path))
                return;
            
            // Convert to project-relative path
            if (path.StartsWith(Application.dataPath))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }
            
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer == null)
            {
                EditorUtility.DisplayDialog("Error", "Could not find texture importer", "OK");
                return;
            }
            
            // Configure for heightmap
            importer.isReadable = true;
            importer.sRGBTexture = false;
            importer.mipmapEnabled = false;
            importer.maxTextureSize = 8192;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            
            importer.SaveAndReimport();
            
            EditorUtility.DisplayDialog(
                "Texture Configured",
                $"Heightmap texture configured:\n\n" +
                $"✓ Read/Write Enabled\n" +
                $"✓ sRGB Disabled\n" +
                $"✓ Mipmaps Disabled\n" +
                $"✓ Max Size: 8192\n" +
                $"✓ Compression: None",
                "OK");
        }
        
        private static bool IsPackageInstalled(string packageId)
        {
            var packagePath = $"Packages/{packageId}";
            return Directory.Exists(packagePath) || 
                   File.Exists($"Packages/manifest.json") && 
                   File.ReadAllText("Packages/manifest.json").Contains(packageId);
        }
    }
    
    /// <summary>
    /// Custom inspector for HeightmapVoxelLoader.
    /// </summary>
    [CustomEditor(typeof(HeightmapVoxelLoader))]
    public class HeightmapVoxelLoaderEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var loader = (HeightmapVoxelLoader)target;
            
            // Header
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Voxel Terrain System", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Converts heightmap grayscale to voxelized terrain using Unity DOTS.\n" +
                "Scale: 1 voxel = 3.6cm, 1 chunk = 64³ voxels (~2.3m³)",
                MessageType.Info);
            EditorGUILayout.Space();
            
            // Draw default inspector
            DrawDefaultInspector();
            
            EditorGUILayout.Space();
            
            // Runtime info
            if (Application.isPlaying && loader.IsInitialized)
            {
                EditorGUILayout.LabelField("Runtime Status", EditorStyles.boldLabel);
                EditorGUILayout.LabelField("Status:", "Initialized ✓");
                
                if (GUILayout.Button("Force Reload"))
                {
                    loader.ForceReload();
                }
                
                if (GUILayout.Button("Log Stats"))
                {
                    loader.LogStats();
                }
            }
            else if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("System not initialized. Check console for errors.", MessageType.Warning);
            }
            
            EditorGUILayout.Space();
            
            // Quick actions
            EditorGUILayout.LabelField("Quick Actions", EditorStyles.boldLabel);
            
            if (GUILayout.Button("Check Dependencies"))
            {
                VoxelSystemSetup.CheckDependencies();
            }
            
            if (GUILayout.Button("Open Documentation"))
            {
                VoxelSystemSetup.OpenDocumentation();
            }
        }
    }
}
#endif
