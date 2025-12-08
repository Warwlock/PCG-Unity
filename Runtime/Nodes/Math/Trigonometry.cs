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

        //[Input]
        //public PCGPointData pointsB;

        public string attributeA = DefaultAttributes.LastModifiedAttribute;
        //public string attributeB = DefaultAttributes.LastModifiedAttribute;
        public string attributeOut = DefaultAttributes.LastModifiedAttribute;

        [Output]
        public PCGPointData points;

        public NativeArrayCollection result;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            HandlePointErrors(pointsA);
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

            TrigonometryMathJob jobData = new TrigonometryMathJob
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
