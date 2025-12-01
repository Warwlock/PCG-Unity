using GraphProcessor;
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
            
    }
}
