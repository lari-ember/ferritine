using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Componente que torna uma entidade (Station/Vehicle/Agent) selecionável via click.
/// Armazena dados e notifica observers quando dados mudam.
/// </summary>
public class SelectableEntity : MonoBehaviour
{
    public enum EntityType
    {
        Station,
        Vehicle,
        Agent,
        Building
    }

    [Header("Entity Configuration")]
    public EntityType entityType;
    
    [Header("Entity Data")]
    public StationData stationData;
    public VehicleData vehicleData;
    public AgentData agentData;
    public BuildingData buildingData;
    
    [Header("Visual Feedback")]
    public float highlightIntensity = 0.3f;
    public Color highlightColor = Color.yellow;
    
    // Event fired when entity data is updated (Observer pattern)
    public UnityEvent<object> OnDataUpdated = new UnityEvent<object>();
    
    // Cache original materials for highlight/unhighlight
    private Material[] originalMaterials;
    private Renderer rendererComponent;
    private Renderer[] allRenderers; // Para modelos FBX com múltiplos renderers
    private bool isHighlighted = false;
    
    void Awake()
    {
        // Ensure this GameObject has a collider for raycasting
        EnsureCollider();
        
        // Cache renderer and materials - busca em filhos para modelos FBX
        FindRendererComponents();
    }
    
    /// <summary>
    /// Busca componentes Renderer neste objeto ou nos filhos (para modelos FBX).
    /// </summary>
    void FindRendererComponents()
    {
        // Primeiro tenta no próprio objeto
        rendererComponent = GetComponent<Renderer>();
        
        // Se não encontrou, busca nos filhos (comum em FBX)
        if (rendererComponent == null)
        {
            rendererComponent = GetComponentInChildren<Renderer>();
            
            // Também busca SkinnedMeshRenderer especificamente (modelos animados)
            if (rendererComponent == null)
            {
                rendererComponent = GetComponentInChildren<SkinnedMeshRenderer>();
            }
        }
        
        // Busca TODOS os renderers nos filhos (modelos podem ter múltiplas partes)
        allRenderers = GetComponentsInChildren<Renderer>();
        
        if (rendererComponent != null)
        {
            originalMaterials = rendererComponent.materials;
            Debug.Log($"[SelectableEntity] ✅ Renderer encontrado em '{rendererComponent.gameObject.name}' " +
                      $"(tipo: {rendererComponent.GetType().Name}, total renderers: {allRenderers.Length})");
        }
        else if (allRenderers.Length > 0)
        {
            // Fallback: usa o primeiro renderer encontrado
            rendererComponent = allRenderers[0];
            originalMaterials = rendererComponent.materials;
            Debug.Log($"[SelectableEntity] ✅ Usando fallback renderer em '{rendererComponent.gameObject.name}'");
        }
        else
        {
            Debug.LogWarning($"[SelectableEntity] ⚠️ Nenhum Renderer encontrado em '{gameObject.name}' ou seus filhos");
        }
    }
    
    /// <summary>
    /// Ensures the GameObject has a collider for raycasting.
    /// Adds a box collider if none exists.
    /// </summary>
    void EnsureCollider()
    {
        Collider col = GetComponent<Collider>();
        if (col == null)
        {
            BoxCollider boxCol = gameObject.AddComponent<BoxCollider>();
            Debug.Log($"[SelectableEntity] Added BoxCollider to {gameObject.name}");
        }
    }
    
