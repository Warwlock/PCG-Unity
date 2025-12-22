using GraphProcessor;
using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace PCG
{
    [System.Serializable, NodeMenuItem("Input/Constant", typeof(PCGGraph))]
    public class ConstantNode : BasePCGNode
    {
        [SerializeField]
        public MathOperators.ConstantType constantType;

        public float constantFloat = 0f;
        public Vector3 constantVector = Vector3.zero;
        public int constantInt = 0;

        public string attribute = "Constant";

        [Output]
        public PCGPointData points;

        protected override void Process()
        {
            points = new PCGPointData(1);

            if(constantType == MathOperators.ConstantType.Float)
                points.SetAttributeList(attribute, new float[] { constantFloat });
            if (constantType == MathOperators.ConstantType.Vector3)
                points.SetAttributeList(attribute, new Vector3[] { constantVector });
            if (constantType == MathOperators.ConstantType.Int)
                points.SetAttributeList(attribute, new int[] { constantInt });
        }
    }
}
