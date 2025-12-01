using GraphProcessor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace PCG
{
    public class PCGGraph : BaseGraph
    {
        [SerializeField] public Mesh debugMesh = null;
        [SerializeField] public Material debugMaterial = null;

        public GraphicsBuffer _densityBuffer;

        [HideInInspector] public List<Matrix4x4> debugPointMatrices = new();
        [HideInInspector] public List<float> debugPointDensities = new();
        public int debugPointsCount;
        [HideInInspector] public bool readyForDebugRender;

        public void ClearDebugPoints()
        {
            _densityBuffer?.Dispose();
            debugPointMatrices.Clear();
            debugPointDensities.Clear();
            debugPointsCount = 0;
            readyForDebugRender = false;
        }

        public void CreateDebugPoints()
        {
            _densityBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, debugPointsCount, sizeof(float));
            readyForDebugRender = true;
        }

        public void ClearDelegates()
        {
            OnStart = null;
            OnUpdate = null;
        }

        public void CallOnStart()
        {
            OnStart?.Invoke();
        }

        public void CallOnUpdate()
        {
            OnUpdate?.Invoke();
        }

        public event Action OnStart;

        public event Action OnUpdate;
    }
}
