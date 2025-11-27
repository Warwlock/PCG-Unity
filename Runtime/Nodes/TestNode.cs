using UnityEngine;
using GraphProcessor;
using Unity.Jobs;
using Unity.Collections;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Custom/TestNode", typeof(PCGGraph))]
    public class TestNode : BaseJobNode
    {
        [Input(name = "In"), SerializeField]
        public float input;

        /*[Input(name = "In"), SerializeField]
        public Vector3[] inputPoints;*/

        [Output(name = "Out")]
        public float output;

        public NativeArray<float> result;

        public override string name => "HelloWorld";

        protected override void Process()
        {
            handle.Complete();

            output = result[0];
            Debug.Log(output);

            result.Dispose();
        }

        public override JobHandle OnStartJobProcess()
        {
            inputPorts.PullDatas();

            result = new NativeArray<float>(1, Allocator.TempJob);
            TestNodeJob jobData = new TestNodeJob
            {
                input = input,
                result = result
            };
            handle = jobData.Schedule();

            return handle;
        }
    }

    struct TestNodeJob : IJob
    {
        public float input;
        public NativeArray<float> result;

        public void Execute()
        {
            result[0] = input * 42;
        }
    }
}
