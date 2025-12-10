using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    struct MathJob : IJob
    {
        public int mathFunctions;
        public int countA;
        public int countB;
        public int dimensionA;
        public int dimensionB;
        public NativeArray<float> inPoint;
        public NativeArray<float> result;

        public void Execute()
        {
            int bMul = dimensionB > 1 ? 1 : 0;

            for (int i = 0; i < dimensionA; i++)
            {
                for (int a = 0; a < countA; a++)
                {
                    var indexA = i * countA + a;
                    var indexB = a % countB;
                    indexB = i * countB * bMul + indexB;

                    if (mathFunctions == 0)
                        result[indexA] += inPoint[indexB];
                    else if (mathFunctions == 1)
                        result[indexA] -= inPoint[indexB];
                    else if (mathFunctions == 2)
                        result[indexA] *= inPoint[indexB];
                    else if (mathFunctions == 3)
                        result[indexA] /= inPoint[indexB];
                    else if (mathFunctions == 5)
                        result[indexA] = math.pow(result[indexA], inPoint[indexB]);
                    else if (mathFunctions == 6)
                        result[indexA] = math.log10(result[indexA]) / math.log10(inPoint[indexB]);
                }
            }
        }
    }

    struct TrigonometryMathJob : IJob
    {
        public int mathFunctions;
        public int countA;
        public NativeArray<float> result;

        public void Execute()
        {
            for (int a = 0; a < countA; a++)
            {
                if (mathFunctions == 0) result[a] = math.sin(result[a]);
                if (mathFunctions == 1) result[a] = math.cos(result[a]);
                if (mathFunctions == 2) result[a] = math.tan(result[a]);
                if (mathFunctions == 4) result[a] = math.asin(result[a]);
                if (mathFunctions == 5) result[a] = math.acos(result[a]);
                if (mathFunctions == 6) result[a] = math.atan(result[a]);
            }
        }
    }

    struct ComparisonMathJob : IJob
    {
        public int mathFunctions;
        public int countA;
        public int countB;
        public int dimensionA;
        public int dimensionB;
        public NativeArray<float> inPoint;
        public NativeArray<float> result;

        public void Execute()
        {
            int bMul = dimensionB > 1 ? 1 : 0;

            for (int i = 0; i < dimensionA; i++)
            {
                for (int a = 0; a < countA; a++)
                {
                    var indexA = i * countA + a;
                    var indexB = a % countB;
                    indexB = i * countB * bMul + indexB;

                    if (mathFunctions == 0) result[indexA] = math.min(result[indexA], inPoint[indexB]);
                    if (mathFunctions == 1) result[indexA] = math.max(result[indexA], inPoint[indexB]);
                    if (mathFunctions == 2) result[indexA] = result[indexA] < inPoint[indexB] ? 1f : 0f;
                    if (mathFunctions == 3) result[indexA] = result[indexA] > inPoint[indexB] ? 1f : 0f;
                }
            }
        }
    }

    struct RoundingMathJob : IJob
    {
        public int mathFunctions;
        public int countA;
        public NativeArray<float> result;

        public void Execute()
        {
            for (int a = 0; a < countA; a++)
            {
                if (mathFunctions == 0) result[a] = math.round(result[a]);
                if (mathFunctions == 1) result[a] = math.floor(result[a]);
                if (mathFunctions == 2) result[a] = math.ceil(result[a]);
            }
        }
    }
}
