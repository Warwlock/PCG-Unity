using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG.Editor
{
    public class AttributeInspectorWindow : EditorWindow
    {
        public PCGGraph graph;
        public BasePCGNode node;

        BasePCGNode currentNode;

        MultiColumnListView listView;
        ToolbarButton nodeNameButton;

        public static void CreateWindow(PCGGraph graph)
        {
            AttributeInspectorWindow window = GetWindow<AttributeInspectorWindow>("Attribute Inspector", true, typeof(PCGGraphWindow));
            Debug.Log(graph);
            window.Init(graph);
        }

        public void Init(PCGGraph graph)
        {
            this.graph = graph;
        }

        void CreateGUI()
        {
            /*if (graph == null) return;

            var node = graph.GetDebugAttributeNode();

            if (node == null)
            {
                root.Add(new Label("Enable attribute debug for any node"));
            }
            else
            {
                root.Add(new Label("YES"));
            }*/

            VisualElement root = rootVisualElement;

            //root.Add(new IMGUIContainer(DrawImGUIToolbar));

            
            root.Add(DrawToolbar());

            //var scrollView = new ScrollView(ScrollViewMode.VerticalAndHorizontal);

            listView = new MultiColumnListView();

            listView.columns.Add(new Column() { title = "Index"});
            listView.columns[0].bindCell = (element, index) => (element as Label).text = index.ToString();

            root.Add(listView);
        }

        private void Update()
        {
            if (graph == null) return;

            node = graph.GetDebugAttributeNode();

            if(currentNode == node)
                return;
            currentNode = node;

            nodeNameButton.text = node?.name ?? "Null";

            if (node == null)
            {
                listView.itemsSource = null;
                return;
            }

            listView.itemsSource = new bool[(node as Math).pointsA.Count];

            listView.columns.Clear();
            listView.columns.Add(new Column() { title = "Index" });
            listView.columns[0].bindCell = (element, index) => (element as Label).text = index.ToString();
            foreach (var key in (node as Math).pointsA.Attributes.Keys)
            {
                listView.columns.Add(new Column() { title = key });
            }
            listView.Rebuild();

            //listView.RefreshItems();

            //VisualElement root = rootVisualElement;
            //root.Clear();

            //root.Add(new IMGUIContainer(DrawImGUIToolbar));

            /*if (node == null)
            {
                root.Add(new Label("Enable attribute debug for any node"));
            }
            else
            {
                root.Add(new Label(node.GetType().ToString()));
            }*/
        }

        VisualElement DrawToolbar()
        {
            var toolbar = new Toolbar();
            nodeNameButton = new ToolbarButton();
            nodeNameButton.text = node?.name ?? "Null";
            toolbar.Add(nodeNameButton);

            return toolbar;
        }
    }
}
