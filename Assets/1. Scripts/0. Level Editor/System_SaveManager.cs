using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using static GameSystem;

public class System_SaveManager : MonoBehaviour
{ 
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


    public string prefix = "Stage_";
    public void SaveStageDataWithStageNumber(int stageNumber)
    {
        StageDataHandler dataHandler = new StageDataHandler();

        // Handle stage data.
        dataHandler.AddStageData(par: GameManager.Par, placeableObjects: POFactory.RegistedPOs);

        var data = JsonUtility.ToJson(dataHandler);
        saveSystem.SaveData(data, $"{prefix}{stageNumber}");
    }

    public StageDataHandler LoadStageData(int stageNumber)
    {
        return LoadStageData<StageDataHandler>($"{prefix}{stageNumber}");
    }

    public StageDataHandler LoadStageData(string title)
    {
        var data = saveSystem.LoadData($"{prefix}{title}");
        if (String.IsNullOrEmpty(data)) return null;

        return JsonUtility.FromJson<StageDataHandler>(data);
    }

    private T LoadStageData<T>(string name) where T : class
    {
        var data = saveSystem.LoadData(name);
        if (String.IsNullOrEmpty(data)) return null;

        return JsonUtility.FromJson<T>(data);
    }

    // Function for users to upload their custom stages to the server.
    public async Task UploadStageDataAync(string title, string description)
    {
        if (POFactory.RegisteredSingletonPOs.Count < 4) return;

        // Handle stage data.
        StageDataHandler dataHandler = new StageDataHandler();
        dataHandler.AddStageData(par: GameManager.Par, placeableObjects: POFactory.RegistedPOs);

        // Save it.
        var data = JsonUtility.ToJson(dataHandler);
        saveSystem.SaveData(data, $"{prefix}{title}");

        // Upload it.(The file name is the same with the title.)
        await CloudManager.UploadStageDataAsync(GameManager.GetUserID(), title, description);
    }

    public async Task<string> DownloadStageDataAsync()
    {
        return await CloudManager.DownloadRandomStageDataAsync();
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
            SaveManager.SaveStageDataWithStageNumber(int.Parse(indexToSave));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        indexToLoad = EditorGUILayout.TextField("Loaded Data Index: ", indexToLoad);
        if (GUILayout.Button("Load"))
        {
            var data = SaveManager.LoadStageData(int.Parse(indexToLoad));
            GameManager.SetupStage(data);
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
#endif