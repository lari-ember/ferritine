using System.Collections.Generic;
using UnityEngine;

namespace Voxel
{
    public class ChunkSystem
    {
        private Dictionary<Vector3Int, Chunk> chunks;

        public ChunkSystem()
        {
            chunks = new Dictionary<Vector3Int, Chunk>();
        }

        public Chunk GetOrCreateChunk(Vector3Int chunkCoord)
        {
            if (chunks.TryGetValue(chunkCoord, out Chunk chunk))
            {
                return chunk;
            }

            chunk = new Chunk();
            chunks.Add(chunkCoord, chunk);
            return chunk;
        }
    }
}