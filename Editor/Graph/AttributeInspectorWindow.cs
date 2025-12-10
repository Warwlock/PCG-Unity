using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        Toolbar toolbar;
        ToolbarButton nodeNameButton;

        List<FieldInfo> newFields = new List<FieldInfo>();
        PCGPointData pointData;

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
            VisualElement root = rootVisualElement;
            
            root.Add(AddToolbar());

            listView = new MultiColumnListView();

            root.Add(listView);
        }

        private void Update()
        {
            if (graph == null) return;

            if (!graph.readyForDebugRender) return;

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

            var fields = node.GetNodeFields();
            if (fields == null) return;

            newFields.Clear();
            foreach (var field in fields)
            {
                if(field.FieldType == typeof(PCGPointData))
                {
                    Debug.Log((field.GetValue(node) as PCGPointData).Count);
                    newFields.Add(field);
                }
            }
            RefreshToolbar();
            RefreshListView(newFields[0]);
        }

        VisualElement AddToolbar()
        {
            toolbar = new Toolbar();
            nodeNameButton = new ToolbarButton();
            nodeNameButton.text = node?.name ?? "Null";
            toolbar.Add(nodeNameButton);

            return toolbar;
        }

        void RefreshToolbar()
        {
            toolbar.Clear();
            nodeNameButton = new ToolbarButton();
            nodeNameButton.text = node?.name ?? "Null";
            toolbar.Add(nodeNameButton);

            toolbar.Add(new ToolbarSpacer());

            foreach (var field in newFields)
            {
                var portButton = new ToolbarButton() { text = field.Name };
                portButton.clicked += () => { RefreshListView(field); };
                toolbar.Add(portButton);
            }
        }

        void RefreshListView(FieldInfo field)
        {
            pointData = field.GetValue(node) as PCGPointData;
            Debug.Log(pointData.Count);
            listView.itemsSource = new bool[pointData.Count];

            listView.columns.Clear();
            listView.columns.Add(new Column() { title = "Index", width = 60 });
            listView.columns[0].bindCell = (element, index) => (element as Label).text = index.ToString();

            if (pointData.Attributes == null)
            {
                Debug.LogWarning("Process graph one time to view attribute data!");
                return;
            }

            foreach (var attribute in pointData.Attributes)
            {
                listView.columns.Add(new Column()
                {
                    title = attribute.Key + $"({attribute.Value.GetDataType().Name})",
                    width = 120,
                    bindCell = (element, index) => (element as Label).text = pointData.GetAttributeObject(attribute.Key, index)?.ToString() ?? "NULL"
                });
            }
            listView.Rebuild();
        }
    }
}
