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
            (graph as PCGGraph).debugPointsCount += points.Count;

            if (points == null)
            {
                Debug.LogError("Points null!");
                return;
            }
            else if (points.IsEmpty())
            {
                Debug.LogError("Points empty!");
                return;
            }

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 pos = new Vector3();
                pos = points.GetAttribute<Vector3>(DefaultAttributes.Pos, i);

                float density = points.GetAttribute<float>(DefaultAttributes.Density, i);

                Matrix4x4 posMatrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one * pointSize);

                (graph as PCGGraph).debugPointMatrices.Add(posMatrix);
                (graph as PCGGraph).debugPointDensities.Add(density);
            }
        }
    }
}
