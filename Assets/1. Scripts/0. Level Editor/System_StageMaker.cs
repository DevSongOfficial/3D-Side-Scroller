using System;
using UnityEditor;
using UnityEngine;
using static GameSystem;

public class System_StageMaker : MonoBehaviour
{
    public event Action OnLoadStart;
    public event Action OnLoadComplete;

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
        SaveManager.SaveData(data, indexToSave);
    }

    public void LoadStage(int indexToLoad)
    {
        OnLoadStart?.Invoke();

        var data = SaveManager.LoadData(indexToLoad);
        if (String.IsNullOrEmpty(data)) return;

        SaveDataHandler dataHandler = JsonUtility.FromJson<SaveDataHandler>(data);

        // Handle game data.
        GameManager.SetPar(dataHandler.gameData.par);

        // Handle prefab datas.
        POFactory.RemoveEveryRegisterdPO();
        PlaceableObjectBase.ClearTile();
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
}


#if UNITY_EDITOR
[CustomEditor(typeof(System_StageMaker))]
public class StageMakerEditor : Editor
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
            StageMaker.SaveStage(int.Parse(indexToSave));
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        indexToLoad = EditorGUILayout.TextField("Loaded Data Index: ", indexToLoad);
        if (GUILayout.Button("Load"))
        {
            StageMaker.LoadStage(int.Parse(indexToLoad));
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();
    }
}
#endif