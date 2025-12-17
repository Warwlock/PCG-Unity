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

        MeshDataArray meshDataArray;

        public override JobHandle Process(JobHandle dependsOn)
        {
            int totalChunks = graph.chunkX * graph.chunkY;
            meshDataArray = Mesh.AllocateWritableMeshData(totalChunks);

            int pointsPerChunk = (int)graph.chunkSize * (int)graph.chunkSize;
            PointsToTerrainJob chunkMeshJob = new PointsToTerrainJob()
            {
                numX = (int)graph.chunkSize,
                numY = (int)graph.chunkSize,
                points = graph.points,
                meshDataArray = meshDataArray,
                useLOD = useLOD,
                slope = slope,
                bias = bias
            };
            dependsOn = chunkMeshJob.ScheduleParallel(totalChunks, BATCH_COUNT, dependsOn);

            return dependsOn;
        }

        public override void OnJobCompleted()
        {
            int totalChunks = graph.chunkX * graph.chunkY;
            Mesh[] meshes = new Mesh[totalChunks];
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i] = new Mesh();
                meshes[i].name = "Chunk" + i;
            }
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, meshes);
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i].RecalculateBounds();
            }
            graph.terrainMeshes.AddRange(meshes);
        }
    }
}
