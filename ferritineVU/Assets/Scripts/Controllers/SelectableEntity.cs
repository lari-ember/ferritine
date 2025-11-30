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
    private bool isHighlighted = false;
    
    void Awake()
    {
        // Ensure this GameObject has a collider for raycasting
        EnsureCollider();
        
        // Cache renderer and materials
        rendererComponent = GetComponent<Renderer>();
        if (rendererComponent != null)
        {
            originalMaterials = rendererComponent.materials;
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
        if (isHighlighted || rendererComponent == null) return;
        
        foreach (Material mat in rendererComponent.materials)
        {
            // Enable emission keyword
            mat.EnableKeyword("_EMISSION");
            
            // Set emission color
            Color emissionColor = highlightColor * highlightIntensity;
            mat.SetColor("_EmissionColor", emissionColor);
        }
        
        isHighlighted = true;
    }
    
    /// <summary>
    /// Removes highlight from the entity.
    /// </summary>
    public void Unhighlight()
    {
        if (!isHighlighted || rendererComponent == null) return;
        
        foreach (Material mat in rendererComponent.materials)
        {
            // Disable emission
            mat.SetColor("_EmissionColor", Color.black);
            mat.DisableKeyword("_EMISSION");
        }
        
        isHighlighted = false;
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

