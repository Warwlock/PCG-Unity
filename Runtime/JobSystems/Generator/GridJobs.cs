using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    struct SimplePointGridJob : IJob
    {
        public int numX;
        public int numY;
        public float pointDst;
        public NativeArray<float> grid;

        public void Execute()
        {
            for (int y = 0; y < numY; y++)
            {
                for (int x = 0; x < numX; x++)
                {
                    int index = x + (y * numX);
                    float posX = x * pointDst;
                    float posZ = y * pointDst;

                    grid[index] = posX;
                    grid[index + numX * numY * 2] = posZ;
                }
            }
        }
    }
}
