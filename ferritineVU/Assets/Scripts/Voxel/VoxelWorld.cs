using UnityEngine;
using System.Collections.Generic;

namespace Voxel {
    /// <summary>
    /// Gerenciador principal do mundo de voxels.
    /// 
    /// Responsabilidades:
    /// - Carregamento/descarregamento dinâmico de chunks baseado na posição da câmera
    /// - Pool de GameObjects para reaproveitamento e redução de GC
    /// - Gerenciamento de memória com descarte progressivo de chunks distantes
    /// - Integração com TerrainWorld para dados de altura
    /// 
    /// Arquitetura:
    /// - _chunkObjects: chunks visuais ativos (GameObject com mesh)
    /// - _knownChunkData: dados de voxel em memória (byte[,,])
    /// - _chunkPool: pool de GameObjects inativos para reuso
    /// 
    /// Performance:
    /// - Usa PreloadProfile para configuração baseada em qualidade
    /// - Descarte progressivo evita picos de GC
    /// - Pool de objetos reduz alocações
    /// </summary>
    public class VoxelWorld : MonoBehaviour {
        
        #region Referências e Configuração
        
        [Header("Referências")]
        [Tooltip("Fonte de dados de altura do terreno")]
        public TerrainWorld terrain;
        
        [Tooltip("Container pai para os chunks instanciados")]
        public Transform terrainHolder;
        
        [Tooltip("Material aplicado a todos os chunks")]
        public Material voxelMaterial;
        
        [Header("Configuração de Carregamento")]
        [Tooltip("Perfil de configuração (opcional). Se não definido, usa os valores manuais abaixo.")]
        public PreloadProfile preloadProfile;
        
        [Tooltip("Raio em metros para preload (usado se preloadProfile = null)")]
        public float raioPreload = 200f;

        [Header("Limites de Performance (Manual)")]
        [Tooltip("Raio de retenção de dados em chunks (RAM)")]
        public int dadosRetencaoRadius = 2;
        
        [Tooltip("Chunks a descartar por frame (evita picos)")]
        public int dadosRetencaoBatchPerFrame = 32;
        
        [Tooltip("Intervalo entre checagens de descarte (segundos)")]
        public float unloadInterval = 0.5f;
        
        [Tooltip("Limite máximo de raio em chunks")]
        [Range(4, 64)] public int maxChunkRadius = 24;
        
        #endregion
        
        #region Estado Interno
        
        /// <summary>Chunks visuais ativos (posição → GameObject)</summary>
        private Dictionary<Vector2Int, GameObject> _chunkObjects = new Dictionary<Vector2Int, GameObject>();
        
        /// <summary>Referência à transform da câmera</summary>
        private Transform _cam;

        /// <summary>Controle para evitar spam de warnings</summary>
        private bool _terrainLayerWarningShown;

        /// <summary>Centro atual para cálculo de preload</summary>
        private Vector2Int _currentChunkCenter = new Vector2Int(int.MinValue, int.MinValue);

        /// <summary>Pool de GameObjects inativos</summary>
        private Stack<GameObject> _chunkPool = new Stack<GameObject>();
        
        /// <summary>Tamanho inicial do pool</summary>
        private const int PoolInitialSize = 32;
        
        /// <summary>Tamanho máximo do pool (evita uso excessivo de RAM)</summary>
        private const int PoolMaxSize = 128;
        
        #endregion

        #region Pool de Objetos
        
        /// <summary>
        /// Inicializa o pool de objetos de chunk para reaproveitamento.
        /// </summary>
        private void InicializarPool() {
            for (int i = 0; i < PoolInitialSize; i++) {
                var go = CriarChunkGameObject();
                go.SetActive(false);
                _chunkPool.Push(go);
            }
        }

        /// <summary>
        /// Obtém um GameObject do pool ou cria um novo se necessário.
        /// </summary>
        private GameObject ObterChunkDoPool() {
            if (_chunkPool.Count > 0) {
                var go = _chunkPool.Pop();
                go.SetActive(true);
                return go;
            }
            return CriarChunkGameObject();
        }

