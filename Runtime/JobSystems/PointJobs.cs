using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    struct TranslatePointsJob : IJob
    {
        public bool absolute;
        public Vector3 max;
        public Vector3 min;
        public NativeArray<Vector3> vector;
        public NativeArray<float> density;

        public void Execute()
        {
            if (absolute)
                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] = Vector3.Lerp(min, max, density[i]);
                }
            else
                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] += Vector3.Lerp(min, max, density[i]);
                }
        }
    }

    struct RotatePointsJob : IJob
    {
        public bool absolute;
        public Vector3 max;
        public Vector3 min;
        public NativeArray<Vector3> vector;
        public NativeArray<float> density;

        public void Execute()
        {
            var maxRot = Quaternion.Euler(max);
            var minRot = Quaternion.Euler(min);
            if (absolute)
                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] = Quaternion.Lerp(minRot, maxRot, density[i]).eulerAngles;
                }
            else
                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] = (Quaternion.Euler(vector[i]) * Quaternion.Lerp(minRot, maxRot, density[i])).eulerAngles;
                }
        }
    }

    struct ScalePointsJob : IJob
    {
        public bool absolute;
        public Vector3 max;
        public Vector3 min;
        public NativeArray<Vector3> vector;
        public NativeArray<float> density;

        public void Execute()
        {
            if (absolute)
                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] = Vector3.Lerp(min, max, density[i]);
                }
            else
                for (int i = 0; i < vector.Length; i++)
                {
                    vector[i] = Vector3.Scale(vector[i], Vector3.Lerp(min, max, density[i]));
                }
        }
    }
}
