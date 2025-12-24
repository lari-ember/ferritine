// filepath: /home/larisssa/Documentos/codigos/ferritine/ferritineVU/Assets/Scripts/Utils/ObjectPool.cs
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    /// <summary>
    /// Simple multi-pool manager for GameObjects by name.
    /// Usage:
    ///   InitializePool("agents", agentPrefab, parent, 50);
    ///   var go = Get("agents");
    ///   Return("agents", go);
    /// Includes basic prewarm and stats logging.
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {
        private class Pool
        {
            public readonly Queue<GameObject> Inactive = new Queue<GameObject>();
            public GameObject Prefab;
            public Transform Parent;
            public int CreatedCount;
        }

        private readonly Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

        /// <summary>
        /// Creates or reconfigures a named pool and prewarms instances.
        /// </summary>
        public void InitializePool(string poolName, GameObject prefab, Transform parent, int prewarmCount)
        {
            if (string.IsNullOrEmpty(poolName) || prefab == null)
            {
                Debug.LogError("[ObjectPool] Invalid InitializePool args.");
                return;
            }

            if (!_pools.TryGetValue(poolName, out var pool))
            {
                pool = new Pool();
                _pools[poolName] = pool;
            }

            pool.Prefab = prefab;
            pool.Parent = parent != null ? parent : transform;

            // Prewarm only up to requested count total
            int toCreate = Mathf.Max(0, prewarmCount - pool.Inactive.Count);
            for (int i = 0; i < toCreate; i++)
            {
                var go = Instantiate(pool.Prefab, pool.Parent);
                go.SetActive(false);
                pool.Inactive.Enqueue(go);
                pool.CreatedCount++;
            }

            Debug.Log($"[ObjectPool] '{poolName}' ready | prefab={prefab.name} | prewarmed={prewarmCount} | totalCreated={pool.CreatedCount}");
        }

        /// <summary>
        /// Gets an instance from the pool or creates a new one if empty.
        /// </summary>
        public GameObject Get(string poolName)
        {
            if (!_pools.TryGetValue(poolName, out var pool) || pool.Prefab == null)
            {
                Debug.LogError($"[ObjectPool] Pool '{poolName}' not initialized.");
                return null;
            }

            GameObject go;
            
            if (pool.Inactive.Count > 0)
            {
                // Get from pool
                go = pool.Inactive.Dequeue();
            }
            else
            {
                // Create new instance
                go = Instantiate(pool.Prefab, pool.Parent);
                pool.CreatedCount++;
                Debug.Log($"[ObjectPool] Pool '{poolName}' expanded - new total: {pool.CreatedCount}");
            }

            // Ensure parent
            if (go.transform.parent != pool.Parent)
            {
                go.transform.SetParent(pool.Parent);
            }
            
            // Ensure active
            if (!go.activeSelf)
            {
                go.SetActive(true);
            }
            
            return go;
        }

        /// <summary>
        /// Returns an instance back to the named pool.
        /// </summary>
        public void Return(string poolName, GameObject instance)
        {
            if (instance == null) return;
            if (!_pools.TryGetValue(poolName, out var pool))
            {
                Debug.LogWarning($"[ObjectPool] Return called for unknown pool '{poolName}'. Destroying instance.");
                Destroy(instance);
                return;
            }

            instance.SetActive(false);
            if (instance.transform.parent != pool.Parent)
                instance.transform.SetParent(pool.Parent);
            pool.Inactive.Enqueue(instance);
        }

        /// <summary>
        /// Logs current stats for a named pool.
        /// </summary>
        public void LogPoolStats(string poolName)
        {
            if (_pools.TryGetValue(poolName, out var pool))
            {
                Debug.Log($"[ObjectPool] Stats '{poolName}': inactive={pool.Inactive.Count} created={pool.CreatedCount} prefab={(pool.Prefab!=null?pool.Prefab.name:"null")}");
            }
            else
            {
                Debug.LogWarning($"[ObjectPool] Stats requested for unknown pool '{poolName}'.");
            }
        }
    }
}

