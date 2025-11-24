using System;

[Serializable]
public class VehicleData
{
    // UUID serializado como string no JSON, convertido para Guid
    public string id;
    public string name;
    public string vehicle_type;
    public int passengers;
    public int capacity;
    public string status;
    public string current_station_id;
    public string current_route_id;
    public float fuel_level;
    
    // Propriedades helper para trabalhar com Guid (safe parsing)
    public Guid GetIdAsGuid() => Guid.TryParse(id, out Guid result) ? result : Guid.Empty;
    public Guid GetCurrentStationIdAsGuid() => Guid.TryParse(current_station_id, out Guid result) ? result : Guid.Empty;
    public Guid GetCurrentRouteIdAsGuid() => Guid.TryParse(current_route_id, out Guid result) ? result : Guid.Empty;
    
    // Validação de UUID
    public bool HasValidId() => Guid.TryParse(id, out _);
}

