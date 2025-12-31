using UnityEngine;
using System.Collections.Generic;

namespace Voxel {
    public static class ChunkMeshGenerator {
        /// <summary>
        /// Gera a mesh do chunk sem greedy meshing: desenha todas as faces expostas de cada voxel.
        /// </summary>
        public static Mesh BuildMesh(TerrainWorld world, ChunkData data, float scale) {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            int w = ChunkData.Largura;
            int h = 256; // altura do chunk
            int d = ChunkData.Largura;

            // Direções das 6 faces (X+, X-, Y+, Y-, Z+, Z-)
            Vector3Int[] directions = new Vector3Int[] {
                new Vector3Int(1, 0, 0), new Vector3Int(-1, 0, 0),
                new Vector3Int(0, 1, 0), new Vector3Int(0, -1, 0),
                new Vector3Int(0, 0, 1), new Vector3Int(0, 0, -1)
            };

            // Para cada voxel do chunk
            for (int x = 0; x < w; x++) {
                for (int y = 0; y < h; y++) {
                    for (int z = 0; z < d; z++) {
                        byte v = data.voxels[x, y, z];
                        if (v == 0) continue; // Ar, não desenha
                        Vector3Int pos = new Vector3Int(x, y, z);
                        // Para cada face
                        for (int dir = 0; dir < 6; dir++) {
                            Vector3Int offset = directions[dir];
                            int nx = x + offset.x;
                            int ny = y + offset.y;
                            int nz = z + offset.z;
                            bool faceExposta = false;
                            // Checa se está fora do chunk (borda)
                            if (nx < 0 || nx >= w || ny < 0 || ny >= h || nz < 0 || nz >= d) {
                                // Se for borda, considera exposta
                                faceExposta = true;
                            } else {
                                // Se vizinho for ar, desenha face
                                if (data.voxels[nx, ny, nz] == 0) faceExposta = true;
                            }
                            if (faceExposta) {
                                AddFace(vertices, triangles, uvs, pos, dir, scale);
                            }
                        }
                    }
                }
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        // Adiciona uma face do cubo na direção dir
        private static void AddFace(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, Vector3Int pos, int dir, float scale) {
            // Posições dos vértices de um cubo unitário
            Vector3[] faceVerts = new Vector3[4];
            switch (dir) {
                case 0: // X+
                    faceVerts[0] = new Vector3(1, 0, 0);
                    faceVerts[1] = new Vector3(1, 1, 0);
                    faceVerts[2] = new Vector3(1, 1, 1);
                    faceVerts[3] = new Vector3(1, 0, 1);
                    break;
                case 1: // X-
                    faceVerts[0] = new Vector3(0, 0, 1);
                    faceVerts[1] = new Vector3(0, 1, 1);
                    faceVerts[2] = new Vector3(0, 1, 0);
                    faceVerts[3] = new Vector3(0, 0, 0);
                    break;
                case 2: // Y+
                    faceVerts[0] = new Vector3(0, 1, 1);
                    faceVerts[1] = new Vector3(1, 1, 1);
                    faceVerts[2] = new Vector3(1, 1, 0);
                    faceVerts[3] = new Vector3(0, 1, 0);
                    break;
                case 3: // Y-
                    faceVerts[0] = new Vector3(0, 0, 0);
                    faceVerts[1] = new Vector3(1, 0, 0);
                    faceVerts[2] = new Vector3(1, 0, 1);
                    faceVerts[3] = new Vector3(0, 0, 1);
                    break;
                case 4: // Z+
                    faceVerts[0] = new Vector3(0, 0, 1);
                    faceVerts[1] = new Vector3(1, 0, 1);
                    faceVerts[2] = new Vector3(1, 1, 1);
                    faceVerts[3] = new Vector3(0, 1, 1);
                    break;
                case 5: // Z-
                    faceVerts[0] = new Vector3(1, 0, 0);
                    faceVerts[1] = new Vector3(0, 0, 0);
                    faceVerts[2] = new Vector3(0, 1, 0);
                    faceVerts[3] = new Vector3(1, 1, 0);
                    break;
            }
            int vCount = vertices.Count;
            for (int i = 0; i < 4; i++) {
                vertices.Add((pos + faceVerts[i]) * scale);
                uvs.Add(new Vector2(faceVerts[i].x, faceVerts[i].y));
            }
            triangles.Add(vCount);
            triangles.Add(vCount + 1);
            triangles.Add(vCount + 2);
            triangles.Add(vCount);
            triangles.Add(vCount + 2);
            triangles.Add(vCount + 3);
        }
    }
}