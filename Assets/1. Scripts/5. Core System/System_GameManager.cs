using Firebase.Functions;
using Firebase;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Networking;
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
                    ClearedStages = Enumerable.Repeat(ScoreType.InComplete, StageCount + 1).ToArray();
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

    // Handle player info.
    public PlayerCharacter Player { get; private set; }
    private static string id = "000000_ADMIN"; // YYMMDD_ + random alphbets.
    public string GetUserID() => id;

    // Hnadle user-made courses.
    private static Dictionary<string, object> userStageData;
    public Dictionary<string, object> GetUserStageData() => userStageData;
    public void SetUserStageData(Dictionary<string, object> userStageData) => System_GameManager.userStageData = userStageData;
    

    // Handle the stage to start.
    public event Action<int> OnStageNumberChanged;
    private static int stageToStart = 1;
    public int GetStageNumber() => stageToStart;
    public void SetStageNumber(int stageNumber)
    {
        if (stageNumber <= 0 || stageNumber > StageCount) return;

        stageToStart = stageNumber;
        OnStageNumberChanged?.Invoke(stageToStart);
    }
    // Check if player completed previous stage.
    public bool CanStartTheStage(int stageNumber)
    {
        if (stageNumber == 1) return true;

        return GetScoreOfStageByIndex(stageNumber - 1, out ScoreType score) && score != ScoreType.InComplete;
    }

    public readonly static int StageCount = 18;
    private static ScoreType[] ClearedStages = new ScoreType[StageCount + 1]; // Stages that have been completed by player.
    public void SetClearedStages(ScoreType[] clearedStages) => ClearedStages = clearedStages;
    public bool GetScoreOfStageByIndex(int index, out ScoreType score)
    {
        if (index <= 0 || index > StageCount)
        {
            score = ScoreType.InComplete;
            return false;
        }
        else
        {
            score = ClearedStages[index];
            return true;
        }
    }

    public void IncreaseStageNumber() => SetStageNumber(stageToStart + 1);
    public void DecreaaseStageNumber() => SetStageNumber(stageToStart - 1);

    public void GoToMainMenu()
    {
        SceneLoader.LoadScene(Scene.Menu, TransitionEffect.FadeToBlack);
    }

    public void GoToNextStage()
    {
        IncreaseStageNumber();
        SceneLoader.LoadScene(Scene.Main, TransitionEffect.FadeToBlack);
    }


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
        var dataHandler = SaveManager.LoadStageData(stage);
        SetupStage(dataHandler);

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

    public event Action OnStageSetup;
    public void SetupStage(StageDataHandler dataHandler)
    {
        // Handle prefab datas.
        POFactory.RemoveEveryRegisterdPO();
        PlaceableGround.ClearTile();
        foreach (var prefab in dataHandler.prefabDatas)
        {
            var po = POFactory.CreatePO(prefab.type);
            po.transform.position = prefab.position.GetValue();
            po.transform.eulerAngles = prefab.eulerAngles.GetValue();

            po.AsGround()?.AddToTile();
        }

        // Handle game data.
        GameManager.SetPar(dataHandler.par);

        LevelEditorManager.SetPlayMode(PlayMode.Editing);

        OnStageSetup?.Invoke();
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
        if (SceneLoader.IsMainScene && scoreType.BetterThan(ClearedStages[GetStageNumber()]))
        {
            ClearedStages[GetStageNumber()] = scoreType;
            SaveManager.SaveGameData(ClearedStages);
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
        if (Input.GetKeyDown(KeyCode.R) && SceneLoader.IsMainScene)
            SceneLoader.LoadScene(Scene.Main, TransitionEffect.FadeToBlack);

        if(Input.GetKeyDown(KeyCode.Escape))
            SceneLoader.LoadScene(Scene.Menu, TransitionEffect.FadeToBlack);
    }
}