using GraphProcessor;
using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Windows;

namespace PCG
{
    public abstract class BaseJobNode : BaseNode
    {
        public JobHandle handle;

        [HideInInspector]
        public string errorString;
        public abstract JobHandle OnStartJobProcess();
        public static JobHandle emptyHandle = new JobHandle();

        public void OnEndJobProcess()
        {
            ExceptionToLog.Call(() => Process());

            InvokeOnProcessed();

            outputPorts.PushDatas();
        }

        protected bool CheckNull(object property)
        {
            if(property == null)
            {
                errorString = $"Some of ports didn't connected";
                Debug.LogError(errorString);
                return true;
            }
            return false;
        }

        protected void HandlePointErrors(PCGPointData points)
        {
            if(points == null)
            {
                throw new Exception("Points are null!");
            }
        }

        protected void HandleCouplePointErrors(PCGPointData pointsA, PCGPointData pointsB, string attributeA, string attributeB)
        {
            HandlePointErrors(pointsA);
            HandlePointErrors(pointsB);

            if (pointsA.Count < pointsB.Count)
            {
                throw new Exception($"Mismatch between the number of points from pointsB[{pointsB.Count}] and pointsA[{pointsA.Count}]");
            }

            if (pointsA.GetDataType(attributeA) != pointsB.GetDataType(attributeB))
            {
                throw new Exception($"Mismatch between the types from pointsB[{pointsB.Count}] and pointsA[{pointsA.Count}]");
            }
        }
            
    }
}
