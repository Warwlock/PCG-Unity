using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static PCG.MathOperators;
using static UnityEngine.Rendering.VirtualTexturing.Debugging;

namespace PCG.Terrain
{
    [System.Serializable, NodeMenuItem("Generator/ChunkedGridTerrain", typeof(PCGTerrainGraph))]
    public class ChunkedGridTerrain : BaseChainJobNode
    {
        public float pointDst = 0.1f;

        [Output]
        public int output;

        public override JobHandle Process(JobHandle dependsOn)
        {
            ChunkGridTerrainJob jobData = new ChunkGridTerrainJob
            {
                numX = (int)graph.chunkSize,
                numY = (int)graph.chunkSize,
                chunkX = graph.chunkX,
                chunkY = graph.chunkY,
                pointDst = pointDst,
                result = graph.points
            };
            dependsOn = jobData.Schedule(dependsOn);

            return dependsOn;
        }
    }
}
