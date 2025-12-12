using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using NoiseDotNet;
using Unity.Mathematics;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Generator/NoiseGenerator", typeof(PCGGraph))]
    public class NoiseGenerator : BaseJobNode
    {
        [SerializeField]
        public MathOperators.NoiseFunctions noiseFunctions;

        [Input]
        public PCGPointData pointsIn;

        [Input, SerializeField]
        public Vector3 frequency = new Vector3(0.1f, 0.1f, 0.1f);
        [Input, SerializeField]
        public float amplitude = 1f, lacunarity = 2f, gain = 0.5f;

        [Input, SerializeField, Range(1, 16)]
        public int octaves = 1;

        string attributeIn = DefaultAttributes.Pos;
        public string attributeOut = DefaultAttributes.Density;

        [Output]
        public PCGPointData points;

        public NativeArray<float> result;
        public NativeArray<float> result1;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandlePointErrors(pointsIn)) return emptyHandle;
            points = new PCGPointData(pointsIn);

            points.CreateAttribute(attributeOut, 0f);

            result = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);
            result1 = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);
            handle = JobCreator(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points.SetAttributeList(attributeOut, result.ToArray());

            result.Dispose();
            result1.Dispose();
        }

        JobHandle JobCreator(JobHandle dependsOn = default)
        {
            NativeArray<Vector3> arrayPos;
            NativeArray<float> x;
            NativeArray<float> y;
            NativeArray<float> z;

            if(noiseFunctions == MathOperators.NoiseFunctions.PerlinNoise)
            {
                arrayPos = new NativeArray<Vector3>(pointsIn.GetAttributeList<Vector3>(DefaultAttributes.Pos), Allocator.TempJob);
                x = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);
                y = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);
                z = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);

                SeparateAllVector3Job separateJob = new SeparateAllVector3Job
                {
                    array = arrayPos,
                    x = x,
                    y = y,
                    z = z
                };
                dependsOn = separateJob.Schedule(dependsOn);


                NativeArrayNoiseJob noiseJob = new NativeArrayNoiseJob
                {
                    noiseType = NoiseType.GradientNoise3D,
                    seed = (graph as PCGGraph).seed,
                    xFrequency = frequency.x,
                    yFrequency = frequency.y,
                    zFrequency = frequency.z,
                    amplitude1 = amplitude,
                    amplitude2 = amplitude,
                    lacunarity = lacunarity,
                    persistence = gain,
                    octaves = octaves,
                    xBuffer = x,
                    yBuffer = y,
                    zBuffer = z,
                    output1Buffer = result,
                    output2Buffer = result1
                };
                dependsOn = noiseJob.Schedule(dependsOn);

                arrayPos.Dispose(dependsOn);
                x.Dispose(dependsOn);
                y.Dispose(dependsOn);
                z.Dispose(dependsOn);

                return dependsOn;
            }
            else if (noiseFunctions == MathOperators.NoiseFunctions.CellularNoise)
            {
                arrayPos = new NativeArray<Vector3>(pointsIn.GetAttributeList<Vector3>(DefaultAttributes.Pos), Allocator.TempJob);
                x = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);
                y = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);
                z = new NativeArray<float>(pointsIn.Count, Allocator.TempJob);

                SeparateAllVector3Job separateJob = new SeparateAllVector3Job
                {
                    array = arrayPos,
                    x = x,
                    y = y,
                    z = z
                };
                dependsOn = separateJob.Schedule(dependsOn);

                NativeArrayNoiseJob noiseJob = new NativeArrayNoiseJob
                {
                    noiseType = NoiseType.CellularNoise3D,
                    seed = (graph as PCGGraph).seed,
                    xFrequency = frequency.x,
                    yFrequency = frequency.y,
                    zFrequency = frequency.z,
                    amplitude1 = amplitude,
                    amplitude2 = amplitude,
                    lacunarity = lacunarity,
                    persistence = gain,
                    octaves = octaves,
                    xBuffer = x,
                    yBuffer = y,
                    zBuffer = z,
                    output1Buffer = result,
                    output2Buffer = result1
                };
                dependsOn = noiseJob.Schedule(dependsOn);

                arrayPos.Dispose(dependsOn);
                x.Dispose(dependsOn);
                y.Dispose(dependsOn);
                z.Dispose(dependsOn);

                return dependsOn;
            }
            else
            {
                RandomNoiseJob noiseJob = new RandomNoiseJob
                {
                    seed = (graph as PCGGraph).seed,
                    result = result
                };
                return noiseJob.Schedule(dependsOn);
            }
        }
    }
}
