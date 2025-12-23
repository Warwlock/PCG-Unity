using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    public class NativeArrayCollection
    {
        public NativeArray<float> floatArray;
        public NativeArray<Vector3> vector3Array;
        public NativeArray<int> intArray;

        public int stripAxis = 0;
        public int Count = 0;
        public int dimension = 1;

        public Type collectionType { get; set; }

        public NativeArrayCollection(Type arrayType, int arrayLength)
        {
            collectionType = arrayType;
            Count = arrayLength;

            if (arrayType == typeof(float))
            {
                floatArray = new NativeArray<float>(arrayLength, Allocator.TempJob);
                dimension = 1;
            }
            if (arrayType == typeof(Vector3))
            {
                vector3Array = new NativeArray<Vector3>(arrayLength, Allocator.TempJob);
                dimension = 3;
            }
            if (arrayType == typeof(int))
            {
                intArray = new NativeArray<int>(arrayLength, Allocator.TempJob);
                dimension = 1;
            }
        }

        public NativeArrayCollection(PCGPointData pointData, string attributeName, int arrayLength)
        {
            collectionType = pointData.GetDataType(attributeName);
            stripAxis = pointData.stripAxis;
            Count = pointData.Count;

            if (collectionType == typeof(float))
            {
                floatArray = new NativeArray<float>(arrayLength, Allocator.TempJob);
                dimension = 1;
            }
            if (collectionType == typeof(Vector3))
            {
                vector3Array = new NativeArray<Vector3>(arrayLength, Allocator.TempJob);
                dimension = 3;
            }
            if (collectionType == typeof(int))
            {
                intArray = new NativeArray<int>(arrayLength, Allocator.TempJob);
                dimension = 1;
            }
        }

        public NativeArrayCollection(PCGPointData pointData, string attributeName)
        {
            collectionType = pointData.GetDataType(attributeName);
            stripAxis = pointData.stripAxis;
            Count = pointData.Count;

            if (collectionType == typeof(float))
            {
                floatArray = new NativeArray<float>(pointData.GetAttributeList<float>(attributeName), Allocator.TempJob);
                dimension = 1;
            }
            if (collectionType == typeof(Vector3))
            {
                vector3Array = new NativeArray<Vector3>(pointData.GetAttributeList<Vector3>(attributeName), Allocator.TempJob);
                dimension = 3;
            }
            if (collectionType == typeof(int))
            {
                intArray = new NativeArray<int>(pointData.GetAttributeList<int>(attributeName), Allocator.TempJob);
                dimension = 1;
            }
        }

        public JobHandle CreateFlattenVector3Job(JobHandle dependsOn = default)
        {
            if (collectionType != typeof(Vector3))
                return dependsOn;

            if (stripAxis == 0)
            {
                floatArray = new NativeArray<float>(vector3Array.Length * 3, Allocator.TempJob);

                FlattenVector3Job flattenJobA = new FlattenVector3Job
                {
                    count = vector3Array.Length,
                    vector = vector3Array,
                    result = floatArray
                };

                return flattenJobA.Schedule(dependsOn);
            }
            else
            {
                floatArray = new NativeArray<float>(vector3Array.Length, Allocator.TempJob);
                dimension = 1;

                SeparateVector3Job flattenJobA = new SeparateVector3Job
                {
                    count = vector3Array.Length,
                    axis = stripAxis,
                    vector = vector3Array,
                    result = floatArray
                };

                return flattenJobA.Schedule(dependsOn);
            }
        }

        public JobHandle CreateUnflattenVector3Job(JobHandle dependsOn = default)
        {
            if (collectionType != typeof(Vector3))
                return dependsOn;

            if (stripAxis == 0)
            {
                UnflattenVector3Job combineResult = new UnflattenVector3Job
                {
                    count = floatArray.Length / 3,
                    array = floatArray,
                    result = vector3Array
                };
                return combineResult.Schedule(dependsOn);
            }
            else
            {
                CombineVector3Job combineResult = new CombineVector3Job
                {
                    count = floatArray.Length,
                    axis = stripAxis,
                    array = floatArray,
                    result = vector3Array
                };
                return combineResult.Schedule(dependsOn);
            }
        }

        public JobHandle CreateStripVector3Job(int axis, JobHandle dependsOn = default)
        {
            if (collectionType != typeof(Vector3))
                return dependsOn;

            floatArray = new NativeArray<float>(vector3Array.Length, Allocator.TempJob);

            SeparateVector3Job flattenJobA = new SeparateVector3Job
            {
                count = vector3Array.Length,
                axis = stripAxis == 0 ? axis : stripAxis,
                vector = vector3Array,
                result = floatArray
            };

            return flattenJobA.Schedule(dependsOn);
        }

        public void SetPointAttributeList(PCGPointData points, string attribute)
        {
            if (collectionType == typeof(Vector3))
                points.SetAttributeList(attribute, vector3Array.ToArray());
            if (collectionType == typeof(float))
                points.SetAttributeList(attribute, floatArray.ToArray());
            if (collectionType == typeof(int))
                points.SetAttributeList(attribute, intArray.ToArray());
        }

        public void Dispose(JobHandle dependsOn = default)
        {
            floatArray.Dispose(dependsOn);
            vector3Array.Dispose(dependsOn);
            intArray.Dispose(dependsOn);
        }
    }
}
