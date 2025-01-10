using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UIElements;

public enum FileType
{
    StageData,
    GameData
}

public class SaveSystem
{
    #region Data Handling
    public void SaveData(string data, string name, FileType type)
    {
        if (WriteToFile(name, data, type))
        {
            Debug.Log("<color=cyan>Save Completed</color>");
        }
    }

    public string LoadData(string name, FileType type)
    {
        string data = string.Empty;
        if (ReadFromFile(name, out data, type))
        {
            Debug.Log("<color=cyan>Load Completed</color>");
        }
        return data;
    }

    private bool WriteToFile(string name, string content, FileType type)
    {
        string fullPath;
        if (type == FileType.GameData)
            fullPath = Path.Combine(Application.persistentDataPath, name);
        else
            fullPath = Path.Combine(Application.dataPath, "Stage Data", name);

        try
        {
            File.WriteAllText(fullPath, content);
            Debug.Log(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error when saving: " + e.Message);
        }
        return false;
    }

    private bool ReadFromFile(string name, out string content, FileType type)
    {
        string fullPath;
        if (type == FileType.GameData)
            fullPath = Path.Combine(Application.persistentDataPath, name);
        else
            fullPath = Path.Combine(Application.dataPath, "Stage Data", name);
        
        try
        {
            content = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("Error when loading: " + e.Message);
            content = string.Empty;
        }
        return false;
    }
    #endregion
}

[Serializable]
public class StageDataHandler
{
    // Data for each stage to save and load.
    public byte par;
    public List<SerializedPrefabData> prefabDatas = new List<SerializedPrefabData>();

    public void AddStageData(byte par, List<PlaceableObjectBase> placeableObjects)
    {
        this.par = par;

        foreach (PlaceableObjectBase po in placeableObjects)
            prefabDatas.Add(new SerializedPrefabData(po.Type, po.transform.position, po.transform.eulerAngles));
    }
}

[Serializable]
public class GameDataHandler
{
    // Data for the whole game
    public List<ScoreType> scores = new List<ScoreType>();
    
    public void AddGameData(ScoreType[] scores)
    {
        foreach(ScoreType score in scores)
            this.scores.Add(score);
    }
}

[Serializable]
public class SerializedPrefabData
{
    public Prefab.PO type;
    public SerializedVector3 position;
    public SerializedVector3 eulerAngles;

    public SerializedPrefabData(Prefab.PO type, Vector3 position, Vector3 eulerAngles)
    {
        this.type = type;
        this.position = new SerializedVector3(position);
        this.eulerAngles = new SerializedVector3(eulerAngles);
    }
}

[Serializable]
public class SerializedVector3
{
    public float x, y, z;

    public SerializedVector3(Vector3 position)
    {
        x = position.x;
        y = position.y;
        z = position.z;
    }

    public Vector3 GetValue()
    {
        return new Vector3(x, y, z);
    }
}