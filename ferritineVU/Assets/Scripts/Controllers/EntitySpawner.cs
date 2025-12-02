using UnityEngine;
using System.Collections.Generic;

public class EntitySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject agentPrefab;
    public GameObject vehiclePrefab;
    public GameObject stationPrefab;

    // Dicionários = facilitam atualização em tempo real
    private Dictionary<string, GameObject> spawnedAgents = new();
    private Dictionary<string, GameObject> spawnedVehicles = new();
    private Dictionary<string, GameObject> spawnedStations = new();

    // Chamado pelo FerritineAPIClient via evento
    public void UpdateWorldEntities(WorldState state)
    {
        if (state == null) return;

        // WorldState usa List<T> — convertendo para arrays para manter as assinaturas originais
        UpdateStations(state.stations != null ? state.stations.ToArray() : null);
        UpdateVehicles(state.vehicles != null ? state.vehicles.ToArray() : null);
        UpdateAgents(state.agents != null ? state.agents.ToArray() : null);
    }

    // ------------------- STATIONS -------------------
    private void UpdateStations(StationData[] stations)
    {
        if (stations == null) return;

        foreach (var s in stations)
        {
            if (!spawnedStations.ContainsKey(s.id))
            {
                GameObject obj = Instantiate(stationPrefab);
                obj.name = $"Station_{s.name}";
                spawnedStations[s.id] = obj;
            }

            GameObject stationObj = spawnedStations[s.id];
            stationObj.transform.position = new Vector3(s.x, 0, s.y);
        }
    }

    // ------------------- VEHICLES -------------------
    private void UpdateVehicles(VehicleData[] vehicles)
    {
        if (vehicles == null) return;

        foreach (var v in vehicles)
        {
            if (!spawnedVehicles.ContainsKey(v.id))
            {
                GameObject obj = Instantiate(vehiclePrefab);
                obj.name = $"Vehicle_{v.name}";
                spawnedVehicles[v.id] = obj;
            }

            GameObject vehObj = spawnedVehicles[v.id];
            
        }
    }

    // ------------------- AGENTS -------------------
    private void UpdateAgents(AgentData[] agents)
    {
        if (agents == null) return;


        foreach (var a in agents)
        {
            if (!spawnedAgents.ContainsKey(a.id))
            {
                GameObject obj = Instantiate(agentPrefab);
                obj.name = $"Agent_{a.name}";
                spawnedAgents[a.id] = obj;
            }

            GameObject agentObj = spawnedAgents[a.id];

            // Agentes serão colocados em clusters ao redor da estação 1 (temporário)
            if (spawnedStations.Count > 0)
            {
                var station = spawnedStations.Values.GetEnumerator();
                station.MoveNext();
                Vector3 basePos = station.Current.transform.position;

                // espalhar os agentes em forma de círculo
                float angle = agents.GetHashCode() % 360;
                float radius = 2f;
                Vector3 offset = new Vector3(
                    Mathf.Cos(angle) * radius,
                    0,
                    Mathf.Sin(angle) * radius
                );

                agentObj.transform.position = basePos + offset;
            }
        }
    }
}
