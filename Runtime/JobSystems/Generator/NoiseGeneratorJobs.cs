#define QUADRATIC

using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using NoiseDotNet;

namespace PCG
{
    struct RandomNoiseJob : IJobFor
    {
        public Unity.Mathematics.Random random;
        public NativeArray<float> result;

        public void Execute(int i)
        {
            result[i] = random.NextFloat();
        }
    }

    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct GradientNoise3DJob : IJobFor
    {
        public int seed;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude;

        public NativeArray<float> xBuffer, yBuffer, zBuffer, outputBuffer;

        [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
        public void Execute(int i)
        {
            outputBuffer[i] = (Noise.GradientNoise3DVector(xBuffer[i] * xFrequency, yBuffer[i] * yFrequency, zBuffer[i] * zFrequency, seed) * amplitude + 1) * 0.5f;
        }
    }

    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct GradientNoise3DFractalJob : IJobFor
    {
        public int seed;
        public int octaves;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude;
        public float lacunarity, persistence;

        public NativeArray<float> xBuffer, yBuffer, zBuffer, outputBuffer;

        [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
        public void Execute(int i)
        {
            float amplitudeA = amplitude;
            float3 frequency = new float3(xFrequency, yFrequency, zFrequency);
            float total = 0.0f;
            float maxAmplitudeA = 0.0f;

            for (int octave = 0; octave < octaves; octave++)
            {
                total += Noise.GradientNoise3DVector(xBuffer[i] * frequency.x, yBuffer[i] * frequency.y, zBuffer[i] * frequency.z, seed) * amplitudeA;
                maxAmplitudeA += amplitudeA;
                frequency *= lacunarity;
                amplitudeA *= persistence;
            }
            outputBuffer[i] = (total / maxAmplitudeA + 1) * 0.5f;
        }
    }

    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct CellularNoise3DJob : IJobFor
    {
        public int seed;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude1, amplitude2;

        public NativeArray<float> xBuffer, yBuffer, zBuffer, output1Buffer, output2Buffer;

        public void Execute(int i)
        {
            (float centerDist, float edgeDist) = Noise.CellularNoise3DVector(xBuffer[i] * xFrequency, yBuffer[i] * yFrequency, zBuffer[i] * zFrequency, seed);
            output1Buffer[i] = (centerDist * amplitude1 + 1) * 0.5f; // CenterDist
            output2Buffer[i] = (edgeDist * amplitude2 + 1) * 0.5f; // EdgeDist
        }
    }

    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct CellularNoise3DFractalJob : IJobFor
    {
        public int seed;
        public int octaves;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude1, amplitude2;
        public float lacunarity, persistence;

        public NativeArray<float> xBuffer, yBuffer, zBuffer, output1Buffer, output2Buffer;

        public void Execute(int i)
        {
            float amplitudeA = amplitude1;
            float amplitudeB = amplitude2;
            float3 frequency = new float3(xFrequency, yFrequency, zFrequency);
            float totalA = 0.0f;
            float totalB = 0.0f;
            float maxAmplitudeA = 0.0f;
            float maxAmplitudeB = 0.0f;

            for (int octave = 0; octave < octaves; octave++)
            {
                (float centerDist, float edgeDist) = Noise.CellularNoise3DVector(xBuffer[i] * frequency.x, yBuffer[i] * frequency.y, zBuffer[i] * frequency.z, seed);
                totalA += (centerDist * amplitudeA + 1) * 0.5f; // CenterDist
                totalB += (edgeDist * amplitudeB + 1) * 0.5f; // EdgeDist

                maxAmplitudeA += amplitudeA;
                maxAmplitudeB += amplitudeB;
                frequency *= lacunarity;
                amplitudeA *= persistence;
                amplitudeB *= persistence;
            }
            output1Buffer[i] = (totalA / maxAmplitudeA + 1) * 0.5f;
            output2Buffer[i] = (totalB / maxAmplitudeB + 1) * 0.5f;
        }
    }
}