        /// <summary>
        /// Devolve um GameObject ao pool, desativando-o e limpando meshes.
        /// Se o pool estiver cheio, destrói o objeto para liberar memória.
        /// </summary>
        private void DevolverChunkAoPool(GameObject go) {
            if (go == null) return;
            
            // Limpa as meshes para liberar memória
            var mf = go.GetComponent<MeshFilter>();
            var mc = go.GetComponent<MeshCollider>();
            
            if (mf != null && mf.sharedMesh != null) {
                Destroy(mf.sharedMesh);
                mf.sharedMesh = null;
            }
            
            if (mc != null && mc.sharedMesh != null) {
                mc.sharedMesh = null; // MeshCollider compartilha a mesh do MeshFilter
            }
            
            // Se o pool já está no tamanho máximo, destrói o GameObject
            if (_chunkPool.Count >= PoolMaxSize) {
                Destroy(go);
                return;
            }
            
            go.SetActive(false);
            _chunkPool.Push(go);
        }

        /// <summary>
        /// Cria um novo GameObject de chunk (sem mesh/collider).
        /// </summary>
        private GameObject CriarChunkGameObject() {
            var go = new GameObject("Chunk_Pooled");
            go.transform.SetParent(terrainHolder);
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            go.AddComponent<MeshCollider>();
            return go;
        }
        
        #endregion

        #region Lifecycle

        void Start() {
            // Safe assignment: Camera.main can be null in edit mode or tests
            if (Camera.main != null) _cam = Camera.main.transform;
            InicializarPool();
            
            // Subscreve ao evento de mudança de voxel para regenerar meshes
            if (terrain != null) {
                terrain.OnVoxelChanged += OnVoxelModificado;
            }
            
            // Força carregamento inicial de chunks ao redor da origem ou da câmera
            // Isso garante que haja terreno visível imediatamente
            Vector2Int initialCenter = new Vector2Int(0, 0);
            if (_cam != null && terrain != null) {
                int camX = Mathf.FloorToInt(_cam.position.x / (ChunkData.Largura * terrain.escalaVoxel));
                int camZ = Mathf.FloorToInt(_cam.position.z / (ChunkData.Largura * terrain.escalaVoxel));
                initialCenter = new Vector2Int(camX, camZ);
            }
            
            _currentChunkCenter = initialCenter;
            AtualizarChunks(initialCenter);
            
            // Log de debug com informações do perfil
            if (preloadProfile != null) {
                Debug.Log($"[VoxelWorld] Inicializado com PreloadProfile.\n{preloadProfile.GetDebugInfo(terrain.escalaVoxel)}\nPool Max: {PoolMaxSize}, GC Interval: {_gcInterval}s");
            } else {
                int raio = GetRaioEmChunks();
                Debug.Log($"[VoxelWorld] Inicializado (modo manual). Centro: {initialCenter}, Escala: {terrain.escalaVoxel}m, Raio: {raio} chunks\nPool Max: {PoolMaxSize}, GC Interval: {_gcInterval}s");
            }
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
            
            // Verifica e regenera chunks com voxels modificados (IsDirty)
            RegenerarChunksSujos();

            // Periodicamente agendamos descarte de chunks distantes
            var (_, _, interval) = GetParametrosDescarte();
            if (Time.time - _lastUnloadTime >= interval) {
                _lastUnloadTime = Time.time;
                AgendarDescarte(_currentChunkCenter); // Usa o centro fixo
            }
            
            // Força coleta de lixo periódica se houver chunks descartados
            if (Time.time - _lastGCTime >= _gcInterval && _chunksDescartadosDesdeUltimoGC > 0) {
                _lastGCTime = Time.time;
                
                // Log de monitoramento ANTES do GC
                Debug.Log($"[VoxelWorld] Chunks Data: {_knownChunkData.Count}, " +
                          $"Fila Descarte: {_filaDescarteDados.Count}, " +
                          $"Visuais: {_chunkObjects.Count}, " +
                          $"Descartados: {_chunksDescartadosDesdeUltimoGC}");
                
                System.GC.Collect();
                _chunksDescartadosDesdeUltimoGC = 0;
            }
        }
        
