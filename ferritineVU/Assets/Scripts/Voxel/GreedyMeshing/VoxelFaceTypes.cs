using UnityEngine;

namespace Voxel.GreedyMeshing {
    /// <summary>
    /// Direções das faces laterais (apenas eixo horizontal, como especificado).
    /// Faces verticais (Y+ e Y-) são ignoradas para detecção de bordas.
    /// </summary>
    public enum HorizontalDirection {
        XPositive = 0,  // Leste  (+X)
        XNegative = 1,  // Oeste  (-X)
        ZPositive = 2,  // Norte  (+Z)
        ZNegative = 3   // Sul    (-Z)
    }
    
    /// <summary>
    /// Tipos de conexão de um voxel com vizinhos do mesmo tipo.
    /// Baseado no número e configuração de faces laterais conectadas.
    /// 
    /// Analogia visual:
    /// - Isolated: ■ (bloco sozinho, cercado de ar)
    /// - Chain:    ■■ (dois blocos em linha)
    /// - Corner:   ■  (formato L)
    ///             ■
    /// - Straight: ■■■ (linha reta, conectado nos dois lados opostos)
    /// - TShape:   ■■■ (formato T, 3 conexões)
    ///              ■
    /// - Full:     ■■■ (cruz completa, 4 conexões)
    ///             ■■■
    ///             ■■■
    /// </summary>
    public enum VoxelConnectionType {
        /// <summary>Tipo 1: Nenhuma face lateral conectada (voxel isolado)</summary>
        Isolated = 0,
        
        /// <summary>Tipo 2: Uma face conectada (pode formar par/retângulo)</summary>
        Single = 1,
        
        /// <summary>Tipo 3: Duas faces conectadas em L (expande em duas direções perpendiculares)</summary>
        Corner = 2,
        
        /// <summary>Tipo 4: Duas faces conectadas em I (faces opostas, linha reta)</summary>
        Straight = 3,
        
        /// <summary>Tipo 5: Três faces conectadas em T (expande em 3 direções)</summary>
        TShape = 4,
        
        /// <summary>Tipo 6: Todas as 4 faces conectadas (cruz/+, expande em todas)</summary>
        Full = 5
    }
    
    /// <summary>
    /// Representa uma face de voxel candidata a ser incluída na mesh.
    /// Usado pelo algoritmo de Greedy Meshing.
    /// </summary>
    public struct VoxelFace {
        /// <summary>Posição do voxel no chunk (coordenadas locais)</summary>
        public Vector3Int VoxelPosition;
        
        /// <summary>Direção da face (qual lado do cubo)</summary>
        public int Direction;
        
        /// <summary>Tipo/cor do bloco (para garantir que só fundimos blocos iguais)</summary>
        public byte BlockType;
        
        /// <summary>Se a face está na borda do chunk</summary>
        public bool IsChunkBorder;
        
        /// <summary>Conexões laterais com vizinhos do mesmo tipo (4 booleanos: N, E, S, W)</summary>
        public bool[] Connections;
        
        /// <summary>Tipo de conexão classificado</summary>
        public VoxelConnectionType ConnectionType;
        
        public VoxelFace(Vector3Int pos, int dir, byte type) {
            VoxelPosition = pos;
            Direction = dir;
            BlockType = type;
            IsChunkBorder = false;
            Connections = new bool[4];
            ConnectionType = VoxelConnectionType.Isolated;
        }
    }
    
    /// <summary>
    /// Dados de um quad mesclado pelo Greedy Meshing.
    /// Representa um retângulo que cobre múltiplos voxels.
    /// </summary>
    public struct MergedQuad {
        /// <summary>Posição do canto inferior esquerdo (coordenadas de voxel)</summary>
        public Vector3Int Origin;
        
        /// <summary>Largura em voxels (eixo U)</summary>
        public int Width;
        
        /// <summary>Altura em voxels (eixo V)</summary>
        public int Height;
        
        /// <summary>Direção da face</summary>
        public int Direction;
        
        /// <summary>Tipo do bloco</summary>
        public byte BlockType;
        
        /// <summary>Se usa diagonal principal (para consistência visual)</summary>
        public bool UseMainDiagonal;
        
        public MergedQuad(Vector3Int origin, int width, int height, int dir, byte type) {
            Origin = origin;
            Width = width;
            Height = height;
            Direction = dir;
            BlockType = type;
            UseMainDiagonal = (origin.x + origin.y + origin.z) % 2 == 0;
        }
    }
    
    /// <summary>
    /// Container para dados de mesh gerados.
    /// </summary>
    public class MeshData {
        public Vector3[] Vertices;
        public int[] Triangles;
        public Vector2[] UVs;
        public Vector3[] Normals;
        public Color32[] Colors;
        
        public int VertexCount;
        public int TriangleCount;
        
        public MeshData(int estimatedFaces = 1024) {
            int vertCapacity = estimatedFaces * 4;
            int triCapacity = estimatedFaces * 6;
            
            Vertices = new Vector3[vertCapacity];
            Triangles = new int[triCapacity];
            UVs = new Vector2[vertCapacity];
            Normals = new Vector3[vertCapacity];
            Colors = new Color32[vertCapacity];
            
            VertexCount = 0;
            TriangleCount = 0;
        }
        
        /// <summary>Garante capacidade para mais vértices</summary>
        public void EnsureCapacity(int additionalVerts, int additionalTris) {
            if (VertexCount + additionalVerts > Vertices.Length) {
                int newSize = Mathf.Max(Vertices.Length * 2, VertexCount + additionalVerts);
                System.Array.Resize(ref Vertices, newSize);
                System.Array.Resize(ref UVs, newSize);
                System.Array.Resize(ref Normals, newSize);
                System.Array.Resize(ref Colors, newSize);
            }
            
            if (TriangleCount + additionalTris > Triangles.Length) {
                int newSize = Mathf.Max(Triangles.Length * 2, TriangleCount + additionalTris);
                System.Array.Resize(ref Triangles, newSize);
            }
        }
        
        /// <summary>Converte para Mesh do Unity</summary>
        public Mesh ToMesh() {
            var mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            
            // Copia apenas os dados usados
            var verts = new Vector3[VertexCount];
            var tris = new int[TriangleCount];
            var uvList = new Vector2[VertexCount];
            var normals = new Vector3[VertexCount];
            var colors = new Color32[VertexCount];
            
            System.Array.Copy(Vertices, verts, VertexCount);
            System.Array.Copy(Triangles, tris, TriangleCount);
            System.Array.Copy(UVs, uvList, VertexCount);
            System.Array.Copy(Normals, normals, VertexCount);
            System.Array.Copy(Colors, colors, VertexCount);
            
            mesh.vertices = verts;
            mesh.triangles = tris;
            mesh.uv = uvList;
            mesh.normals = normals;
            mesh.colors32 = colors;
            
            mesh.RecalculateBounds();
            
            return mesh;
        }
    }
}

