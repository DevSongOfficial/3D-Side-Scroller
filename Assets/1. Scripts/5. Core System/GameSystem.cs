using UnityEngine;

public sealed class GameSystem : MonoBehaviour
{
    public static System_GameManager GameManager => gameManager;
    private static System_GameManager gameManager;

    public static System_InputManager InputManager => inputManager;
    private static System_InputManager inputManager;


    public static System_FXManager FXManager => fxManager;
    private static System_FXManager fxManager;

    public static System_UIManager UIManager => uiManager;
    private static System_UIManager uiManager;

    public static System_LevelEditorManager LevelEditorManager => levelEditorManager;
    private static System_LevelEditorManager levelEditorManager;

    public static System_SaveManager SaveManager => saveManager;
    private static System_SaveManager saveManager;

    public static System_SceneLoader SceneLoader => sceneLoader;
    private static System_SceneLoader sceneLoader;

    public static System_CloudManager CloudManager => cloudManager;
    private static System_CloudManager cloudManager;

    public static Factory_POFactory POFactory => poFactory;
    private static Factory_POFactory poFactory;

    private void Awake()
    {
        gameManager     = FindObjectOfType<System_GameManager>();
        inputManager    = FindObjectOfType<System_InputManager>();
        fxManager       = FindObjectOfType<System_FXManager>();
        uiManager       = FindObjectOfType<System_UIManager>();
        saveManager     = FindObjectOfType<System_SaveManager>();
        sceneLoader     = FindObjectOfType<System_SceneLoader>();
        cloudManager    = FindObjectOfType<System_CloudManager>();
        poFactory       = FindObjectOfType<Factory_POFactory>();

        var levelEditorManagerPrefab = AssetManager.GetPrefab(Prefab.Debugger.System_LevelEditorManager);
        levelEditorManager = Instantiate(levelEditorManagerPrefab, transform).GetComponent<System_LevelEditorManager>();
    }

    private void Start()
    {
        if(SceneLoader.IsMakerScene)
            levelEditorManager.gameObject.SetActive(true);
    }
}