using UnityEngine;

namespace Voxel {
    [CreateAssetMenu(fileName = "PreloadProfile", menuName = "Voxel/Preload Profile")]
    public class PreloadProfile : ScriptableObject {
        public float distanciaBase = 500f; // Metros
        public float fatorVelocidade = 0.5f; // Multiplicador para a projeção da câmera
        public float intervaloChecagem = 0.5f; // Segundos entre cada atualização de visibilidade
    }
}

