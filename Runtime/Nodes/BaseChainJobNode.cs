using GraphProcessor;
using PCG.Terrain;
using Unity.Jobs;

namespace PCG
{
    public abstract class BaseChainJobNode : BasePCGNode
    {
        public new PCGTerrainGraph graph => base.graph as PCGTerrainGraph;

        internal static int BATCH_COUNT = 64;

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
