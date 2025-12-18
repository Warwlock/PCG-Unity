using GraphProcessor;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

namespace PCG
{
    public class PCGGraph : BaseGraph
    {
        public int seed { get; set; }
        [SerializeField] public Mesh debugMesh = null;
        [SerializeField] public Material debugMaterial = null;

        // Debug Fields
        public GraphicsBuffer _densityBuffer;
        public MaterialPropertyBlock _matProps;

        [HideInInspector] public List<Matrix4x4> debugPointMatrices = new();
        [HideInInspector] public List<float> debugPointDensities = new();
        public int debugPointsCount;

        public bool readyForDebugRender { get; set; }
        public bool isGraphProcessed { get; set; } = false;


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
            _matProps = new MaterialPropertyBlock();
            _densityBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, debugPointsCount, sizeof(float));
            readyForDebugRender = true;
        }

        public void AfterNodesProcessed()
        {
            isGraphProcessed = true;
            CreateDebugPoints();
        }

        public void ClearDelegates()
        {
            OnStart = null;
            OnUpdate = null;
        }

        public void CallOnStart()
        {
            isGraphProcessed = false;

            OnStart?.Invoke();
        }

        public void CallOnUpdate()
        {
            OnUpdate?.Invoke();
        }

        public event Action OnStart;

        public event Action OnUpdate;

        public BasePCGNode GetDebugAttributeNode()
        {
            return nodes.Where(x => (x as BasePCGNode).debugAttribute).FirstOrDefault() as BasePCGNode;
        }
    }
}
