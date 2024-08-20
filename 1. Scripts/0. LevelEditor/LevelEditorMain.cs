using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEditorMain : MonoBehaviour
{
    public LevelEditorManager LevelEditorManager => levelEditorManager;
    public LevelEditorUI EditorUI => ui;
    public Camera EditorCamera => camera;

    [SerializeField] private LevelEditorManager levelEditorManager;
    [SerializeField] private LevelEditorUI ui;
    [SerializeField] private new Camera camera;
}
