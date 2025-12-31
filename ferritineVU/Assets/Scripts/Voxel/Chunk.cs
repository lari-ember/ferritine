/*using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voxel {
    public class Chunk {
        public const int Largura = 32;
        public const int Altura = 32;
        public float EscalaVoxel = 0.1f; // 10 cm

        // Tornar Dados público para serialização/paging
        public byte[,,] Dados = new byte[Largura, Altura, Largura];

        // Referência para o mundo que gerencia chunks (usada quando consultamos voxels fora deste chunk)
        private VoxelWorld _world;
        // Posição deste chunk no grid do mundo (em chunks)
        public Vector2Int PosicaoNoMundo;

        // Listas para construção da malha
        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();

        // Construtor que recebe a referência ao mundo e a posição do chunk
        public Chunk(VoxelWorld world, Vector2Int posicao) {
            this._world = world;
            this.PosicaoNoMundo = posicao;
        }

        // Mantive compatibilidade: método antigo que recebia Texture2D
        public void PopulateFromHeightmap(Texture2D mapa, int offsetX, int offsetZ) {
            // Backwards-compatible: cria um cache temporário local e chama a nova rotina
            if (mapa == null) return;
            Color32[] cache = mapa.GetPixels32();
            PopulateFromHeightmapCache(cache, mapa.width, mapa.height, offsetX, offsetZ);
        }

        // Novo método: popula os dados usando o array cacheado (Color32[]) para evitar chamadas GetPixel em runtime
        public void PopulateFromHeightmapCache(Color32[] mapCache, int mapWidth, int mapHeight, int offsetX, int offsetZ) {
            if (mapCache == null) return;

            for (int x = 0; x < Largura; x++) {
                for (int z = 0; z < Largura; z++) {
                    int pixelX = offsetX + x;
                    int pixelZ = offsetZ + z;

                    // Proteção de borda
                    if (pixelX < 0 || pixelZ < 0 || pixelX >= mapWidth || pixelZ >= mapHeight) {
                        // Fora do mapa -> deixar ar
                        for (int y = 0; y < Altura; y++) Dados[x, y, z] = (byte)Voxel.BlockType.Ar;
                        continue;
                    }

                    int index = pixelZ * mapWidth + pixelX;
                    Color32 c = mapCache[index];
                    float h = c.r / 255f; // assumimos grayscale no canal R
                    int vHeight = Mathf.FloorToInt(h * 100f);

                    for (int y = 0; y < Altura; y++) {
                        if (y <= vHeight) Dados[x, y, z] = (byte)Voxel.BlockType.Grama;
                        else Dados[x, y, z] = (byte)Voxel.BlockType.Ar;
                    }
                }
            }
        }

        // Gera mesh pulando voxels (passo>1) para LOD macro
        public Mesh GenerateMeshWithStep(int step) {
            if (step <= 1) return GenerateMesh();

            _vertices.Clear(); _triangles.Clear(); _uvs.Clear();

            for (int x = 0; x < Largura; x += step) {
                for (int y = 0; y < Altura; y += step) {
                    for (int z = 0; z < Largura; z += step) {
                        // se qualquer voxel no bloco passo for sólido, fazemos um cubo (approx)
                        bool solid = false;
                        for (int sx = 0; sx < step && !solid; sx++) for (int sy = 0; sy < step && !solid; sy++) for (int sz = 0; sz < step && !solid; sz++) {
                            int gx = Mathf.Min(x+sx, Largura-1);
                            int gy = Mathf.Min(y+sy, Altura-1);
                            int gz = Mathf.Min(z+sz, Largura-1);
                            if (Dados[gx, gy, gz] != (byte)Voxel.BlockType.Ar) solid = true;
                        }

                        if (solid) {
                            // cria um cubo do tamanho do passo (aprox)
                            AddCubeApprox(x, y, z, step);
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.uv = _uvs.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        void AddCubeApprox(int x, int y, int z, int size) {
            int vCount = _vertices.Count;
            float s = EscalaVoxel * size;
            Vector3 pos = new Vector3(x * EscalaVoxel, y * EscalaVoxel, z * EscalaVoxel);

            _vertices.Add(pos + new Vector3(0, 0, 0)); _vertices.Add(pos + new Vector3(s, 0, 0));
            _vertices.Add(pos + new Vector3(s, s, 0)); _vertices.Add(pos + new Vector3(0, s, 0));
            _vertices.Add(pos + new Vector3(0, 0, s)); _vertices.Add(pos + new Vector3(s, 0, s));
            _vertices.Add(pos + new Vector3(s, s, s)); _vertices.Add(pos + new Vector3(0, s, s));

            // front
            _triangles.Add(vCount+0); _triangles.Add(vCount+2); _triangles.Add(vCount+1);
            _triangles.Add(vCount+0); _triangles.Add(vCount+3); _triangles.Add(vCount+2);
            // back
            _triangles.Add(vCount+4); _triangles.Add(vCount+5); _triangles.Add(vCount+6);
            _triangles.Add(vCount+4); _triangles.Add(vCount+6); _triangles.Add(vCount+7);
            // left
            _triangles.Add(vCount+0); _triangles.Add(vCount+4); _triangles.Add(vCount+7);
            _triangles.Add(vCount+0); _triangles.Add(vCount+7); _triangles.Add(vCount+3);
            // right
            _triangles.Add(vCount+1); _triangles.Add(vCount+2); _triangles.Add(vCount+6);
            _triangles.Add(vCount+1); _triangles.Add(vCount+6); _triangles.Add(vCount+5);
            // top
            _triangles.Add(vCount+2); _triangles.Add(vCount+3); _triangles.Add(vCount+7);
            _triangles.Add(vCount+2); _triangles.Add(vCount+7); _triangles.Add(vCount+6);
            // bottom
            _triangles.Add(vCount+0); _triangles.Add(vCount+5); _triangles.Add(vCount+4);
            _triangles.Add(vCount+0); _triangles.Add(vCount+1); _triangles.Add(vCount+5);

            for (int i = 0; i < 8; i++) _uvs.Add(new Vector2(0,0));
        }

        // Greedy meshing implementation (reduces quads by merging faces)
        public Mesh GenerateGreedyMesh() {
            // We'll implement a simple 3D greedy meshing that creates quads for exposed faces and merges adjacent quads.
            // This implementation focuses on performance and clarity, not absolute optimal merging.
            _vertices.Clear(); _triangles.Clear(); _uvs.Clear();

            int W = Largura, H = Altura, D = Largura;

            // For each axis (0=X,1=Y,2=Z), run a 2D greedy on slices
            for (int axis = 0; axis < 3; axis++) {
                int u = (axis + 1) % 3; // first perp axis
                int v = (axis + 2) % 3; // second perp axis

                int[] dims = new int[3] { W, H, D };
                int iMax = dims[axis], jMax = dims[u], kMax = dims[v];

                // mask for the slice
                int[] mask = new int[jMax * kMax];

                for (int i = 0; i < iMax; i++) {
                    // build mask
                    for (int j = 0; j < jMax; j++) {
                        for (int k = 0; k < kMax; k++) {
                            byte a = GetVoxelAtAxis(axis, i, j, k);
                            byte b = GetVoxelAtAxis(axis, i - 1, j, k);
                            // if face between a and b is exposed, mask entry = a!=0 ? a : (b!=0 ? -b : 0)
                            int mi = j * kMax + k;
                            if ((a != 0) && (b == 0)) mask[mi] = a; // face towards negative axis
                            else if ((a == 0) && (b != 0)) mask[mi] = -b; // face towards positive axis
                            else mask[mi] = 0;
                        }
                    }

                    // greedy merge on mask
                    for (int j = 0; j < jMax; j++) {
                        for (int k = 0; k < kMax; ) {
                            int mi = j * kMax + k;
                            int c = mask[mi];
                            if (c != 0) {
                                // compute width
                                int w = 1;
                                while (k + w < kMax && mask[mi + w] == c) w++;

                                // compute height
                                int h = 1;
                                bool done = false;
                                while (j + h < jMax && !done) {
                                    for (int m = 0; m < w; m++) {
                                        if (mask[(j + h) * kMax + k + m] != c) { done = true; break; }
                                    }
                                    if (!done) h++;
                                }

                                // Create quad covering (j,k) .. (j+h-1, k+w-1) at slice i
                                CreateGreedyQuad(axis, i, j, k, w, h, c);

                                // zero out mask
                                for (int hh = 0; hh < h; hh++) for (int ww = 0; ww < w; ww++) mask[(j + hh) * kMax + (k + ww)] = 0;

                                k += w;
                            } else k++;
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.uv = _uvs.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        // Helper: returns voxel value at axis-aligned coordinates (axis,i,j,k) mapped to x,y,z
        byte GetVoxelAtAxis(int axis, int i, int j, int k) {
            int x = 0, y = 0, z = 0;
            switch (axis) {
                case 0: x = i; y = j; z = k; break; // slice along X
                case 1: x = j; y = i; z = k; break; // slice along Y
                default: x = j; y = k; z = i; break; // slice along Z
            }
            if (x < 0 || x >= Largura || y < 0 || y >= Altura || z < 0 || z >= Largura) return 0;
            return Dados[x, y, z];
        }

        void CreateGreedyQuad(int axis, int i, int j, int k, int w, int h, int c) {
            // compute 4 corner positions in x,y,z
            Vector3[] quad = new Vector3[4];
            float s = EscalaVoxel;
            // map local j,k to x,y,z start
            int x0=0,y0=0,z0=0;
            if (axis == 0) { x0 = i; y0 = j; z0 = k; }
            else if (axis == 1) { x0 = j; y0 = i; z0 = k; }
            else { x0 = j; y0 = k; z0 = i; }

            // depending on face direction (sign of c), determine normal
            bool negative = c > 0 ? true : false;
            // corners in local space
            Vector3 p0 = new Vector3(x0 * s, y0 * s, z0 * s);
            Vector3 du, dv;
            if (axis == 0) { du = new Vector3(0, w * s, 0); dv = new Vector3(0, 0, h * s); }
            else if (axis == 1) { du = new Vector3(w * s, 0, 0); dv = new Vector3(0, 0, h * s); }
            else { du = new Vector3(w * s, 0, 0); dv = new Vector3(0, h * s, 0); }

            quad[0] = p0;
            quad[1] = p0 + du;
            quad[2] = p0 + du + dv;
            quad[3] = p0 + dv;

            int vCount = _vertices.Count;
            // if face direction is negative (facing -axis), we flip winding
            if (negative) {
                _vertices.Add(quad[0]); _vertices.Add(quad[1]); _vertices.Add(quad[2]); _vertices.Add(quad[3]);
                _triangles.Add(vCount); _triangles.Add(vCount+1); _triangles.Add(vCount+2);
                _triangles.Add(vCount); _triangles.Add(vCount+2); _triangles.Add(vCount+3);
            } else {
                _vertices.Add(quad[0]); _vertices.Add(quad[3]); _vertices.Add(quad[2]); _vertices.Add(quad[1]);
                _triangles.Add(vCount); _triangles.Add(vCount+1); _triangles.Add(vCount+2);
                _triangles.Add(vCount); _triangles.Add(vCount+2); _triangles.Add(vCount+3);
            }
            _uvs.Add(new Vector2(0,0)); _uvs.Add(new Vector2(1,0)); _uvs.Add(new Vector2(1,1)); _uvs.Add(new Vector2(0,1));
        }

        // Make GenerateMesh use greedy mesher by default
        public Mesh GenerateMesh() {
            return GenerateGreedyMesh();
        }

        // Se a coordenada está dentro deste chunk, lê direto daqui.
        // Se estiver fora, delega a consulta ao VoxelWorld para ler o chunk vizinho e evitar costuras.
        byte GetVoxel(int x, int y, int z) {
            if (x >= 0 && x < Largura && y >= 0 && y < Altura && z >= 0 && z < Largura)
                return Dados[x, y, z];

            // Se saiu do limite deste chunk, converte para coordenadas globais e pergunta ao world
            if (_world == null) {
                // Sem referência ao world, considera ar (0) para evitar crashs
                return 0;
            }

            int globalX = x + (PosicaoNoMundo.x * Largura);
            int globalY = y;
            int globalZ = z + (PosicaoNoMundo.y * Largura);

            return _world.GetVoxelNoMundo(globalX, globalY, globalZ);
        }

        // Função auxiliar usada pelo World para ler este chunk localmente
        public byte GetVoxelLocal(int x, int y, int z) {
            if (x < 0 || x >= Largura || y < 0 || y >= Altura || z < 0 || z >= Largura) return 0;
            return Dados[x, y, z];
        }

        void CreateFace(int x, int y, int z, Vector3 direcao) {
            int vCount = _vertices.Count;
            float s = EscalaVoxel;

            // Posição base do voxel multiplicada pela escala real
            Vector3 pos = new Vector3(x * s, y * s, z * s);

            // Lógica de vértices simplificada por direção
            if (direcao == Vector3.up) {
                _vertices.Add(pos + new Vector3(0, s, 0)); _vertices.Add(pos + new Vector3(0, s, s));
                _vertices.Add(pos + new Vector3(s, s, s)); _vertices.Add(pos + new Vector3(s, s, 0));
            } else if (direcao == Vector3.down) {
                _vertices.Add(pos + new Vector3(0, 0, 0)); _vertices.Add(pos + new Vector3(s, 0, 0));
                _vertices.Add(pos + new Vector3(s, 0, s)); _vertices.Add(pos + new Vector3(0, 0, s));
            } else if (direcao == Vector3.right) {
                _vertices.Add(pos + new Vector3(s, 0, 0)); _vertices.Add(pos + new Vector3(s, s, 0));
                _vertices.Add(pos + new Vector3(s, s, s)); _vertices.Add(pos + new Vector3(s, 0, s));
            } else if (direcao == Vector3.left) {
                _vertices.Add(pos + new Vector3(0, 0, 0)); _vertices.Add(pos + new Vector3(0, 0, s));
                _vertices.Add(pos + new Vector3(0, s, s)); _vertices.Add(pos + new Vector3(0, s, 0));
            } else if (direcao == Vector3.forward) {
                _vertices.Add(pos + new Vector3(0, 0, s)); _vertices.Add(pos + new Vector3(s, 0, s));
                _vertices.Add(pos + new Vector3(s, s, s)); _vertices.Add(pos + new Vector3(0, s, s));
            } else if (direcao == Vector3.back) {
                _vertices.Add(pos + new Vector3(0, 0, 0)); _vertices.Add(pos + new Vector3(0, s, 0));
                _vertices.Add(pos + new Vector3(s, s, 0)); _vertices.Add(pos + new Vector3(s, 0, 0));
            }

            // Adiciona os triângulos (ordem horária para o Unity renderizar a frente)
            _triangles.Add(vCount); _triangles.Add(vCount + 1); _triangles.Add(vCount + 2);
            _triangles.Add(vCount); _triangles.Add(vCount + 2); _triangles.Add(vCount + 3);

            // UVs simples (mapeamento de textura)
            _uvs.Add(new Vector2(0, 0)); _uvs.Add(new Vector2(0, 1));
            _uvs.Add(new Vector2(1, 1)); _uvs.Add(new Vector2(1, 0));
        }
    }
}
*/