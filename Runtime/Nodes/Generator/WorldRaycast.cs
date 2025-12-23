using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Generator/WorldRaycast", typeof(PCGGraph))]
    public class WorldRaycast : BaseJobNode
    {
        [Input]
        public PCGPointData pointsIn;

        [SerializeField]
        public string originAttribute = DefaultAttributes.Pos;
        public Vector3 rayDirection = Vector3.down;
        public bool alignToSurfaceNormal = true;

        [Output]
        public PCGPointData points;
        public NativeArray<Vector3> resultPos;
        public NativeArray<Vector3> resultRot;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandlePointErrors(pointsIn)) return emptyHandle;

            points = new PCGPointData(pointsIn);

            points.CreateAttribute(DefaultAttributes.Pos, Vector3.zero);
            points.CreateAttribute(DefaultAttributes.Rot, Vector3.zero);

            resultPos = new NativeArray<Vector3>(pointsIn.GetAttributeList<Vector3>(originAttribute), Allocator.TempJob);
            resultRot = new NativeArray<Vector3>(pointsIn.GetAttributeList<Vector3>(DefaultAttributes.Rot), Allocator.TempJob);

            handle = JobCreator(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points.SetAttributeList(DefaultAttributes.Rot, resultRot.ToArray());
            points.SetAttributeList(DefaultAttributes.Pos, resultPos.ToArray());

            resultPos.Dispose();
            resultRot.Dispose();
        }

        JobHandle JobCreator(JobHandle dependsOn = default)
        {
            var commands = new NativeArray<RaycastCommand>(pointsIn.Count, Allocator.TempJob);
            var results = new NativeArray<RaycastHit>(pointsIn.Count, Allocator.TempJob);

            WorldRaycastConstructorJob constructJob = new WorldRaycastConstructorJob
            {
                raycastCommand = commands,
                origins = resultPos,
                direction = rayDirection
            };
            dependsOn = constructJob.Schedule(dependsOn);

            dependsOn = RaycastCommand.ScheduleBatch(commands, results, 1, dependsOn);
            commands.Dispose(dependsOn);

            WorldRaycastApplyJob applyJob = new WorldRaycastApplyJob
            {
                results = results,
                positions = resultPos,
                rotations = resultRot,
                alignNormal = alignToSurfaceNormal,
                inverseDirection = (-rayDirection).normalized
            };
            dependsOn = applyJob.Schedule(dependsOn);
            results.Dispose(dependsOn);

            return dependsOn;
        }
    }
}
