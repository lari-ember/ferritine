using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Object pool for selection pin indicators.
/// Manages a pool of reusable 3D pin GameObjects to avoid instantiate/destroy overhead.
/// </summary>
public class SelectionPinPool : MonoBehaviour
{
    public static SelectionPinPool Instance { get; private set; }
    
    [Header("Pool Settings")]
    [Tooltip("Prefab for the selection pin indicator")]
    public GameObject pinPrefab;
    
    [Tooltip("Number of pins to pre-create")]
    public int prewarmCount = 5;
    
    [Tooltip("Maximum pool size")]
    public int maxPoolSize = 10;
    
    private Queue<GameObject> availablePins = new Queue<GameObject>();
    private List<GameObject> activePins = new List<GameObject>();
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
        
        // Create container for pooled objects
        poolContainer = new GameObject("SelectionPinPool_Container").transform;
        poolContainer.SetParent(transform);
        
        // Prewarm pool
        Prewarm();
    }
    
    /// <summary>
    /// Pre-creates pins for the pool.
    /// </summary>
    void Prewarm()
    {
        if (pinPrefab == null)
        {
            Debug.LogError("[SelectionPinPool] Pin prefab is not assigned!");
            return;
        }
        
        for (int i = 0; i < prewarmCount; i++)
        {
            CreatePin();
        }
        
        Debug.Log($"[SelectionPinPool] Prewarmed with {prewarmCount} pins");
    }
    
    /// <summary>
    /// Creates a new pin and adds it to the pool.
    /// </summary>
    GameObject CreatePin()
    {
        GameObject pin = Instantiate(pinPrefab, poolContainer);
        pin.SetActive(false);
        availablePins.Enqueue(pin);
        return pin;
    }
    
    /// <summary>
    /// Gets a pin from the pool.
    /// </summary>
    public GameObject Get()
    {
        GameObject pin;
        
        if (availablePins.Count > 0)
        {
            pin = availablePins.Dequeue();
        }
        else if (activePins.Count < maxPoolSize)
        {
            pin = CreatePin();
        }
        else
        {
            Debug.LogWarning("[SelectionPinPool] Pool exhausted! Consider increasing maxPoolSize.");
            return null;
        }
        
        pin.SetActive(true);
        activePins.Add(pin);
        return pin;
    }
    
    /// <summary>
    /// Returns a pin to the pool.
    /// </summary>
    public void Return(GameObject pin)
    {
        if (pin == null) return;
        
        pin.SetActive(false);
        pin.transform.SetParent(poolContainer);
        pin.transform.localPosition = Vector3.zero;
        
        activePins.Remove(pin);
        availablePins.Enqueue(pin);
    }
    
    /// <summary>
    /// Returns all active pins to the pool.
    /// </summary>
    public void ReturnAll()
    {
        // Create a copy of the list to avoid modification during iteration
        List<GameObject> pinsToReturn = new List<GameObject>(activePins);
        
        foreach (GameObject pin in pinsToReturn)
        {
            Return(pin);
        }
    }
    
    /// <summary>
    /// Gets statistics about the pool.
    /// </summary>
    public void LogStats()
    {
        Debug.Log($"[SelectionPinPool] Available: {availablePins.Count}, Active: {activePins.Count}, Total: {availablePins.Count + activePins.Count}");
    }
}

