using GraphProcessor;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
namespace PCG.Biome
{
    [System.Serializable, NodeMenuItem("OutputNode", typeof(PCGBiomeGraph))]
    public class OutputNode : BaseBiomeJobNode
    {
        public override JobHandle Process(JobHandle dependsOn)
        {
            return default;
        }
    }
}
