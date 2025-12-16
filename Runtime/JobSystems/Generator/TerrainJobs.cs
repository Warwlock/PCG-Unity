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
        [ReadOnly] public NativeSlice<Vector3> slice;
        public NativeArray<Vector3> normals;
        public MeshData meshData;

        public bool useLOD;
        public float slope, bias;

        public void Execute()
        {

            meshData.SetVertexBufferParams(numX * numY, new VertexAttributeDescriptor(VertexAttribute.Position),
                new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1));
            var vertices = meshData.GetVertexData<Vector3>(0);
            slice.CopyTo(vertices);

            int meshSimplificationIncrement = 1;
            int lodAmount = GetLodAmount();

            if (useLOD)
            {
                meshData.lodCount = lodAmount;
                meshData.lodSelectionCurve = new Mesh.LodSelectionCurve(slope, bias);
            }

            int quadsX = numX - 1;
            int quadsY = numY - 1;
            uint[] lodStartIndex = new uint[lodAmount];
            uint[] lodIndexCount = new uint[lodAmount];

            int indexCount = 0;
            for (int i = 0; i < lodAmount; i++)
            {
                int amountIncrement = i == 0 ? 1 : i * 2;
                int amount = ((quadsX - 2) * (quadsX - 2)) * 6 / (amountIncrement * amountIncrement);
                lodIndexCount[i] = (uint)amount;

                lodStartIndex[i] = (uint)indexCount;

                indexCount += amount;
            }

            int normalIndexAmount = (quadsX * quadsY) * 6;
            meshData.SetIndexBufferParams(normalIndexAmount, IndexFormat.UInt32);
            var indices = meshData.GetIndexData<uint>();

            // Triangle calculation for normals
            int iIndex = 0;
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
                    indices[iIndex++] = (uint)bl;
                    indices[iIndex++] = (uint)tl;
                    indices[iIndex++] = (uint)br;

                    // Triangle 2 (Bottom-Right -> Top-Left -> Top-Right)
                    indices[iIndex++] = (uint)br;
                    indices[iIndex++] = (uint)tl;
                    indices[iIndex++] = (uint)tr;
                }
            }

            // Normal calculation
            int triangleAmount = normalIndexAmount / 3;
            for (int i = 0; i < triangleAmount; i++)
            {
                int normalTriIndex = i * 3;
                int indexVertexA = (int)indices[normalTriIndex];
                int indexVertexB = (int)indices[normalTriIndex + 1];
                int indexVertexC = (int)indices[normalTriIndex + 2];

                Vector3 pointA = vertices[indexVertexA];
                Vector3 pointB = vertices[indexVertexB];
                Vector3 pointC = vertices[indexVertexC];

                Vector3 sideAB = pointB - pointA;
                Vector3 sideAC = pointC - pointA;
                var triNormal = Vector3.Cross(sideAB, sideAC).normalized;

                normals[indexVertexA] += triNormal;
                normals[indexVertexB] += triNormal;
                normals[indexVertexC] += triNormal;
            }
            for (int i = 0; i < normals.Length; i++)
            {
                normals[i].Normalize();
            }
            normals.CopyTo(meshData.GetVertexData<Vector3>(1));

            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
            indices = meshData.GetIndexData<uint>();

            // Full triangle calculation
            iIndex = 0;
            for (int i = 0; i < lodAmount; i++)
            {
                meshSimplificationIncrement = i == 0 ? 1 : i * 2;
                for (int y = 1; y < quadsY - 1; y += meshSimplificationIncrement)
                {
                    for (int x = 1; x < quadsX - 1; x += meshSimplificationIncrement)
                    {
                        int rootIndex = x + (y * (numX));

                        int bl = rootIndex;                     // Bottom-Left
                        int br = rootIndex + 1 * meshSimplificationIncrement;             // Bottom-Right
                        int tl = rootIndex + (numX) * meshSimplificationIncrement;          // Top-Left (Row above)
                        int tr = rootIndex + (numX) * meshSimplificationIncrement + 1 * meshSimplificationIncrement;  // Top-Right

                        // Triangle 1 (Bottom-Left -> Top-Left -> Bottom-Right)
                        indices[iIndex++] = (uint)bl;
                        indices[iIndex++] = (uint)tl;
                        indices[iIndex++] = (uint)br;

                        // Triangle 2 (Bottom-Right -> Top-Left -> Top-Right)
                        indices[iIndex++] = (uint)br;
                        indices[iIndex++] = (uint)tl;
                        indices[iIndex++] = (uint)tr;
                    }
                }
            }

            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount));

            if (useLOD)
            {
                for (int i = 0; i < lodAmount; i++)
                {
                    meshData.SetLod(0, i, new MeshLodRange(lodStartIndex[i], lodIndexCount[i]));
                }
            }
        }

        int GetLodAmount()
        {
            if (!useLOD) return 1;
            switch (numX)
            {
                case 243:
                    return 7;
                case 123:
                    return 7;
                case 75:
                    return 5;
                case 51:
                default:
                    return 5;
                case 27:
                    return 5;
                case 15:
                    return 4;
            }
        }
    }
}
