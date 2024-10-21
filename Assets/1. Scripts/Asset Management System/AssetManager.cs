using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
   public static GameObject GetPrefab(string folderName, string prefabName)
    {
        var prefab = Resources.Load<GameObject>($"Prefabs/{folderName}/{prefabName}");
        if (prefab is null)
        {
            Debug.LogWarning($"Invalid name or path: Prefabs/{folderName}/{prefabName}");
            return null;
        }

        return prefab;
    }

    public static GameObject GetPrefab<T>(T prefabType) where T : Enum
    {
        ReadOnlySpan<char> span = typeof(T).ToString().AsSpan();
        ReadOnlySpan<char> result = span.Slice(nameof(Prefab).Length + 1);
        return GetPrefab(result.ToString(), prefabType.ToString());
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AssetManager))]
public class PrefabScriptEditor : Editor
{
    // You can rename all the variables you want
    private static readonly string FolderName = "Prefabs"; // Name of the file where prefab files are located in the project.


    private readonly string Path_Resources = $"{Application.dataPath}/Resources/{FolderName}";
    private readonly string Path_PrefabScript = $"{Application.dataPath}/1. Scripts/Asset Management System/Prefab.cs";

    private Stack<StringBuilder> previousSources = new Stack<StringBuilder>();

    private StringBuilder GenerateNewScript(bool saveThisScript = true)
    {
        var sourceCode = new StringBuilder("// This script's automatically generated by AssetManager.cs");
        sourceCode.AppendLine("\npublic struct Prefab {");

        var enumInfo = new DirectoryInfo(Path_Resources);
        var enums = enumInfo.GetDirectories(); // Use GetDirectories() to get folders' name cuz they don't have file extenstion.
        
        for (int i = 0; i < enums.Length; i++)
        {
            sourceCode.AppendLine($"\tpublic enum {enums[i].Name} {{"); // Must use item.Name, or you'll get the full name of it including the path. 
            var constantInfo = new DirectoryInfo($"{Path_Resources}/{enums[i].Name}");
            var constants = constantInfo.GetFiles("*.prefab", SearchOption.AllDirectories); // Use GetFiles() to get files' name.
                                                                                            // Avoid loading .meta files either by using "*.extensionName".
            for (int j = 0; j < constants.Length; j++)
            {
                sourceCode.AppendLine($"\t\t{constants[j].Name.Replace(".prefab","")},"); // Don't forget to remove file extension.
            }
            
            sourceCode.AppendLine("\t}");
        }

        sourceCode.AppendLine("}");

        return GenerateNewScript(sourceCode, saveThisScript);
    }

    private StringBuilder GenerateNewScript(StringBuilder sourceCode, bool saveThisScript = true)
    {
        if (File.Exists(Path_PrefabScript))
            File.WriteAllText(Path_PrefabScript, string.Empty); // This is how you should clear contents of a text file. (better than File.Delete();)
        else
        {
            Debug.LogWarning($"<color=red>File Missing!\nPrefab.cs is not in the folder: {Path_PrefabScript}</color>");
            return null;
        }

        File.AppendAllText(Path_PrefabScript, sourceCode + "\n");
        

        if (!saveThisScript)
        {
            Debug.Log("<color=cyan>Reverted Completely</color>");
            return null;
        }

        Debug.Log("<color=cyan>Save Completed</color>");
        return sourceCode;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Save Prefabs"))
        {
            Debug.Log("<color=yellow>Saving..</color>");

            var newScript = GenerateNewScript();
            if (newScript is null) return;
            
            previousSources.Push(newScript);
        }

        if (GUILayout.Button("Revert Changes"))
        {
            if (previousSources.Count == 0)
            {
                Debug.LogWarning("Can't revert - no longer have saved data");
                return;
            }

            GenerateNewScript(previousSources.Pop(), saveThisScript: false);
        }
        GUILayout.EndHorizontal();
    }
}
#endif