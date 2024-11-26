using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GameSystem;

public class System_SaveManager : MonoBehaviour
{ 
    public event Action OnLoadStart;
    public event Action OnLoadComplete;

    private SaveSystem saveSystem;

    private void Awake()
    {
        saveSystem = new SaveSystem();
    }

    public void SaveStage(int indexToSave)
    {
        SaveDataHandler dataHandler = new SaveDataHandler();

        // Handle game data.
        dataHandler.AddGameData(par: GameManager.Par);

        // Handle prefab datas.
        foreach (var placeableObject in POFactory.RegistedPOs)
        {
            dataHandler.AddPrefab(placeableObject.Type, placeableObject.transform.position, placeableObject.transform.eulerAngles);
        }
        var data = JsonUtility.ToJson(dataHandler);
        saveSystem.SaveData(data, indexToSave);
    }

    public void LoadStage(int indexToLoad)
    {
        OnLoadStart?.Invoke();

        var data = saveSystem.LoadData(indexToLoad);
        if (String.IsNullOrEmpty(data)) return;

        SaveDataHandler dataHandler = JsonUtility.FromJson<SaveDataHandler>(data);

        // Handle game data.
        GameManager.SetPar(dataHandler.gameData.par);

        // Handle prefab datas.
        POFactory.RemoveEveryRegisterdPO();

        foreach (var prefab in dataHandler.prefabDatas)
        {
            var po = POFactory.CreatePO(prefab.type);
            po.transform.position = prefab.position.GetValue();
            po.transform.eulerAngles = prefab.eulerAngles.GetValue();

            po.AsGround()?.AddToTile();
        }
        LevelEditorManager.SetPlayMode(PlayMode.Editing);

        OnLoadComplete?.Invoke();
    }

    // Function for users to upload their stages to the server.
    [ContextMenu("Upload the stage")]
    public void UploadStage()
    {
        if (POFactory.RegisteredSingletonPOs.Count < 4) return;

        SaveDataHandler dataHandler = new SaveDataHandler();

        // Handle game data.
        dataHandler.AddGameData(par: GameManager.Par);

        // Handle prefab datas.
        foreach (var placeableObject in POFactory.RegistedPOs)
        {
            dataHandler.AddPrefab(placeableObject.Type, placeableObject.transform.position, placeableObject.transform.eulerAngles);
        }
        var data = JsonUtility.ToJson(dataHandler);

        
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(System_SaveManager))]
public class SaveManagerEditor : Editor
{
    private string indexToSave;
    private string indexToLoad;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.BeginVertical();

        GUILayout.BeginHorizontal();
        indexToSave = EditorGUILayout.TextField("Saved Data Index: ", indexToSave);
        if (GUILayout.Button("Save"))
        {
            SaveManager.SaveStage(int.Parse(indexToSave));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        indexToLoad = EditorGUILayout.TextField("Loaded Data Index: ", indexToLoad);
        if (GUILayout.Button("Load"))
        {
            SaveManager.LoadStage(int.Parse(indexToLoad));
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
#endif