using GraphProcessor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG.Editor
{
    public class PCGGraphView : BaseGraphView
    {
        protected override void KeyDownCallback(KeyDownEvent e)
        {
            base.KeyDownCallback(e);
            if (e.keyCode == KeyCode.W)
            {
                if (selection.Count == 1)
                {
                    var node = (selection[0] as BaseNodeView).nodeTarget as BasePCGNode;
                    node.EnableAttributeDebug(node.debugAttribute);
                }
            }

            if (e.keyCode == KeyCode.D)
            {
                if (selection.Count == 1)
                {
                    var node = selection[0] as BaseNodeView;
                    node.ToggleDebug();
                }
            }
        }
        public PCGGraphView(EditorWindow window) : base(window)
        {
        }
    }
}
