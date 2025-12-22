using GraphProcessor;
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Math/Filter", typeof(PCGGraph))]
    public class Filter : BaseJobNode
    {
        [SerializeField]
        public MathOperators.Compare compare;

        public float value;

        [Input]
        public PCGPointData pointsIn;

        public string attributeIn = DefaultAttributes.LastModifiedAttribute;
        public string attributeOut = DefaultAttributes.LastModifiedAttribute;

        [Output]
        public PCGPointData pointsFiltrate;
        [Output]
        public PCGPointData pointsCake;

        public NativeArray<float> input;
        public NativeList<int> filtrateIndices;
        public NativeList<int> cakeIndices;

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            if (HandlePointErrors(pointsIn)) return emptyHandle;

            input = new NativeArray<float>(pointsIn.GetAttributeList<float>(attributeIn), Allocator.TempJob);
            filtrateIndices = new NativeList<int>(0, Allocator.TempJob);
            cakeIndices = new NativeList<int>(0, Allocator.TempJob);

            handle = JobCreator(handle);

            return handle;
        }

        protected override void Process()
        {
            handle.Complete();

            pointsFiltrate = new PCGPointData(pointsIn, filtrateIndices.AsArray().ToArray());
            pointsCake = new PCGPointData(pointsIn, cakeIndices.AsArray().ToArray());

            input.Dispose();
            filtrateIndices.Dispose();
            cakeIndices.Dispose();
        }

        JobHandle JobCreator(JobHandle dependsOn = default)
        {
            FilterIndexerJob jobData = new FilterIndexerJob
            {
                compareFunction = (int)compare,
                constant = value,
                input = input,
                filtrate = filtrateIndices,
                cake = cakeIndices
            };
            dependsOn = jobData.Schedule(dependsOn);

            return dependsOn;
        }
    }
}
