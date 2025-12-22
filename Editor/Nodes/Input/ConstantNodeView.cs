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
    [NodeCustomEditor(typeof(ConstantNode))]
    public class ConstantNodeView : AttributeStatisticsView
    {
        ConstantNode node;
        VisualElement floatElement;
        VisualElement vectorElement;
        VisualElement intElement;

        public override void Enable()
        {
            base.Enable();
        }

        public override bool IsSelected(VisualElement selectionContainer)
        {
            return base.IsSelected(selectionContainer);
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            base.BuildContextualMenu(evt);
        }

        protected override void DrawDefaultInspector(bool fromInspector = false)
        {
            Debug.Log("Updated");
            node = nodeTarget as ConstantNode;
            //base.DrawDefaultInspector(fromInspector);

            var fields = nodeTarget.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
				// Filter fields from the BaseNode type since we are only interested in user-defined fields
				// (better than BindingFlags.DeclaredOnly because we keep any inherited user-defined fields) 
				.Where(f => f.DeclaringType != typeof(BaseNode));

            fields = nodeTarget.OverrideFieldOrder(fields).Reverse();

            var enumField = fields.FirstOrDefault(f => f.Name == "constantType");
            if (enumField != null)
                DrawField(enumField);

            var attributeField = fields.FirstOrDefault(f => f.Name == "attribute");
            if (attributeField != null)
                DrawField(attributeField);

            var floatField = fields.FirstOrDefault(f => f.Name == "constantFloat");
            if (floatField != null)
                floatElement = DrawField(floatField);
            var vectorField = fields.FirstOrDefault(f => f.Name == "constantVector");
            if (vectorField != null)
                vectorElement = DrawField(vectorField);
            var intField = fields.FirstOrDefault(f => f.Name == "constantInt");
            if (intField != null)
                intElement = DrawField(intField);
        }

        void UpdateVisibility()
        {
            if(node.constantType == MathOperators.ConstantType.Float)
            {
                floatElement.style.display = DisplayStyle.Flex;
                vectorElement.style.display = DisplayStyle.None;
                intElement.style.display = DisplayStyle.None;
            }

            if (node.constantType == MathOperators.ConstantType.Vector3)
            {
                floatElement.style.display = DisplayStyle.None;
                vectorElement.style.display = DisplayStyle.Flex;
                intElement.style.display = DisplayStyle.None;
            }

            if (node.constantType == MathOperators.ConstantType.Int)
            {
                floatElement.style.display = DisplayStyle.None;
                vectorElement.style.display = DisplayStyle.None;
                intElement.style.display = DisplayStyle.Flex;
            }
        }

        VisualElement DrawField(FieldInfo field)
        {
            string displayName = ObjectNames.NicifyVariableName(field.Name);

				var inspectorNameAttribute = field.GetCustomAttribute<InspectorNameAttribute>();
				if (inspectorNameAttribute != null)
					displayName = inspectorNameAttribute.displayName;
            var elem = AddControlField(field, displayName, false, UpdateVisibility);
            return elem;
        }
    }
}