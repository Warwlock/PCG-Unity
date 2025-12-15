using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Mesh/PointsToTerrain", typeof(PCGGraph))]
    public class PointsToTerrain : BaseJobNode
    {
        [SerializeField]
        public int chunkX = 5, chunkY = 5;

        [Input]
        public PCGPointData pointsIn;

        public NativeArray<float> result;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            result = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);

            PointsToTerrainJob jobData = new PointsToTerrainJob
            {
                value = value,
                count = pointsIn.Count,
                result = result
            };
            handle = jobData.Schedule();

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            pointsIn.SetAttributeList(DefaultAttributes.Density, result.ToArray());

            points = pointsIn;

            result.Dispose();
        }

        struct PointsToTerrainJob : IJob
        {
            public float value;
            public int count;
            public NativeArray<float> result;

            public void Execute()
            {
                for(int i = 0; i < count; i++)
                {
                    result[i] = value * 2;
                }
            }
        }
    }
}
