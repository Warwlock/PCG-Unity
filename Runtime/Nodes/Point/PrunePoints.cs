using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Point/PrunePoints", typeof(PCGGraph))]
    public class PrunePoints : BaseJobNode
    {
        [Input]
        public PCGPointData pointsIn;

        [Input, SerializeField]
        public float radius = 1f;

        [Output]
        public PCGPointData points;

        public NativeList<int> indices;
        public NativeArray<Vector3> positions;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandlePointErrors(pointsIn)) return emptyHandle;

            indices = new NativeList<int>(0, Allocator.TempJob);
            positions = new NativeArray<Vector3>(pointsIn.GetAttributeList<Vector3>(DefaultAttributes.Pos), Allocator.TempJob);

            handle = JobCreator(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points = new PCGPointData(pointsIn, indices.AsArray().ToArray());

            indices.Dispose();
        }

        JobHandle JobCreator(JobHandle dependsOn = default)
        {
            PrunePointsJob jobData = new PrunePointsJob
            {
                radius = radius,
                indices = indices,
                positions = positions
            };
            dependsOn = jobData.Schedule(dependsOn);

            return dependsOn;
        }
    }
}
