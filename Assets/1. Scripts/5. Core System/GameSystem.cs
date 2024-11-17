using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public sealed  class GameSystem : MonoBehaviour
{
    public static System_GameManager GameManager => gameManager;
    private static System_GameManager gameManager;

    public static System_SaveManager SaveManager => saveManager;
    private static System_SaveManager saveManager;

    public static System_FXManager FXManager => fxManager;
    private static System_FXManager fxManager;

    public static System_UIManager UIManager => uiManager;
    private static System_UIManager uiManager;

    public static System_LevelEditorManager LevelEditorManager => levelEditorManager;
    private static System_LevelEditorManager levelEditorManager;

    public static System_StageMaker StageMaker => stageMaker;
    public static System_StageMaker stageMaker;

    private void Awake()
    {
        gameManager = FindObjectOfType<System_GameManager>();
        saveManager = FindObjectOfType<System_SaveManager>();
        fxManager = FindObjectOfType<System_FXManager>();
        uiManager = FindObjectOfType<System_UIManager>();
        stageMaker = FindObjectOfType<System_StageMaker>();

        var levelEditorManagerPrefab = AssetManager.GetPrefab(Prefab.Debugger.System_LevelEditorManager);
        levelEditorManager = Instantiate(levelEditorManagerPrefab, transform).GetComponent<System_LevelEditorManager>();
    }

    private void Start()
    {
        levelEditorManager.gameObject.SetActive(true);
    }
}