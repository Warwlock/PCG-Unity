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
    [NodeCustomEditor(typeof(BaseJobNode))]
    public class AttributeStatisticsView : BaseNodeView
    {
        public override void Enable()
        {
            var node = nodeTarget as BaseJobNode;

            base.Enable();
        }
    }
}
