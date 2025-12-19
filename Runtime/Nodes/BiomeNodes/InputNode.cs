using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
namespace PCG.Biome
{
    [System.Serializable, NodeMenuItem("InputNode", typeof(PCGBiomeGraph))]
    public class InputNode : BaseBiomeJobNode
    {
        public override JobHandle Process(JobHandle dependsOn)
        {
            return default;
        }
    }
}
