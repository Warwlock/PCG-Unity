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

            result = new NativeArrayCollection(pointsA, attributeA);
            inPoint = new NativeArrayCollection(pointsB, attributeB);

            handle = MathJobCreator(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(pointsA);

            if (result.collectionType == typeof(Vector3))
                points.SetAttributeList(attributeOut, result.vector3Array.ToArray());
            if(result.collectionType == typeof(float))
                points.SetAttributeList(attributeOut, result.floatArray.ToArray());

            result.Dispose();
            inPoint.Dispose();
        }

        JobHandle MathJobCreator(JobHandle dependsOn)
        {
            int axesA = result.collectionType == typeof(Vector3) ? 3 : 1;
            int axesB = inPoint.collectionType == typeof(Vector3) ? 3 : 1;

            JobHandle flattenJobAHandle = dependsOn;
            if (result.collectionType == typeof(Vector3))
                flattenJobAHandle = result.CreateFlattenVector3Job(dependsOn);

            JobHandle flattenJobBHandle = dependsOn;
            if (inPoint.collectionType == typeof(Vector3))
                flattenJobBHandle = inPoint.CreateFlattenVector3Job(flattenJobAHandle);

            MathJob jobData = new MathJob
            {
                mathFunctions = (int)mathFunctions,
                countA = pointsA.Count,
                countB = pointsB.Count,
                MultiplierA = result.stripAxis > 0 ? -result.stripAxis : axesA, // Support one axis (.X) (negative) and dimensions (1-3 axis) (positive)
                MultiplierB = inPoint.stripAxis > 0 ? -inPoint.stripAxis : axesB,
                inPoint = inPoint.floatArray,
                result = result.floatArray
            };
            JobHandle jobDataHandle = jobData.Schedule(flattenJobBHandle);

            if (result.collectionType == typeof(Vector3))
                jobDataHandle = result.CreateCombineVector3Job(jobDataHandle);

            return jobDataHandle;
        }
    }
}
