using UnityEngine;
using GraphProcessor;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Custom/TestNode", typeof(PCGGraph))]
    public class TestNode : BaseNode
    {
        [Input(name = "In"), SerializeField]
        public float input;

        [Output(name = "Out")]
        public float output;

        public override string name => "HelloWorld";

        protected override void Process()
        {
            output = input * 42;
            Debug.Log(output);
        }
    }
}
