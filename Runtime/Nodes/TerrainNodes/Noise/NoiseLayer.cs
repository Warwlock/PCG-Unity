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
    [System.Serializable, NodeMenuItem("Noise/NoiseLayer", typeof(PCGTerrainGraph))]
    public class NoiseLayer : BaseChainJobNode
    {
        [SerializeField]
        public MathOperators.NoiseFunctions noiseFunctions;

        [Input]
        public int input;

        [Input, SerializeField]
        public Vector3 frequency = new Vector3(0.1f, 0.1f, 0.1f);
        [Input, SerializeField]
        public float amplitude = 1f, lacunarity = 2f, gain = 0.5f;

        [Input, SerializeField, Range(1, 16)]
        public int octaves = 1;

        string attributeIn = DefaultAttributes.Pos;
        public string attributeOut = DefaultAttributes.Density;

        [Output]
        public int output;

        public override JobHandle Process(JobHandle dependsOn)
        {
            Debug.Log("StartB");
            if (noiseFunctions == MathOperators.NoiseFunctions.PerlinNoise)
            {
                if (octaves == 1)
                {
                    GradientNoise3DTerrainJob noiseJob = new GradientNoise3DTerrainJob
                    {
                        seed = graph.seed,
                        xFrequency = frequency.x,
                        yFrequency = frequency.y,
                        zFrequency = frequency.z,
                        amplitude = amplitude,
                        verts = graph.points
                    };
                    dependsOn = noiseJob.ScheduleParallel(graph.points.Length, graph.points.Length / batchDivisor, dependsOn);
                }
                else
                {
                    GradientNoise3DFractalTerrainJob noiseJob = new GradientNoise3DFractalTerrainJob
                    {
                        seed = graph.seed,
                        xFrequency = frequency.x,
                        yFrequency = frequency.y,
                        zFrequency = frequency.z,
                        amplitude = amplitude,
                        lacunarity = lacunarity,
                        persistence = gain,
                        octaves = octaves,
                        verts = graph.points
                    };
                    dependsOn = noiseJob.ScheduleParallel(graph.points.Length, graph.points.Length / batchDivisor, dependsOn);
                }
            }
            else if (noiseFunctions == MathOperators.NoiseFunctions.CellularNoise)
            {
                if (octaves == 1)
                {
                    CellularNoise3DTerrainJob noiseJob = new CellularNoise3DTerrainJob
                    {
                        seed = graph.seed,
                        xFrequency = frequency.x,
                        yFrequency = frequency.y,
                        zFrequency = frequency.z,
                        amplitude1 = amplitude,
                        amplitude2 = amplitude,
                        verts = graph.points
                    };
                    dependsOn = noiseJob.ScheduleParallel(graph.points.Length, graph.points.Length / batchDivisor, dependsOn);
                }
                else
                {
                    CellularNoise3DFractalTerrainJob noiseJob = new CellularNoise3DFractalTerrainJob
                    {
                        seed = graph.seed,
                        xFrequency = frequency.x,
                        yFrequency = frequency.y,
                        zFrequency = frequency.z,
                        amplitude1 = amplitude,
                        amplitude2 = amplitude,
                        lacunarity = lacunarity,
                        persistence = gain,
                        octaves = octaves,
                        verts = graph.points
                    };
                    dependsOn = noiseJob.ScheduleParallel(graph.points.Length, graph.points.Length / batchDivisor, dependsOn);
                }
            }
            else
            {
                RandomNoiseTerrainJob noiseJob = new RandomNoiseTerrainJob
                {
                    random = new Unity.Mathematics.Random((uint)graph.seed),
                    verts = graph.points
                };
                dependsOn = noiseJob.ScheduleParallel(graph.points.Length, graph.points.Length / batchDivisor, dependsOn);
            }

            return dependsOn;
        }

        public override void OnJobCompleted()
        {
            Debug.Log("NoiseLayer!");
        }
    }
}
