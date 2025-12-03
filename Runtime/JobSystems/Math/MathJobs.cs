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
            for (int i = 0; i < MultiplierA; i++)
            {
                for (int a = 0; a < countA; a++)
                {
                    var indexA = (MultiplierA - 1) * countA + a;
                    var indexB = a % countB;
                    indexB = (MultiplierB - 1) * countB + indexB;

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
}
