using GraphProcessor;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Utils/AttributeStatistics", typeof(PCGGraph))]
    public class AttributeStatistics : BaseJobNode
    {
        [SerializeField]
        public string attributeName;

        [Input]
        public PCGPointData pointsIn;

        [Output]
        public PCGPointData points;

        public NativeArray<float> result;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            result = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);
            AttributeStatisticsJob jobData = new AttributeStatisticsJob
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

        struct AttributeStatisticsJob : IJob
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
