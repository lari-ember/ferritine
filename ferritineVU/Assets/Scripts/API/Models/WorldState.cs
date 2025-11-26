using System;
using System.Collections.Generic;


[Serializable]
public class WorldState
{
    // Metadados de tempo
    public string timestamp;
    public string simulation_time;
    
    // Entidades principais (compatível com backend models.py)
    public List<AgentData> agents;
    public List<VehicleData> vehicles;
    public List<StationData> stations;
    public List<RouteData> routes;
    public List<OperatorData> operators;
    public List<BuildingData> buildings;
    public List<EventData> events;
    public List<ProfessionData> professions;
    public List<RoutineData> routines;
    
    // Métricas agregadas
    public MetricsData metrics;
}

