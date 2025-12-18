using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using static PCG.MathOperators;

namespace PCG.Terrain
{
    public class PCGTerrainGraph : PCGGraph
    {
        public NativeArray<float3> points;

        public ChunkSizeEnum chunkSize = ChunkSizeEnum._48;
        public int chunkX = 10, chunkY = 10;

        public bool generateCollisions;
    }
}
