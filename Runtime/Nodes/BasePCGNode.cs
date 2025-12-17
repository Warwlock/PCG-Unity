using GraphProcessor;
using UnityEngine;

namespace PCG
{
    public abstract class BasePCGNode : BaseNode
    {
        public new PCGGraph graph => base.graph as PCGGraph;

        [HideInInspector]
        public bool debugAttribute;

        public void EnableAttributeDebug(bool currentBool)
        {
            foreach (var node in graph.nodes)
            {
                node.RemoveMessage("Attribute Debug");
                (node as BasePCGNode).debugAttribute = false;
            }
            
            debugAttribute = !currentBool;

            if (debugAttribute)
                AddMessage("Attribute Debug", NodeMessageType.Info);
        }

        public void OnOpen()
        {
            debugAttribute = false;
        }

        protected bool HandleNullErrors(bool isNull)
        {
            ClearMessages();
            if (isNull)
            {
                AddMessage("Input object is null!", NodeMessageType.Error);
                return true;
            }
            return false;
        }

        protected bool HandlePointErrors(PCGPointData points)
        {
            ClearMessages();
            if (points == null)
            {
                AddMessage("Points null!", NodeMessageType.Error);
                return true;
            }
            else if (points.IsEmpty())
            {
                AddMessage("Points empty!", NodeMessageType.Error);
                return true;
            }
            return false;
        }

        protected bool HandleCouplePointErrors(PCGPointData pointsA, PCGPointData pointsB, string attributeA, string attributeB, bool supportFloatVectorCouple = false)
        {
            if (HandlePointErrors(pointsA)) return true;
            if (HandlePointErrors(pointsB)) return true;

            ClearMessages();
            if (pointsA.Count < pointsB.Count)
            {
                string message = $"Mismatch between the number of points from pointsB[{ pointsB.Count}] and pointsA[{ pointsA.Count}]";
                AddMessage(message, NodeMessageType.Error);
                return true;
            }

            if (pointsA.GetDataType(attributeA) != pointsB.GetDataType(attributeB))
            {
                if (supportFloatVectorCouple && pointsB.GetDataType(attributeB) == typeof(float) && 
                    (pointsA.GetDataType(attributeA) == typeof(Vector3) ||
                    pointsA.GetDataType(attributeA) == typeof(Vector2) ||
                    pointsA.GetDataType(attributeA) == typeof(Quaternion))) { Debug.Log("Supported float-vector"); }
                else 
                {
                    string message = $"Mismatch between the types from pointsB[{pointsB.Count}] and pointsA[{pointsA.Count}]";
                    AddMessage(message, NodeMessageType.Error);
                    return true;
                }
            }

            return false;
        }
            
    }
}
