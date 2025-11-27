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

        public abstract JobHandle OnStartJobProcess();

        public void OnEndJobProcess()
        {
            ExceptionToLog.Call(() => Process());

            InvokeOnProcessed();

            outputPorts.PushDatas();
        }
    }
}
