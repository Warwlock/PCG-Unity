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
    [System.Serializable, NodeMenuItem("Terrain/PointsToTerrain", typeof(PCGTerrainGraph))]
    public class PointsToTerrain : BaseChainJobNode
    {
        [Header("LOD")]
        public bool useLOD;
        public float slope = 2, bias = 18;

        [Input]
        public NativeArray<float3> points;

        [Input]
        public NativeArray<half4> vertexColorPoints;

        MeshDataArray meshDataArray;
        NativeArray<Bounds> boundsArray;

        public override JobHandle Process(JobHandle dependsOn)
        {
            Debug.Log("StartC");
            int totalChunks = graph.chunkX * graph.chunkY;

            VertexAttributeDescriptor[] descriptor;
            if (!vertexColorPoints.IsCreated)
            {
                descriptor = new[]
                {
                    new VertexAttributeDescriptor(VertexAttribute.Position),
                    new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
                    new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 2, 2)
                };
            }
            else
            {
                descriptor = new[]
                {
                    new VertexAttributeDescriptor(VertexAttribute.Position),
                    new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
                    new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float16, 2, 2),
                    new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.Float16, 4, 3)
                };
            }

            var descriptorArray = new NativeArray<VertexAttributeDescriptor>(descriptor, Allocator.TempJob);

            int pointsPerChunk = (int)graph.chunkSize * (int)graph.chunkSize;
            PointsToTerrainJob chunkMeshJob = new PointsToTerrainJob()
            {
                numX = (int)graph.chunkSize,
                numY = (int)graph.chunkSize,
                points = points,
                colorPoints = vertexColorPoints,
                bounds = boundsArray,
                descriptor = descriptorArray,
                meshDataArray = meshDataArray,
                useLOD = useLOD,
                slope = slope,
                bias = bias
            };
            dependsOn = chunkMeshJob.ScheduleParallel(totalChunks, totalChunks / batchDivisor, dependsOn);

            descriptorArray.Dispose(dependsOn);
            points.Dispose(dependsOn);
            
            if(vertexColorPoints.IsCreated)
                vertexColorPoints.Dispose(dependsOn);

            return dependsOn;
        }

        public void SetMeshes(NativeArray<Bounds> boundsArray, MeshDataArray meshDataArray)
        {
            this.boundsArray = boundsArray;
            this.meshDataArray = meshDataArray;
        }

        public override void OnJobCompleted()
        {
            Debug.Log("YESSS!");
        }
    }
}
