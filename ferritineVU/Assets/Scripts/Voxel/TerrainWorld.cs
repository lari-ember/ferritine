using UnityEngine;
using System.Collections.Generic;

namespace Voxel {
    public class TerrainWorld : MonoBehaviour {
        [Header("Configuração do Mapa")]
        public Texture2D heightmap; // Arraste seu PNG de Curitiba aqui
        public Material voxelMaterial; // Material do voxel, atribua no Inspector
        
        [Header("Escala do Terreno")]
        [Tooltip("Tamanho de cada voxel em metros. Exemplo: 0.5 = 50cm, 0.1 = 10cm")]
        public float escalaVoxel = 0.5f;
        
        [Tooltip("Escala base de referência. Cada pixel do heightmap representa esta medida em metros.")]
        public float escalaBaseMetros = 0.5f; // 1 pixel = 50cm no mundo real
        
        [Tooltip("Altura máxima do mundo em metros")]
        public float alturaMaximaMetros = 128f; // 128 metros de altura máxima

        /// <summary>
        /// Fator de densidade: quantos voxels são necessários para cobrir 1 pixel do heightmap.
        /// Se escalaVoxel = 0.1f e escalaBase = 0.5f, fator = 5 (5 voxels por pixel).
        /// </summary>
        public int FatorDensidade => Mathf.Max(1, Mathf.RoundToInt(escalaBaseMetros / escalaVoxel));
        
        /// <summary>
        /// Altura máxima em voxels (convertido de metros para número de blocos).
        /// </summary>
        public int AlturaMaximaVoxels => Mathf.RoundToInt(alturaMaximaMetros / escalaVoxel);

        // Cache de dados (Privado)
        private Color32[] _hCache;
        private int _mW, _mH;
        private Dictionary<Vector2Int, ChunkData> _chunks = new Dictionary<Vector2Int, ChunkData>();

        void Awake() {
            if (heightmap == null) {
                Debug.LogError("TerrainWorld: Heightmap não atribuído!");
                return;
            }
            if (voxelMaterial == null) {
                Debug.LogError("TerrainWorld: Material do voxel não atribuído! Por favor, atribua no Inspector.");
            }
            
            // Valida configuração de escala
            if (escalaVoxel <= 0) {
                Debug.LogError("TerrainWorld: escalaVoxel deve ser maior que 0!");
                escalaVoxel = 0.5f;
            }
            
            Debug.Log($"[TerrainWorld] Inicializando com escala {escalaVoxel}m ({escalaVoxel * 100}cm). " +
                      $"Fator de densidade: {FatorDensidade}x. Altura máxima: {AlturaMaximaVoxels} voxels ({alturaMaximaMetros}m)");
            
            // Prepara o cache para leitura rápida
            _mW = heightmap.width;
            _mH = heightmap.height;
            _hCache = heightmap.GetPixels32();
            
            // Debug: mostra informações do heightmap e amostra alguns valores
            Debug.Log($"[TerrainWorld] Heightmap carregado: {_mW}x{_mH} pixels, {_hCache.Length} amostras");
            
            // Amostra cantos do heightmap para verificar se está sendo lido corretamente
            if (_hCache.Length > 0) {
                int centerIdx = (_mH / 2) * _mW + (_mW / 2);
                Debug.Log($"[TerrainWorld] Amostras - Canto(0,0): R={_hCache[0].r}, " +
                          $"Centro: R={_hCache[centerIdx].r}, " +
                          $"Último: R={_hCache[_hCache.Length - 1].r}");
            }
        }

        /// <summary>
        /// Retorna o valor do voxel em coordenadas GLOBAIS (mundo)
        /// Busca no chunk correspondente se ele existir
        /// </summary>
        public byte GetVoxelAt(int globalX, int globalY, int globalZ) {
            // Calcula qual chunk contém esta coordenada
            int chunkX = Mathf.FloorToInt((float)globalX / ChunkData.Largura);
            int chunkZ = Mathf.FloorToInt((float)globalZ / ChunkData.Largura);
            Vector2Int chunkPos = new Vector2Int(chunkX, chunkZ);
    
            // Verifica se o chunk existe no cache
            if (_chunks.ContainsKey(chunkPos)) {
                // Converte coordenada global para local do chunk
                int localX = globalX - (chunkX * ChunkData.Largura);
                int localZ = globalZ - (chunkZ * ChunkData.Largura);
        
                // Verifica limites
                int maxY = Mathf.Min(AlturaMaximaVoxels, 512);
                if (localX >= 0 && localX < ChunkData.Largura && 
                    globalY >= 0 && globalY < maxY && 
                    localZ >= 0 && localZ < ChunkData.Largura) {
                    return _chunks[chunkPos].voxels[localX, globalY, localZ];
                }
            }
    
            // Se o chunk não existe, usa o heightmap como fallback
            int maxHeight = Mathf.Min(AlturaMaximaVoxels, 512);
            if (globalY < 0 || globalY >= maxHeight) return 0;
    
            float heightMeters = GetHeight(globalX, globalZ);
            int heightVoxels = Mathf.FloorToInt(heightMeters / escalaVoxel);
    
            return (globalY < heightVoxels) ? (byte)1 : (byte)0;
        }

