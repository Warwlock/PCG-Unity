using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG.Editor
{
    public class PCGToolbar : ToolbarView
    {
        ToolbarButtonData showParameters;
        public PCGToolbar(BaseGraphView graphView) : base(graphView)
        {
        }

        protected override void AddButtons()
        {
            AddButton("Save Gaph", () => { EditorUtility.SetDirty(graphView.graph); AssetDatabase.SaveAssets(); });

            AddButton("Open Attribute Inspector", () => { AttributeInspectorWindow.CreateWindow(graphView as PCGGraphView); });

            bool exposedParamsVisible = graphView.GetPinnedElementStatus<ExposedParameterView>() != DropdownMenuAction.Status.Hidden;
            showParameters = AddToggle("Show Parameters", exposedParamsVisible, (v) => graphView.ToggleView<ExposedParameterView>());

            AddButton("Show In Project", () => EditorGUIUtility.PingObject(graphView.graph), false);
        }
    }
}
