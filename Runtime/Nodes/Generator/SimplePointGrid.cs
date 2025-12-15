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

        public NativeArray<float> grid;
        public NativeArray<Vector3> vectorGrid;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            grid = new NativeArray<float>(XPoints * YPoints * 3, Allocator.TempJob);
            vectorGrid = new NativeArray<Vector3>(XPoints * YPoints, Allocator.TempJob);

            SimplePointGridJob createGridJob = new SimplePointGridJob
            {
                numX = XPoints,
                numY = YPoints,
                pointDst = pointDistance,
                grid = grid
            };

            UnflattenVector3Job combineJob = new UnflattenVector3Job
            {
                count = XPoints * YPoints,
                array = grid,
                result = vectorGrid
            };

            JobHandle createGridHandle = createGridJob.Schedule();
            handle = combineJob.Schedule(createGridHandle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(XPoints * YPoints);

            points.SetAttributeList(DefaultAttributes.Pos, vectorGrid.ToArray());

            grid.Dispose();
            vectorGrid.Dispose();
        }
    }
}
