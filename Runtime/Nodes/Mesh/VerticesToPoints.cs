using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Mesh/VerticesToPoints", typeof(PCGGraph))]
    public class VerticesToPoints : BaseJobNode
    {
        [Input, SerializeField]
        public Mesh mesh;

        [Output]
        public PCGPointData points;

        public NativeArray<Vector3> vertices;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandleNullErrors(mesh == null)) return emptyHandle;

            using(var dataArray = Mesh.AcquireReadOnlyMeshData(mesh))
            {
                var data = dataArray[0];
                vertices = new NativeArray<Vector3>(mesh.vertexCount, Allocator.TempJob);
                data.GetVertices(vertices);
            }

            PCGEmptyJob emptyJob = new PCGEmptyJob();
            handle = emptyJob.Schedule();

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(mesh.vertices.Length);

            points.SetAttributeList(DefaultAttributes.Pos, vertices.ToArray());

            vertices.Dispose();
        }
    }
}
