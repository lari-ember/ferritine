using UnityEngine;

namespace Voxel {
    public class VoxelMap : MonoBehaviour {
        public Texture2D heightmap;
        public Material voxelMaterial;
        [SerializeField] private Transform chunksParent;

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

            GerarMapaCompleto();
        }

        void GerarMapaCompleto() {
            // Calcula quantos chunks cabem na imagem
            int chunksX = heightmap.width / Chunk.Largura;
            int chunksZ = heightmap.height / Chunk.Largura;

            for (int x = 0; x < chunksX; x++) {
                for (int z = 0; z < chunksZ; z++) {
                    // Calcula o pixel inicial deste chunk na imagem
                    int pixelX = x * Chunk.Largura;
                    int pixelZ = z * Chunk.Largura;

                    CriarChunk(x, z, pixelX, pixelZ);
                }
            }
        }

        void CriarChunk(int xID, int zID, int offsetX, int offsetZ) {
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
            novoChunk.PopulateFromHeightmap(heightmap, offsetX, offsetZ);
            
            Mesh mesh = novoChunk.GenerateMesh();
            meshFilter.mesh = mesh;

            // 5. Colisor
            chunkObj.AddComponent<MeshCollider>();
        }
    }
}
