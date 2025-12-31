using System.Collections.Generic;
using UnityEngine;

namespace Voxel {
    // Pool simples para GameObject de chunk e Mesh reuse
    public class ChunkPool : MonoBehaviour {
        public static ChunkPool Instance { get; private set; }
        public GameObject chunkPrefab; // opcional: prefab com MeshFilter/Renderer
        public Transform poolParent;

        private Stack<GameObject> _pooledObjects = new Stack<GameObject>();

        void Awake() {
            if (Instance == null) Instance = this;
            else Destroy(this);
            if (poolParent == null) {
                GameObject p = new GameObject("ChunkPool");
                p.transform.SetParent(this.transform, false);
                poolParent = p.transform;
            }
        }

        public GameObject Rent(string goName) {
            GameObject go = null;
            if (_pooledObjects.Count > 0) {
                go = _pooledObjects.Pop();
                go.SetActive(true);
                go.name = goName;
            } else {
                if (chunkPrefab != null) go = Instantiate(chunkPrefab, poolParent);
                else go = new GameObject(goName);
            }
            return go;
        }

        public void Return(GameObject go) {
            if (go == null) return;
            go.SetActive(false);
            go.transform.SetParent(poolParent, false);
            _pooledObjects.Push(go);
        }
    }
}
