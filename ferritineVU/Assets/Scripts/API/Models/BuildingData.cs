using System;

/// <summary>
/// Data model for Building entities received from API.
/// Simplified version for basic display and teleport validation.
/// </summary>
[Serializable]
public class BuildingData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string name;
    public string building_type;
    public int x;
    public int y;
    public bool is_constructed;
    
    // Propriedades helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

