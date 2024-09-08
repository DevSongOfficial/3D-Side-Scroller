using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelEditorMain : MonoBehaviour
{
    public LevelEditorUI EditorUI => ui;
    public Camera EditorCamera => camera;

    [SerializeField] private LevelEditorUI ui;
    [SerializeField] private new Camera camera;
}
