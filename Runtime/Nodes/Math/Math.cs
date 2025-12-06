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

            if (CheckNull(pointsA)) return emptyHandle;
            if (CheckNull(pointsB)) return emptyHandle;

            if (pointsA.Count < pointsB.Count)
            {
                throw new Exception($"Mismatch between the number of points from pointsB[{pointsB.Count}] and pointsA[{pointsA.Count}]");
            }

            if(pointsA.GetDataType(attributeA) != pointsB.GetDataType(attributeB))
            {
                throw new Exception($"Mismatch between the types from pointsB[{pointsB.Count}] and pointsA[{pointsA.Count}]");
            }

            /*inPoint = new NativeArray<float>(pointsB.GetAttributeList<float>(attributeB), Allocator.TempJob);
            result = new NativeArray<float>(pointsA.GetAttributeList<float>(attributeA), Allocator.TempJob);*/

            result = new NativeArrayCollection(pointsA, attributeA);
            inPoint = new NativeArrayCollection(pointsB, attributeB);

            handle = Vector3Math(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(pointsA);

            points.SetAttributeList(attributeOut, result.vector3Array.ToArray());

            result.Dispose();
            inPoint.Dispose();
        }

        JobHandle Vector3Math(JobHandle dependsOn)
        {
            NativeArray<float> interArrayA = new NativeArray<float>(pointsA.Count * 3, Allocator.TempJob);
            FlattenVector3Job flattenJobA = new FlattenVector3Job
            {
                count = pointsA.Count,
                vector = result.vector3Array,
                result = interArrayA
            };
            JobHandle flattenJobAHandle = flattenJobA.Schedule(dependsOn);

            NativeArray<float> interArrayB = new NativeArray<float>(pointsB.Count * 3, Allocator.TempJob);
            FlattenVector3Job flattenJobB = new FlattenVector3Job
            {
                count = pointsB.Count,
                vector = inPoint.vector3Array,
                result = interArrayB
            };
            JobHandle flattenJobBHandle = flattenJobB.Schedule(flattenJobAHandle);


            MathJob jobData = new MathJob
            {
                mathFunctions = (int)mathFunctions,
                countA = pointsA.Count,
                countB = pointsB.Count,
                MultiplierA = result.stripAxis > 0 ? -result.stripAxis : 3,
                MultiplierB = inPoint.stripAxis > 0 ? -inPoint.stripAxis : 3,
                inPoint = interArrayB,
                result = interArrayA
            };
            JobHandle jobDataHandle = jobData.Schedule(flattenJobBHandle);

            CombineVector3Job combineResult = new CombineVector3Job
            {
                count = pointsA.Count,
                array = interArrayA,
                result = result.vector3Array
            };
            JobHandle combineHandle = combineResult.Schedule(jobDataHandle);

            interArrayA.Dispose(combineHandle);
            interArrayB.Dispose(jobDataHandle);

            return combineHandle;
        }
    }
}
