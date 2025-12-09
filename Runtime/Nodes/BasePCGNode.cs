using GraphProcessor;

namespace PCG
{
    public abstract class BasePCGNode : BaseNode
    {
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

        protected bool HandleCouplePointErrors(PCGPointData pointsA, PCGPointData pointsB, string attributeA, string attributeB)
        {
            if (HandlePointErrors(pointsA)) return true;
            if (HandlePointErrors(pointsB)) return true;

            ClearMessages();
            if (pointsA.Count < pointsB.Count)
            {
                string message = $"Mismatch between the number of points from pointsB[{ pointsB.Count}] and pointsA[{ pointsA.Count}]";
                AddMessage(message, NodeMessageType.Error);
                return true;
                //throw new Exception(message);
            }

            if (pointsA.GetDataType(attributeA) != pointsB.GetDataType(attributeB))
            {
                string message = $"Mismatch between the types from pointsB[{pointsB.Count}] and pointsA[{pointsA.Count}]";
                AddMessage(message, NodeMessageType.Error);
                return true;
                //throw new Exception(message);
            }

            return false;
        }
            
    }
}
