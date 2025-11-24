using System;

[Serializable]
public class RouteData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string name;
    public string code;
    public string route_type;
    public float fare;
    public int frequency;
    public bool is_active;
    
    // Propriedade helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

