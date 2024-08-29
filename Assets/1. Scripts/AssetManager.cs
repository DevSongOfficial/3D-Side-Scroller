using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Name of each prefab in the 'Prefabs' Folder have to match PrefabType
// Naming example: General_Cube_1
//                 UI_SpawnButton_1
public struct Prefab
{
    // General prefabs are placeable objects you can create in the custom editor.
    // buttons' automatically gonna be added whenever you add new items here.
    public enum General
    {
        // Probs(Items)
        Cube_1,
        GolfBall,

        // Characters
        ZombieCharacter_1,
    }

    public enum UI
    {
        SpawnButton_1
    }

    public enum Debugger
    {
        Sphere_1
    }
}
public class AssetManager : MonoBehaviour
{
    public static GameObject GetPrefab(string name)
    {
        var prefab = Resources.Load<GameObject>($"Prefabs/{name}");
        if (prefab is null)
        {
            Debug.LogWarning($"Invalid name or path: {name}");
            return null;
        }

        return prefab;
    }

    public static GameObject GetPrefab<T>(T prefabType) where T : Enum
    {
        // todo: Check if using Span is faster than iterator + string.
        ReadOnlySpan<char> span = $"{typeof(T)}_{prefabType}".AsSpan();
        ReadOnlySpan<char> result = span.Slice(nameof(Prefab).Length + 1);
        return GetPrefab(result.ToString());
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
