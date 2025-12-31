/*using UnityEngine;

namespace Voxel {
    public class VoxelMap : MonoBehaviour {
        public Texture2D heightmap;
        public Material voxelMaterial;
        [SerializeField] private Transform chunksParent;
        [Header("Dangerous")]
        [Tooltip("Se verdadeiro, GerarMapaCompleto() rodará no Start. Deixe false para evitar alocar todo o mundo na RAM.")]
        public bool gerarNoStart = false;

        void Start() {
            if (heightmap == null) {
                Debug.LogError("Heightmap não encontrado!");
                return;
            }

            // Garante que exista um parent para os chunks gerados por este script
            if (chunksParent == null) {
                GameObject parentGO = new GameObject("Chunks");
                parentGO.transform.SetParent(this.transform, false);
                chunksParent = parentGO.transform;
            }

            if (!gerarNoStart) {
                Debug.LogWarning("VoxelMap: gerarNoStart está false — GerarMapaCompleto() não será executado automaticamente.");
                return;
            }

            GerarMapaCompleto();
        }

        void GerarMapaCompleto() {
            // Calcula quantos chunks cabem na imagem
            int chunksX = heightmap.width / Chunk.Largura;
            int chunksZ = heightmap.height / Chunk.Largura;

            // Cache do heightmap (lido uma vez)
            Color32[] cache = heightmap.GetPixels32();
            int mapW = heightmap.width;
            int mapH = heightmap.height;

            for (int x = 0; x < chunksX; x++) {
                for (int z = 0; z < chunksZ; z++) {
                    // Calcula o pixel inicial deste chunk na imagem
                    int pixelX = x * Chunk.Largura;
                    int pixelZ = z * Chunk.Largura;

                    CriarChunk(x, z, pixelX, pixelZ, cache, mapW, mapH);
                }
            }
        }

        void CriarChunk(int xID, int zID, int offsetX, int offsetZ, Color32[] cache, int mapW, int mapH) {
            // 1. Criar o GameObject
            GameObject chunkObj = new GameObject($"Chunk_{xID}_{zID}");
            chunkObj.transform.SetParent(chunksParent, false);

            // 2. Posicionar o Chunk no mundo 3D (EscalaVoxel * Tamanho do Chunk)
            float worldX = xID * Chunk.Largura * 0.1f; 
            float worldZ = zID * Chunk.Largura * 0.1f;
            chunkObj.transform.position = new Vector3(worldX, 0, worldZ);

            // 3. Adicionar Componentes
            MeshFilter meshFilter = chunkObj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = chunkObj.AddComponent<MeshRenderer>();
            meshRenderer.material = voxelMaterial;

            // 4. Gerar Dados e Mesh
            // Criamos o Chunk sem um VoxelWorld (null) — GetVoxel trata world nulo como AR.
            Chunk novoChunk = new Chunk(null, new Vector2Int(xID, zID));
            novoChunk.PopulateFromHeightmapCache(cache, mapW, mapH, offsetX, offsetZ);
            
            Mesh mesh = novoChunk.GenerateMesh();
            meshFilter.mesh = mesh;

            // 5. Colisor
            var collider = chunkObj.AddComponent<MeshCollider>();
            collider.sharedMesh = mesh;
        }
    }
}
*/