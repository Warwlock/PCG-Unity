using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    struct TranslatePointsJob : IJobFor
    {
        public Vector3 max;
        public Vector3 min;
        public NativeArray<Vector3> vector;
        public NativeArray<float> density;

        public void Execute(int i)
        {
            vector[i] += Vector3.Lerp(min, max, density[i]);
        }
    }

    struct TranslatePointsAbsoluteJob : IJobFor
    {
        public Vector3 max;
        public Vector3 min;
        public NativeArray<Vector3> vector;
        public NativeArray<float> density;

        public void Execute(int i)
        {
            vector[i] = Vector3.Lerp(min, max, density[i]);
        }
    }

    struct RotatePointsJob : IJobFor
    {
        public Quaternion max;
        public Quaternion min;
        public NativeArray<Vector3> vector;
        public NativeArray<float> density;

        public void Execute(int i)
        {
            vector[i] = (Quaternion.Euler(vector[i]) * Quaternion.Lerp(min, max, density[i])).eulerAngles;
        }
    }

    struct RotatePointsAbsoluteJob : IJobFor
    {
        public Quaternion max;
        public Quaternion min;
        public NativeArray<Vector3> vector;
        public NativeArray<float> density;

        public void Execute(int i)
        {
            vector[i] = Quaternion.Lerp(min, max, density[i]).eulerAngles;
        }
    }

    struct ScalePointsJob : IJobFor
    {
        public Vector3 max;
        public Vector3 min;
        public NativeArray<Vector3> vector;
        public NativeArray<float> density;

        public void Execute(int i)
        {
            vector[i] = Vector3.Scale(vector[i], Vector3.Lerp(min, max, density[i]));
        }
    }

    struct ScalePointsAbsoluteJob : IJobFor
    {
        public Vector3 max;
        public Vector3 min;
        public NativeArray<Vector3> vector;
        public NativeArray<float> density;

        public void Execute(int i)
        {
            vector[i] = Vector3.Lerp(min, max, density[i]);
        }
    }
}
