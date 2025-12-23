using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Spawner/SpawnGameObject", typeof(PCGGraph))]
    public class SpawnGameObject : BasePCGNode
    {
        [Input]
        public PCGPointData pointsIn;
        public GameObject prefab;

        protected override void Process()
        {
            for(int i = 0; i < pointsIn.Count; i++)
            {
                graph.SpawnGameObject(prefab, pointsIn.GetAttribute<Vector3>(DefaultAttributes.Pos, i),
                pointsIn.GetAttribute<Vector3>(DefaultAttributes.Rot, i),
                pointsIn.GetAttribute<Vector3>(DefaultAttributes.Sca, i));
            }
        }
    }
}
