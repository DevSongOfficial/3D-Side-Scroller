using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameSystem;

public enum Layer
{
    Default         = 0,
    
    Environment     = 7,
    Character       = 8,
    Ground          = 9,
    Wall            = 10,

    TriggerArea     = 14,

    Placeable       = 20,
    Damageable      = 24,
    Interactable    = 25,
    
    RaycastBlockerForLevelEditor = 31,
}

public enum Tag
{
    Untagged, Player, Enemy, Prob, HoleCup, 

    // Ground Types
    Green, Dirt, Grass, Sand, Water,

    // Cameras
    MainCamera, EditorCamera,
}

public enum ScoreType
{
    InComplete    = -99, // When player hasn't completed this stage.

    HoleInOne    = -10,

    Phoenix      = -6,
    Ostritch     = -5,
    Condor       = -4,
    Albatross    = -3,
    Eagle        = -2,
    Birdie       = -1,
    Par              = 0,
    Bogey               = 1,
    DoubleBogey         = 2,
    TripleBogey         = 3,
    QuadrupleBogey      = 4,
    QuintupleBogey      = 5,
    SextupleBogey       = 6,
    SeptupleBogey       = 7,
}

public sealed class System_GameManager : MonoBehaviour
{
    private void Awake()
    {
        Player = FindObjectOfType<PlayerCharacter>();
    }

    private void Start()
    {
        if (IsMakerScene) return;

        // Load and start the stage.
        GameStart(stageToStart);
    }

    // Handle the stage to start.
    private static byte stageToStart = 3;
    public void GoToMainMenu()
    {
        LoadScene(Scene.Menu);
    }

    public void GoToNextStage()
    {
        stageToStart++;
        LoadScene(Scene.Main);
    }

    public PlayerCharacter Player { get; private set; }
    
    public void SetPar(byte par) => Par = par;
    public byte Par { get; private set; }



    public event Action OnGameStart;
    public event Action OnGameFinished;

    public void GameStart(int stage)
    {
        SaveManager.LoadStage(stage);
        StartCoroutine(GameStartRoutine());
    }

    private IEnumerator GameStartRoutine()
    {
        var golfBall = POFactory.GetRegisteredSingletonPO<GolfBall>();
        golfBall.OnHit += Player.IncrementStroke;

        // Wait for others to be prepared.
        yield return new LateUpdate();

        // Initialize objects placed by level editor manager. (No need for further use of level editor in the scene so deactivates it.)
        LevelEditorManager.SetPlayMode(PlayMode.Playing);
        LevelEditorManager.gameObject.SetActive(false);

        // Initilize UIs.
        UIManager.CloseUI(UIManager.UI.Panel_StageClear);

        InputManager.ToggleInput(true);

        OnGameStart?.Invoke();
    }

    public void BallInTheCup()
    {
        OnGameFinished?.Invoke();

        InputManager.ToggleInput(false);

        // Calculate the round result of this stage.
        int score = Player.StrokeCount - Par;
        ScoreType scoreType = Player.StrokeCount == 1 ?  ScoreType.HoleInOne : (ScoreType)score;

        // Show UIs.
        UIManager.PopupUI(UIManager.UI.Panel_StageClear);
        UIManager.SetText(UIManager.UI.Text_StageClear, UIManager.AddSpaceBeforeUppercase(scoreType.ToString()));
    }
}