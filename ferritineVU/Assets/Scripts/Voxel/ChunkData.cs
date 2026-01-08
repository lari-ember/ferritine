using UnityEngine;

namespace Voxel {
    public class ChunkData {
        public const int Largura = 32;
        public int Altura { get; private set; }
        public Vector2Int pos;
        public byte[,,] voxels;

        public ChunkData(Vector2Int p, int altura = 256) {
            pos = p;
            Altura = Mathf.Min(altura, 512); // Limita para não estourar memória
            voxels = new byte[Largura, Altura, Largura];
        }

        /// <summary>
        /// Preenche o chunk com dados do heightmap, respeitando o fator de densidade.
        /// </summary>
        /// <param name="cache">Cache de pixels do heightmap</param>
        /// <param name="mW">Largura do heightmap em pixels</param>
        /// <param name="mH">Altura do heightmap em pixels</param>
        /// <param name="alturaMaximaMetros">Altura máxima do mundo em metros</param>
        /// <param name="escalaVoxel">Tamanho de cada voxel em metros</param>
        /// <param name="fatorDensidade">Quantos voxels por pixel do heightmap</param>
        public void PopulateFromCache(Color32[] cache, int mW, int mH, float alturaMaximaMetros, float escalaVoxel, int fatorDensidade) {
            for (int x = 0; x < Largura; x++) {
                for (int z = 0; z < Largura; z++) {
                    // Calcula a posição global do voxel
                    int gX = pos.x * Largura + x;
                    int gZ = pos.y * Largura + z;

                    // IMPORTANTE: Ignora coordenadas negativas (fora do mapa)
                    if (gX < 0 || gZ < 0) continue;

                    // Converte coordenada de voxel para coordenada de pixel no heightmap
                    // Com interpolação bilinear para suavizar quando fatorDensidade > 1
                    float pixelX = (float)gX / fatorDensidade;
                    float pixelZ = (float)gZ / fatorDensidade;
                    
                    // Interpolação bilinear
                    int x0 = Mathf.FloorToInt(pixelX);
                    int z0 = Mathf.FloorToInt(pixelZ);
                    int x1 = x0 + 1;
                    int z1 = z0 + 1;
                    
                    // Clamp aos limites
                    x0 = Mathf.Clamp(x0, 0, mW - 1);
                    x1 = Mathf.Clamp(x1, 0, mW - 1);
                    z0 = Mathf.Clamp(z0, 0, mH - 1);
                    z1 = Mathf.Clamp(z1, 0, mH - 1);
                    
                    // SEGURANÇA: Se completamente fora da imagem, pula
                    if (x0 < 0 || x0 >= mW || z0 < 0 || z0 >= mH) continue;

                    // Pesos para interpolação
                    float tx = pixelX - Mathf.Floor(pixelX);
                    float tz = pixelZ - Mathf.Floor(pixelZ);

                    // Corrige possível inversão de origem do heightmap (Unity: origem canto inferior esquerdo)
                    int idx00 = (mH - 1 - z0) * mW + x0;
                    int idx10 = (mH - 1 - z0) * mW + x1;
                    int idx01 = (mH - 1 - z1) * mW + x0;
                    int idx11 = (mH - 1 - z1) * mW + x1;
                    
                    // Proteção de índice
                    idx00 = Mathf.Clamp(idx00, 0, cache.Length - 1);
                    idx10 = Mathf.Clamp(idx10, 0, cache.Length - 1);
                    idx01 = Mathf.Clamp(idx01, 0, cache.Length - 1);
                    idx11 = Mathf.Clamp(idx11, 0, cache.Length - 1);
                    
                    // Amostra os 4 pixels vizinhos (normalizado 0-1)
                    float h00 = cache[idx00].r / 255f;
                    float h10 = cache[idx10].r / 255f;
                    float h01 = cache[idx01].r / 255f;
                    float h11 = cache[idx11].r / 255f;
                    
                    // Interpolação bilinear
                    float hNorm = Mathf.Lerp(
                        Mathf.Lerp(h00, h10, tx),
                        Mathf.Lerp(h01, h11, tx),
                        tz
                    );

                    // Converte altura normalizada para número de voxels
                    // alturaMetros = hNorm * alturaMaximaMetros
                    // surfaceHeight = alturaMetros / escalaVoxel
                    int surfaceHeight = Mathf.FloorToInt((hNorm * alturaMaximaMetros) / escalaVoxel);

                    // Garante pelo menos 1 voxel em y=0 (solo mínimo)
                    if (surfaceHeight == 0) surfaceHeight = 1;

                    int limitY = Mathf.Min(surfaceHeight, Altura - 1); 
                    
                    // ==================== GEOLOGIA EM CAMADAS ====================
                    // Inspirado na geologia de Curitiba: camadas superiores (solo orgânico),
                    // intermediárias (argila/terra), e profundas (rocha/granito).
                    
                    for (int y = 0; y < limitY; y++) {
                        // CAMADA DE TOPO (últimos 1-2 blocos): Grama ou vegetação
                        if (y == surfaceHeight - 1) {
                            // Superfície: grama se altitude moderada, areia se muito baixo
                            if (surfaceHeight < 5) {
                                voxels[x, y, z] = (byte)BlockType.Areia; // Áreas baixas (próximo a rios)
                            } else {
                                voxels[x, y, z] = (byte)BlockType.Grama; // Cobertura vegetal
                            }
                        }
                        // CAMADA INTERMEDIÁRIA (2-5 blocos abaixo da superfície): Terra/Argila
                        else if (y > surfaceHeight - 6) {
                            // Solos superficiais típicos de Curitiba
                            voxels[x, y, z] = (byte)BlockType.Terra;
                        }
                        // CAMADA INTERMEDIÁRIA PROFUNDA (6-12 blocos): Argila/Laterita
                        else if (y > surfaceHeight - 13) {
                            // Camada de argila vermelha característica do Primeiro Planalto Paranaense
                            voxels[x, y, z] = (byte)BlockType.Argila;
                        }
                        // CAMADA PROFUNDA (abaixo de 12 blocos): Rocha
                        else {
                            // Rocha matriz: Granito (comum na região de Curitiba)
                            voxels[x, y, z] = (byte)BlockType.Granito;
                        }
                        
                        // EXCEÇÃO: Áreas muito baixas podem ter água
                        // Nível da água em Curitiba (aproximadamente 900m acima do nível do mar)
                        // Aqui simplificamos: se altura < 3, pode ser água
                        if (surfaceHeight < 3 && y >= surfaceHeight - 1) {
                            voxels[x, y, z] = (byte)BlockType.Agua;
                        }
                    }
                }
            }
        }
    }
}