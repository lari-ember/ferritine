using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour
{
    public FerritineAPIClient apiClient;
    public StationSpawner stationSpawner;
    public GameObject vehiclePrefab;

    [Tooltip("Intervalo (s) entre atualizações do estado do mundo")]
    public float pollInterval = 2f;

    private Dictionary<string, GameObject> spawnedVehicles = new Dictionary<string, GameObject>();

    private void Start()
    {
        if (apiClient == null) apiClient = FindObjectOfType<FerritineAPIClient>();
        if (stationSpawner == null) stationSpawner = FindObjectOfType<StationSpawner>();

        StartCoroutine(PollLoop());
    }

    private IEnumerator PollLoop()
    {
        while (true)
        {
            yield return apiClient.GetWorldState(OnWorldStateReceived, OnError);
            yield return new WaitForSeconds(pollInterval);
        }
    }

    private void OnWorldStateReceived(WorldState state)
    {
        if (state == null) return;

        // Spawn/refresh stations
        if (stationSpawner != null)
        {
            foreach (var s in state.stations)
                stationSpawner.SpawnOrUpdateStation(s);
        }

        // Update vehicles
        foreach (var v in state.vehicles)
        {
            if (!spawnedVehicles.TryGetValue(v.id, out GameObject go))
            {
                go = Instantiate(vehiclePrefab, v.position.ToVector3(), Quaternion.identity);
                go.name = $"Vehicle_{v.id}";
                spawnedVehicles[v.id] = go;
            }
            else
            {
                // simple position update
                go.transform.position = Vector3.Lerp(go.transform.position, v.position.ToVector3(), 0.5f);
            }
        }

        // TODO: handle agents and metrics (e.g., update UI)
    }

    private void OnError(string error)
    {
        Debug.LogWarning($"FerritineAPI error: {error}");
    }
}

