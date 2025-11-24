using System;

[Serializable]
public class AgentData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string name;
    public string status;
    public string location_type;
    public string location_id;
    public int energy_level;
    public float wallet;
    
    // Propriedades helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    public Guid GetLocationIdAsGuid() => Guid.TryParse(location_id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

