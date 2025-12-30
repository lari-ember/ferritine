using System.Collections.Generic;
using UnityEngine;

namespace Voxel {
    public enum ZonaTipo { Invalido, Residencial, Comercial, Industrial, Parque, Via }

    [System.Serializable]
    public class Lote {
        public Vector2Int posicaoGlobal;
        public ZonaTipo zona;
        public bool estaValido;
        public string motivoInvalidez;
        
        // O DetailWorld usará isso para saber o que spawnar
        public int nivelDensidade; 
    }

    public class CityLayer : MonoBehaviour {
        private VoxelWorld _world;
        private Dictionary<Vector2Int, Lote> gradeUrbana = new Dictionary<Vector2Int, Lote>();

        public void Inicializar(VoxelWorld voxelWorld) {
            this._world = voxelWorld;
        }

        // A regra madura: CityLayer pergunta ao Terrain
        public void AvaliarLote(Vector2Int pos) {
            Lote novoLote = new Lote { posicaoGlobal = pos, zona = ZonaTipo.Residencial };
            
            // Consulta o mundo físico
            float inclinacao = _world.GetInclinacao(pos.x, pos.y);
            float alturaM = _world.GetAlturaBase(pos.x, pos.y);

            // Converte a altura (metros) para índice de voxel (y)
            int yIndex = Mathf.Clamp(Mathf.RoundToInt(alturaM / _world.escalaVoxel), 0, Chunk.Altura - 1);
            byte tipoSolo = _world.GetVoxelNoMundo(pos.x, yIndex, pos.y);

            if (inclinacao > 15f) {
                novoLote.estaValido = false;
                novoLote.motivoInvalidez = "Muito inclinado";
            } else if (tipoSolo == (byte)BlockType.Agua) {
                novoLote.estaValido = false;
                novoLote.motivoInvalidez = "Sobre a água";
            } else {
                novoLote.estaValido = true;
            }

            gradeUrbana[pos] = novoLote;
            
            // Notifica o DetailWorld que algo mudou (Event-Driven)
            OnZoneChanged?.Invoke(novoLote);
        }

        public delegate void ZoneAction(Lote lote);
        public static event ZoneAction OnZoneChanged;

        // Leitura externa da grade urbana
        public Lote GetLote(Vector2Int pos) {
            if (gradeUrbana.ContainsKey(pos)) return gradeUrbana[pos];
            return null;
        }
    }
}
