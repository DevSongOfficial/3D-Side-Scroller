using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

// Name of each prefab in the 'Prefabs' Folder have to match PrefabType
public class AssetsManager : MonoBehaviour
{
    public enum PrefabType
    {
        Cube,



        ZombieCharacter_1,
    }

    #region Singleton
    private static AssetsManager instance;
    public static AssetsManager GetAsset()
    {
        if (instance is null) instance = GameObject.Find("GameManager").AddComponent<AssetsManager>();
        return instance;
    }
    #endregion

    public static GameObject GetPrefab(string name)
    {
        var prefab = Resources.Load<GameObject>($"Prefabs/{name}");
        if (prefab is null)
        {
            Debug.LogWarning($"Invalid path: {name}");
            return null;
        }

        return prefab;
    }

    public static GameObject GetPrefab(PrefabType prefabType)
    {
        return GetPrefab(prefabType.ToString());
    }

    public static GameObject SpawnPrefab(string name, Transform parent = null)
    {
        return GameObject.Instantiate(GetPrefab(name), parent);
    }
    
    public static GameObject SpawnPrefab(PrefabType prefabType, Transform parent = null)
    {
        return GameObject.Instantiate(GetPrefab(prefabType), parent);
    }



    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

}
