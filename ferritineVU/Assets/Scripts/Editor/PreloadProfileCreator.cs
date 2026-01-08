using UnityEngine;
using UnityEditor;
using Voxel;

/// <summary>
/// Menu de contexto para criar PreloadProfiles pré-configurados rapidamente.
/// </summary>
public class PreloadProfileCreator : MonoBehaviour {
    
    [MenuItem("Assets/Create/Voxel/Preload Profiles/All Presets", priority = 1)]
    public static void CreateAllPresets() {
        CreateExplorationProfile();
        CreateBuildingProfile();
        CreateIndoorProfile();
        CreatePerformanceProfile();
        
        Debug.Log("✅ Todos os PreloadProfiles criados em Assets/Resources/PreloadProfiles/");
        AssetDatabase.Refresh();
    }
    
    [MenuItem("Assets/Create/Voxel/Preload Profiles/Exploration (Mundo Aberto)", priority = 2)]
    public static void CreateExplorationProfile() {
        var profile = ScriptableObject.CreateInstance<PreloadProfile>();
        profile.raioDesejadoMetros = 300f;
        profile.maxChunkRadius = 32;
        profile.dadosRetencaoRadius = 5;
        profile.dadosRetencaoBatchPerFrame = 12;
        profile.unloadInterval = 0.5f;
        
        SaveProfile(profile, "PreloadProfile_Exploration");
    }
    
    [MenuItem("Assets/Create/Voxel/Preload Profiles/Building (City Builder)", priority = 3)]
    public static void CreateBuildingProfile() {
        var profile = ScriptableObject.CreateInstance<PreloadProfile>();
        profile.raioDesejadoMetros = 200f;
        profile.maxChunkRadius = 24;
        profile.dadosRetencaoRadius = 4;
        profile.dadosRetencaoBatchPerFrame = 16;
        profile.unloadInterval = 0.5f;
        
        SaveProfile(profile, "PreloadProfile_Building");
    }
    
    [MenuItem("Assets/Create/Voxel/Preload Profiles/Indoor (Alta Definição)", priority = 4)]
    public static void CreateIndoorProfile() {
        var profile = ScriptableObject.CreateInstance<PreloadProfile>();
        profile.raioDesejadoMetros = 50f;
        profile.maxChunkRadius = 16;
        profile.dadosRetencaoRadius = 3;
        profile.dadosRetencaoBatchPerFrame = 24;
        profile.unloadInterval = 0.3f;
        
        SaveProfile(profile, "PreloadProfile_Indoor");
    }
    
    [MenuItem("Assets/Create/Voxel/Preload Profiles/Performance (Baixo Spec)", priority = 5)]
    public static void CreatePerformanceProfile() {
        var profile = ScriptableObject.CreateInstance<PreloadProfile>();
        profile.raioDesejadoMetros = 150f;
        profile.maxChunkRadius = 16;
        profile.dadosRetencaoRadius = 2;
        profile.dadosRetencaoBatchPerFrame = 32;
        profile.unloadInterval = 0.3f;
        
        SaveProfile(profile, "PreloadProfile_Performance");
    }
    
    private static void SaveProfile(PreloadProfile profile, string fileName) {
        string path = "Assets/Resources/PreloadProfiles";
        
        // Cria a pasta se não existir
        if (!AssetDatabase.IsValidFolder("Assets/Resources")) {
            AssetDatabase.CreateFolder("Assets", "Resources");
        }
        if (!AssetDatabase.IsValidFolder(path)) {
            AssetDatabase.CreateFolder("Assets/Resources", "PreloadProfiles");
        }
        
        string fullPath = $"{path}/{fileName}.asset";
        
        // Se já existe, sobrescreve
        var existing = AssetDatabase.LoadAssetAtPath<PreloadProfile>(fullPath);
        if (existing != null) {
            EditorUtility.CopySerialized(profile, existing);
            EditorUtility.SetDirty(existing);
            Debug.Log($"✏️ PreloadProfile atualizado: {fullPath}");
        } else {
            AssetDatabase.CreateAsset(profile, fullPath);
            Debug.Log($"✅ PreloadProfile criado: {fullPath}");
        }
        
        AssetDatabase.SaveAssets();
    }
}

