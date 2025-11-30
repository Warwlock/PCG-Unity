using GraphProcessor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

namespace PCG.Editor
{
    [NodeCustomEditor(typeof(BaseJobNode))]
    public class AttributeStatisticsView : BaseNodeView
    {
        int _choiceIndex = 0;

        public override void Enable()
        {
            var node = nodeTarget as BaseJobNode;

            DrawDefaultInspector();

            //controlsContainer.Add(new Label("Hello"));

            var fields = nodeTarget.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(f => f.DeclaringType != typeof(BaseNode));

            foreach (var field in fields)
            {
                DropDownAttribute dropdownAttribute = field.GetCustomAttribute<DropDownAttribute>();
                if(dropdownAttribute != null)
                {
                    var serializedProperty = FindSerializedProperty(field.Name);
                    List<string> choiceList = (List<string>)field.GetValue(serializedProperty.serializedObject.targetObject);

                    var dropDown = new DropdownField(field.Name, choiceList, _choiceIndex);
                    Debug.Log(dropDown.index);
                    controlsContainer.Add(dropDown);
                }
            }


        }
    }
}
