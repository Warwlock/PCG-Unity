using GraphProcessor;
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Math/Math", typeof(PCGGraph))]
    public class Math : BaseJobNode
    {
        [SerializeField]
        public MathOperators.BasicFunctions mathFunctions;

        [Input]
        public PCGPointData pointsA;

        [Input]
        public PCGPointData pointsB;

        public string attributeA = DefaultAttributes.LastModifiedAttribute;
        public string attributeB = DefaultAttributes.LastModifiedAttribute;
        public string attributeOut = DefaultAttributes.LastModifiedAttribute;

        [Output]
        public PCGPointData points;

        public NativeArrayCollection result;  // Points A
        public NativeArrayCollection inPoint; // Points B

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandleCouplePointErrors(pointsA, pointsB, attributeA, attributeB, true)) return emptyHandle;

            result = new NativeArrayCollection(pointsA, attributeA);
            inPoint = new NativeArrayCollection(pointsB, attributeB);

            handle = JobCreator(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(pointsA);
            result.SetPointAttributeList(points, attributeOut);

            result.Dispose();
            inPoint.Dispose();
        }

        JobHandle JobCreator(JobHandle dependsOn)
        {
            dependsOn = result.CreateFlattenVector3Job(dependsOn);
            dependsOn = inPoint.CreateFlattenVector3Job(dependsOn);

            MathJob jobData = new MathJob
            {
                mathFunctions = (int)mathFunctions,
                countA = result.Count,
                countB = inPoint.Count,
                dimensionA = result.dimension,
                dimensionB = inPoint.dimension,
                inPoint = inPoint.floatArray,
                result = result.floatArray
            };
            dependsOn = jobData.Schedule(dependsOn);

            dependsOn = result.CreateUnflattenVector3Job(dependsOn);

            return dependsOn;
        }
    }
}
