using UnityEngine;

namespace Voxel {
    public class VoxelMap : MonoBehaviour {
        public Texture2D heightmap; // Arraste seu PNG de Curitiba aqui
        public Material voxelMaterial; // Crie um material simples e arraste aqui

        void Start() {
            if (heightmap == null) {
                Debug.LogError("Por favor, coloque o PNG do Heightmap no script VoxelMap!");
                return;
            }

            // Criar o objeto visual do Chunk
            GameObject chunkObj = new GameObject("Chunk_Curitiba_01");
            chunkObj.transform.parent = this.transform;

            MeshFilter meshFilter = chunkObj.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = chunkObj.AddComponent<MeshRenderer>();
            meshRenderer.material = voxelMaterial;

            // Executar a geração
            Chunk novoChunk = new Chunk();
            // Lendo os primeiros 32x32 pixels do PNG
            novoChunk.PopulateFromHeightmap(heightmap, 0, 0);
            meshFilter.mesh = novoChunk.GenerateMesh();

            // Adiciona um colisor para você poder clicar/quebrar depois
            chunkObj.AddComponent<MeshCollider>();
        }
    }
}
