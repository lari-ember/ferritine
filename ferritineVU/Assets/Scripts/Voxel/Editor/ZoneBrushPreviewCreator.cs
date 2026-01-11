using UnityEngine;

namespace Voxel.Editor {
    /// <summary>
    /// Script para criar programaticamente o prefab de preview do ZoneBrush.
    /// Adicione este script a um GameObject vazio na cena e execute o método CreatePreviewPrefab().
    /// Depois pode ser removido.
    /// 
    /// Alternativamente, crie manualmente:
    /// 1. Crie um Quad (3D Object > Quad)
    /// 2. Rotacione -90° no eixo X (para ficar horizontal)
    /// 3. Aplique um material transparente
    /// 4. Salve como prefab em Assets/Prefabs/ZoneBrushPreview.prefab
    /// </summary>
    public class ZoneBrushPreviewCreator : MonoBehaviour {
        
        [Header("Material Settings")]
        public Color defaultColor = new Color(0.3f, 1f, 0.3f, 0.5f);
        
        // Cached shader property IDs
        private static readonly int ModePropertyId = Shader.PropertyToID("_Mode");
        private static readonly int SrcBlendPropertyId = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlendPropertyId = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWritePropertyId = Shader.PropertyToID("_ZWrite");
        
        /// <summary>
        /// Cria o GameObject de preview em runtime.
        /// Pode ser chamado pelo ZoneBrush se o prefab não estiver atribuído.
        /// </summary>
        public static GameObject CreatePreviewObject() {
            // Cria o quad
            GameObject preview = GameObject.CreatePrimitive(PrimitiveType.Quad);
            preview.name = "ZoneBrushPreview";
            
            // Rotaciona para ficar horizontal
            preview.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            
            // Remove o collider (não queremos interação)
            var collider = preview.GetComponent<Collider>();
            if (collider != null) {
                Destroy(collider);
            }
            
            // Cria material transparente
            Renderer renderer = preview.GetComponent<Renderer>();
            if (renderer != null) {
                // Usa shader padrão transparente
                Material mat = new Material(Shader.Find("Sprites/Default"));
                mat.color = new Color(0.3f, 1f, 0.3f, 0.5f);
                
                // Configura para renderizar como transparente usando IDs cacheados
                mat.SetFloat(ModePropertyId, 3); // Transparent
                mat.SetInt(SrcBlendPropertyId, (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetInt(DstBlendPropertyId, (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                mat.SetInt(ZWritePropertyId, 0);
                mat.DisableKeyword("_ALPHATEST_ON");
                mat.EnableKeyword("_ALPHABLEND_ON");
                mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                mat.renderQueue = 3000;
                
                renderer.material = mat;
            }
            
            return preview;
        }
        
#if UNITY_EDITOR
        /// <summary>
        /// Cria o prefab no editor. Use via menu de contexto ou botão no Inspector.
        /// </summary>
        [ContextMenu("Create Preview Prefab")]
        public void CreatePreviewPrefab() {
            GameObject preview = CreatePreviewObject();
            
            // Salva como prefab
            string path = "Assets/Prefabs/ZoneBrushPreview.prefab";
            
            // Cria a pasta se não existir
            if (!System.IO.Directory.Exists("Assets/Prefabs")) {
                System.IO.Directory.CreateDirectory("Assets/Prefabs");
            }
            
            // Salva o prefab
            UnityEditor.PrefabUtility.SaveAsPrefabAsset(preview, path);
            
            Debug.Log($"[ZoneBrushPreviewCreator] Prefab criado em: {path}");
            
            // Remove o objeto da cena
            DestroyImmediate(preview);
            
            // Refresh do AssetDatabase
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
    }
}

