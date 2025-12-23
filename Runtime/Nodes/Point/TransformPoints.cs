using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Point/TransformPoints", typeof(PCGGraph))]
    public class TransformPoints : BaseJobNode
    {
        [Input]
        public PCGPointData pointsIn;

        [Input, SerializeField]
        public Vector3 OffsetMin, OffsetMax;

        [Input, SerializeField]
        public Vector3 RotationMin, RotationMax;

        [Input, SerializeField]
        public Vector3 ScaleMin = Vector3.one, ScaleMax = Vector3.one;

        [Output]
        public PCGPointData points;

        [SerializeField]
        public bool AbsoluteOffset, AbsoluteRotation, AbsoluteScale;

        public NativeArray<Vector3> resultPos;
        public NativeArray<Vector3> resultRot;
        public NativeArray<Vector3> resultSca;
        public NativeArray<float> density;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandlePointErrors(pointsIn)) return emptyHandle;

            points = new PCGPointData(pointsIn);

            points.CreateAttribute(DefaultAttributes.Pos, Vector3.zero);
            points.CreateAttribute(DefaultAttributes.Rot, Vector3.zero);
            points.CreateAttribute(DefaultAttributes.Sca, Vector3.one);
            points.CreateAttribute(DefaultAttributes.Density, 0f);

            resultPos = new NativeArray<Vector3>(points.GetAttributeList<Vector3>(DefaultAttributes.Pos), Allocator.TempJob);
            resultRot = new NativeArray<Vector3>(points.GetAttributeList<Vector3>(DefaultAttributes.Rot), Allocator.TempJob);
            resultSca = new NativeArray<Vector3>(points.GetAttributeList<Vector3>(DefaultAttributes.Sca), Allocator.TempJob);
            density = new NativeArray<float>(points.GetAttributeList<float>(DefaultAttributes.Density), Allocator.TempJob);

            handle = JobCreator(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            points.SetAttributeList(DefaultAttributes.Rot, resultRot.ToArray());
            points.SetAttributeList(DefaultAttributes.Sca, resultSca.ToArray());
            points.SetAttributeList(DefaultAttributes.Pos, resultPos.ToArray());

            resultPos.Dispose();
            resultRot.Dispose();
            resultSca.Dispose();
            density.Dispose();
        }

        JobHandle JobCreator(JobHandle dependsOn = default)
        {
            TranslatePointsJob translateData = new TranslatePointsJob
            {
                absolute = AbsoluteOffset,
                max = OffsetMax,
                min = OffsetMin,
                density = density,
                vector = resultPos
            };
            dependsOn = translateData.Schedule(dependsOn);

            RotatePointsJob rotateData = new RotatePointsJob
            {
                absolute = AbsoluteRotation,
                max = RotationMax,
                min = RotationMin,
                density = density,
                vector = resultRot
            };
            dependsOn = rotateData.Schedule(dependsOn);

            ScalePointsJob scaleData = new ScalePointsJob
            {
                absolute = AbsoluteScale,
                max = ScaleMax,
                min = ScaleMin,
                density = density,
                vector = resultSca
            };
            dependsOn = scaleData.Schedule(dependsOn);

            return dependsOn;
        }
    }
}
