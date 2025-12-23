using System.Collections;
using System.Linq;
using Unity.Jobs;
using UnityEngine;

namespace PCG
{
    [ExecuteAlways]
    public class PCGGraphComponent : MonoBehaviour
    {
        public PCGGraph pcgGraph;
        public bool processGraph = false;
        public int seed = 42;

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (processGraph)
            {
                processGraph = false;
                pcgGraph.seed = seed;
                pcgGraph.parentTransform = transform;
                pcgGraph.ClearDebugPoints();
                pcgGraph.CallOnStart();
                StartCoroutine(ProcessGraphCoroutine(pcgGraph));
            }
            if(pcgGraph.debugPointsCount > 0 && pcgGraph.debugMesh != null && pcgGraph.debugMaterial != null && pcgGraph._densityBuffer != null && pcgGraph.readyForDebugRender)
            {
                pcgGraph._densityBuffer.SetData(pcgGraph.debugPointDensities);
                pcgGraph.debugMaterial.SetBuffer("_InstanceDensityBuffer", pcgGraph._densityBuffer);

                var rparams = new RenderParams(pcgGraph.debugMaterial) { matProps = pcgGraph._matProps };

                const int BATCH_LIMIT = 400;
                int totalPoints = pcgGraph.debugPointsCount;

                for (int i = 0; i < totalPoints; i += BATCH_LIMIT)
                {
                    int currentBatchCount = Mathf.Min(BATCH_LIMIT, totalPoints - i);
                    pcgGraph._matProps.SetInteger("_InstanceIDOffset", i);

                    Graphics.RenderMeshInstanced(rparams, pcgGraph.debugMesh, 0, pcgGraph.debugPointMatrices, currentBatchCount, i);
                }
                pcgGraph.CallOnUpdate();
            }
        }

        IEnumerator ProcessGraphCoroutine(PCGGraph graph)
        {
            var sortedNodes = graph.nodes.Where(n => n.computeOrder >= 0).OrderBy(n => n.computeOrder).ToList();

            int maxLoopCount = 0;
            for (int executionIndex = 0; executionIndex < sortedNodes.Count; executionIndex++)
            {
                maxLoopCount++;
                if (maxLoopCount > 10000)
                {
                    Debug.LogError("Exceeded 10000 node limit or something went horribly wrong.");
                    yield break;
                }

                var node = sortedNodes[executionIndex];

                if (node.computeOrder < 0 || !node.canProcess)
                    continue;

                if (node is BaseJobNode bjn)
                {
                    JobHandle handle = bjn.OnStartJobProcess();

                    if (handle.Equals(BaseJobNode.emptyHandle))
                    {
                        Debug.Log("A Node Empty");
                        continue;
                    }

                    while (!handle.IsCompleted)
                    {
                        yield return null;
                    }

                    if (handle.IsCompleted)
                    {
                        bjn.OnEndJobProcess();
                    }
                    else
                    {
                        yield return null;
                    }
                }
                else
                {
                    node.OnProcess();
                }
            }

            pcgGraph.CreateDebugPoints();
        }
    }
}
