using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    struct FlattenVector3Job : IJob
    {
        public int count;
        public NativeArray<Vector3> vector;
        public NativeArray<float> result;

        public void Execute()
        {
            for(int i = 0; i < count; i++)
            {
                result[i] = vector[i].x;
                result[i + count] = vector[i].y;
                result[i + count + count] = vector[i].z;
            }
        }
    }

    struct UnflattenVector3Job : IJob
    {
        public int count;
        public NativeArray<float> array;
        public NativeArray<Vector3> result;

        public void Execute()
        {
            for(int i = 0; i < count; i++)
            {
                Vector3 vector = new Vector3();

                vector.x = array[i];
                vector.y = array[count + i];
                vector.z = array[count * 2 + i];

                result[i] = vector;
            }
        }
    }

    struct SeparateVector3Job : IJob
    {
        public int count;
        public int axis;
        public NativeArray<Vector3> vector;
        public NativeArray<float> result;

        public void Execute() // There is a better way but I felt lazy to write
        {
            for (int i = 0; i < count; i++)
            {
                if (axis == 1)
                    result[i] = vector[i].x;
                else if (axis == 2)
                    result[i] = vector[i].y;
                else if (axis == 3)
                    result[i] = vector[i].z;
            }
        }
    }

    struct SeparateAllVector3Job : IJobFor
    {
        public NativeArray<Vector3> array;
        public NativeArray<float> x;
        public NativeArray<float> y;
        public NativeArray<float> z;

        public void Execute(int i)
        {
            x[i] = array[i].x;
            y[i] = array[i].y;
            z[i] = array[i].z;
        }
    }

    struct SeparateAllFloat3Job : IJobFor
    {
        public NativeArray<float3> array;
        public NativeArray<float> x;
        public NativeArray<float> y;
        public NativeArray<float> z;

        public void Execute(int i)
        {
            x[i] = array[i].x;
            y[i] = array[i].y;
            z[i] = array[i].z;
        }
    }

    struct CombineVector3Job : IJob
    {
        public int count;
        public int axis;
        public NativeArray<float> array;
        public NativeArray<Vector3> result;

        public void Execute() // There is a better way but I felt lazy to write
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 vector = result[i];
                if (axis == 1)
                    vector.x = array[i];
                else if (axis == 2)
                    vector.y = array[i];
                else if (axis == 3)
                    vector.z = array[i];

                result[i] = vector;
            }
        }
    }

    // ------------------------ Vector2 Jobs ------------------------
    struct FlattenVector2Job : IJob
    {
        public int count;
        public NativeArray<Vector2> vector;
        public NativeArray<float> result;

        public void Execute()
        {
            for (int i = 0; i < count; i++)
            {
                result[i] = vector[i].x;
                result[i + count] = vector[i].y;
            }
        }
    }

    struct SeparateVector2Job : IJob
    {
        public int count;
        public int axis;
        public NativeArray<Vector2> vector;
        public NativeArray<float> result;

        public void Execute() // There is a better way but I felt lazy to write
        {
            for (int i = 0; i < count; i++)
            {
                if (axis == 1)
                    result[i] = vector[i].x;
                else if (axis == 2)
                    result[i] = vector[i].y;
            }
        }
    }

    struct CombineVector2Job : IJob
    {
        public int count;
        public NativeArray<float> array;
        public NativeArray<Vector2> result;

        public void Execute()
        {
            for (int i = 0; i < count; i++)
            {
                Vector2 vector = new Vector2();

                vector.x = array[i];
                vector.y = array[count + i];

                result[i] = vector;
            }
        }
    }
}
