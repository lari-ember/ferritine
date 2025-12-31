using UnityEngine;

namespace Voxel {
    public class ChunkData {
        public const int Largura = 32;
        public Vector2Int pos;
        public byte[,,] voxels;

        public ChunkData(Vector2Int p) {
            pos = p;
            voxels = new byte[Largura, 256, Largura];
        }

        public void PopulateFromCache(Color32[] cache, int mW, int mH, int maxH) {
            for (int x = 0; x < Largura; x++) {
                for (int z = 0; z < Largura; z++) {
                    // Calcula a posição global no mapa
                    int gX = pos.x * Largura + x;
                    int gZ = pos.y * Largura + z;

                    // SEGURANÇA: Se a posição estiver fora da imagem (negativa ou maior que a largura), pula
                    if (gX < 0 || gX >= mW || gZ < 0 || gZ >= mH) continue;

                    // Corrige possível inversão de origem do heightmap (Unity: origem canto inferior esquerdo)
                    int index = (mH - 1 - gZ) * mW + gX; // Inverte Z
                    if (index < 0 || index >= cache.Length) continue;

                    // Converte a cor para altura
                    int height = Mathf.FloorToInt((cache[index].r / 255f) * maxH);

                    // Garante pelo menos 1 voxel em y=0 (solo mínimo)
                    if (height == 0) height = 1;

                    int limitY = Mathf.Min(height, 511); 
                    for (int y = 0; y < limitY; y++) {
                        voxels[x, y, z] = 1; 
                    }

                    // Debug: loga regiões problemáticas
                    if (height == 1) {
                        Debug.Log($"[DEBUG] Chunk {pos} x={x} z={z} height=1 index={index}");
                    }
                }
            }
        }
    }
}