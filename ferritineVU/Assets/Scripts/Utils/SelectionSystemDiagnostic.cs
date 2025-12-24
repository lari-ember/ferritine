using UnityEngine;

/// <summary>
/// Script de diagnóstico para verificar configuração do sistema de seleção.
/// Adicione este componente a qualquer GameObject para executar verificações.
/// </summary>
public class SelectionSystemDiagnostic : MonoBehaviour
{
    [Header("Executar Diagnóstico")]
    [SerializeField] private bool runOnStart = true;
    [SerializeField] private bool showDetailedInfo = true;
    
    void Start()
    {
        if (runOnStart)
        {
            RunDiagnostic();
        }
    }
    
    [ContextMenu("Executar Diagnóstico Completo")]
    public void RunDiagnostic()
    {
        Debug.Log("═══════════════════════════════════════════════════");
        Debug.Log("🔍 DIAGNÓSTICO DO SISTEMA DE SELEÇÃO DE ENTIDADES");
        Debug.Log("═══════════════════════════════════════════════════");
        
        CheckSelectableLayer();
        CheckCameraController();
        CheckObjectPool();
        CheckSelectionPinPool();
        CheckSelectableEntities();
        
        Debug.Log("═══════════════════════════════════════════════════");
        Debug.Log("✅ Diagnóstico concluído!");
        Debug.Log("═══════════════════════════════════════════════════");
    }
    
    void CheckSelectableLayer()
    {
        Debug.Log("\n[1/5] Verificando Layer 'Selectable'...");
        
        int layerIndex = LayerMask.NameToLayer("Selectable");
        
        if (layerIndex == -1)
        {
            Debug.LogError("   ❌ Layer 'Selectable' NÃO EXISTE!");
            Debug.LogError("   → Solução: Edit → Project Settings → Tags and Layers");
            Debug.LogError("   → Adicione 'Selectable' em um slot vazio de User Layer");
        }
        else
        {
            Debug.Log($"   ✅ Layer 'Selectable' encontrada (index: {layerIndex})");
            
            LayerMask mask = LayerMask.GetMask("Selectable");
            Debug.Log($"   ℹ️  LayerMask value: {mask.value}");
        }
    }
    
    void CheckCameraController()
    {
        Debug.Log("\n[2/5] Verificando CameraController...");
        
        CameraController controller = FindFirstObjectByType<CameraController>();
        
        if (controller == null)
        {
            Debug.LogError("   ❌ CameraController não encontrado na cena!");
            Debug.LogError("   → Solução: Adicione CameraController à Main Camera");
        }
        else
        {
            Debug.Log($"   ✅ CameraController encontrado em: {controller.gameObject.name}");
            
            if (showDetailedInfo)
            {
                Debug.Log($"   ℹ️  Selectable Layer Mask: {controller.selectableLayer.value}");
                Debug.Log($"   ℹ️  Selection Pin Prefab: {(controller.selectionPinPrefab != null ? "✓ Atribuído" : "✗ NULL")}");
                Debug.Log($"   ℹ️  Movement Speed: {controller.movementSpeed}");
                Debug.Log($"   ℹ️  Zoom Speed: {controller.zoomSpeed}");
            }
            
            // Verificar se layer mask está configurado
            if (controller.selectableLayer.value == 0)
            {
                Debug.LogWarning("   ⚠️  Selectable Layer Mask = 0 (nada será detectado!)");
                Debug.LogWarning("   → Configure o campo 'Selectable Layer' no Inspector");
            }
        }
    }
    
    void CheckObjectPool()
    {
        Debug.Log("\n[3/5] Verificando ObjectPool...");
        
        ObjectPool pool = FindFirstObjectByType<ObjectPool>();
        
        if (pool == null)
        {
            Debug.LogError("   ❌ ObjectPool não encontrado na cena!");
            Debug.LogError("   → Solução: Adicione GameObject com componente ObjectPool");
        }
        else
        {
            Debug.Log($"   ✅ ObjectPool encontrado em: {pool.gameObject.name}");
        }
    }
    
