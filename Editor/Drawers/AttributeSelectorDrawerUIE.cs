using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace PCG.Editor
{
    /*[CustomPropertyDrawer(typeof(AttributeSelector))]
    public class AttributeSelectorDrawerUIE : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var container = new VisualElement();

            var testField = new PropertyField(property.FindPropertyRelative("attributeString"));

            testField.RegisterValueChangeCallback(evt => { property.serializedObject.ApplyModifiedProperties(); });

            property.serializedObject.ApplyModifiedProperties();

            container.Add(testField);

            return container;
        }
    }*/
}
