using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AssetsManager : MonoBehaviour
{
    // Prefabs
    public Dictionary<string, GameObject> Prefabs = new Dictionary<string, GameObject>();
    public GameObject Prefab_Cube;

    #region Initialization
    private static AssetsManager instance;
    public static AssetsManager GetAsset()
    {
        if (instance == null) instance = GameObject.Find("GameManager").AddComponent<AssetsManager>();
        return instance;
    }    

    public static GameObject GetPrefabByName(string prefabName)
    {
        //return Prefabs.TryGetValue(prefabName, out GameObject)
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        instance = this;

        Prefabs.Add(Prefab_Cube.name, Prefab_Cube);
    }
    #endregion
}
