using GraphProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG.Terrain
{
    [System.Serializable, NodeMenuItem("Noise/NoiseColorLayer", typeof(PCGTerrainGraph))]
    public class NoiseColorLayer : BaseChainJobNode
    {
        [SerializeField]
        public MathOperators.NoiseFunctions noiseFunctions;

        [Input]
        public NativeArray<half4> pointsIn;

        [Input, SerializeField]
        public Vector3 frequency = new Vector3(0.1f, 0.1f, 0.1f);
        [Input, SerializeField]
        public float amplitude = 1f;

        [Output(allowMultiple = false)]
        public NativeArray<half4> points;

        public override JobHandle Process(JobHandle dependsOn)
        {
            Debug.Log("StartB");
            if (noiseFunctions == MathOperators.NoiseFunctions.PerlinNoise)
            {
                GradientNoise3DColorJob noiseJob = new GradientNoise3DColorJob
                {
                    seed = graph.seed,
                    xFrequency = frequency.x,
                    yFrequency = frequency.y,
                    zFrequency = frequency.z,
                    amplitude = amplitude,
                    color = pointsIn
                };
                dependsOn = noiseJob.ScheduleParallel(pointsIn.Length, pointsIn.Length / batchDivisor, dependsOn);

            }
            else if (noiseFunctions == MathOperators.NoiseFunctions.CellularNoise)
            {
                CellularNoise3DColorJob noiseJob = new CellularNoise3DColorJob
                {
                    seed = graph.seed,
                    xFrequency = frequency.x,
                    yFrequency = frequency.y,
                    zFrequency = frequency.z,
                    amplitude1 = amplitude,
                    amplitude2 = amplitude,
                    color = pointsIn
                };
                dependsOn = noiseJob.ScheduleParallel(pointsIn.Length, pointsIn.Length / batchDivisor, dependsOn);
            }
            else
            {
                RandomNoiseColorJob noiseJob = new RandomNoiseColorJob
                {
                    random = new Unity.Mathematics.Random((uint)graph.seed),
                    color = pointsIn
                };
                dependsOn = noiseJob.ScheduleParallel(pointsIn.Length, pointsIn.Length / batchDivisor, dependsOn);
            }

            points = pointsIn;

            return dependsOn;
        }

        public override void OnJobCompleted()
        {
            Debug.Log("NoiseLayer!");
        }
    }
}
