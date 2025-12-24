// filepath: /home/larisssa/Documentos/codigos/ferritine/ferritineVU/Assets/Scripts/Entities/Agent3D.cs
using UnityEngine;

namespace Entities
{
    /// <summary>
    /// Agent3D: Lightweight component that syncs AgentData into presentation components.
    /// - Forwards status to AgentAnimator
    /// - Forwards data to SelectableEntity
    /// - Optionally caches VehicleMover for future extensions
    /// </summary>
    public class Agent3D : MonoBehaviour
    {
        private AgentAnimator _agentAnimator;
        private SelectableEntity _selectableEntity;
        private VehicleMover _mover;

        /// <summary>
        /// Last received AgentData snapshot.
        /// </summary>
        public AgentData CurrentData { get; private set; }

        void Awake()
        {
            _agentAnimator = GetComponent<AgentAnimator>();
            _selectableEntity = GetComponent<SelectableEntity>();
            _mover = GetComponent<VehicleMover>();
        }

        /// <summary>
        /// Updates this agent's runtime state based on backend data.
        /// </summary>
        public void UpdateAgentData(AgentData data)
        {
            if (data == null) return;
            CurrentData = data;

            // Ensure component references
            if (_agentAnimator == null) _agentAnimator = GetComponent<AgentAnimator>();
            if (_selectableEntity == null) _selectableEntity = GetComponent<SelectableEntity>();
            if (_mover == null) _mover = GetComponent<VehicleMover>();

            // Forward status to animator
            if (_agentAnimator != null)
            {
                _agentAnimator.UpdateStatus(data.status);
            }

            // Forward data to selectable entity (keeps inspector/UI in sync)
            if (_selectableEntity != null)
            {
                _selectableEntity.UpdateData(data);
            }

            // Movement is driven externally by WorldController via VehicleMover.targetPosition.
            // We keep the mover cached for future use (e.g., speed adjustments by status).
        }
    }
}
