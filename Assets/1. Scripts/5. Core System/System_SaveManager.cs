using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class System_SaveManager : MonoBehaviour
{
    public event Action OnSaveData;
    public event Action OnLoadData;

    private const string prefix = "Data_";
    private int dataIndex = 0;

    public void SaveData(string data)
    {
        if (WriteToFile(prefix + dataIndex, data))
        {
            OnSaveData?.Invoke();
            Debug.Log("<color=cyan>Save Completed</color>");
        }
    }

    public string LoadData()
    {
        string data = string.Empty;
        if (ReadFromFile(prefix + dataIndex, out data))
        {
            OnLoadData?.Invoke();
            Debug.Log("<color=cyan>Load Completed</color>");
        }
        return data;
    }

    private bool WriteToFile(string name, string content)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, name);

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

    private bool ReadFromFile(string name, out string content)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, name);
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
}

[Serializable]
public class SaveDataHandler
{
    public List<SerializedPrefabData> prefabDatas = new List<SerializedPrefabData>();

    public void Add(Prefab.General type, Vector3 position, Vector3 eulerAngles)
    {
        prefabDatas.Add(new SerializedPrefabData(type, position, eulerAngles));
    }
}

[Serializable]
public class SerializedPrefabData
{
    public Prefab.General type;
    public SerializedVector3 position;
    public SerializedVector3 eulerAngles;

    public SerializedPrefabData(Prefab.General type, Vector3 position, Vector3 eulerAngles)
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
        this.x = position.x;
        this.y = position.y;
        this.z = position.z;
    }

    public Vector3 GetValue()
    {
        return new Vector3(x, y, z);
    }
}
