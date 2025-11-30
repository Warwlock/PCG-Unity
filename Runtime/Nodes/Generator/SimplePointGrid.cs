using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Generator/SimplePointGrid", typeof(PCGGraph))]
    public class SimplePointGrid : BaseJobNode
    {
        [SerializeField]
        public int XPoints = 10;

        [SerializeField]
        public int YPoints = 10;

        [SerializeField]
        public float pointDistance = 1f;

        [Output]
        public PCGPointData points;

        public NativeArray<float> gridX;
        public NativeArray<float> gridZ;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            gridX = new NativeArray<float>(XPoints * YPoints, Allocator.TempJob);
            gridZ = new NativeArray<float>(XPoints * YPoints, Allocator.TempJob);
            CreateGridJob jobData = new CreateGridJob
            {
                numX = XPoints,
                numY = YPoints,
                pointDst = pointDistance,
                gridX = gridX,
                gridZ = gridZ
            };
            handle = jobData.Schedule();

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(XPoints * YPoints);

            points.SetAttributeList(DefaultAttributes.PosX, gridX.ToArray());
            points.SetAttributeList(DefaultAttributes.PosZ, gridZ.ToArray());

            Debug.Log(points.Count);

            gridX.Dispose();
            gridZ.Dispose();
        }

        struct CreateGridJob : IJob
        {
            public int numX;
            public int numY;
            public float pointDst;
            public NativeArray<float> gridX;
            public NativeArray<float> gridZ;

            public void Execute()
            {
                for (int y = 0; y < numY; y++)
                {
                    for (int x = 0; x < numX; x++)
                    {
                        int index = x + (y * numX);
                        float posX = x * pointDst;
                        float posZ = y * pointDst;

                        gridX[index] = posX;
                        gridZ[index] = posZ;
                    }
                }
            }
        }
    }
}
