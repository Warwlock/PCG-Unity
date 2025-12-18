using PCG.Terrain;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Mesh;

namespace PCG
{
    //[ExecuteAlways]
    public class PCGGraphComponent : MonoBehaviour
    {
        public PCGGraph pcgGraph;
        public bool processGraph = false;
        public int seed = 42;
        public Material mat;

        Mesh[] meshes = new Mesh[0];
        List<GameObject> terrainObjs = new List<GameObject>();

        void Start()
        {
            meshes = new Mesh[(pcgGraph as PCGTerrainGraph).chunkX * (pcgGraph as PCGTerrainGraph).chunkY];
            for (int i = 0; i < meshes.Length; i++)
            {
                meshes[i] = new Mesh();
                meshes[i].hideFlags = HideFlags.DontSaveInBuild | HideFlags.DontSaveInEditor;
                meshes[i].name = "Chunk" + i;
            }
            foreach (var mesh in meshes)
            {
                terrainObjs.Add(CreateTerrainObjects(mesh, (pcgGraph as PCGTerrainGraph).generateCollisions));
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (processGraph)
            {
                processGraph = false;
                /*for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    SmartDestroy(transform.GetChild(i).gameObject);
                }*/
                pcgGraph.seed = seed;
                pcgGraph.ClearDebugPoints();
                pcgGraph.CallOnStart();
                if (pcgGraph is PCGTerrainGraph terrainGraph)
                {
                    ProcessTerrainGraph(pcgGraph as PCGTerrainGraph);
                }
                else
                    StartCoroutine(ProcessGraphCoroutine(pcgGraph));
            }
            if (pcgGraph.debugPointsCount > 0 && pcgGraph.debugMesh != null && pcgGraph.debugMaterial != null && pcgGraph._densityBuffer != null && pcgGraph.readyForDebugRender)
            {
                pcgGraph._densityBuffer.SetData(pcgGraph.debugPointDensities);
                pcgGraph.debugMaterial.SetBuffer("_InstanceDensityBuffer", pcgGraph._densityBuffer);

                var rparams = new RenderParams(pcgGraph.debugMaterial) { matProps = pcgGraph._matProps };

                const int BATCH_LIMIT = 400;
                int totalPoints = pcgGraph.debugPointsCount;

                for (int i = 0; i < totalPoints; i += BATCH_LIMIT)
                {
                    int currentBatchCount = Mathf.Min(BATCH_LIMIT, totalPoints - i);
                    pcgGraph._matProps.SetInteger("_InstanceIDOffset", i);

                    Graphics.RenderMeshInstanced(rparams, pcgGraph.debugMesh, 0, pcgGraph.debugPointMatrices, currentBatchCount, i);
                }
                pcgGraph.CallOnUpdate();
            }
        }

        public void GenerateButton()
        {
            UnityEngine.Random.InitState(System.DateTime.Now.ToString("o").GetHashCode());
            seed = UnityEngine.Random.Range(1, 999999999);
            processGraph = true;
        }

        GameObject CreateTerrainObjects(Mesh mesh, bool generateCollision)
        {
            var obj = new GameObject();
            obj.transform.parent = transform;
            var mf = obj.AddComponent<MeshFilter>();
            var mr = obj.AddComponent<MeshRenderer>();

            mr.material = mat;
            mf.mesh = mesh;

            if (generateCollision)
            {
                var col = obj.AddComponent<MeshCollider>();
            }

            return obj;
        }

        void ApplyCollisions()
        {
            for (int i = 0; i < terrainObjs.Count; i++)
            {
                var obj = terrainObjs[i];
                if (obj.TryGetComponent<MeshCollider>(out var collider))
                {
                    collider.sharedMesh = meshes[i];
                }
                else
                {
                    var col = obj.AddComponent<MeshCollider>();
                    col.sharedMesh = meshes[i];
                }
            }
        }

        IEnumerator ProcessGraphCoroutine(PCGGraph graph)
        {
            var sortedNodes = graph.nodes.Where(n => n.computeOrder >= 0).OrderBy(n => n.computeOrder).ToList();

            int maxLoopCount = 0;
            for (int executionIndex = 0; executionIndex < sortedNodes.Count; executionIndex++)
            {
                maxLoopCount++;
                if (maxLoopCount > 10000)
                {
                    Debug.LogError("Exceeded 10000 node limit or something went horribly wrong.");
                    yield break;
                }

                var node = sortedNodes[executionIndex];

                if (node.computeOrder < 0 || !node.canProcess)
                    continue;

                if (node is BaseJobNode bjn)
                {
                    JobHandle handle = bjn.OnStartJobProcess();

                    if (handle.Equals(BaseJobNode.emptyHandle))
                    {
                        Debug.Log("A Node Empty");
                        continue;
                    }

                    while (!handle.IsCompleted)
                    {
                        yield return null;
                    }

                    if (handle.IsCompleted)
                    {
                        bjn.OnEndJobProcess();
                    }
                    else
                    {
                        yield return null;
                    }
                }
                else if (node is BaseChainJobNode bcjn)
                {
                    dependsOn = bcjn.Process(dependsOn);
                }
                else
                {
                    node.OnProcess();
                }
            }

            pcgGraph.AfterNodesProcessed();
        }

        JobHandle dependsOn = default;
        void ProcessTerrainGraph(PCGTerrainGraph graph)
        {
            var sortedNodes = graph.nodes.Where(n => n.computeOrder >= 0).OrderBy(n => n.computeOrder).ToList();

            int totalPoints = ((int)graph.chunkSize * graph.chunkX) * ((int)graph.chunkSize * graph.chunkY);
            graph.points = new NativeArray<float3>(totalPoints, Allocator.Persistent);

            int maxLoopCount = 0;
            bool haveColliderNode = false;
            for (int executionIndex = 0; executionIndex < sortedNodes.Count; executionIndex++)
            {
                maxLoopCount++;
                if (maxLoopCount > 10000)
                {
                    Debug.LogError("Exceeded 10000 node limit or something went horribly wrong.");
                    return;
                }

                var node = sortedNodes[executionIndex];

                if (node.computeOrder < 0 || !node.canProcess)
                    return;

                if (node is BaseChainJobNode bcjn)
                {
                    if(executionIndex == 0)
                        dependsOn = bcjn.OnProcess(dependsOn);

                    BaseChainJobNode nextNode = null;
                    if (executionIndex + 1 != sortedNodes.Count)
                        nextNode = sortedNodes[executionIndex + 1] as BaseChainJobNode;

                    StartCoroutine(WaitJobCompletion(bcjn, nextNode, dependsOn));

                    if (node is Terrain.TerrainCollider)
                        haveColliderNode = true;
                }
                else
                {
                    Debug.LogError("Node is not supported.");
                }
            }

            StartCoroutine(LastProcess(dependsOn, haveColliderNode));
        }

        IEnumerator WaitJobCompletion(BaseChainJobNode node, BaseChainJobNode nextNode, JobHandle dependecy)
        {
            while (!dependsOn.IsCompleted)
                yield return null;            

            dependsOn.Complete();
            node.OnJobCompleted();
            if (nextNode != null)
            {
                if (nextNode is PointsToTerrain pttnext)
                {
                    var meshDataArray = Mesh.AllocateWritableMeshData(meshes.Length);
                    pttnext.SetMeshes(meshes, ref meshDataArray);
                }
                dependsOn = nextNode.OnProcess(dependsOn);
            }
        }

        IEnumerator LastProcess(JobHandle dependsOne, bool generateCollision)
        {
            while (!dependsOn.IsCompleted)
                yield return null;

            dependsOn.Complete();
            (pcgGraph as PCGTerrainGraph).points.Dispose(dependsOn);
            if((pcgGraph as PCGTerrainGraph).generateCollisions)
            {
                NativeArray<EntityId> meshIds = new NativeArray<EntityId>(meshes.Length, Allocator.TempJob);

                for (int i = 0; i < meshes.Length; ++i)
                {
                    meshIds[i] = meshes[i].GetEntityId();
                }

                BakeTerrainColliderMeshJob job = new BakeTerrainColliderMeshJob
                {
                    meshIds = meshIds
                };
                JobHandle handle = default;
                handle = job.ScheduleParallel(meshIds.Length, meshIds.Length / (SystemInfo.processorCount / 2), handle);
                meshIds.Dispose(handle);

                while (!handle.IsCompleted)
                    yield return null;

                handle.Complete();
                ApplyCollisions();
            }
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
