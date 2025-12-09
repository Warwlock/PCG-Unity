using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace PCG.Editor
{
    [NodeCustomEditor(typeof(BasePCGNode))]
    public class AttributeStatisticsView : BaseNodeView
    {
        public override void Enable()
        {
            var node = nodeTarget as BasePCGNode;

            node.OnOpen();
            base.Enable();
        }

        public override bool IsSelected(VisualElement selectionContainer)
        {
            return base.IsSelected(selectionContainer);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            var node = nodeTarget as BasePCGNode;
            base.BuildContextualMenu(evt);

            evt.menu.AppendAction("Attribute Debug", (e) => node.EnableAttributeDebug(node.debugAttribute));
        }
    }
}
