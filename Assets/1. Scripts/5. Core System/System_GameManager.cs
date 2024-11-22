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
    InComplete    = -99, // Player hasn't completed this stage.

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
    public void GoToMainMenu()
    {
        LoadScene(Scene.Menu);
    }

    public void GoToNextStage()
    {
        stageToStart++;
        LoadScene(Scene.Main);
    }

    private static byte stageToStart = 3;

    private void Awake()
    {
        Player = FindObjectOfType<PlayerCharacter>();
        input = GetComponent<PlayerInput>();
        LevelEditorManager.OnEditorModeToggled += (bool enable) => ToggleInput(!enable);
    }

    private void Start()
    {
        // Load and start the stage.
        if (IsMakerScene) return;
        GameStart(stageToStart);
    }


    [Header("Map Transform")]
    // Transform where every object being placed at runtime is going be here.
    [SerializeField] private Transform map;
    public void AttachToMap(Transform objectToBeAttached)
    {
        objectToBeAttached.SetParent(map);
    }

    // This is where unused objects will be located, which is automatically going to be removed on changing scene.
    [SerializeField] private Transform lagacy;
    public void MoveToLagacy(Transform objectToBeRemoved)
    {
        objectToBeRemoved.SetParent(lagacy);
    }



    [Header("Input System")]
    [SerializeField] private PlayerInput input;
    public void ToggleInput(bool enable) => input.enabled = enable;
    #region Inputs
    public event Action<MovementDirection> Input_OnChangeDirection;
    public event Action<ZAxisMovementDirection> Input_OnChangeZDirection;
    public event Action Input_OnJump;
    public event Action<bool> Input_OnClick;
    public event Action<Vector2> Input_OnDrag;
    public event Action Input_OnInteract;
    public event Action Input_OnSwitchClub;
    public event Action Input_OnTogglePickup;

    // Automatically called by Player Input Action
    private void OnMove(InputValue value) // [A/D], [LeftArrow/RightArrow] key down & up
    {
        var valueConverted = (int)value.Get<float>();
        var directionToMove = (MovementDirection)valueConverted;
        Input_OnChangeDirection.Invoke(directionToMove);
    }

    private void OnZMove(InputValue value)
    {
        var valueConverted = (int)value.Get<float>();
        if (valueConverted == 0) return;

        var directionToMove = (ZAxisMovementDirection)valueConverted;
        Input_OnChangeZDirection?.Invoke(directionToMove);
    }

    private void OnJump() // [Space Bar] pressed
    {
        Input_OnJump.Invoke();
    }

    private void OnClick(InputValue value) // [Mouse 0]
    {
        var mouseDown = (int)value.Get<float>() == 1;
        Input_OnClick.Invoke(mouseDown);
    }

    private void OnDrag(InputValue value) // [Cursor position] changed
    {
        var mousePosition = value.Get<Vector2>();
        Input_OnDrag.Invoke(mousePosition);
    }

    private void OnInteract() // [E] pressed
    {
        Input_OnInteract.Invoke();
    }

    private void OnSwitchClub() // [Q] pressed
    {
        Input_OnSwitchClub.Invoke();
    }

    private void OnTogglePickup() // [R] pressed
    {
        Input_OnTogglePickup.Invoke();
    }
    #endregion


    public PlayerCharacter Player { get; private set; }
    
    // Singleton objects that must be required to start a stage.
    public GolfBall GolfBall { get; private set; }
    public PlaceableSpawnPoint SpawnPoint { get; private set; }
    private Placard placard;

    public void GetReferenceToSingletonObjects()
    {
        GolfBall = FindObjectOfType<GolfBall>();
        SpawnPoint = FindObjectOfType<PlaceableSpawnPoint>();
        placard = FindObjectOfType<Placard>();
    }

    public void SetPar(byte par) => Par = par;
    public byte Par { get; private set; }

    public event Action OnGameStart;
    public event Action OnGameFinished;
    public void GameStart(int stage)
    {
        StageMaker.LoadStage(stage);
        StartCoroutine(GameStartRoutine());
    }
    private IEnumerator GameStartRoutine()
    {
        // Get reference to singleton objects.
        GetReferenceToSingletonObjects();

        if (GolfBall == null || SpawnPoint == null || placard == null)
        {
            // Reference error
            LoadScene(Scene.Menu);
            yield break;
        }

        GolfBall.OnHit += Player.IncrementStroke;

        // Wait for others to be prepared.
        yield return new LateUpdate();

        // Initialize objects placed by level editor manager. (No need for further use of level editor in the scene so deactivates it.)
        LevelEditorManager.SetPlayMode(PlayMode.Playing);
        LevelEditorManager.gameObject.SetActive(false);

        // Initilize UIs.
        UIManager.CloseUI(UIManager.UI.Panel_StageClear);

        ToggleInput(true);

        OnGameStart?.Invoke();
    }
    public void BallInTheCup()
    {
        OnGameFinished?.Invoke();

        ToggleInput(false);

        // Calculate the round result of this stage.
        int score = Player.StrokeCount - Par;
        ScoreType scoreType = Player.StrokeCount == 1 ?  ScoreType.HoleInOne : (ScoreType)score;

        // Show UIs.
        UIManager.PopupUI(UIManager.UI.Panel_StageClear);
        UIManager.SetText(UIManager.UI.Text_StageClear, UIManager.AddSpaceBeforeUppercase(scoreType.ToString()));
    }

    public event Action OnGreen;
    public event Action OnExitGreen;
    public void BallOnTheGreen()
    {
        OnGreen?.Invoke();
    }
    public void BallOutTheGreen()
    {
        OnExitGreen?.Invoke();
    }
}