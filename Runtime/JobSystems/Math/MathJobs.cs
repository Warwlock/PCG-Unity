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
        public NativeArray<float> inPoint;
        public NativeArray<float> result;

        public void Execute()
        {
            for (int a = 0; a < countA; a++)
            {
                var indexA = a;
                var indexB = a % countB;

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
        public NativeArray<float> inPoint;
        public NativeArray<float> result;

        public void Execute()
        {
            for (int a = 0; a < countA; a++)
            {
                var index = a % countB;
                if (mathFunctions == 0) result[a] = math.min(result[a], inPoint[index]);
                if (mathFunctions == 1) result[a] = math.max(result[a], inPoint[index]);
                if (mathFunctions == 2) result[a] = result[a] < inPoint[index] ? 1f : 0f;
                if (mathFunctions == 3) result[a] = result[a] > inPoint[index] ? 1f : 0f;
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