        /// <summary>
        /// Define o valor de um voxel em coordenadas GLOBAIS (mundo).
        /// Cria o chunk se não existir. Notifica observadores da mudança.
        /// </summary>
        /// <param name="globalX">Coordenada X global</param>
        /// <param name="globalY">Coordenada Y global (altura)</param>
        /// <param name="globalZ">Coordenada Z global</param>
        /// <param name="value">Novo valor do voxel (BlockType)</param>
        /// <returns>True se a modificação foi bem-sucedida</returns>
        public bool SetVoxelAt(int globalX, int globalY, int globalZ, byte value) {
            // Calcula qual chunk contém esta coordenada
            int chunkX = Mathf.FloorToInt((float)globalX / ChunkData.Largura);
            int chunkZ = Mathf.FloorToInt((float)globalZ / ChunkData.Largura);
            Vector2Int chunkPos = new Vector2Int(chunkX, chunkZ);
            
            // Garante que o chunk existe
            ChunkData chunk = GetGarantirChunk(chunkPos);
            
            // Converte coordenada global para local do chunk
            int localX = globalX - (chunkX * ChunkData.Largura);
            int localZ = globalZ - (chunkZ * ChunkData.Largura);
            
            // Verifica limites
            int maxY = Mathf.Min(AlturaMaximaVoxels, 512);
            if (localX < 0 || localX >= ChunkData.Largura || 
                globalY < 0 || globalY >= maxY || 
                localZ < 0 || localZ >= ChunkData.Largura) {
                Debug.LogWarning($"[TerrainWorld] SetVoxelAt: Coordenadas fora dos limites ({globalX}, {globalY}, {globalZ})");
                return false;
            }
            
            // Define o voxel
            chunk.voxels[localX, globalY, localZ] = value;
            
            // Marca o chunk como modificado (para regenerar mesh)
            chunk.IsDirty = true;
            
            // Notifica observers
            OnVoxelChanged?.Invoke(new Vector3Int(globalX, globalY, globalZ), value);
            
            return true;
        }
        
        /// <summary>
        /// Evento disparado quando um voxel é modificado.
        /// Útil para sistemas que precisam reagir a mudanças no terreno.
        /// </summary>
        public event System.Action<Vector3Int, byte> OnVoxelChanged;

        // Retorna a altura em metros numa posição (X, Z) em coordenadas de voxel
        // IMPORTANTE: Esta função interpola o heightmap quando o fator de densidade > 1
        // NOTA: Coordenadas negativas são tratadas como fora do mapa (retorna 0)
        public float GetHeight(int voxelX, int voxelZ) {
            // Coordenadas negativas = fora do mapa
            if (voxelX < 0 || voxelZ < 0) return 0f;
            
            // Converte coordenada de voxel para coordenada de pixel no heightmap
            float pixelX = (float)voxelX / FatorDensidade;
            float pixelZ = (float)voxelZ / FatorDensidade;
            
            // Interpolação bilinear para maior precisão
            int x0 = Mathf.FloorToInt(pixelX);
            int z0 = Mathf.FloorToInt(pixelZ);
            int x1 = x0 + 1;
            int z1 = z0 + 1;
            
            // Clamp aos limites do heightmap
            x0 = Mathf.Clamp(x0, 0, _mW - 1);
            x1 = Mathf.Clamp(x1, 0, _mW - 1);
            z0 = Mathf.Clamp(z0, 0, _mH - 1);
            z1 = Mathf.Clamp(z1, 0, _mH - 1);
            
            // Pesos para interpolação
            float tx = pixelX - Mathf.Floor(pixelX);
            float tz = pixelZ - Mathf.Floor(pixelZ);
            
            // CONSISTÊNCIA: Usa mesma convenção de índice que PopulateFromCache
            // Unity heightmaps: origem no canto inferior esquerdo, então invertemos Z
            int idx00 = (_mH - 1 - z0) * _mW + x0;
            int idx10 = (_mH - 1 - z0) * _mW + x1;
            int idx01 = (_mH - 1 - z1) * _mW + x0;
            int idx11 = (_mH - 1 - z1) * _mW + x1;
            
            // Proteção de índice
            idx00 = Mathf.Clamp(idx00, 0, _hCache.Length - 1);
            idx10 = Mathf.Clamp(idx10, 0, _hCache.Length - 1);
            idx01 = Mathf.Clamp(idx01, 0, _hCache.Length - 1);
            idx11 = Mathf.Clamp(idx11, 0, _hCache.Length - 1);
            
            // Amostra os 4 pixels vizinhos
            float h00 = _hCache[idx00].r / 255f;
            float h10 = _hCache[idx10].r / 255f;
            float h01 = _hCache[idx01].r / 255f;
            float h11 = _hCache[idx11].r / 255f;
            
            // Interpolação bilinear
            float h = Mathf.Lerp(
                Mathf.Lerp(h00, h10, tx),
                Mathf.Lerp(h01, h11, tx),
                tz
            );
            
            return h * alturaMaximaMetros;
        }