        /// <summary>
        /// Verifica todos os chunks visíveis e regenera a mesh dos que foram modificados.
        /// Chamado a cada frame no Update.
        /// </summary>
        private void RegenerarChunksSujos() {
            // Lista temporária para evitar modificar dicionário durante iteração
            List<Vector2Int> chunksSujos = null;
            
            foreach (var kvp in _chunkObjects) {
                Vector2Int pos = kvp.Key;
                
                // Busca os dados do chunk no TerrainWorld
                // GetGarantirChunk retorna o chunk existente se já foi criado
                if (_knownChunkData.Contains(pos)) {
                    ChunkData dados = terrain.GetGarantirChunk(pos);
                    if (dados != null && dados.IsDirty) {
                        if (chunksSujos == null) {
                            chunksSujos = new List<Vector2Int>();
                        }
                        chunksSujos.Add(pos);
                    }
                }
            }
            
            // Regenera os chunks sujos
            if (chunksSujos != null) {
                foreach (var pos in chunksSujos) {
                    RegenerarMeshChunk(pos);
                }
            }
        }
        
        /// <summary>
        /// Regenera a mesh de um chunk específico.
        /// Chamado quando voxels são modificados via SetVoxelAt.
        /// </summary>
        private void RegenerarMeshChunk(Vector2Int pos) {
            if (!_chunkObjects.ContainsKey(pos)) return;
            
            ChunkData dados = terrain.GetGarantirChunk(pos);
            if (dados == null) return;
            
            GameObject obj = _chunkObjects[pos];
            var mf = obj.GetComponent<MeshFilter>();
            var mc = obj.GetComponent<MeshCollider>();
            
            // Destrói a mesh antiga para liberar memória
            if (mf.sharedMesh != null) {
                Destroy(mf.sharedMesh);
            }
            
            // Gera nova mesh
            Mesh novaMesh = ChunkMeshGenerator.BuildMesh(terrain, dados, terrain.escalaVoxel);
            mf.mesh = novaMesh;
            mc.sharedMesh = novaMesh;
            
            // Limpa a flag
            dados.IsDirty = false;
            
            Debug.Log($"[VoxelWorld] Mesh do chunk {pos} regenerada");
        }
        
        /// <summary>
        /// Handler chamado quando um voxel é modificado no TerrainWorld.
        /// Regenera imediatamente a mesh do chunk afetado.
        /// </summary>
        private void OnVoxelModificado(Vector3Int posicaoVoxel, byte novoValor) {
            // Calcula qual chunk contém este voxel
            int chunkX = Mathf.FloorToInt((float)posicaoVoxel.x / ChunkData.Largura);
            int chunkZ = Mathf.FloorToInt((float)posicaoVoxel.z / ChunkData.Largura);
            Vector2Int chunkPos = new Vector2Int(chunkX, chunkZ);
            
            // Regenera a mesh do chunk se ele estiver visível
            if (_chunkObjects.ContainsKey(chunkPos)) {
                RegenerarMeshChunk(chunkPos);
            }
            
            // Verifica se o voxel está na borda do chunk (pode afetar chunks vizinhos)
            int localX = posicaoVoxel.x - (chunkX * ChunkData.Largura);
            int localZ = posicaoVoxel.z - (chunkZ * ChunkData.Largura);
            
            // Borda X- (primeiro voxel do chunk)
            if (localX == 0) {
                var vizinho = new Vector2Int(chunkX - 1, chunkZ);
                if (_chunkObjects.ContainsKey(vizinho)) {
                    var dadosVizinho = terrain.GetGarantirChunk(vizinho);
                    if (dadosVizinho != null) dadosVizinho.IsDirty = true;
                }
            }
            // Borda X+ (último voxel do chunk)
            if (localX == ChunkData.Largura - 1) {
                var vizinho = new Vector2Int(chunkX + 1, chunkZ);
                if (_chunkObjects.ContainsKey(vizinho)) {
                    var dadosVizinho = terrain.GetGarantirChunk(vizinho);
                    if (dadosVizinho != null) dadosVizinho.IsDirty = true;
                }
            }
            // Borda Z-
            if (localZ == 0) {
                var vizinho = new Vector2Int(chunkX, chunkZ - 1);
                if (_chunkObjects.ContainsKey(vizinho)) {
                    var dadosVizinho = terrain.GetGarantirChunk(vizinho);
                    if (dadosVizinho != null) dadosVizinho.IsDirty = true;
                }
            }
            // Borda Z+
            if (localZ == ChunkData.Largura - 1) {
                var vizinho = new Vector2Int(chunkX, chunkZ + 1);
                if (_chunkObjects.ContainsKey(vizinho)) {
                    var dadosVizinho = terrain.GetGarantirChunk(vizinho);
                    if (dadosVizinho != null) dadosVizinho.IsDirty = true;
                }
            }
        }
        
