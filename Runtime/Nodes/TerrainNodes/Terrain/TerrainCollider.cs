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
    [System.Serializable, NodeMenuItem("Terrain/TerrainCollider", typeof(PCGTerrainGraph))]
    public class TerrainCollider : BaseChainJobNode
    {
        [Input]
        public Mesh[] meshes;

        public override JobHandle Process(JobHandle dependsOn)
        {
            Debug.Log(meshes.Length);

            NativeArray<EntityId> meshIds = new NativeArray<EntityId>(meshes.Length, Allocator.TempJob);

            for (int i = 0; i < meshes.Length; ++i)
            {
                meshIds[i] = meshes[i].GetEntityId();
            }

            BakeTerrainColliderMeshJob job = new BakeTerrainColliderMeshJob
            {
                meshIds = meshIds
            };

            dependsOn = job.ScheduleParallel(meshIds.Length, BATCH_COUNT, dependsOn);
            meshIds.Dispose(dependsOn);

            return dependsOn;
        }

        public override void OnJobCompleted()
        {
            Debug.Log("MeshCollider!");
        }
    }
}
