using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum Layer
{
    PlaceableObject = 20,
    InteractableObject = 25,
    RaycastBlockerForLevelEditor = 31,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    private static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    public int GetLayerMask(Layer layer)
    {
        return (int)layer;
    }

    public static bool LayerCheck(GameObject go, Layer layer)
    {
        return go.layer == (int)layer;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            if (LevelEditorManager.Mode == EditorMode.None) LevelEditorManager.SetEditorMode(EditorMode.Editing);
            else if (LevelEditorManager.Mode == EditorMode.Editing) LevelEditorManager.SetEditorMode(EditorMode.None);
        }

    }
}
