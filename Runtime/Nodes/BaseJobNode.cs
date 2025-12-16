using GraphProcessor;
using System;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Windows;

namespace PCG
{
    public abstract class BaseJobNode : BasePCGNode
    {
        public JobHandle handle;
        public abstract JobHandle OnStartJobProcess();
        public static JobHandle emptyHandle = new JobHandle();

        internal static int BATCH_COUNT = 64;

        bool parallelJobs = false;

        public void OnEndJobProcess()
        {
            debugAttribute = false;
            ExceptionToLog.Call(() => Process());

            InvokeOnProcessed();

            outputPorts.PushDatas();
        }
    }
}
