using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldState
{
    public List<AgentData> agents = new List<AgentData>();
    public List<VehicleData> vehicles = new List<VehicleData>();
    public List<StationData> stations = new List<StationData>();
    public MetricsData metrics = new MetricsData();
}

