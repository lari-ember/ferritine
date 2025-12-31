using UnityEngine;
using System.Collections.Generic;

namespace Voxel {
    public class TerrainWorld : MonoBehaviour {
        [Header("Configuração do Mapa")]
        public Texture2D heightmap; // Arraste seu PNG de Curitiba aqui
        public float escalaVoxel = 0.5f; // Tamanho de cada bloco (0.5 = 50cm)
        public int alturaMaximaMundo = 256; // Altura máxima dobrada para 256 blocos

        // Cache de dados (Privado)
        private Color32[] _hCache;
        private int _mW, _mH;
        private Dictionary<Vector2Int, ChunkData> _chunks = new Dictionary<Vector2Int, ChunkData>();

        void Awake() {
            if (heightmap == null) {
                Debug.LogError("TerrainWorld: Heightmap não atribuído!");
                return;
            }
            
            // Prepara o cache para leitura rápida
            _mW = heightmap.width;
            _mH = heightmap.height;
            _hCache = heightmap.GetPixels32();
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
                if (localX >= 0 && localX < ChunkData.Largura && 
                    globalY >= 0 && globalY < 512 && 
                    localZ >= 0 && localZ < ChunkData.Largura) {
                    return _chunks[chunkPos].voxels[localX, globalY, localZ];
                }
            }
    
            // Se o chunk não existe, usa o heightmap como fallback
            if (globalY < 0 || globalY >= 512) return 0;
    
            float heightMeters = GetHeight(globalX, globalZ);
            int heightVoxels = Mathf.FloorToInt(heightMeters / escalaVoxel);
    
            return (globalY < heightVoxels) ? (byte)1 : (byte)0;
        }

        // Retorna a altura em metros numa posição (X, Z)
        public float GetHeight(int x, int z) {
            if (x < 0 || x >= _mW || z < 0 || z >= _mH) return 0;
            // Lê o canal R do pixel e converte para altura real
            return (_hCache[z * _mW + x].r / 255f) * alturaMaximaMundo * escalaVoxel;
        }

        // Calcula inclinação (diferença de altura)
        public float GetSlope(int x, int z) {
            float h1 = GetHeight(x, z);
            float h2 = GetHeight(x + 1, z);
            return Mathf.Abs(h1 - h2);
        }

        // Gerencia os dados dos Chunks
        public ChunkData GetGarantirChunk(Vector2Int p) {
            if (!_chunks.ContainsKey(p)) {
                ChunkData novo = new ChunkData(p);
                // Preenche os voxels baseado no cache de imagem
                novo.PopulateFromCache(_hCache, _mW, _mH, alturaMaximaMundo);
                _chunks.Add(p, novo);
            }
            return _chunks[p];
        }

        public void RemoveChunkData(Vector2Int p) {
            if (_chunks.ContainsKey(p)) _chunks.Remove(p);
        }

        // Heurística simples para derivar um BlockType a partir do heightmap cacheado.
        // Esta função não gera chunks; apenas interpreta o heightmap em um tipo de solo
        // adequado para decisões de CityLayer/Gameplay.
        public BlockType GetSoilBlockType(int x, int z) {
            if (_hCache == null) return BlockType.Terra;
            if (x < 0 || x >= _mW || z < 0 || z >= _mH) return BlockType.Terra;

            int idx = z * _mW + x;
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
        public SoilStats GetSoilStats(int x, int z) {
            BlockType type = GetSoilBlockType(x, z);
            return SoilProperties.Get(type);
        }
    }
}