        void OnDestroy() {
            // Desinscreve do evento
            if (terrain != null) {
                terrain.OnVoxelChanged -= OnVoxelModificado;
            }
        }
        
        #endregion
        
        #region Estado de Processamento
        
        private bool _estaProcessando;
        private Queue<Vector2Int> _filaDeCriacao = new Queue<Vector2Int>();

        // Rastreia quais chunk data foram garantidos (criamos ou solicitados)
        // Usado para decidir o que pode ser desalocado do TerrainWorld
        private HashSet<Vector2Int> _knownChunkData = new HashSet<Vector2Int>();

        // Fila de descarte de dados (chunk positions) e controle de processamento
        private Queue<Vector2Int> _filaDescarteDados = new Queue<Vector2Int>();
        private HashSet<Vector2Int> _dadosAgendadosDescarte = new HashSet<Vector2Int>();
        private bool _descarteProcessando;

        private float _lastUnloadTime;
        private float _lastGCTime;
        private float _gcInterval = 5f; // Força GC a cada 5 segundos
        private int _chunksDescartadosDesdeUltimoGC;
        
        #endregion
        
        #region Gerenciamento de Chunks

        /// <summary>
        /// Calcula o raio em chunks, usando PreloadProfile se disponível, senão usa cálculo manual.
        /// </summary>
        private int GetRaioEmChunks() {
            if (preloadProfile != null) {
                return preloadProfile.CalcularRaioEmChunks(terrain.escalaVoxel);
            }
            
            // Fallback: cálculo manual
            int raioCalculado = Mathf.CeilToInt(raioPreload / (ChunkData.Largura * terrain.escalaVoxel));
            return Mathf.Min(raioCalculado, maxChunkRadius);
        }
        
        /// <summary>
        /// Retorna os parâmetros de descarte, usando PreloadProfile se disponível.
        /// </summary>
        private (int retencaoRadius, int batchPerFrame, float interval) GetParametrosDescarte() {
            if (preloadProfile != null) {
                return (preloadProfile.dadosRetencaoRadius, 
                        preloadProfile.dadosRetencaoBatchPerFrame, 
                        preloadProfile.unloadInterval);
            }
            return (dadosRetencaoRadius, dadosRetencaoBatchPerFrame, unloadInterval);
        }

