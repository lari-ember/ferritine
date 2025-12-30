using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voxel {
    public class VoxelWorld : MonoBehaviour {
        [Header("Configurações do Mundo")]
        public Texture2D heightmap;
        public Material voxelMaterial;
        public float escalaVoxel = 0.1f; // 10cm

        [Header("Performance/LOD")]
        public int renderDistance = 8; // Chunks ao redor da câmera

        [Header("Preload")]
        public PreloadProfile config;
        public Transform cameraTransform;
        [SerializeField] private Transform chunksParent;

         // O "Cérebro" que conhece todos os pedaços de Curitiba
        private Dictionary<Vector2Int, Chunk> _chunks = new Dictionary<Vector2Int, Chunk>();
        // Armazena os GameObjects visuais (instanciados) por chunk
        private Dictionary<Vector2Int, GameObject> chunkObjects = new Dictionary<Vector2Int, GameObject>();
        // Marca chunks em criação para evitar duplicatas
        private HashSet<Vector2Int> creatingChunks = new HashSet<Vector2Int>();

        private Vector3 lastCameraPos;
        private int totalChunksX = 0;
        private int totalChunksZ = 0;

        void Start() {
            // Inicializa dados puros (mas não cria objetos visuais em massa)
            InicializarMundo();

            // Auto-wire CityLayer para facilitar testes
            CityLayer city = GetComponent<CityLayer>();
            if (city == null) city = gameObject.AddComponent<CityLayer>();
            city.Inicializar(this);

            // Preload defaults
            if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
            if (config == null) config = ScriptableObject.CreateInstance<PreloadProfile>();
            lastCameraPos = cameraTransform != null ? cameraTransform.position : Vector3.zero;

            // Calcula limites
            totalChunksX = heightmap.width / Chunk.Largura;
            totalChunksZ = heightmap.height / Chunk.Largura;

            // Garante que exista um parent para todos os chunks (evita que fiquem na raiz da cena)
            if (chunksParent == null) {
                GameObject parentGO = new GameObject("Chunks");
                parentGO.transform.SetParent(this.transform, false);
                chunksParent = parentGO.transform;
            }

            // Inicia rotina de atualização (não cria tudo de uma vez)
            StartCoroutine(UpdateChunksRoutine());
        }

        void InicializarMundo() {
            int chunksX = heightmap.width / Chunk.Largura;
            int chunksZ = heightmap.height / Chunk.Largura;

            // Apenas cria os dados em memória — objetos visuais serão instanciados sob demanda
            for (int x = 0; x < chunksX; x++) {
                for (int z = 0; z < chunksZ; z++) {
                    Vector2Int pos = new Vector2Int(x, z);
                    Chunk novoChunk = new Chunk(this, pos);
                    novoChunk.PopulateFromHeightmap(heightmap, x * Chunk.Largura, z * Chunk.Largura);
                    _chunks.Add(pos, novoChunk);
                }
            }
        }

        void CriarObjetoChunk(Chunk chunk) {
            GameObject chunkObj = new GameObject($"Chunk_{chunk.PosicaoNoMundo.x}_{chunk.PosicaoNoMundo.y}");
            chunkObj.transform.SetParent(chunksParent, false);
            chunkObj.transform.position = new Vector3(
                chunk.PosicaoNoMundo.x * Chunk.Largura * escalaVoxel,
                0,
                chunk.PosicaoNoMundo.y * Chunk.Largura * escalaVoxel
            );

            MeshFilter mf = chunkObj.AddComponent<MeshFilter>();
            MeshRenderer mr = chunkObj.AddComponent<MeshRenderer>();
            mr.material = voxelMaterial;

            mf.mesh = chunk.GenerateMesh();
            chunkObj.AddComponent<MeshCollider>().sharedMesh = mf.mesh;

            // Guarda referência
            chunkObjects[chunk.PosicaoNoMundo] = chunkObj;
        }

        IEnumerator UpdateChunksRoutine() {
            while (true) {
                if (cameraTransform != null && config != null) {
                    GerenciarVisibilidade();
                }
                yield return new WaitForSeconds(config != null ? config.intervaloChecagem : 0.5f);
            }
        }

        void GerenciarVisibilidade() {
            // Regra: Distância base + (Velocidade x Fator)
            float velocidade = 0f;
            if (cameraTransform != null) {
                velocidade = (cameraTransform.position - lastCameraPos).magnitude / (config != null ? config.intervaloChecagem : 0.5f);
            }
            float raioPreload = (config != null ? config.distanciaBase : 500f) + (velocidade * (config != null ? config.fatorVelocidade : 2f));
            lastCameraPos = cameraTransform != null ? cameraTransform.position : lastCameraPos;

            if (cameraTransform == null) return;

            int camChunkX = Mathf.FloorToInt(cameraTransform.position.x / (Chunk.Largura * escalaVoxel));
            int camChunkZ = Mathf.FloorToInt(cameraTransform.position.z / (Chunk.Largura * escalaVoxel));

            int raioChunks = Mathf.CeilToInt(raioPreload / (Chunk.Largura * escalaVoxel));

            // Percorre apenas a vizinhança ao redor da câmera
            for (int x = camChunkX - raioChunks; x <= camChunkX + raioChunks; x++) {
                for (int z = camChunkZ - raioChunks; z <= camChunkZ + raioChunks; z++) {
                    // Verifica limites
                    if (x < 0 || z < 0 || x >= totalChunksX || z >= totalChunksZ) continue;

                    Vector2Int pos = new Vector2Int(x, z);
                    // Distância real do centro do chunk
                    Vector3 worldPos = new Vector3(x * Chunk.Largura * escalaVoxel, 0, z * Chunk.Largura * escalaVoxel);
                    float dist = Vector3.Distance(cameraTransform.position, worldPos);

                    if (dist <= raioPreload) {
                        // Precisa ativar/instanciar
                        if (!chunkObjects.ContainsKey(pos) && !creatingChunks.Contains(pos)) {
                            StartCoroutine(CriarChunkGradualmente(pos));
                        } else if (chunkObjects.ContainsKey(pos) && chunkObjects[pos] != null) {
                            chunkObjects[pos].SetActive(true);
                        }
                    } else {
                        // Desativa se existir
                        if (chunkObjects.ContainsKey(pos) && chunkObjects[pos] != null) {
                            chunkObjects[pos].SetActive(false);
                        }
                    }
                }
            }
        }

        IEnumerator CriarChunkGradualmente(Vector2Int pos) {
            if (!_chunks.ContainsKey(pos)) yield break;
            creatingChunks.Add(pos);

            // Reserva espaço para evitar duplicatas
            chunkObjects.Add(pos, null);

            // Cria dados se necessário (já temos no _chunks)
            Chunk novoChunk = _chunks[pos];

            // Permite que Unity respire um frame
            yield return null;

            // Cria o objeto visual
            InstantiateChunkObject(novoChunk);

            creatingChunks.Remove(pos);
        }

        void InstantiateChunkObject(Chunk chunk) {
            // Cria GameObject e guarda em chunkObjects
            GameObject chunkObj = new GameObject($"Chunk_{chunk.PosicaoNoMundo.x}_{chunk.PosicaoNoMundo.y}");
            chunkObj.transform.SetParent(chunksParent, false);
            chunkObj.transform.position = new Vector3(
                chunk.PosicaoNoMundo.x * Chunk.Largura * escalaVoxel,
                0,
                chunk.PosicaoNoMundo.y * Chunk.Largura * escalaVoxel
            );

            MeshFilter mf = chunkObj.AddComponent<MeshFilter>();
            MeshRenderer mr = chunkObj.AddComponent<MeshRenderer>();
            mr.material = voxelMaterial;

            mf.mesh = chunk.GenerateMesh();
            chunkObj.AddComponent<MeshCollider>().sharedMesh = mf.mesh;

            chunkObjects[chunk.PosicaoNoMundo] = chunkObj;
        }

         // Função crucial: Busca um voxel em qualquer lugar do mundo
         public byte GetVoxelNoMundo(int x, int y, int z) {
             // Converte coordenada global para coordenada de chunk
             int cx = Mathf.FloorToInt(x / (float)Chunk.Largura);
             int cz = Mathf.FloorToInt(z / (float)Chunk.Largura);

             // Coordenada local dentro do chunk
             int lx = x - (cx * Chunk.Largura);
             int lz = z - (cz * Chunk.Largura);

             Vector2Int cPos = new Vector2Int(cx, cz);
            if (_chunks.ContainsKey(cPos)) {
                return _chunks[cPos].GetVoxelLocal(lx, y, lz);
             }
             return 0; // Ar se o chunk não existir
         }

         // Retorna a altura (em metros) do topo mais alto do solo não-ar na coluna (x,z)
         public float GetAlturaBase(int x, int z) {
             for (int y = Chunk.Altura - 1; y >= 0; y--) {
                 if (GetVoxelNoMundo(x, y, z) != (byte)BlockType.Ar) 
                     return y * escalaVoxel;
             }
             return 0;
         }
         
         // Retorna uma medida simples de inclinação entre (x,z) e (x+1,z) em "porcentagem" aproximada
         public float GetInclinacao(int x, int z) {
             float h1 = GetAlturaBase(x, z);
             float h2 = GetAlturaBase(x + 1, z);
             return Mathf.Abs(h1 - h2) * 100f; // Escala simples para comparação
         }
     }
 }
