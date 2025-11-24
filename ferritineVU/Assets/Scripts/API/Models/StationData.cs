using System;

[Serializable]
public class StationData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string name;
    public string station_type;
    public int x;
    public int y;
    public int queue_length;
    public int max_queue;
    public bool is_operational;
    
    // Propriedade helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

