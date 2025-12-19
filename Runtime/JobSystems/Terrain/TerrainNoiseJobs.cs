using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using NoiseDotNet;

namespace PCG
{
    struct RandomNoiseTerrainJob : IJobFor
    {
        public Unity.Mathematics.Random random;
        public NativeArray<float3> verts;

        public void Execute(int i)
        {
            float3 val = verts[i];
            val.y += random.NextFloat();
            verts[i] = val;
        }
    }

    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct GradientNoise3DTerrainJob : IJobFor
    {
        public int seed;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude;

        public NativeArray<float3> verts;

        [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
        public void Execute(int i)
        {
            float3 val = verts[i];
            float3 freq = new float3(xFrequency, yFrequency, zFrequency);
            val.y += (Noise.GradientNoise3DVector(verts[i].x * xFrequency, verts[i].y * yFrequency, verts[i].z * zFrequency, seed) * amplitude + 1) * 0.5f;
            verts[i] = val;
        }
    }

    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct GradientNoise3DFractalTerrainJob : IJobFor
    {
        public int seed;
        public int octaves;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude;
        public float lacunarity, persistence;

        public NativeArray<float3> verts;

        [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
        public void Execute(int i)
        {
            float amplitudeA = amplitude;
            float3 frequency = new float3(xFrequency, yFrequency, zFrequency);
            float total = 0.0f;
            float maxAmplitudeA = 0.0f;

            for (int octave = 0; octave < octaves; octave++)
            {
                total += Noise.GradientNoise3DVector(verts[i].x * frequency.x, verts[i].y * frequency.y, verts[i].z * frequency.z, seed) * amplitudeA;
                maxAmplitudeA += amplitudeA;
                frequency *= lacunarity;
                amplitudeA *= persistence;
            }
            float3 val = verts[i];
            val.y += (total + 1) * 0.5f;
            verts[i] = val;
        }
    }

    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct CellularNoise3DTerrainJob : IJobFor
    {
        public int seed;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude1, amplitude2;

        public NativeArray<float3> verts;

        public void Execute(int i)
        {
            (float centerDist, float edgeDist) = Noise.CellularNoise3DVector(verts[i].x * xFrequency, verts[i].y * yFrequency, verts[i].z * zFrequency, seed);
            float3 val = verts[i];
            val.y += (centerDist * amplitude1 + 1) * 0.5f; // CenterDist
            verts[i] = val;
            //output2Buffer[i] = (edgeDist * amplitude2 + 1) * 0.5f; // EdgeDist
        }
    }

    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct CellularNoise3DFractalTerrainJob : IJobFor
    {
        public int seed;
        public int octaves;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude1, amplitude2;
        public float lacunarity, persistence;

        public NativeArray<float3> verts;

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
                (float centerDist, float edgeDist) = Noise.CellularNoise3DVector(verts[i].x * frequency.x, verts[i].y * frequency.y, verts[i].z * frequency.z, seed);
                totalA += (centerDist * amplitudeA + 1) * 0.5f; // CenterDist
                totalB += (edgeDist * amplitudeB + 1) * 0.5f; // EdgeDist

                maxAmplitudeA += amplitudeA;
                maxAmplitudeB += amplitudeB;
                frequency *= lacunarity;
                amplitudeA *= persistence;
                amplitudeB *= persistence;
            }
            float3 val = verts[i];
            val.y += (totalA / maxAmplitudeA + 1) * 0.5f;
            verts[i] = val;
            //output2Buffer[i] = (totalB / maxAmplitudeB + 1) * 0.5f;
        }
    }

    struct RandomNoiseColorJob : IJobFor
    {
        public Unity.Mathematics.Random random;
        public NativeArray<half4> color;

        public void Execute(int i)
        {
            color[i] = new half4(random.NextFloat4());
        }
    }

    [BurstCompile(FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct GradientNoise3DColorJob : IJobFor
    {
        public int seed;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude;

        public NativeArray<half4> color;

        [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
        public void Execute(int i)
        {
            half4 val = color[i];
            val.x = new half((Noise.GradientNoise3DVector(color[i].x * xFrequency, color[i].y * yFrequency, color[i].z * zFrequency, seed + 0) * amplitude + 1) * 0.5f);
            val.y = new half((Noise.GradientNoise3DVector(color[i].x * xFrequency, color[i].y * yFrequency, color[i].z * zFrequency, seed + 1) * amplitude + 1) * 0.5f);
            val.z = new half((Noise.GradientNoise3DVector(color[i].x * xFrequency, color[i].y * yFrequency, color[i].z * zFrequency, seed + 2) * amplitude + 1) * 0.5f);
            val.w = new half((Noise.GradientNoise3DVector(color[i].x * xFrequency, color[i].y * yFrequency, color[i].z * zFrequency, seed + 3) * amplitude + 1) * 0.5f);
            color[i] = val;
        }
    }

    [BurstCompile(CompileSynchronously = true, FloatMode = FloatMode.Fast, FloatPrecision = FloatPrecision.Low)]
    public struct CellularNoise3DColorJob : IJobFor
    {
        public int seed;
        public float xFrequency, yFrequency, zFrequency;
        public float amplitude1, amplitude2;

        public NativeArray<half4> color;

        public void Execute(int i)
        {
            (float centerDistX, float edgeDistX) = Noise.CellularNoise3DVector(color[i].x * xFrequency, color[i].y * yFrequency, color[i].z * zFrequency, seed + 0);
            (float centerDistY, float edgeDistY) = Noise.CellularNoise3DVector(color[i].x * xFrequency, color[i].y * yFrequency, color[i].z * zFrequency, seed + 1);
            (float centerDistZ, float edgeDistZ) = Noise.CellularNoise3DVector(color[i].x * xFrequency, color[i].y * yFrequency, color[i].z * zFrequency, seed + 2);
            (float centerDistW, float edgeDistW) = Noise.CellularNoise3DVector(color[i].x * xFrequency, color[i].y * yFrequency, color[i].z * zFrequency, seed + 3);
            half4 val = color[i];
            val.x = new half((centerDistX * amplitude1 + 1) * 0.5f); // CenterDist
            val.y = new half((centerDistY * amplitude1 + 1) * 0.5f);
            val.z = new half((centerDistZ * amplitude1 + 1) * 0.5f);
            val.w = new half((centerDistW * amplitude1 + 1) * 0.5f);
            color[i] = val;
            //output2Buffer[i] = (edgeDist * amplitude2 + 1) * 0.5f; // EdgeDist
        }
    }
}
