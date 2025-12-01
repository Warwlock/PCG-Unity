using GraphProcessor;
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Math/Trigonometry", typeof(PCGGraph))]
    public class Trigonometry : BaseJobNode
    {
        [SerializeField]
        public MathOperators.TrigonometricFunctions mathFunctions;

        [Input]
        public PCGPointData pointsA;

        /*[Input]
        public*/ PCGPointData pointsB;

        public string attributeA = DefaultAttributes.LastModifiedAttribute;
        /*public*/ string attributeB = DefaultAttributes.LastModifiedAttribute;
        public string attributeOut = DefaultAttributes.LastModifiedAttribute;

        [Output]
        public PCGPointData points;

        public NativeArray<float> inPoint;
        public NativeArray<float> result;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (CheckNull(pointsA)) return emptyHandle;
            //if (CheckNull(pointsB)) return emptyHandle;

            /*if (pointsA.Count < pointsB.Count)
            {
                throw new Exception($"Mismatch between the number of points from pointsB[{pointsB.Count}] and pointsA[{pointsA.Count}]");
            }*/

            //inPoint = new NativeArray<float>(pointsB.GetAttributeList<float>(attributeB), Allocator.TempJob);
            result = new NativeArray<float>(pointsA.GetAttributeList<float>(attributeA), Allocator.TempJob);
            MathJob jobData = new MathJob
            {
                mathFunctions = (int)mathFunctions,
                countA = pointsA.Count,
                //countB = pointsB.Count,
                //inPoint = inPoint,
                result = result
            };
            handle = jobData.Schedule();

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(pointsA);

            points.SetAttributeList(attributeOut, result.ToArray());

            result.Dispose();
            inPoint.Dispose();
        }

        struct MathJob : IJob
        {
            public int mathFunctions;
            public int countA;
            //public int countB;
            //public NativeArray<float> inPoint;
            public NativeArray<float> result;

            public void Execute()
            {
                for (int a = 0; a < countA; a++)
                {
                    //var index = a % countB;
                    if (mathFunctions == 0) result[a] = math.sin(result[a]);
                    if (mathFunctions == 1) result[a] = math.cos(result[a]);
                    if (mathFunctions == 2) result[a] = math.tan(result[a]);
                    if (mathFunctions == 4) result[a] = math.asin(result[a]);
                    if (mathFunctions == 5) result[a] = math.acos(result[a]);
                    if (mathFunctions == 6) result[a] = math.atan(result[a]);
                }
            }
        }
    }
}
