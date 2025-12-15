using Unity.Collections;
using Unity.Jobs;
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

    struct ChunkGridTerrainJob : IJob
    {
        public int numX, numY;
        public int chunkX, chunkY;
        public float pointDst;
        public NativeArray<Vector3> result;

        public void Execute()
        {
            float chunkDstX = pointDst * numX;
            float chunkDstY = pointDst * numY;
            int pointsPerChunk = numX * numY;

            for (int y = 0; y < chunkY; y++)
            {
                for (int x = 0; x < chunkX; x++)
                {
                    ChunkPointGen(x, y, pointsPerChunk);
                }
            }
        }

        void ChunkPointGen(int cX, int cY, int pointsPerChunk)
        {
            int chunkIndex = (cY * chunkX) + cX;

            float chunkOffsetX = cX * (numX - 1) * pointDst;
            float chunkOffsetZ = cY * (numY - 1) * pointDst;

            for (int y = 0; y < numY; y++)
            {
                for (int x = 0; x < numX; x++)
                {
                    int localIndex = (y * numX) + x;
                    int index = (chunkIndex * pointsPerChunk) + localIndex;

                    float posX = x * pointDst + chunkOffsetX;
                    float posZ = y * pointDst + chunkOffsetZ;

                    result[index] = new Vector3(posX, 0, posZ);
                }
            }
        }
    }
}
