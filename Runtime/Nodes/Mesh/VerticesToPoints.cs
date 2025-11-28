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

        public NativeArray<float> resultX;
        public NativeArray<float> resultY;
        public NativeArray<float> resultZ;

        public NativeArray<Vector3> vertices;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            using(var dataArray = Mesh.AcquireReadOnlyMeshData(mesh))
            {
                var data = dataArray[0];
                vertices = new NativeArray<Vector3>(mesh.vertexCount, Allocator.TempJob);
                data.GetVertices(vertices);
            }

            resultX = new NativeArray<float>(mesh.vertexCount, Allocator.TempJob);
            resultY = new NativeArray<float>(mesh.vertexCount, Allocator.TempJob);
            resultZ = new NativeArray<float>(mesh.vertexCount, Allocator.TempJob);


            VerticesToPointsJob jobData = new VerticesToPointsJob
            {
                count = mesh.vertices.Length,
                vertices = vertices,
                resultX = resultX,
                resultY = resultY,
                resultZ = resultZ
            };
            handle = jobData.Schedule();

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(mesh.vertices.Length);

            points.SetAttributeList(DefaultAttributes.PosX, resultX.ToArray());
            points.SetAttributeList(DefaultAttributes.PosY, resultY.ToArray());
            points.SetAttributeList(DefaultAttributes.PosZ, resultZ.ToArray());

            vertices.Dispose();
            resultX.Dispose();
            resultY.Dispose();
            resultZ.Dispose();
        }

        struct VerticesToPointsJob : IJob
        {
            public int count;
            public NativeArray<Vector3> vertices;
            public NativeArray<float> resultX;
            public NativeArray<float> resultY;
            public NativeArray<float> resultZ;

            public void Execute()
            {
                for(int i = 0; i < count; i++)
                {
                    resultX[i] = vertices[i].x;
                    resultY[i] = vertices[i].y;
                    resultZ[i] = vertices[i].z;
                }
            }
        }
    }
}
