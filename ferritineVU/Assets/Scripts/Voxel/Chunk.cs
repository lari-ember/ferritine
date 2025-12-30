using System.Collections.Generic;
using UnityEngine;

namespace Voxel {
    public enum BlockType : byte {
        Ar = 0, Grama = 1, Terra = 2, Granito = 3, Andesito = 4, Diorito = 5,
        Calcario = 6, Areia = 7, Cascalho = 8, Argila = 9, Carvao = 10, Madeira = 11,
        Folha = 12, Ouro = 13, Ferro = 14, Diamante = 15, Agua = 16
    }

    public class Chunk {
        public const int Largura = 32;
        public const int Altura = 32;
        public float EscalaVoxel = 0.1f; // 10 cm

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

        public void PopulateFromHeightmap(Texture2D mapa, int offsetX, int offsetZ) {
            for (int x = 0; x < Largura; x++) {
                for (int z = 0; z < Largura; z++) {
                    // Lê o pixel e converte para altura (multiplicando por 100 voxels)
                    float h = mapa.GetPixel(x + offsetX, z + offsetZ).grayscale;
                    int vHeight = Mathf.FloorToInt(h * 100);

                    for (int y = 0; y < Altura; y++) {
                        if (y <= vHeight) Dados[x, y, z] = (byte)BlockType.Grama;
                        else Dados[x, y, z] = (byte)BlockType.Ar;
                    }
                }
            }
        }

        public Mesh GenerateMesh() {
            _vertices.Clear();
            _triangles.Clear();
            _uvs.Clear();

            for (int x = 0; x < Largura; x++) {
                for (int y = 0; y < Altura; y++) {
                    for (int z = 0; z < Largura; z++) {
                        if (Dados[x, y, z] != (byte)BlockType.Ar) {
                            MakeCube(x, y, z);
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.vertices = _vertices.ToArray();
            mesh.triangles = _triangles.ToArray();
            mesh.uv = _uvs.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        void MakeCube(int x, int y, int z) {
            // Face Culling: Só cria a face se o vizinho for AR ou estiver fora do limite
            if (GetVoxel(x, y + 1, z) == 0) CreateFace(x, y, z, Vector3.up);
            if (GetVoxel(x, y - 1, z) == 0) CreateFace(x, y, z, Vector3.down);
            if (GetVoxel(x + 1, y, z) == 0) CreateFace(x, y, z, Vector3.right);
            if (GetVoxel(x - 1, y, z) == 0) CreateFace(x, y, z, Vector3.left);
            if (GetVoxel(x, y, z + 1) == 0) CreateFace(x, y, z, Vector3.forward);
            if (GetVoxel(x, y, z - 1) == 0) CreateFace(x, y, z, Vector3.back);
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
