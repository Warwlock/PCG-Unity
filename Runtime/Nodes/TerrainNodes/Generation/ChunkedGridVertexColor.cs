using GraphProcessor;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using static PCG.MathOperators;
using MeshDataArray = UnityEngine.Mesh.MeshDataArray;

namespace PCG.Terrain
{
    [System.Serializable, NodeMenuItem("Generator/ChunkedGridVertexColor", typeof(PCGTerrainGraph))]
    public class ChunkedGridVertexColor : BaseChainJobNode
    {
        [Output(allowMultiple = false)]
        public NativeArray<half4> points;

        public override JobHandle Process(JobHandle dependsOn)
        {
            Debug.Log("StartVert");
            int totalPoints = ((int)graph.chunkSize * graph.chunkX) * ((int)graph.chunkSize * graph.chunkY);
            points = new NativeArray<half4>(totalPoints, Allocator.Persistent);

            ChunkGridVertexColorJob jobData = new ChunkGridVertexColorJob
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
            Debug.Log("VertexColor!");
        }
    }
}
