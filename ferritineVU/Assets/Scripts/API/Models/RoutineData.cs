using System;

[Serializable]
public class RoutineData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string name;
    public string description;
    public RoutineTaskData[] tasks;
    
    // Propriedade helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

[Serializable]
public class RoutineTaskData
{
    public int hour;
    public string activity;
    public string location_id;
    public int duration;
    
    // Helper para trabalhar com Guid (safe parsing)
    public Guid GetLocationIdAsGuid() => Guid.TryParse(location_id, out Guid result) ? result : Guid.Empty;
}

