using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("EXAMPLE/NoiseGenerator", typeof(PCGGraph))]
    public class NoiseGenerator : BaseJobNode
    {
        [SerializeField]
        public float value = 10;

        [Input]
        public PCGPointData pointsIn;

        [Output]
        public PCGPointData points;

        public NativeArray<float> result;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            result = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);
            NoiseGeneratorJob jobData = new NoiseGeneratorJob
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

        struct NoiseGeneratorJob : IJob
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
