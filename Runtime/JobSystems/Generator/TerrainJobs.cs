using Unity.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;
using MeshData = UnityEngine.Mesh.MeshData;

namespace PCG
{
    struct PointsToTerrainJob : IJob
    {
        public int numX, numY;
        public float slope, bias;
        [ReadOnly] public NativeSlice<Vector3> slice;
        public MeshData meshData;

        public void Execute()
        {
            meshData.SetVertexBufferParams(numX * numY, new VertexAttributeDescriptor(VertexAttribute.Position),
                    new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1));
            slice.CopyTo(meshData.GetVertexData<Vector3>());

            int meshSimplificationIncrement = 0;
            int lodAmount = GetLodAmount();
            meshData.lodCount = lodAmount;
            meshData.lodSelectionCurve = new Mesh.LodSelectionCurve(slope, bias);

            int quadsX = numX - 1;
            int quadsY = numY - 1;
            uint[] lodStartIndex = new uint[lodAmount];
            uint[] lodIndexCount = new uint[lodAmount];

            int indexCount = 0;
            for(int i = 0; i < lodAmount; i++)
            {
                int amountIncrement = i == 0 ? 1 : i * 2;
                int amount = (quadsX * quadsY) * 6 / (amountIncrement * amountIncrement);
                lodIndexCount[i] = (uint)amount;

                lodStartIndex[i] = (uint)indexCount;

                indexCount += amount;
            }

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
            var indexData = meshData.GetIndexData<uint>();

            meshData.subMeshCount = lodAmount;

            int iIndex = 0;
            for (int i = 0; i < lodAmount; i++)
            {
                meshSimplificationIncrement = i == 0 ? 1 : i * 2;
                for (int y = 0; y < quadsY; y += meshSimplificationIncrement)
                {
                    for (int x = 0; x < quadsX; x += meshSimplificationIncrement)
                    {
                        int rootIndex = x + (y * numX);

                        int bl = rootIndex;                     // Bottom-Left
                        int br = rootIndex + 1 * meshSimplificationIncrement;             // Bottom-Right
                        int tl = rootIndex + numX * meshSimplificationIncrement;          // Top-Left (Row above)
                        int tr = rootIndex + numX * meshSimplificationIncrement + 1 * meshSimplificationIncrement;  // Top-Right

                        // Triangle 1 (Bottom-Left -> Top-Left -> Bottom-Right)
                        indexData[iIndex++] = (uint)bl;
                        indexData[iIndex++] = (uint)tl;
                        indexData[iIndex++] = (uint)br;

                        // Triangle 2 (Bottom-Right -> Top-Left -> Top-Right)
                        indexData[iIndex++] = (uint)br;
                        indexData[iIndex++] = (uint)tl;
                        indexData[iIndex++] = (uint)tr;
                    }
                }
            }

            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount));

            for(int i = 0; i < lodAmount; i++)
            {
                meshData.SetLod(0, i, new MeshLodRange(lodStartIndex[i], lodIndexCount[i]));
            }
        }

        int GetLodAmount()
        {
            switch (numX)
            {
                case 241:
                    return 7;
                case 121:
                    return 7;
                case 73:
                    return 5;
                case 49:
                default:
                    return 5;
                case 25:
                    return 5;
                case 13:
                    return 4;
            }
        }
    }
}
