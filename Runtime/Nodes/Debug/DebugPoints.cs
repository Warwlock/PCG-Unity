using UnityEngine;
using GraphProcessor;
using Unity.Jobs;
using Unity.Collections;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Debug/DebugPoints", typeof(PCGGraph))]
    public class DebugPoints : BaseNode
    {
        [Input]
        public PCGPointData points;

        protected override void Process()
        {
            for (int i = 0; i < points.Count; i++)
            {
                var obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                var xPos = points.GetAttribute<float>("PosX", i);
                var zPos = points.GetAttribute<float>("PosZ", i);

                obj.transform.localScale = Vector3.one * 0.5f;
                obj.transform.position = new Vector3(xPos, 0, zPos);
            }   
        }
    }
}
