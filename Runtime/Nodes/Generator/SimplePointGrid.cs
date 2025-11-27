using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Windows;

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
        public NativeArray<float> gridY;

        public override JobHandle OnStartJobProcess()
        {
            Debug.Log("JobStart");
            inputPorts.PullDatas();

            gridX = new NativeArray<float>(XPoints * YPoints, Allocator.TempJob);
            gridY = new NativeArray<float>(XPoints * YPoints, Allocator.TempJob);
            CreateGridJob jobData = new CreateGridJob
            {
                numX = XPoints,
                numY = YPoints,
                pointDst = pointDistance,
                gridX = gridX,
                gridY = gridY
            };
            handle = jobData.Schedule();

            Debug.Log("JobStart2");

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            Debug.Log("JobEnd");

            points = new PCGPointData();
            points.InitializePoints(XPoints * YPoints);

            points.SetAttributeList("PosX", gridX.ToArray());
            points.SetAttributeList("PosZ", gridY.ToArray());

            Debug.Log("PointCopy");

            Debug.Log(points.Count);

            gridX.Dispose();
            gridY.Dispose();
        }

        struct CreateGridJob : IJob
        {
            public int numX;
            public int numY;
            public float pointDst;
            public NativeArray<float> gridX;
            public NativeArray<float> gridY;

            public void Execute()
            {
                for (int y = 0; y < numY; y++)
                {
                    for (int x = 0; x < numX; x++)
                    {
                        int index = x + (y * numX);
                        float posX = x * pointDst;
                        float posY = y * pointDst;

                        gridX[index] = posX;
                        gridY[index] = posY;
                    }
                }
            }
        }
    }
}
