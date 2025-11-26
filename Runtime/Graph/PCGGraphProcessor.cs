using GraphProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PCG
{
    public class PCGGraphProcessor : BaseGraphProcessor
    {
        internal static Dictionary<PCGGraph, HashSet<PCGGraphProcessor>> processorInstances = new Dictionary<PCGGraph, HashSet<PCGGraphProcessor>>();
        List<BaseNode> sortedNodes;

        public PCGGraphProcessor(BaseGraph graph) : base(graph)
        {
            processorInstances.TryGetValue(graph as PCGGraph, out var hashset);
            if (hashset == null)
                hashset = processorInstances[graph as PCGGraph] = new HashSet<PCGGraphProcessor>();

            hashset.Add(this);
        }

        struct ProcessingScope : IDisposable
        {
            PCGGraphProcessor processor;

            public ProcessingScope(PCGGraphProcessor processor)
            {
                this.processor = processor;
                processor.isProcessing++;
            }

            public void Dispose() => processor.isProcessing--;
        }

        internal int isProcessing = 0;

        public static PCGGraphProcessor GetOrCreate(PCGGraph graph)
        {
            PCGGraphProcessor.processorInstances.TryGetValue(graph, out var processorSet);
            if (processorSet == null)
                return new PCGGraphProcessor(graph);
            else
                return processorSet.FirstOrDefault(p => p != null);
        }

        public static void RunOnce(PCGGraph graph)
        {
            var processor = GetOrCreate(graph);
            processor.Run();
        }

        public override void Run()
        {
            using (new ProcessingScope(this))
            {
                UpdateComputeOrder();

                int maxLoopCount = 0;
                for (int executionIndex = 0; executionIndex < sortedNodes.Count; executionIndex++)
                {
                    maxLoopCount++;
                    if (maxLoopCount > 10000)
                        return;

                    BaseNode node = sortedNodes[executionIndex];
                    ProcessNode(node);
                }
            }
        }

        public override void UpdateComputeOrder()
        {
            sortedNodes = graph.nodes.Where(n => n.computeOrder >= 0).OrderBy(n => n.computeOrder).ToList();
        }

        void ProcessNode(BaseNode node)
        {
            if (node.computeOrder < 0 || !node.canProcess)
                return;

            node.OnProcess();
        }
    }
}