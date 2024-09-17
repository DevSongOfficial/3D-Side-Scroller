using UnityEngine;

public class GameSystem : MonoBehaviour
{
    public static System_GameManager GameManager => gameManager;
    private static System_GameManager gameManager;

    public static System_UIManager UIManager => uiManager;
    private static System_UIManager uiManager;

    public static System_LevelEditorManager LevelEditorManager => levelEditorManager;
    private static System_LevelEditorManager levelEditorManager;

    private void Awake()
    {
        gameManager = FindObjectOfType<System_GameManager>();
        uiManager = FindObjectOfType<System_UIManager>();

        var levelEditorManagerPrefab = AssetManager.GetPrefab(Prefab.Debugger.System_LevelEditorManager);
        levelEditorManager = Instantiate(levelEditorManagerPrefab, transform).GetComponent<System_LevelEditorManager>();
    }

    private void Start()
    {
        levelEditorManager.gameObject.SetActive(true);
    }
}
