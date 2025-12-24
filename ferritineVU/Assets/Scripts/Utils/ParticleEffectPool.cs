using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Object pool for particle effects (teleport spawn/despawn, etc).
/// Manages multiple particle system prefabs and auto-returns them after playback.
/// </summary>
public class ParticleEffectPool : MonoBehaviour
{
    public static ParticleEffectPool Instance { get; private set; }
    
    [System.Serializable]
    public class PooledParticleType
    {
        public string typeName;
        public GameObject prefab;
        public int prewarmCount = 5;
    }
    
    [Header("Pool Settings")]
    public List<PooledParticleType> particleTypes = new List<PooledParticleType>();
    
    [Tooltip("Maximum instances per type")]
    public int maxPerType = 20;
    
    // Pools organized by type name
    private Dictionary<string, Queue<GameObject>> availablePools = new Dictionary<string, Queue<GameObject>>();
    private Dictionary<string, List<GameObject>> activePools = new Dictionary<string, List<GameObject>>();
    private Transform poolContainer;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Create container
        poolContainer = new GameObject("ParticleEffectPool_Container").transform;
        poolContainer.SetParent(transform);
        
        // Initialize pools
        InitializePools();
    }
    
    /// <summary>
    /// Initializes all particle pools.
    /// </summary>
    void InitializePools()
    {
        foreach (var particleType in particleTypes)
        {
            if (particleType.prefab == null)
            {
                Debug.LogWarning($"[ParticleEffectPool] Prefab not assigned for type: {particleType.typeName}");
                continue;
            }
            
            availablePools[particleType.typeName] = new Queue<GameObject>();
            activePools[particleType.typeName] = new List<GameObject>();
            
            // Prewarm
            for (int i = 0; i < particleType.prewarmCount; i++)
            {
                CreateParticle(particleType.typeName, particleType.prefab);
            }
            
            Debug.Log($"[ParticleEffectPool] Initialized '{particleType.typeName}' with {particleType.prewarmCount} instances");
        }
    }
    
    /// <summary>
    /// Creates a new particle effect instance.
    /// </summary>
    GameObject CreateParticle(string typeName, GameObject prefab)
    {
        GameObject particle = Instantiate(prefab, poolContainer);
        particle.name = $"{typeName}_{availablePools[typeName].Count}";
        particle.SetActive(false);
        availablePools[typeName].Enqueue(particle);
        return particle;
    }
    
    /// <summary>
    /// Gets a particle effect from the pool.
    /// </summary>
    public GameObject Get(string typeName, Vector3 position, Quaternion rotation)
    {
        if (!availablePools.ContainsKey(typeName))
        {
            Debug.LogError($"[ParticleEffectPool] Unknown particle type: {typeName}");
            return null;
        }
        
        GameObject particle;
        
        if (availablePools[typeName].Count > 0)
        {
            particle = availablePools[typeName].Dequeue();
        }
        else if (activePools[typeName].Count < maxPerType)
        {
            // Find prefab
            PooledParticleType particleType = particleTypes.Find(p => p.typeName == typeName);
            if (particleType == null || particleType.prefab == null)
            {
                Debug.LogError($"[ParticleEffectPool] Prefab not found for type: {typeName}");
                return null;
            }
            
            particle = CreateParticle(typeName, particleType.prefab);
        }
        else
        {
            Debug.LogWarning($"[ParticleEffectPool] Pool exhausted for type: {typeName}");
            return null;
        }
        
        // Setup particle
        particle.transform.position = position;
        particle.transform.rotation = rotation;
        particle.SetActive(true);
        activePools[typeName].Add(particle);
        
        // Auto-return after particle system finishes
        ParticleSystem ps = particle.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            StartCoroutine(AutoReturnAfterDuration(typeName, particle, ps.main.duration + ps.main.startLifetime.constantMax));
        }
        else
        {
            Debug.LogWarning($"[ParticleEffectPool] No ParticleSystem found on {typeName}. Won't auto-return.");
        }
        
        return particle;
    }
    
    /// <summary>
    /// Plays a particle effect at a position and auto-returns it.
    /// </summary>
    public void Play(string typeName, Vector3 position)
    {
        GameObject particle = Get(typeName, position, Quaternion.identity);
        if (particle != null)
        {
            ParticleSystem ps = particle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }
    }
    
    /// <summary>
    /// Returns a particle effect to the pool.
    /// </summary>
    public void Return(string typeName, GameObject particle)
    {
        if (particle == null || !activePools.ContainsKey(typeName)) return;
        
        particle.SetActive(false);
        particle.transform.SetParent(poolContainer);
        particle.transform.localPosition = Vector3.zero;
        
        activePools[typeName].Remove(particle);
        availablePools[typeName].Enqueue(particle);
    }
    
    /// <summary>
    /// Coroutine to automatically return particle after duration.
    /// </summary>
    IEnumerator AutoReturnAfterDuration(string typeName, GameObject particle, float duration)
    {
        yield return new WaitForSeconds(duration);
        Return(typeName, particle);
    }
    
    /// <summary>
    /// Returns all active particles to the pool.
    /// </summary>
    public void ReturnAll()
    {
        foreach (var kvp in activePools)
        {
            List<GameObject> particlesToReturn = new List<GameObject>(kvp.Value);
            foreach (GameObject particle in particlesToReturn)
            {
                Return(kvp.Key, particle);
            }
        }
    }
    
    /// <summary>
    /// Logs statistics for all pools.
    /// </summary>
    public void LogStats()
    {
        foreach (var kvp in availablePools)
        {
            string typeName = kvp.Key;
            int available = kvp.Value.Count;
            int active = activePools[typeName].Count;
            Debug.Log($"[ParticleEffectPool] {typeName} - Available: {available}, Active: {active}, Total: {available + active}");
        }
    }
}

