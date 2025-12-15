using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Generator/ChunkedGridTerrain", typeof(PCGGraph))]
    public class ChunkedGridTerrain : BaseJobNode
    {
        [SerializeField]
        public int pointsPerChunk = 10;
        public int chunkX = 5, chunkY = 5;
        public float pointDst = 0.1f;

        [Output]
        public PCGPointData points;

        public NativeArray<Vector3> result;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            int totalPoints = (pointsPerChunk * chunkX) * (pointsPerChunk * chunkY);
            result = new NativeArray<Vector3>(totalPoints, Allocator.TempJob);

            ChunkGridTerrainJob jobData = new ChunkGridTerrainJob
            {
                numX = pointsPerChunk,
                numY = pointsPerChunk,
                chunkX = chunkX,
                chunkY = chunkY,
                pointDst = pointDst,
                result = result
            };

            handle = jobData.Schedule(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            int totalPoints = (pointsPerChunk * chunkX) * (pointsPerChunk * chunkY);
            points = new PCGPointData(totalPoints);

            points.SetAttributeList(DefaultAttributes.Pos, result.ToArray());

            result.Dispose();
        }
    }
}
