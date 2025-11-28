using UnityEngine;
using UnityEditor;
using System.IO;

namespace PCG.Editor
{
    public static class PCGCustomScripts
    {
        [MenuItem("Assets/Create/PCG/New Job Node", false, 83)]
        public static void CreateJobNode()
        {
            var template = Resources.Load<TextAsset>("ScriptTemplates/JobNodeTemplate.cs");
            var path = AssetDatabase.GetAssetPath(template);

            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(path, Path.GetFileNameWithoutExtension(path));
        }
    }
}
