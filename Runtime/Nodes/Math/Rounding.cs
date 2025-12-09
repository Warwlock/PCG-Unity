using GraphProcessor;
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Math/Rounding", typeof(PCGGraph))]
    public class Rounding : BaseJobNode
    {
        [SerializeField]
        public MathOperators.Rounding mathFunctions;

        [Input]
        public PCGPointData pointsA;

        public string attributeA = DefaultAttributes.LastModifiedAttribute;
        public string attributeOut = DefaultAttributes.LastModifiedAttribute;

        [Output]
        public PCGPointData points;

        public NativeArrayCollection result;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandlePointErrors(pointsA)) return emptyHandle;
            result = new NativeArrayCollection(pointsA, attributeA);

            handle = JobCreator(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(pointsA);
            result.SetPointAttributeList(points, attributeOut);

            result.Dispose();
        }

        JobHandle JobCreator(JobHandle dependsOn = default)
        {
            dependsOn = result.CreateFlattenVector3Job(dependsOn);

            RoundingMathJob jobData = new RoundingMathJob
            {
                mathFunctions = (int)mathFunctions,
                countA = result.floatArray.Length,
                result = result.floatArray
            };
            dependsOn = jobData.Schedule(dependsOn);

            dependsOn = result.CreateUnflattenVector3Job(dependsOn);

            return dependsOn;
        }
    }
}