    /// <summary>
    /// Highlights the entity by increasing emission on all materials.
    /// </summary>
    public void Highlight()
    {
        Debug.Log($"[SelectableEntity] Highlight() chamado em: {gameObject.name}");
        
        if (isHighlighted)
        {
            Debug.LogWarning($"[SelectableEntity] Entidade já está destacada, ignorando.");
            return;
        }
        
        // Tenta encontrar renderer se ainda não tem
        if (rendererComponent == null && (allRenderers == null || allRenderers.Length == 0))
        {
            FindRendererComponents();
        }
        
        // Aplica highlight em TODOS os renderers (modelos FBX podem ter múltiplas partes)
        if (allRenderers != null && allRenderers.Length > 0)
        {
            int count = 0;
            foreach (Renderer rend in allRenderers)
            {
                if (rend == null) continue;
                
                foreach (Material mat in rend.materials)
                {
                    mat.EnableKeyword("_EMISSION");
                    Color emissionColor = highlightColor * highlightIntensity;
                    mat.SetColor("_EmissionColor", emissionColor);
                    count++;
                }
            }
            Debug.Log($"[SelectableEntity] ✅ Highlight aplicado a {count} materiais em {allRenderers.Length} renderers");
            isHighlighted = true;
            return;
        }
        
        // Fallback: renderer único
        if (rendererComponent == null)
        {
            Debug.LogWarning($"[SelectableEntity] ⚠️ Nenhum renderer disponível para highlight em '{gameObject.name}'");
            return;
        }
        
        foreach (Material mat in rendererComponent.materials)
        {
            mat.EnableKeyword("_EMISSION");
            Color emissionColor = highlightColor * highlightIntensity;
            mat.SetColor("_EmissionColor", emissionColor);
        }
        
        isHighlighted = true;
        Debug.Log($"[SelectableEntity] ✅ Highlight aplicado com sucesso!");
    }
    
    /// <summary>
    /// Removes highlight from the entity.
    /// </summary>
    public void Unhighlight()
    {
        if (!isHighlighted)
        {
            return;
        }
        
        // Remove highlight de TODOS os renderers (modelos FBX)
        if (allRenderers != null && allRenderers.Length > 0)
        {
            foreach (Renderer rend in allRenderers)
            {
                if (rend == null) continue;
                
                foreach (Material mat in rend.materials)
                {
                    mat.SetColor("_EmissionColor", Color.black);
                    mat.DisableKeyword("_EMISSION");
                }
            }
            isHighlighted = false;
            Debug.Log($"[SelectableEntity] ✅ Highlight removido de {allRenderers.Length} renderers");
            return;
        }
        
        // Fallback: renderer único
        if (rendererComponent == null)
        {
            Debug.LogWarning($"[SelectableEntity] ⚠️ Nenhum renderer disponível para unhighlight em '{gameObject.name}'");
            return;
        }
        
        foreach (Material mat in rendererComponent.materials)
        {
            mat.SetColor("_EmissionColor", Color.black);
            mat.DisableKeyword("_EMISSION");
        }
        
        isHighlighted = false;
        Debug.Log($"[SelectableEntity] ✅ Highlight removido com sucesso!");
    }
    
    /// <summary>
    /// Gets the display name for this entity.
    /// </summary>
    public string GetDisplayName()
    {
        switch (entityType)
        {
            case EntityType.Station:
                return stationData?.name ?? "Unknown Station";
            case EntityType.Vehicle:
                return vehicleData?.name ?? "Unknown Vehicle";
            case EntityType.Agent:
                return agentData?.name ?? "Unknown Agent";
            case EntityType.Building:
                return buildingData?.name ?? "Unknown Building";
            default:
                return "Unknown Entity";
        }
    }
    
    /// <summary>
    /// Gets the entity ID as string.
    /// </summary>
    public new string GetEntityId()
    {
        switch (entityType)
        {
            case EntityType.Station:
                return stationData?.id;
            case EntityType.Vehicle:
                return vehicleData?.id;
            case EntityType.Agent:
                return agentData?.id;
            case EntityType.Building:
                return buildingData?.id;
            default:
                return null;
        }
    }
    
    /// <summary>
    /// Updates entity data and notifies observers.
    /// </summary>
    public void UpdateData(object newData)
    {
        switch (entityType)
        {
            case EntityType.Station:
                stationData = newData as StationData;
                break;
            case EntityType.Vehicle:
                vehicleData = newData as VehicleData;
                break;
            case EntityType.Agent:
                agentData = newData as AgentData;
                break;
            case EntityType.Building:
                buildingData = newData as BuildingData;
                break;
        }
        
        // Notify observers that data has changed
        OnDataUpdated?.Invoke(newData);
    }
    
    void OnDestroy()
    {
        // Cleanup: remove all listeners
        OnDataUpdated.RemoveAllListeners();
    }
}

