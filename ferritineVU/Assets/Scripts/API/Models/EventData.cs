using System;

[Serializable]
public class EventData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string event_type;
    public string description;
    public string agent_id;
    public string building_id;
    public string vehicle_id;
    public string occurred_at;
    public int simulation_time;
    
    // Propriedades helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    public Guid GetAgentIdAsGuid() => Guid.TryParse(agent_id, out Guid result) ? result : Guid.Empty;
    public Guid GetBuildingIdAsGuid() => Guid.TryParse(building_id, out Guid result) ? result : Guid.Empty;
    public Guid GetVehicleIdAsGuid() => Guid.TryParse(vehicle_id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