    void CheckSelectionPinPool()
    {
        Debug.Log("\n[4/5] Verificando SelectionPinPool...");
        
        SelectionPinPool pinPool = FindFirstObjectByType<SelectionPinPool>();
        
        if (pinPool == null)
        {
            Debug.LogWarning("   ⚠️  SelectionPinPool não encontrado (opcional)");
            Debug.LogWarning("   → Pins de seleção serão instanciados diretamente do prefab");
        }
        else
        {
            Debug.Log($"   ✅ SelectionPinPool encontrado em: {pinPool.gameObject.name}");
            
            if (showDetailedInfo)
            {
                Debug.Log($"   ℹ️  Pin Prefab: {(pinPool.selectionPinPrefab != null ? "✓ Atribuído" : "✗ NULL")}");
                Debug.Log($"   ℹ️  Initial Pool Size: {pinPool.initialPoolSize}");
                Debug.Log($"   ℹ️  Auto Expand: {pinPool.autoExpand}");
            }
            
            if (pinPool.selectionPinPrefab == null)
            {
                Debug.LogError("   ❌ Selection Pin Prefab não atribuído!");
                Debug.LogError("   → Atribua um prefab no campo 'selectionPinPrefab'");
            }
        }
    }
    
    void CheckSelectableEntities()
    {
        Debug.Log("\n[5/5] Verificando Entidades Selecionáveis...");
        
        SelectableEntity[] entities = FindObjectsByType<SelectableEntity>(FindObjectsSortMode.None);
        
        if (entities.Length == 0)
        {
            Debug.LogWarning("   ⚠️  Nenhuma entidade selecionável encontrada");
            Debug.LogWarning("   → Execute o jogo para que entidades sejam criadas");
        }
        else
        {
            Debug.Log($"   ✅ {entities.Length} entidades selecionáveis encontradas");
            
            int correctLayer = 0;
            int wrongLayer = 0;
            int missingCollider = 0;
            
            int selectableLayerIndex = LayerMask.NameToLayer("Selectable");
            
            foreach (var entity in entities)
            {
                // Check layer
                if (entity.gameObject.layer == selectableLayerIndex)
                {
                    correctLayer++;
                }
                else
                {
                    wrongLayer++;
                    if (showDetailedInfo)
                    {
                        Debug.LogWarning($"   ⚠️  {entity.gameObject.name} está na layer errada: {LayerMask.LayerToName(entity.gameObject.layer)}");
                    }
                }
                
                // Check collider
                if (entity.GetComponent<Collider>() == null)
                {
                    missingCollider++;
                    if (showDetailedInfo)
                    {
                        Debug.LogWarning($"   ⚠️  {entity.gameObject.name} não tem Collider!");
                    }
                }
            }
            
            Debug.Log($"   ℹ️  Layer correta: {correctLayer}/{entities.Length}");
            
            if (wrongLayer > 0)
            {
                Debug.LogWarning($"   ⚠️  {wrongLayer} entidades com layer incorreta!");
            }
            
            if (missingCollider > 0)
            {
                Debug.LogWarning($"   ⚠️  {missingCollider} entidades sem Collider!");
            }
            
            // Show sample entities
            if (showDetailedInfo && entities.Length > 0)
            {
                Debug.Log("\n   📋 Amostra de entidades:");
                int sampleCount = Mathf.Min(5, entities.Length);
                for (int i = 0; i < sampleCount; i++)
                {
                    var entity = entities[i];
                    string layerName = LayerMask.LayerToName(entity.gameObject.layer);
                    string hasCollider = entity.GetComponent<Collider>() != null ? "✓" : "✗";
                    Debug.Log($"   - {entity.gameObject.name} | Layer: {layerName} | Collider: {hasCollider} | Type: {entity.entityType}");
                }
            }
        }
    }
}

