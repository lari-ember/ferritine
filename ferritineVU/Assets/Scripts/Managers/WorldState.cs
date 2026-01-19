using System;
using UnityEngine;
using API.Models;

namespace Managers
{
    public class WorldStateManager : MonoBehaviour
    {
        public static WorldStateManager Instance { get; private set; }

        public event Action<WorldState> OnWorldStateChanged;

        private WorldState _currentWorldState;

        public WorldState CurrentWorldState
        {
            get { return _currentWorldState.Clone(); }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _currentWorldState = new WorldState();
        }

        public void SetWorldState(WorldState newState)
        {
            _currentWorldState = newState;
            OnWorldStateChanged?.Invoke(_currentWorldState.Clone());
        }
    }
}
