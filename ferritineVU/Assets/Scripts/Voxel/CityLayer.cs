using UnityEngine;
using System;
using System.Collections.Generic;

namespace Voxel {
    // CityLayer é a autoridade lógica para zoneamento urbano. Ela não cria visual nem altera o terreno.
    // Responsabilidades:
    // - Armazenar um dicionário esparso de lotes pintados pelo jogador
    // - Validar lotes consultando o TerrainWorld (autoridade física)
    // - Emitir eventos semânticos (OnLoteChanged) para o DetailWorld usar como entrada visual
    public class CityLayer : MonoBehaviour {
        public TerrainWorld terrain;
        private Dictionary<Vector2Int, Lote> _mapaUrbano = new Dictionary<Vector2Int, Lote>();

        // Evento público que notifica mudanças em lotes. DetailWorld e outros sistemas escutam isto.
        public static event Action<Lote> OnLoteChanged;

        // PintarZona: chamada quando o jogador pinta/edita um lote
        public void PintarZona(Vector2Int pos, ZonaTipo tipo) {
            if (!_mapaUrbano.ContainsKey(pos)) _mapaUrbano[pos] = new Lote { pos = pos };
            
            Lote lote = _mapaUrbano[pos];
            lote.zona = tipo;

            // Validação usando o TerrainWorld (Autoridade Física)
            float slope = terrain.GetSlope(pos.x, pos.y);
            SoilStats soil = terrain.GetSoilStats(pos.x, pos.y);

            // Regras compostas: inclinação + permeabilidade/erosão
            bool okSlope = slope < 0.5f; // 50cm threshold
            bool okSoil = soil.permeability > 0.05f && soil.erosionRate < 0.8f;

            lote.estaValido = okSlope && okSoil;
            if (!okSlope) lote.motivo = "Terreno muito inclinado";
            else if (!okSoil) lote.motivo = "Solo impróprio (erosao/permeabilidade)";
            else lote.motivo = "";

            // Notifica os ouvintes (ex: DetailWorld) que algo mudou no lote
            OnLoteChanged?.Invoke(lote);
        }

        // Revalida um lote existente (usado quando o TerrainWorld muda um local)
        public void RevalidarLote(Vector2Int pos) {
            if (!_mapaUrbano.ContainsKey(pos)) return;
            PintarZona(pos, _mapaUrbano[pos].zona);
        }
    }
}