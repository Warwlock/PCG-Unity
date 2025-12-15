using Unity.Collections;
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
        public MeshData meshData;

        public void Execute()
        {
            meshData.SetVertexBufferParams(numX * numY, new VertexAttributeDescriptor(VertexAttribute.Position),
                    new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1));
            slice.CopyTo(meshData.GetVertexData<Vector3>());

            int quadsX = numX - 1;
            int quadsY = numY - 1;
            int indexCount = (quadsX * quadsY) * 6;
            meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
            var indexData = meshData.GetIndexData<uint>();

            int iIndex = 0;
            for (int y = 0; y < quadsY; y++)
            {
                for (int x = 0; x < quadsX; x++)
                {
                    int rootIndex = x + (y * numX);

                    int bl = rootIndex;             // Bottom-Left
                    int br = rootIndex + 1;         // Bottom-Right
                    int tl = rootIndex + numX;      // Top-Left (Row above)
                    int tr = rootIndex + numX + 1;  // Top-Right

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

            meshData.subMeshCount = 1;
            meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount));
        }
    }
}
