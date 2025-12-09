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
        public string attributeName = DefaultAttributes.LastModifiedAttribute;

        [SerializeField]
        public MathOperators.Statistics statistics;

        [Input]
        public PCGPointData pointsIn;

        [Output]
        public float statisticsResult;

        public NativeArray<float> result;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandlePointErrors(pointsIn)) return emptyHandle;

            result = new NativeArray<float>(pointsIn.GetAttributeList<float>(attributeName), Allocator.TempJob);

            AttributeStatisticsJob jobData = new AttributeStatisticsJob
            {
                statistics = (int)statistics,
                count = pointsIn.Count,
                result = result
            };
            handle = jobData.Schedule();

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            statisticsResult = result[0];

            result.Dispose();
        }

        struct AttributeStatisticsJob : IJob
        {
            public int statistics;
            public int count;
            public NativeArray<float> result;

            public void Execute()
            {
                float value = 0;
                if (statistics == 0)
                {
                    result[0] = Median();
                }

                if(statistics == 1)
                {
                    if((count + 1) % 2 == 0)
                    {
                        result[0] = result[(count + 1) / 2];
                    }
                    else
                    {
                        result[0] = (result[count / 2] + result[count / 2 + 1]) / 2;
                    }
                }

                if(statistics == 2)
                {
                    for (int i = 0; i < count; i++)
                    {
                        value += result[i];
                    }
                    result[0] = value;
                }

                if(statistics == 3)
                {
                    result[0] = MinValue();
                }

                if (statistics == 4)
                {
                    result[0] = MaxValue();
                }

                if(statistics == 5)
                {
                    float maxVal = MaxValue();
                    float minVal = MinValue();

                    result[0] = maxVal - minVal;
                }

                if(statistics == 6)
                {
                    result[0] = Mathf.Sqrt(Variance());
                }

                if (statistics == 7)
                {
                    result[0] = Variance();
                }
            }

            float Median()
            {
                float value = 0;
                for (int i = 0; i < count; i++)
                {
                    value += result[i];
                }
                return value / count;
            }

            float MinValue()
            {
                float value = float.MaxValue;
                for (int i = 0; i < count; i++)
                {
                    if (result[i] < value)
                    {
                        value = result[i];
                    }
                }
                return value;
            }

            float MaxValue()
            {
                float value = float.MinValue;
                for (int i = 0; i < count; i++)
                {
                    if (result[i] > value)
                    {
                        value = result[i];
                    }
                }
                return value;
            }

            float Variance()
            {
                float median = Median();
                float add = 0;
                for(int i = 0; i < count; i++)
                {
                    add += (result[i] - median) * (result[i] - median);
                }
                return add / (count - 1);
            }
        }
    }
}
