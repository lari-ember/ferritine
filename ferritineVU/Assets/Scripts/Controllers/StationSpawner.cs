using System.Collections.Generic;
using UnityEngine;

public class StationSpawner : MonoBehaviour
{
    public GameObject stationPrefab;
    private Dictionary<string, GameObject> spawnedStations = new Dictionary<string, GameObject>();

    public void SpawnOrUpdateStation(StationData data)
    {
        if (data == null) return;

        // StationData tem campos x e y separados, não um Vector3 position
        Vector3 position = new Vector3(data.x, 0, data.y);

        if (spawnedStations.TryGetValue(data.id, out GameObject go))
        {
            go.transform.position = position;
            go.name = $"Station_{data.name}";
        }
        else
        {
            GameObject s = Instantiate(stationPrefab, position, Quaternion.identity);
            s.name = $"Station_{data.name}";
            spawnedStations[data.id] = s;
        }
    }
}

