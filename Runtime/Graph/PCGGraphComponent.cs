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

        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (processGraph)
            {
                processGraph = false;
                pcgGraph.ClearDebugPoints();
                pcgGraph.CallOnStart();
                StartCoroutine(ProcessGraphCoroutine(pcgGraph));
            }
            if(pcgGraph.debugPointsCount > 0 && pcgGraph.debugMesh != null && pcgGraph.debugMaterial != null && pcgGraph._densityBuffer != null && pcgGraph.readyForDebugRender)
            {
                pcgGraph._densityBuffer.SetData(pcgGraph.debugPointDensities);
                pcgGraph.debugMaterial.SetBuffer("_InstanceDensityBuffer", pcgGraph._densityBuffer);

                var rparams = new RenderParams(pcgGraph.debugMaterial) { };

                Graphics.RenderMeshInstanced(rparams, pcgGraph.debugMesh, 0, pcgGraph.debugPointMatrices);
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
