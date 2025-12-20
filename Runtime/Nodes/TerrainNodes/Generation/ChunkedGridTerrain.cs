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
        [Output(allowMultiple = false)]
        public NativeArray<float3> points;

        public override JobHandle Process(JobHandle dependsOn)
        {
            Debug.Log("StartA");
            int totalPoints = ((int)graph.chunkSize * graph.chunkX) * ((int)graph.chunkSize * graph.chunkY);
            points = new NativeArray<float3>(totalPoints, Allocator.Persistent);

            ChunkGridTerrainJob jobData = new ChunkGridTerrainJob
            {
                numX = (int)graph.chunkSize,
                numY = (int)graph.chunkSize,
                chunkX = graph.chunkX,
                chunkY = graph.chunkY,
                pointDst = graph.pointDistance,
                result = points
            };
            dependsOn = jobData.Schedule(dependsOn);

            return dependsOn;
        }

        public override void OnJobCompleted()
        {
            Debug.Log("GridTerrrain!");
        }
    }
}
