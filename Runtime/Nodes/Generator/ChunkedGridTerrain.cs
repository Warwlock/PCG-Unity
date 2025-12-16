using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using static PCG.MathOperators;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Generator/ChunkedGridTerrain", typeof(PCGGraph))]
    public class ChunkedGridTerrain : BaseJobNode
    {
        [SerializeField]
        public ChunkSizeEnum chunkSize = ChunkSizeEnum._48;
        public int chunkX = 5, chunkY = 5;
        public float pointDst = 0.1f;

        [Output]
        public PCGPointData points;

        public NativeArray<Vector3> result;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            int totalPoints = ((int)chunkSize * chunkX) * ((int)chunkSize * chunkY);
            result = new NativeArray<Vector3>(totalPoints, Allocator.TempJob);

            ChunkGridTerrainJob jobData = new ChunkGridTerrainJob
            {
                numX = (int)chunkSize,
                numY = (int)chunkSize,
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

            int totalPoints = ((int)chunkSize * chunkX) * ((int)chunkSize * chunkY);
            points = new PCGPointData(totalPoints);

            points.SetAttributeList(DefaultAttributes.Pos, result.ToArray());

            result.Dispose();
        }
    }
}
