using System.Collections.Generic;
using UnityEngine;

namespace Voxel {
    // DetailWorld é a camada visual passiva que materializa as decisões do CityLayer.
    // Responsabilidades:
    // - Escutar eventos de zoneamento (OnLoteChanged)
    // - Criar/remover visuais simples (cubos, prefabs) baseados no estado do lote
    // - Não altera o CityLayer nem o TerrainWorld
    public class DetailWorld : MonoBehaviour {
        [Header("Prefabs")]
        public GameObject prefabCasa;
        public GameObject prefabInvalido;
        // public ChunkPool pool; // Comentado para simplificar e compilar agora

        // Dicionário que mantém os objetos instanciados por coordenada
        private Dictionary<Vector2Int, GameObject> _detalhes = new Dictionary<Vector2Int, GameObject>();

        void OnEnable() {
            // O CityLayer emite OnLoteChanged quando um lote é pintado ou revalidado
            CityLayer.OnLoteChanged += AtualizarVisual;
        }

        void OnDisable() {
            CityLayer.OnLoteChanged -= AtualizarVisual;
        }

        // AtualizarVisual: Reactivo, chamado sempre que um lote muda.
        // O método decide qual prefab usar (ou remover tudo) e instancia/organiza na hierarquia.
        void AtualizarVisual(Lote data) {
            Vector2Int coord = data.pos;

            // 1. Remove visual anterior se houver
            if (_detalhes.ContainsKey(coord)) {
                var old = _detalhes[coord];
                if (old != null) Destroy(old);
                _detalhes.Remove(coord);
            }

            // 2. Se a zona for removida ou nula, paramos aqui
            if (data == null || data.zona == ZonaTipo.Nenhuma) return;

            // 3. Escolhe o prefab baseado na validade ou tipo (usa ZonaHelper para agrupar categorias)
            GameObject modelo = null;

            if (!data.estaValido) {
                modelo = prefabInvalido;
            } else if (ZonaHelper.IsResidential(data.zona)) {
                modelo = prefabCasa;
            } else if (ZonaHelper.IsCommercial(data.zona)) {
                // modelo = prefabComercial; // Exemplo futuro
            }

            // 4. Instancia o visual (posiciona no grid usando escala do terreno)
            if (modelo != null) {
                GameObject go = Instantiate(modelo);

                // Conversão de Grid para World Position
                // Nota: Verifique se o TerrainWorld.escalaVoxel é 0.1f ou 0.5f e ajuste aqui
                float scale = 0.5f; // Ajuste conforme seu TerrainWorld
                float worldX = coord.x * scale;
                float worldZ = coord.y * scale;

                // Pega a altura do terreno para o prédio não flutuar (Opcional, mas recomendado)
                // Se não tiver referência ao Terrain, use um valor fixo ou raycast
                float alturaY = 0;

                go.transform.position = new Vector3(worldX, alturaY, worldZ);
                go.transform.SetParent(this.transform); // Organiza na hierarquia

                _detalhes[coord] = go;
            }
        }
    }
}