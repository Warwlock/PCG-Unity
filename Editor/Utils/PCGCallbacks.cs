using PCG.Terrain;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace PCG.Editor
{
    public class PCGCallbacks
    {
        public static readonly string Extension = "asset";

        [MenuItem("Assets/Create/PCG/New PCG Graph", false, 83)]
        public static void CreateNewPCGGraph()
        {
            var graphItem = ScriptableObject.CreateInstance<CreatePCGGraphAction>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, graphItem,
               $"NewGraph.{Extension}", null, null);
        }

        [MenuItem("Assets/Create/PCG/New PCG Terrain Graph", false, 83)]
        public static void CreateNewPCGTerrainGraph()
        {
            var graphItem = ScriptableObject.CreateInstance<CreatePCGTerrainGraphAction>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, graphItem,
               $"NewGraph.{Extension}", null, null);
        }

        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID);

            var path = AssetDatabase.GetAssetPath(asset);
            var graph = GetGraphAtPath(path);

            if(graph == null)
                return false;

            PCGGraphWindow.Open(graph);
            return true;
        }

        public static PCGGraph GetGraphAtPath(string path)
            => AssetDatabase.LoadAllAssetsAtPath(path).FirstOrDefault(o => o is PCGGraph) as PCGGraph;

        class CreatePCGGraphAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                PCGGraph graph = CreateNewGraph();

                graph.name = Path.GetFileNameWithoutExtension(pathName);

                AssetDatabase.CreateAsset(graph, pathName);

                ProjectWindowUtil.ShowCreatedAsset(graph);
                Selection.activeObject = graph;
                EditorApplication.delayCall += () => EditorGUIUtility.PingObject(graph);

            }

            PCGGraph CreateNewGraph()
            {
                PCGGraph g = CreateInstance(typeof(PCGGraph)) as PCGGraph;

                return g;
            }
        }

        class CreatePCGTerrainGraphAction : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                PCGTerrainGraph graph = CreateNewGraph();

                graph.name = Path.GetFileNameWithoutExtension(pathName);

                AssetDatabase.CreateAsset(graph, pathName);

                ProjectWindowUtil.ShowCreatedAsset(graph);
                Selection.activeObject = graph;
                EditorApplication.delayCall += () => EditorGUIUtility.PingObject(graph);

            }

            PCGTerrainGraph CreateNewGraph()
            {
                PCGTerrainGraph g = CreateInstance(typeof(PCGTerrainGraph)) as PCGTerrainGraph;

                return g;
            }
        }
    }
}
