using System.Collections.Generic;
using UnityEngine;

public class StationSpawner : MonoBehaviour
{
    public GameObject stationPrefab;
    private Dictionary<string, GameObject> spawnedStations = new Dictionary<string, GameObject>();

    public void SpawnOrUpdateStation(StationData data)
    {
        if (data == null) return;

        if (spawnedStations.TryGetValue(data.id, out GameObject go))
        {
            go.transform.position = data.position.ToVector3();
            go.name = $"Station_{data.id}";
        }
        else
        {
            GameObject s = Instantiate(stationPrefab, data.position.ToVector3(), Quaternion.identity);
            s.name = $"Station_{data.id}";
            spawnedStations[data.id] = s;
        }
    }
}

