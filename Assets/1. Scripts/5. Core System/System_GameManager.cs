using Cinemachine;
using System;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameSystem;

public enum Layer
{
    Default = 0,
    Character = 8,
    Ground = 9,
    Wall = 10,
    Placeable = 20,
    Damageable = 24,
    Interactable = 25,
    RaycastBlockerForLevelEditor = 31,
}

public enum Tag
{
    Untagged, Player, Enemy, Prob
}

public class System_GameManager : MonoBehaviour
{

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        LevelEditorManager.OnEditorModeToggled += (bool enable) => ToggleInput(!enable);
    }

    private void Start()
    {
        if (loadGameOnStart) LoadGame();
    }

    [Header("Main Camera & Cinemachine")]
    [SerializeField] private CinemachineBrain cinemachineBrain;
    public void SetCameraUpdateMethod(CinemachineBrain.UpdateMethod method)
    {
        cinemachineBrain.m_UpdateMethod = method;
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
        var data = SaveManager.LoadData();
        if (String.IsNullOrEmpty(data)) return;

        SaveDataHandler dataHandler = JsonUtility.FromJson<SaveDataHandler>(data);

        LevelEditorManager.RemoveEveryRegisterdObject();

        foreach (var prefab in dataHandler.prefabDatas)
        {
            var placeableObject = Instantiate(AssetManager.GetPrefab(prefab.type).GetComponent<PlaceableObjectBase>());
            placeableObject.SetType(prefab.type);
            placeableObject.transform.position = prefab.position.GetValue();
            placeableObject.transform.eulerAngles = prefab.eulerAngles.GetValue();

            PlaceableObjectBase.RegisterPlaceableObject(placeableObject);

            LevelEditorManager.SetPlayMode(PlayMode.Editing);
        }
    }
}