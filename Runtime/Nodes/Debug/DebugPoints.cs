using UnityEngine;
using GraphProcessor;
using Unity.Jobs;
using Unity.Collections;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Debug/DebugPoints", typeof(PCGGraph))]
    public class DebugPoints : BasePCGNode
    {
        [SerializeField]
        public float pointSize = 0.1f;

        [Input]
        public PCGPointData points;

        protected override void Process()
        {
            (graph as PCGGraph).debugPointsCount += points.Count;

            if (HandlePointErrors(points)) return;

            for (int i = 0; i < points.Count; i++)
            {
                Vector3 pos = new Vector3();
                Quaternion rot = new Quaternion();
                Vector3 sca = new Vector3();

                pos = points.GetAttribute<Vector3>(DefaultAttributes.Pos, i);
                rot = Quaternion.Euler(points.GetAttribute<Vector3>(DefaultAttributes.Rot, i));
                sca = points.GetAttribute(DefaultAttributes.Sca, i, Vector3.one);

                float density = points.GetAttribute<float>(DefaultAttributes.Density, i);

                Matrix4x4 posMatrix = Matrix4x4.TRS(pos, rot, sca * pointSize);

                (graph as PCGGraph).debugPointMatrices.Add(posMatrix);
                (graph as PCGGraph).debugPointDensities.Add(density);
            }
        }
    }
}
