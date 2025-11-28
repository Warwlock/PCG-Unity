using UnityEngine;
using GraphProcessor;
using Unity.Jobs;
using Unity.Collections;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Debug/DebugPoints", typeof(PCGGraph))]
    public class DebugPoints : BaseNode
    {
        [SerializeField]
        public float pointSize = 0.1f;

        [Input]
        public PCGPointData points;

        protected override void Process()
        {
            for (int i = 0; i < points.Count; i++)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var xPos = points.GetAttribute<float>("PosX", i);
                var yPos = points.GetAttribute<float>("PosY", i);
                var zPos = points.GetAttribute<float>("PosZ", i);

                obj.transform.localScale = Vector3.one * pointSize;
                obj.transform.position = new Vector3(xPos, yPos, zPos);
            }   
        }
    }
}
