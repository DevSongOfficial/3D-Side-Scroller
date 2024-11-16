using Cinemachine;
using System;
using System.IO;
using UnityEditor.Rendering;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using static GameSystem;

public enum Layer
{
    Default = 0,
    Environment = 7,
    Character = 8,
    Ground = 9,
    Wall = 10,
    TriggerArea = 14,
    Placeable = 20,
    Damageable = 24,
    Interactable = 25,
    RaycastBlockerForLevelEditor = 31,
}

public enum Tag
{
    Untagged, Player, Enemy, Prob, HoleCup, 

    // Ground Types
    Green, Dirt, Grass, Sand, Water
}

public sealed class System_GameManager : MonoBehaviour
{
    [ContextMenu("Reload")]
    public void ReloadScnene()
    {
        SceneManager.LoadScene(0);

    }


    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        LevelEditorManager.OnEditorModeToggled += (bool enable) => ToggleInput(!enable);
    }

    private void Start()
    {
        if (loadGameOnStart) LoadGame();
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




    [Header("Save & Load")]
    [SerializeField] private bool loadGameOnStart;
    public event Action OnLoadStart;
    public event Action OnLoadComplete;

    private string[] datas = new string[100];

    [ContextMenu("SAVE")]
    public void SaveGame()
    {
        SaveDataHandler dataHandler = new SaveDataHandler();
        foreach(var placeableObject in PlaceableObjectBase.PlaceableObjectsInTheScene)
        {
            dataHandler.Add(placeableObject.Type, placeableObject.transform.position, placeableObject.transform.eulerAngles);
        }
        var data = JsonUtility.ToJson(dataHandler);
        Debug.Log(data);
        SaveManager.SaveData(data);
    }

    [ContextMenu("LOAD")]
    public void LoadGame()
    {
        OnLoadStart?.Invoke();

        var data = SaveManager.LoadData();
        if (String.IsNullOrEmpty(data)) return;

        SaveDataHandler dataHandler = JsonUtility.FromJson<SaveDataHandler>(data);

        LevelEditorManager.RemoveEveryRegisterdObject();

        PlaceableObjectBase.ClearTile();

        foreach (var prefab in dataHandler.prefabDatas)
        {
            var placeableObject = AssetManager.GetPrefab(prefab.type).GetComponent<PlaceableObjectBase>().CreatePlaceableObject();            
            placeableObject.transform.position = prefab.position.GetValue();
            placeableObject.transform.eulerAngles = prefab.eulerAngles.GetValue();

            placeableObject.AsGround()?.AddToTile();

            PlaceableObjectBase.RegisterPlaceableObject(placeableObject);
        }
        LevelEditorManager.SetPlayMode(PlayMode.Editing);
        
        OnLoadComplete?.Invoke();
    }


    public event Action OnGreen;
    public event Action OnExitGreen;
    public event Action OnGameFinished;
    public void BallInTheCup()
    {
        OnGameFinished?.Invoke();
    }
    public void BallOnTheGreen()
    {
        OnGreen?.Invoke();
    }
    public void BallOutTheGreen()
    {
        OnExitGreen?.Invoke();
    }
}