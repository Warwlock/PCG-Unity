using NoiseDotNet;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    struct RandomNoiseJob : IJob
    {
        public int seed;
        public NativeArray<float> result;

        public void Execute()
        {
            var random = new Unity.Mathematics.Random((uint)(100));
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = random.NextFloat();
            }
        }
    }
}
