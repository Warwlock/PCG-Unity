using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    [BurstCompile]
    struct ChunkGridTerrainJob : IJob
    {
        public int numX, numY;
        public int chunkX, chunkY;
        public float pointDst;
        public NativeArray<float3> result;

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

            float chunkOffsetX = cX * (numX - 3) * pointDst;
            float chunkOffsetZ = cY * (numY - 3) * pointDst;

            for (int y = 0; y < numY; y++)
            {
                for (int x = 0; x < numX; x++)
                {
                    int localIndex = (y * numX) + x;
                    int index = (chunkIndex * pointsPerChunk) + localIndex;

                    float posX = x * pointDst + chunkOffsetX;
                    float posZ = y * pointDst + chunkOffsetZ;

                    result[index] = new float3(posX, 0, posZ);
                }
            }
        }
    }

    [BurstCompile]
    struct ChunkGridVertexColorJob : IJob
    {
        public int numX, numY;
        public int chunkX, chunkY;
        public float pointDst;
        public NativeArray<half4> result;

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

            float chunkOffsetX = cX * (numX - 3) * pointDst;
            float chunkOffsetZ = cY * (numY - 3) * pointDst;

            for (int y = 0; y < numY; y++)
            {
                for (int x = 0; x < numX; x++)
                {
                    int localIndex = (y * numX) + x;
                    int index = (chunkIndex * pointsPerChunk) + localIndex;

                    float posX = x * pointDst + chunkOffsetX;
                    float posZ = y * pointDst + chunkOffsetZ;

                    result[index] = new half4(new half(posX), new half(0), new half(posZ), new half(0));
                }
            }
        }
    }
}
