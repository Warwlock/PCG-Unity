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
        public int input;

        Mesh[] meshes;
        MeshDataArray meshDataArray;
        NativeArray<Bounds> boundsArray;

        public override JobHandle Process(JobHandle dependsOn)
        {
            Debug.Log("StartC");
            int totalChunks = graph.chunkX * graph.chunkY;

            var descriptor = new[]
            {
                new VertexAttributeDescriptor(VertexAttribute.Position),
                new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1)
            };

            var descriptorArray = new NativeArray<VertexAttributeDescriptor>(descriptor, Allocator.TempJob);
            boundsArray = new NativeArray<Bounds>(totalChunks, Allocator.TempJob);

            int pointsPerChunk = (int)graph.chunkSize * (int)graph.chunkSize;
            PointsToTerrainJob chunkMeshJob = new PointsToTerrainJob()
            {
                numX = (int)graph.chunkSize,
                numY = (int)graph.chunkSize,
                points = graph.points,
                bounds = boundsArray,
                descriptor = descriptorArray,
                meshDataArray = meshDataArray,
                useLOD = useLOD,
                slope = slope,
                bias = bias
            };
            dependsOn = chunkMeshJob.ScheduleParallel(totalChunks, totalChunks / batchDivisor, dependsOn);

            descriptorArray.Dispose(dependsOn);

            return dependsOn;
        }

        public void SetMeshes(Mesh[] meshes, ref MeshDataArray meshDataArray)
        {
            this.meshes = meshes;
            this.meshDataArray = meshDataArray;
        }

        public override void OnJobCompleted()
        {
            Debug.Log("YESSS!");

            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, meshes);
            for(int i = 0; i < meshes.Length; i++)
            {
                meshes[i].bounds = boundsArray[i];
            }
            boundsArray.Dispose();
        }
    }
}
