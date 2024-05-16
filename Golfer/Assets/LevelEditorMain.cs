using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorMain : MonoBehaviour
{
    [HideInInspector] public LevelEditorManager LevelEditorManager;
    public LevelEditorUI LevelEditorUI;

    private void Awake()
    {
        LevelEditorManager = GetComponent<LevelEditorManager>();
    }
}
