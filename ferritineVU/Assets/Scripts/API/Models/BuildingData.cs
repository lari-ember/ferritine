using System;

[Serializable]
public class BuildingData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string name;
    public string building_type;
    public int x;
    public int y;
    public int width;
    public int height;
    public bool is_constructed;
    public float construction_progress;
    public string owner_id;
    public int capacity;
    public int current_occupancy;
    
    // Propriedades helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    public Guid GetOwnerIdAsGuid() => Guid.TryParse(owner_id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

