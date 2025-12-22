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
    [NodeCustomEditor(typeof(NoiseGenerator))]
    public class NoiseGeneratorView : AttributeStatisticsView
    {
        NoiseGenerator node;
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
            node = nodeTarget as NoiseGenerator;
            base.DrawDefaultInspector(fromInspector);

            /*var fields = nodeTarget.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                // Filter fields from the BaseNode type since we are only interested in user-defined fields
                // (better than BindingFlags.DeclaredOnly because we keep any inherited user-defined fields) 
                .Where(f => f.DeclaringType != typeof(BaseNode));

            fields = nodeTarget.OverrideFieldOrder(fields).Reverse();

            var enumField = fields.FirstOrDefault(f => f.Name == "noiseFunctions");
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
                intElement = DrawField(intField);*/
        }

        void UpdateVisibility()
        {

        }

        VisualElement DrawField(FieldInfo field, bool fromInspector = false)
        {
            bool hasInputAttribute = field.GetCustomAttribute(typeof(InputAttribute)) != null;
            bool hasInputOrOutputAttribute = hasInputAttribute || field.GetCustomAttribute(typeof(OutputAttribute)) != null;
            bool showAsDrawer = !fromInspector && field.GetCustomAttribute(typeof(ShowAsDrawer)) != null;

            string displayName = ObjectNames.NicifyVariableName(field.Name);

            var inspectorNameAttribute = field.GetCustomAttribute<InspectorNameAttribute>();
            if (inspectorNameAttribute != null)
                displayName = inspectorNameAttribute.displayName;
            var elem = AddControlField(field, displayName, false, UpdateVisibility);

            return elem;
        }
    }
}