        // AtualizarChunks: determina quais chunks precisam existir visualmente e enfileira
        // a criação para ser processada aos poucos (evita travamentos ao criar muitos objetos em um frame)
        void AtualizarChunks(Vector2Int centro) {
            int raioEmChunks = GetRaioEmChunks();
            
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

        /// <summary>
        /// Cria ou reaproveita o GameObject do chunk, gera a mesh via ChunkMeshGenerator e adiciona collider.
        /// </summary>
        /// <param name="p">Posição do chunk</param>
        private void CriarObjetoChunk(Vector2Int p) {
            // Garante que os dados do chunk existam
            ChunkData dados = terrain.GetGarantirChunk(p);

            // REMOVIDO: Pré-carregamento dos 4 vizinhos
            // Isso causava vazamento de memória pois os vizinhos não eram registrados
            // em _knownChunkData e nunca eram descarregados
            
            // Cancela qualquer descarte agendado para este chunk (evita remoção de dados que voltou a ser visível)
            CancelScheduledDiscard(p);

            // Marca que temos dados deste chunk (para posterior descarte em lote)
            _knownChunkData.Add(p);

            GameObject obj = ObterChunkDoPool();
            obj.name = $"Chunk_{p.x}_{p.y}";
            obj.transform.SetParent(terrainHolder);
            obj.transform.position = new Vector3(p.x * ChunkData.Largura * terrain.escalaVoxel, 0, p.y * ChunkData.Largura * terrain.escalaVoxel);

            // Configura a layer "Terrain" para permitir raycasting com Physics
            int terrainLayerIndex = LayerMask.NameToLayer("Terrain");
            if (terrainLayerIndex != -1) {
                obj.layer = terrainLayerIndex;
            } else {
                // Fallback: criar log apenas uma vez para evitar spam
                if (!_terrainLayerWarningShown) {
                    Debug.LogWarning("[VoxelWorld] Layer 'Terrain' não encontrada. Crie-a em Edit > Project Settings > Tags and Layers para habilitar raycasting de voxels via Physics.");
                    _terrainLayerWarningShown = true;
                }
            }

            var mf = obj.GetComponent<MeshFilter>();
            var mr = obj.GetComponent<MeshRenderer>();
            var mc = obj.GetComponent<MeshCollider>();
            mf.mesh = ChunkMeshGenerator.BuildMesh(terrain, dados, terrain.escalaVoxel);
            // --- Propaga o material do TerrainWorld ---
            if (terrain.voxelMaterial != null) {
                mr.material = terrain.voxelMaterial;
            } else {
                Debug.LogWarning("VoxelWorld: Material do voxel não atribuído no TerrainWorld. Chunk ficará sem material.");
            }
            mc.sharedMesh = mf.mesh;

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
            int raioEmChunks = GetRaioEmChunks();
            int visualDiscard = raioEmChunks + 2; // Histerese: descarte só além do preload+2
            
            var (retencaoRadius, _, _) = GetParametrosDescarte();

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
                if (dist > retencaoRadius) {
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

        /// <summary>
        /// Remove o GameObject visual do chunk e devolve ao pool.
        /// </summary>
        /// <param name="pos">Posição do chunk</param>
        private void DestruirVisualChunk(Vector2Int pos) {
            if (_chunkObjects.ContainsKey(pos)) {
                var go = _chunkObjects[pos];
                if (go != null) DevolverChunkAoPool(go);
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
                var (_, batchPerFrame, _) = GetParametrosDescarte();
                int batch = Mathf.Min(batchPerFrame, _filaDescarteDados.Count);

                for (int i = 0; i < batch; i++) {
                    Vector2Int pos = _filaDescarteDados.Dequeue();
                    _dadosAgendadosDescarte.Remove(pos);

                    // CORREÇÃO: Remove dados INCONDICIONALMENTE
                    // Se o chunk voltou a ser visível, ele será re-carregado quando necessário
                    // Isso evita o acúmulo infinito de dados em RAM
                    terrain.RemoveChunkData(pos);
                    _knownChunkData.Remove(pos);
                    _chunksDescartadosDesdeUltimoGC++;
                }

                // Espera 1 frame entre batches para não travar
                yield return null;
            }

            _descarteProcessando = false;
        }

        /// <summary>
        /// Retorna o dicionário de chunks visíveis para debug visual.
        /// </summary>
        /// <returns>Dicionário de chunks visuais ativos</returns>
        public Dictionary<Vector2Int, GameObject> GetChunkObjects() { return _chunkObjects; }
        
        /// <summary>
        /// Retorna estatísticas de uso de memória do VoxelWorld.
        /// </summary>
        public string GetMemoryStats() {
            int chunksVisuais = _chunkObjects.Count;
            int chunksDados = _knownChunkData.Count;
            int poolSize = _chunkPool.Count;
            int filaDescarte = _filaDescarteDados.Count;
            int filaCriacao = _filaDeCriacao.Count;
            
            long memoryUsed = System.GC.GetTotalMemory(false) / (1024 * 1024); // MB
            
            return $"[VoxelWorld Memory Stats]\n" +
                   $"Chunks Visuais: {chunksVisuais}\n" +
                   $"Chunks Dados (RAM): {chunksDados}\n" +
                   $"Pool Size: {poolSize}/{PoolMaxSize}\n" +
                   $"Fila Descarte: {filaDescarte}\n" +
                   $"Fila Criação: {filaCriacao}\n" +
                   $"Memória Total: ~{memoryUsed} MB";
        }
        
        #endregion
    }
}