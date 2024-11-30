using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static GameSystem;

public class System_SaveManager : MonoBehaviour
{ 
    public event Action OnStageLoadStart;
    public event Action OnStageLoadComplete;

    private SaveSystem saveSystem = new SaveSystem();

    public void SaveGameData(ScoreType[] clearedStages)
    {
        GameDataHandler dataHandler = new GameDataHandler();

        // Handel game data.
        dataHandler.AddGameData(clearedStages);

        var data = JsonUtility.ToJson(dataHandler);
        saveSystem.SaveData(data, "GameData");
    }

    public bool LoadGameData()
    {
        var data = saveSystem.LoadData("GameData");
        if (String.IsNullOrEmpty(data)) return false;

        GameDataHandler dataHandler = JsonUtility.FromJson<GameDataHandler>(data);
        GameManager.SetClearedStages(dataHandler.scores.ToArray());
        
        return true;
    }


    public void SaveStageData(int indexToSave)
    {
        StageDataHandler dataHandler = new StageDataHandler();

        // Handle stage data.
        dataHandler.AddStageData(par: GameManager.Par, placeableObjects: POFactory.RegistedPOs);

        var data = JsonUtility.ToJson(dataHandler);
        saveSystem.SaveData(data, $"Stage_{indexToSave}");
    }

    public bool LoadStageData(int indexToLoad)
    {
        OnStageLoadStart?.Invoke();

        var data = saveSystem.LoadData($"Stage_{indexToLoad}");
        if (String.IsNullOrEmpty(data)) return false;

        StageDataHandler dataHandler = JsonUtility.FromJson<StageDataHandler>(data);

        // Handle prefab datas.
        POFactory.RemoveEveryRegisterdPO();
        PlaceableGround.ClearTile();
        foreach (var prefab in dataHandler.prefabDatas)
        {
            var po = POFactory.CreatePO(prefab.type);
            po.transform.position = prefab.position.GetValue();
            po.transform.eulerAngles = prefab.eulerAngles.GetValue();

            po.AsGround()?.AddToTile();
        }
        
        // Handle game data.
        GameManager.SetPar(dataHandler.par);

        LevelEditorManager.SetPlayMode(PlayMode.Editing);

        OnStageLoadComplete?.Invoke();

        return true;
    }

    // Function for users to upload their stages to the server.
    [ContextMenu("Upload the stage")]
    public void UploadStageData()
    {
        if (POFactory.RegisteredSingletonPOs.Count < 4) return;

        StageDataHandler dataHandler = new StageDataHandler();

        // Handle stage data.
        dataHandler.AddStageData(par: GameManager.Par, POFactory.RegistedPOs);

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
            SaveManager.SaveStageData(int.Parse(indexToSave));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        indexToLoad = EditorGUILayout.TextField("Loaded Data Index: ", indexToLoad);
        if (GUILayout.Button("Load"))
        {
            SaveManager.LoadStageData(int.Parse(indexToLoad));
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
#endif