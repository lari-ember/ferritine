using UnityEngine;
using System.Collections.Generic;

namespace Voxel {
    public class VoxelWorld : MonoBehaviour {
        public TerrainWorld terrain;
        public Transform terrainHolder;
        public Material voxelMaterial;
        public float raioPreload = 200f;

        // Quantos chunks manter como dados em RAM ao redor da câmera (em chunks)
        public int dadosRetencaoRadius = 4; // padrão pedido
        // Quantos chunks de dados descartar por frame para evitar picos
        public int dadosRetencaoBatchPerFrame = 4; // padrão pedido
        // Intervalo entre checagens de descarte (segundos)
        public float unloadInterval = 1.0f;

        private Dictionary<Vector2Int, GameObject> _chunkObjects = new Dictionary<Vector2Int, GameObject>();
        private Transform _cam;

        // Centro do chunk atualmente considerado para preload
        private Vector2Int _currentChunkCenter = new Vector2Int(int.MinValue, int.MinValue);

        void Start() {
            // Safe assignment: Camera.main can be null in edit mode or tests
            if (Camera.main != null) _cam = Camera.main.transform;
        }

        void Update() {
            if (_cam == null || terrain == null) return; // safety guard

            // Calcula em qual chunk a câmera está
            int camX = Mathf.FloorToInt(_cam.position.x / (ChunkData.Largura * terrain.escalaVoxel));
            int camZ = Mathf.FloorToInt(_cam.position.z / (ChunkData.Largura * terrain.escalaVoxel));
            var center = new Vector2Int(camX, camZ);

            // Só atualiza se a câmera mudou de chunk
            if (center != _currentChunkCenter) {
                _currentChunkCenter = center;
                AtualizarChunks(center);
            }

            // Periodicamente agendamos descarte de chunks distantes
            if (Time.time - _lastUnloadTime >= unloadInterval) {
                _lastUnloadTime = Time.time;
                AgendarDescarte(_currentChunkCenter); // Usa o centro fixo
            }
        }
        
        private bool _estaProcessando = false;
        private Queue<Vector2Int> _filaDeCriacao = new Queue<Vector2Int>();

        // Rastreia quais chunk data foram garantidos (criamos ou solicitados)
        // Usado para decidir o que pode ser desalocado do TerrainWorld
        private HashSet<Vector2Int> _knownChunkData = new HashSet<Vector2Int>();

        // Fila de descarte de dados (chunk positions) e controle de processamento
        private Queue<Vector2Int> _filaDescarteDados = new Queue<Vector2Int>();
        private HashSet<Vector2Int> _dadosAgendadosDescarte = new HashSet<Vector2Int>();
        private bool _descarteProcessando = false;

        private float _lastUnloadTime = 0f;

        // AtualizarChunks: determina quais chunks precisam existir visualmente e enfileira
        // a criação para ser processada aos poucos (evita travamentos ao criar muitos objetos em um frame)
        void AtualizarChunks(Vector2Int centro) {
            int raioEmChunks = Mathf.CeilToInt(raioPreload / (ChunkData.Largura * terrain.escalaVoxel));
            // Carrega apenas dentro do raio de preload
            for (int x = -raioEmChunks; x <= raioEmChunks; x++) {
                for (int z = -raioEmChunks; z <= raioEmChunks; z++) {
                    Vector2Int p = centro + new Vector2Int(x, z);
                    if (!_chunkObjects.ContainsKey(p) && !_filaDeCriacao.Contains(p)) {
                        _filaDeCriacao.Enqueue(p); // Adiciona na fila em vez de criar na hora
                    }
                }
            }
            if (!_estaProcessando && _filaDeCriacao.Count > 0) {
                StartCoroutine(ProcessarFila());
            }
        }

        // Processa a fila de criação um por frame. Isto é uma estratégia simples de batch
        // para distribuir a carga de criação de meshes/objetos ao longo do tempo.
        System.Collections.IEnumerator ProcessarFila() {
            _estaProcessando = true;
            while (_filaDeCriacao.Count > 0) {
                Vector2Int p = _filaDeCriacao.Dequeue();

                // Se já existe (pode ter sido criado enquanto aguardava), pule
                if (_chunkObjects.ContainsKey(p)) continue;

                CriarObjetoChunk(p);
                yield return null; // Espera o próximo frame
            }
            _estaProcessando = false;
        }

