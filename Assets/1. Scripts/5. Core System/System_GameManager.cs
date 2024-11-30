using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        switch (SceneLoader.CurrentScene)
        {
            case Scene.Main: case Scene.Maker:
                Player = FindObjectOfType<PlayerCharacter>();
                break;

            case Scene.Menu:
                if (!SaveManager.LoadGameData())
                    clearedStages = Enumerable.Repeat(ScoreType.InComplete, stageCount + 1).ToArray();
                break;

        }
    }

    private void Start()
    {
        // Load and start the stage.
        if (SceneLoader.IsMainScene) 
            GameStart(stageToStart);
    }

    private void Update()
    {
        HandleEditorOnlyInput();

        ReloadSceneWhenPlayerOrGolfBallOutOfBounds();
    }

    // Handle the stage to start.
    public event Action<int> OnStageNumberChanged;
    private static int stageToStart = 3;
    public void SetStageNumber(int stageNumber)
    {
        if (stageNumber <= 0) return;

        stageToStart = stageNumber;
        OnStageNumberChanged?.Invoke(stageToStart);
    }

    private static int stageCount = 18;
    private static ScoreType[] clearedStages = new ScoreType[stageCount + 1]; // Stages that have been completed by player.
    public void SetClearedStages(ScoreType[] clearedStages) => System_GameManager.clearedStages = clearedStages;


    public void IncreaseStageNumber() => SetStageNumber(stageToStart + 1);
    public void DecreaaseStageNumber() => SetStageNumber(stageToStart - 1);
    public int GetStageNumber() => stageToStart;

    public void GoToMainMenu()
    {
        SceneLoader.LoadScene(Scene.Menu, TransitionEffect.FadeToBlack);
    }

    public void GoToNextStage()
    {
        IncreaseStageNumber();
        SceneLoader.LoadScene(Scene.Main, TransitionEffect.FadeToBlack);
    }

    public PlayerCharacter Player { get; private set; }

    // Par: predetermined number of strokes.
    public byte Par { get; private set; }
    public void SetPar(byte par) => Par = par;

    public event Action OnGameStart;
    public event Action OnGameFinished;

    public void GameStart(int stage)
    {
        StartCoroutine(GameStartRoutine(stage));
    }

    private IEnumerator GameStartRoutine(int stage)
    {
        SaveManager.LoadStageData(stage);

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

        // Save clear data.
        if (SceneLoader.IsMainScene)
        {
            clearedStages[GetStageNumber()] = scoreType;
            SaveManager.SaveGameData(clearedStages);
        }
    }

    private void ReloadSceneWhenPlayerOrGolfBallOutOfBounds()
    {
        if (SceneLoader.IsLoading) return;
        if (!SceneLoader.IsMakerScene && !SceneLoader.IsMainScene) return;
        if (LevelEditorManager.Mode != PlayMode.Playing) return;

        int min = PlaceableGround.GetTheLowestGroundPosition() - 3;
        if (Player.transform.position.y < min || POFactory.GetRegisteredSingletonPO<GolfBall>().transform.position.y < min)
            SceneLoader.LoadScene(SceneLoader.CurrentScene, TransitionEffect.FadeToBlack);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void HandleEditorOnlyInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
            SceneLoader.LoadScene(Scene.Main, TransitionEffect.FadeToBlack);

        if(Input.GetKeyDown(KeyCode.Escape))
            SceneLoader.LoadScene(Scene.Menu, TransitionEffect.FadeToBlack);
    }
}