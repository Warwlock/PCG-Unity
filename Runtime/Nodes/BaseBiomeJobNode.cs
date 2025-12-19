using GraphProcessor;
using PCG.Terrain;
using Unity.Jobs;
using UnityEngine;

namespace PCG.Biome
{
    public abstract class BaseBiomeJobNode : BasePCGNode
    {
        public new PCGBiomeGraph graph => base.graph as PCGBiomeGraph;

        internal static int batchDivisor => SystemInfo.processorCount / 2;

        public abstract JobHandle Process(JobHandle dependsOn);

        public JobHandle OnProcess(JobHandle dependsOn)
        {
            inputPorts.PullDatas();

            dependsOn = Process(dependsOn);
            //ExceptionToLog.Call(() => {  });

            InvokeOnProcessed();

            outputPorts.PushDatas();

            return dependsOn;
        }

        public virtual void OnJobCompleted() { }
    }
}
