using GraphProcessor;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using static PCG.MathOperators;
using MeshDataArray = UnityEngine.Mesh.MeshDataArray;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Mesh/PointsToTerrain", typeof(PCGGraph))]
    public class PointsToTerrain : BaseJobNode
    {
        [SerializeField]
        public ChunkSizeEnum chunkSize = ChunkSizeEnum._48;
        public int chunkX = 5, chunkY = 5;
        [Header("LOD")]
        public bool useLOD;
        public float slope = 2, bias = 18;

        [Input]
        public PCGPointData pointsIn;

        public NativeArray<Vector3> pointPos;

        MeshDataArray meshDataArray;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandlePointErrors(pointsIn)) return emptyHandle;

            pointPos = new NativeArray<Vector3>(pointsIn.GetAttributeList<Vector3>(DefaultAttributes.Pos), Allocator.TempJob);

            handle = JobCreator();

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            int totalChunks = chunkX * chunkY;
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

        JobHandle JobCreator()
        {
            int totalChunks = chunkX * chunkY;
            meshDataArray = Mesh.AllocateWritableMeshData(totalChunks);
            List<JobHandle> dependencies = new List<JobHandle>();

            int pointsPerChunk = (int)chunkSize;
            for (int i = 0; i < totalChunks; i++)
            {
                int startIndex = i * pointsPerChunk * pointsPerChunk;
                NativeSlice<Vector3> slice = new NativeSlice<Vector3>(pointPos, startIndex, pointsPerChunk * pointsPerChunk);
                NativeArray<Vector3> normals = new NativeArray<Vector3>(slice.Length, Allocator.TempJob);

                PointsToTerrainJob chunkMeshJob = new PointsToTerrainJob()
                {
                    numX = pointsPerChunk,
                    numY = pointsPerChunk,
                    slice = slice,
                    normals = normals,
                    meshData = meshDataArray[i],
                    useLOD = useLOD,
                    slope = slope,
                    bias = bias
                };
                var jobHandle = chunkMeshJob.Schedule();
                dependencies.Add(jobHandle);

                normals.Dispose(jobHandle);
            }

            var dependArray = new NativeArray<JobHandle>(dependencies.ToArray(), Allocator.TempJob);
            var dependency = JobHandle.CombineDependencies(dependArray);
            dependArray.Dispose(dependency);
            pointPos.Dispose(dependency);

            return dependency;
        }
    }
}