        // Calcula inclinação (diferença de altura em metros)
        public float GetSlope(int voxelX, int voxelZ) {
            float h1 = GetHeight(voxelX, voxelZ);
            float h2 = GetHeight(voxelX + 1, voxelZ);
            return Mathf.Abs(h1 - h2);
        }

        // Gerencia os dados dos Chunks
        public ChunkData GetGarantirChunk(Vector2Int p) {
            if (!_chunks.ContainsKey(p)) {
                ChunkData novo = new ChunkData(p, AlturaMaximaVoxels);
                // Preenche os voxels baseado no cache de imagem, usando o fator de densidade
                novo.PopulateFromCache(_hCache, _mW, _mH, alturaMaximaMetros, escalaVoxel, FatorDensidade);
                _chunks.Add(p, novo);
            }
            return _chunks[p];
        }

        /// <summary>
        /// Remove os dados de um chunk da memória, liberando o array de voxels.
        /// </summary>
        public void RemoveChunkData(Vector2Int p) {
            if (_chunks.ContainsKey(p)) {
                var chunk = _chunks[p];
                if (chunk != null) {
                    // Limpa explicitamente o array de voxels para ajudar o GC
                    chunk.voxels = null;
                }
                _chunks.Remove(p);
            }
        }

        // Heurística simples para derivar um BlockType a partir do heightmap cacheado.
        // Esta função não gera chunks; apenas interpreta o heightmap em um tipo de solo
        // adequado para decisões de CityLayer/Gameplay.
        // Aceita coordenadas de VOXEL e converte para pixel do heightmap internamente.
        public BlockType GetSoilBlockType(int voxelX, int voxelZ) {
            if (_hCache == null) return BlockType.Terra;
            
            // Coordenadas negativas = fora do mapa
            if (voxelX < 0 || voxelZ < 0) return BlockType.Terra;
            
            // Converte coordenada de voxel para pixel
            int pixelX = Mathf.FloorToInt((float)voxelX / FatorDensidade);
            int pixelZ = Mathf.FloorToInt((float)voxelZ / FatorDensidade);
            
            if (pixelX < 0 || pixelX >= _mW || pixelZ < 0 || pixelZ >= _mH) return BlockType.Terra;

            // CONSISTÊNCIA: Usa mesma convenção de índice (inversão de Z)
            int idx = (_mH - 1 - pixelZ) * _mW + pixelX;
            idx = Mathf.Clamp(idx, 0, _hCache.Length - 1);
            
            float hn = _hCache[idx].r / 255f; // 0..1

            // Regras heurísticas (ajuste conforme real data/geologia local):
            // - Muito baixo (próximo ao mar/lago): Areia/Agua
            // - Baixo: Areia / Laterita
            // - Meio: Terra/Argila
            // - Alto: Rocha (Granito/Basalto)
            if (hn < 0.03f) return BlockType.Agua;
            if (hn < 0.10f) return BlockType.Areia;
            if (hn < 0.20f) return BlockType.Laterita;
            if (hn < 0.65f) return BlockType.Terra;
            return BlockType.Granito;
        }

        // Retorna as propriedades físicas do solo usando o mapeamento central SoilProperties
        // Aceita coordenadas de VOXEL
        public SoilStats GetSoilStats(int voxelX, int voxelZ) {
            BlockType type = GetSoilBlockType(voxelX, voxelZ);
            return SoilProperties.Get(type);
        }
    }
}