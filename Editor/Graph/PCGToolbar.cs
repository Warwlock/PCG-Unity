using GraphProcessor;
using UnityEngine;

namespace PCG.Editor
{
    public class PCGToolbar : ToolbarView
    {
        public PCGToolbar(BaseGraphView graphView) : base(graphView)
        {
        }

        protected override void AddButtons()
        {
            AddButton("Process Graph", () => { Debug.Log("Hello World"); PCGGraphProcessor.RunOnce(graphView.graph as PCGGraph); });
        }
    }
}
