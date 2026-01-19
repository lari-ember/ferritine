using System;
using System.Collections.Generic;
using UnityEngine;


namespace API.Models
{
    [Serializable]
    public class WorldState
    {
        // Metadados de tempo
        public string timestamp;
        [SerializeField] public string simulationTime;
        
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

        // Exemplo de campo de clima para integração futura
        [SerializeField] public string weatherType;

        public WorldState Clone()
        {
            // Cria uma cópia profunda do estado do mundo
            return new WorldState
            {
                timestamp = this.timestamp,
                simulationTime = this.simulationTime,
                agents = this.agents != null ? new List<AgentData>(this.agents) : null,
                vehicles = this.vehicles != null ? new List<VehicleData>(this.vehicles) : null,
                stations = this.stations != null ? new List<StationData>(this.stations) : null,
                routes = this.routes != null ? new List<RouteData>(this.routes) : null,
                operators = this.operators != null ? new List<OperatorData>(this.operators) : null,
                buildings = this.buildings != null ? new List<BuildingData>(this.buildings) : null,
                events = this.events != null ? new List<EventData>(this.events) : null,
                professions = this.professions != null ? new List<ProfessionData>(this.professions) : null,
                routines = this.routines != null ? new List<RoutineData>(this.routines) : null,
                metrics = this.metrics // Se MetricsData for mutável, implemente Clone também
            };
        }
    }
}
