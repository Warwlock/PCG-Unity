using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
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

    struct PrunePointsJob : IJob
    {
        public float radius;
        public NativeArray<Vector3> positions;
        public NativeList<int> indices;

        public void Execute()
        {
            float sqrRadius = radius * radius;

            for (int i = 0; i < positions.Length; i++)
            {
                Vector3 candidatePos = positions[i];
                bool shouldPrune = false;

                for (int j = 0; j < indices.Length; j++)
                {
                    int keptIndex = indices[j];
                    Vector3 keptPos = positions[keptIndex];

                    // squared distance
                    if (Vector3.SqrMagnitude(candidatePos - keptPos) < sqrRadius)
                    {
                        shouldPrune = true;
                        break;
                    }
                }

                if (!shouldPrune)
                {
                    indices.Add(i);
                }
            }
        }
    }

    struct WorldRaycastConstructorJob : IJob
    {
        public NativeArray<RaycastCommand> raycastCommand;
        public NativeArray<Vector3> origins;
        public Vector3 direction;

        public void Execute()
        {
            for(int i = 0; i < origins.Length; i++)
            {
                raycastCommand[i] = new RaycastCommand(origins[i], direction, QueryParameters.Default);
            }
        }
    }

    struct WorldRaycastApplyJob : IJob
    {
        public NativeArray<RaycastHit> results;
        public NativeArray<Vector3> positions;
        public NativeArray<Vector3> rotations;
        public bool alignNormal;

        public void Execute()
        {
            for(int i = 0; i < results.Length; i++)
            {
                if (results[i].point != Vector3.zero) // Very dirty solution; it has to be ".collider" but it only works on main thread
                {
                    positions[i] = results[i].point;
                    if(!alignNormal) continue;

                    Quaternion rotation = Quaternion.Euler(rotations[i]);
                    rotation = Quaternion.FromToRotation(Vector3.forward, results[i].normal) * rotation;
                    rotations[i] = rotation.eulerAngles;
                }
            }
        }
    }
}
