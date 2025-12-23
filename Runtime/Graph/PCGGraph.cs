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

        public GraphicsBuffer _densityBuffer;
        public MaterialPropertyBlock _matProps;

        public List<Matrix4x4> debugPointMatrices = new();
        public List<float> debugPointDensities = new();
        public int debugPointsCount;
        [HideInInspector] public bool readyForDebugRender;

        public Transform parentTransform;

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

        public void ClearDelegates()
        {
            OnStart = null;
            OnUpdate = null;
        }

        public void SpawnGameObject(GameObject prefab, Vector3 pos, Vector3 rot, Vector3 sca)
        {
            var obj = Instantiate(prefab, pos, Quaternion.Euler(rot), parentTransform);
            obj.transform.localScale = sca;
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

        public BasePCGNode GetDebugAttributeNode()
        {
            return nodes.Where(x => (x as BasePCGNode).debugAttribute).FirstOrDefault() as BasePCGNode;
        }

        public static void SmartDestroy(UnityEngine.Object obj)
        {
            if (obj == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                GameObject.DestroyImmediate(obj);
            }
            else
#endif
            {
                GameObject.Destroy(obj);
            }
        }
    }
}
