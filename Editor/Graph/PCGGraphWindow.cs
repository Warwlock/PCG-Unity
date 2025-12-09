using GraphProcessor;
using UnityEditor;
using UnityEngine;


namespace PCG.Editor
{
    public class PCGGraphWindow : BaseGraphWindow
    {
        public static PCGGraphWindow Open(PCGGraph graph)
        {
            // Focus the window if the graph is already opened
            var pcgWindows = Resources.FindObjectsOfTypeAll<PCGGraphWindow>();
            foreach (var pcgWindow in pcgWindows)
            {
                if (pcgWindow.graph == graph)
                {
                    pcgWindow.Show();
                    pcgWindow.Focus();
                    return pcgWindow;
                }
            }

            var graphWindow = EditorWindow.CreateWindow<PCGGraphWindow>();

            graphWindow.Show();
            graphWindow.Focus();

            graphWindow.InitializeGraph(graph);

            return graphWindow;
        }

        protected override void InitializeWindow(BaseGraph graph)
        {
            titleContent = new GUIContent(graph.name);

            if (graphView == null)
            {
                graphView = new PCGGraphView(this);
                graphView.Add(new PCGToolbar(graphView));
            }

            rootView.Add(graphView);
        }

        protected override void InitializeGraphView(BaseGraphView view)
        {
            view.OpenPinned<ExposedParameterView>();
        }
    }
}
