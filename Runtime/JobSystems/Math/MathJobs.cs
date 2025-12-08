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
        public int MultiplierA;
        public int MultiplierB;
        public NativeArray<float> inPoint;
        public NativeArray<float> result;

        public void Execute()
        {
            int bMul = MultiplierB > 1 ? 1 : 0;
            int bAxis = MultiplierB < 0 ? -MultiplierB - 1 : 0;

            int aMul = MultiplierA < 0 ? 1 : MultiplierA;
            int aAxis = MultiplierA < 0 ? -MultiplierA - 1 : 0;

            for (int i = 0; i < aMul; i++)
            {
                for (int a = 0; a < countA; a++)
                {
                    var indexA = i * countA + a + countA * aAxis;
                    var indexB = a % countB;
                    indexB = i * bMul * countB + indexB + countB * bAxis;

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
        public int stripAxis;
        public NativeArray<float> result;

        public void Execute()
        {
            int strip = stripAxis == 0 ? 0 : (stripAxis - 1) * (countA / 3);
            int maxCount = stripAxis == 0 ? countA : countA / 3;

            for (int a = 0; a < maxCount; a++)
            {
                int index = strip + a;
                if (mathFunctions == 0) result[index] = math.sin(result[index]);
                if (mathFunctions == 1) result[index] = math.cos(result[index]);
                if (mathFunctions == 2) result[index] = math.tan(result[index]);
                if (mathFunctions == 4) result[index] = math.asin(result[index]);
                if (mathFunctions == 5) result[index] = math.acos(result[index]);
                if (mathFunctions == 6) result[index] = math.atan(result[index]);
            }
        }
    }
}
