using UnityEngine;
using System.Collections.Generic;
using Utils;

namespace Utils
{
    /// <summary>
    /// Object pool for selection pin indicators.
    /// Implements singleton pattern for easy global access.
    /// </summary>
    public class SelectionPinPool : MonoBehaviour
    {
        [Header("Pin Settings")]
        public GameObject selectionPinPrefab;
        public int initialPoolSize = 10;
        public bool autoExpand = true;
        
        private ObjectPool objectPool;
        private const string POOL_NAME = "selectionPins";
        private Dictionary<string, GameObject> activePins = new Dictionary<string, GameObject>();

        private static SelectionPinPool _instance;
        
        public static SelectionPinPool Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<SelectionPinPool>();
                }
                return _instance;
            }
        }

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                InitializePool();
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void InitializePool()
        {
            if (selectionPinPrefab == null)
            {
                Debug.LogError("[SelectionPinPool] Selection Pin Prefab não está atribuído!");
                return;
            }

            objectPool = FindFirstObjectByType<ObjectPool>();
            if (objectPool == null)
            {
                Debug.LogError("[SelectionPinPool] ObjectPool não encontrado na cena!");
                return;
            }

            // Cria o pool usando InitializePool
            Transform poolParent = transform;
            objectPool.InitializePool(POOL_NAME, selectionPinPrefab, poolParent, initialPoolSize);
            Debug.Log($"[SelectionPinPool] Pool inicializado com {initialPoolSize} pins");
        }

        public GameObject GetPin(Vector3 position, string entityId)
        {
            if (objectPool == null)
            {
                Debug.LogError("[SelectionPinPool] ObjectPool não inicializado!");
                return null;
            }

            // Se já existe um pin ativo para esta entidade, retorna ele
            if (activePins.TryGetValue(entityId, out GameObject existingPin))
            {
                existingPin.transform.position = position;
                existingPin.SetActive(true);
                return existingPin;
            }

            // Obtém um pin do pool usando o método correto
            GameObject pin = objectPool.Get(POOL_NAME);
            if (pin != null)
            {
                pin.transform.position = position;
                pin.SetActive(true);
                activePins[entityId] = pin;
            }

            return pin;
        }

        public void ReturnPin(string entityId)
        {
            if (activePins.TryGetValue(entityId, out GameObject pin))
            {
                objectPool.Return(POOL_NAME, pin);
                activePins.Remove(entityId);
            }
        }

        public void ReturnAllPins()
        {
            foreach (var pin in activePins.Values)
            {
                if (pin != null)
                {
                    objectPool.Return(POOL_NAME, pin);
                }
            }
            activePins.Clear();
        }

        private void OnDestroy()
        {
            ReturnAllPins();
            if (_instance == this)
            {
                _instance = null;
            }
        }
    }
}
