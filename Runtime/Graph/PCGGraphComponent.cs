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
                StartCoroutine(ProcessGraphCoroutine(pcgGraph));
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

                BaseJobNode node = sortedNodes[executionIndex] as BaseJobNode;

                JobHandle handle = node.OnStartJobProcess();

                while (!handle.IsCompleted)
                {
                    yield return null;
                }

                if (handle.IsCompleted)
                {
                    node.OnEndJobProcess();
                }
                else
                {
                    yield return null;
                }
            }
        }
    }
}