        // Cria o GameObject do chunk, gera a mesh via ChunkMeshGenerator e adiciona collider.
        void CriarObjetoChunk(Vector2Int p) {
            // Garante que os dados do chunk existam
            ChunkData dados = terrain.GetGarantirChunk(p);

            terrain.GetGarantirChunk(new Vector2Int(p.x - 1, p.y));
            terrain.GetGarantirChunk(new Vector2Int(p.x + 1, p.y));
            terrain.GetGarantirChunk(new Vector2Int(p.x, p.y - 1));
            terrain.GetGarantirChunk(new Vector2Int(p.x, p.y + 1));
            // Cancela qualquer descarte agendado para este chunk (evita remoção de dados que voltou a ser visível)
            CancelScheduledDiscard(p);

            // Marca que temos dados deste chunk (para posterior descarte em lote)
            _knownChunkData.Add(p);

            GameObject obj = new GameObject($"Chunk_{p.x}_{p.y}");
            obj.transform.SetParent(terrainHolder);
            obj.transform.position = new Vector3(p.x * ChunkData.Largura * terrain.escalaVoxel, 0, p.y * ChunkData.Largura * terrain.escalaVoxel);

            MeshFilter mf = obj.AddComponent<MeshFilter>();
            mf.mesh = ChunkMeshGenerator.BuildMesh(terrain, dados, terrain.escalaVoxel);
            obj.AddComponent<MeshRenderer>().material = voxelMaterial;
            obj.AddComponent<MeshCollider>().sharedMesh = mf.mesh;

            _chunkObjects.Add(p, obj);
        }

        // Cancela um descarte previamente agendado para um chunk
        void CancelScheduledDiscard(Vector2Int pos) {
            if (_dadosAgendadosDescarte.Contains(pos)) _dadosAgendadosDescarte.Remove(pos);
            if (_filaDescarteDados.Count == 0) return;

            // Rebuild queue without pos
            var q = new Queue<Vector2Int>(_filaDescarteDados.Count);
            while (_filaDescarteDados.Count > 0) {
                var item = _filaDescarteDados.Dequeue();
                if (item != pos) q.Enqueue(item);
            }
            _filaDescarteDados = q;
        }

        // AgendarDescarte: calcula quais chunks estão fora do raio visual e do raio de retenção de dados,
        // remove visuais imediatamente e agenda dados para descarte em lote.
        void AgendarDescarte(Vector2Int centro) {
            int raioEmChunks = Mathf.CeilToInt(raioPreload / (ChunkData.Largura * terrain.escalaVoxel));
            int visualDiscard = raioEmChunks + 2; // Histerese: descarte só além do preload+2

            // 1) Remover visuais (GameObjects) que estão muito longe
            var visualsToRemove = new List<Vector2Int>();
            foreach (var kvp in _chunkObjects) {
                Vector2Int pos = kvp.Key;
                float dist = Vector2Int.Distance(pos, centro);
                if (dist > visualDiscard) {
                    visualsToRemove.Add(pos);
                }
            }
            foreach (var pos in visualsToRemove) {
                DestruirVisualChunk(pos);
            }

            // 2) Agendar remoção de dados para chunks que estão fora do raio de retenção
            foreach (var pos in _knownChunkData) {
                float dist = Vector2Int.Distance(pos, centro);
                if (dist > dadosRetencaoRadius) {
                    if (!_dadosAgendadosDescarte.Contains(pos)) {
                        _dadosAgendadosDescarte.Add(pos);
                        _filaDescarteDados.Enqueue(pos);
                    }
                }
            }

            // Inicia o processo de descarte em lote se houver itens
            if (!_descarteProcessando && _filaDescarteDados.Count > 0) {
                StartCoroutine(ProcessarDescarteDados());
            }
        }

        // Destrói o GameObject visual do chunk (mantém os dados na memória)
        void DestruirVisualChunk(Vector2Int pos) {
            if (_chunkObjects.ContainsKey(pos)) {
                var go = _chunkObjects[pos];
                if (go != null) Destroy(go);
                _chunkObjects.Remove(pos);

                // Se estava na fila de criação, removemos a entrada para não recriar desnecessariamente
                if (_filaDeCriacao.Contains(pos)) {
                    // Rebuild a new queue without pos
                    var q = new Queue<Vector2Int>(_filaDeCriacao.Count);
                    while (_filaDeCriacao.Count > 0) {
                        var item = _filaDeCriacao.Dequeue();
                        if (item != pos) q.Enqueue(item);
                    }
                    _filaDeCriacao = q;
                }
            }
        }

        // Processa a fila de descarte de dados em lotes (batch) para evitar GC spikes
        System.Collections.IEnumerator ProcessarDescarteDados() {
            _descarteProcessando = true;

            while (_filaDescarteDados.Count > 0) {
                int batch = Mathf.Min(dadosRetencaoBatchPerFrame, _filaDescarteDados.Count);

                for (int i = 0; i < batch; i++) {
                    Vector2Int pos = _filaDescarteDados.Dequeue();
                    _dadosAgendadosDescarte.Remove(pos);

                    // Se o visual reapareceu, cancela o descarte desse dado
                    if (_chunkObjects.ContainsKey(pos)) {
                        // ainda está visível, reter dados
                        continue;
                    }

                    // Remove os dados do TerrainWorld (free RAM)
                    terrain.RemoveChunkData(pos);
                    _knownChunkData.Remove(pos);

                    // Força uma pequena espera para distribuir a carga
                }

                // Espera 1 frame entre batches para não travar
                yield return null;
            }

            _descarteProcessando = false;
        }
    }
}