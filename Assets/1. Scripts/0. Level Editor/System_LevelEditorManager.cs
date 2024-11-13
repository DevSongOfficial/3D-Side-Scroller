using System;
using UnityEngine;
using UnityEngine.UI;
using static GameSystem;

public enum PlayMode
{
    Playing,
    Editing,
    Placing,
}

public sealed class System_LevelEditorManager : MonoBehaviour
{
    public LevelEditorUI UI => levelEditorUI;
    [SerializeField] private LevelEditorUI levelEditorUI;

    [Header("Key Binding")]
    [SerializeField] private KeyCode switchMode = KeyCode.Tab;
    [SerializeField] private KeyCode reverseRotation = KeyCode.R;
    [SerializeField] private KeyCode selectObject = KeyCode.Mouse0;
    [SerializeField] private KeyCode placeObject = KeyCode.Mouse0;
    [SerializeField] private KeyCode placeInARow = KeyCode.Space;
    [SerializeField] private KeyCode removeObject = KeyCode.Mouse1;
    [SerializeField] private KeyCode toggleVerticalCamera = KeyCode.P;

    [Header("Editor Camera")]
    [SerializeField] private Camera levelEditorCamera;
    public Camera Camera => levelEditorCamera;
    [SerializeField] private Camera levelEditorCamera_vertical;
    [SerializeField] private RawImage rawImage_verticalCamera;

    // Current Editor Mode
    public PlayMode Mode { get; private set; }
    public bool IsEditorActive => Mode == PlayMode.Playing ? false : true; 

    public event Action<bool> OnEditorModeToggled;

    private void Awake()
    {
        PlaceableObjectBase.OnObjectCreatedFromButton += delegate { SetPlayMode(PlayMode.Placing); };
    }

    private void Start()
    {
        SetPlayMode(PlayMode.Playing);
    }

    // Main routine for the Level Editor
    private void Update()
    {
        HandleModeSwitch();

        if (Mode != PlayMode.Placing && Mode != PlayMode.Editing) return;

        HandleObjectMovement();
        HandleObjectRotation();
        HandleObjectSelection();
        HandleObjectPlacement();
        HandleObjectRemovement();
        HandleVerticalCameraActivation();

        UI.MoveScreenDependingOnMousePosition(speed: 5);
    }

    public void SetPlayMode(PlayMode editorMode)
    {
        Mode = editorMode;
        OnEditorModeToggled.Invoke(IsEditorActive);

        // Switch camera.
        Camera.main.depth = IsEditorActive ? -1 : 0;
        Camera.gameObject.SetActive(IsEditorActive);

        // Set main canvas activation.
        UIManager.gameObject.SetActive(!IsEditorActive);

        switch (Mode)
        {
            case PlayMode.Playing:
                PlaceableObjectBase.SetCurrentObject(null);
                break;
            case PlayMode.Editing:
                PlaceableObjectBase.SetCurrentObject(null);
                break;
            case PlayMode.Placing:
                break;
        }
    }    

    private void HandleModeSwitch()
    {
        if (Input.GetKeyDown(switchMode))
        {
            if (Mode == PlayMode.Playing) SetPlayMode(PlayMode.Editing);
            else if (Mode == PlayMode.Editing) SetPlayMode(PlayMode.Playing);
        }
    }

    private void HandleVerticalCameraActivation()
    {
        if (Input.GetKeyDown(toggleVerticalCamera))
        {
            levelEditorCamera_vertical.gameObject.SetActive(!levelEditorCamera_vertical.gameObject.activeSelf);
            rawImage_verticalCamera.gameObject.SetActive(!rawImage_verticalCamera.gameObject.activeSelf);
        }
    }

    private void PlaceSelectedObject()
    {
        if (PlaceableObjectBase.CurrentlySelected is null) return;
        if (!PlaceableObjectBase.CurrentlySelected.CanBePlaced) return;

        PlaceableObjectBase.SetPreviouslyPlacedObject(PlaceableObjectBase.CurrentlySelected);
        SetPlayMode(PlayMode.Editing);

        if (Input.GetKey(placeInARow))
        {
            var po = AssetManager.GetPrefab(PlaceableObjectBase.PreviouslyPlaced.Type).GetComponent<PlaceableObjectBase>();
            po.CreateIfSelectedPleaceableObject();
        }
    }

    private void RemoveSelectedObject()
    {
        if (PlaceableObjectBase.CurrentlySelected is null) return;

        RemovePlaceableObject(PlaceableObjectBase.CurrentlySelected);

        SetPlayMode(PlayMode.Editing);
    }

    public void RemovePlaceableObject(PlaceableObjectBase po)
    {
        PlaceableObjectBase.UnregisterPlaceableObject(po);
        po.SetActive(false);
    }

    public void RemoveEveryRegisterdObject()
    {
        for(int i = PlaceableObjectBase.PlaceableObjectsInTheScene.Count - 1; i >= 0; i--)
        {
            RemovePlaceableObject(PlaceableObjectBase.PlaceableObjectsInTheScene[i]);
        }
    }

    private void HandleObjectPlacement()
    {
        if (Mode != PlayMode.Placing) return;
        if (UI.IsMouseCursorOnTheArea(UI.ObjectSelectionGroup)) return;
        if (Input.GetKeyDown(placeObject))
        { 
            PlaceSelectedObject();
            movementOffset = Vector3.zero;
            return;
        }
    }

    private void HandleObjectRemovement()
    {
        if(PlaceableObjectBase.CurrentlySelected is null) return;

        if (Input.GetKeyDown(removeObject))
        {
            RemoveSelectedObject();
        }
    }

    private void HandleObjectSelection()
    {
        if (Mode != PlayMode.Editing) return;

        if (Input.GetKeyDown(selectObject))
        {
            if(PlaceableObjectBase.CurrentlySelected != null)
            {
                if (!PlaceableObjectBase.CurrentlySelected.CanBePlaced) return;

                PlaceableObjectBase.SetCurrentObject(null);
                movementOffset = Vector3.zero;
                return;
            }

            if(!UI.TryGetComponentFromMousePosition(out PlaceableObjectBase selectedObject, Layer.Placeable))
            {
                 //Debug.Log("Detected Object: NOTHING");
                 return;
            }
            else //.Log($"Detected Object: {selectedObject}");

            PlaceableObjectBase.SetCurrentObject(selectedObject);
            movementOffset = selectedObject.transform.position - UI.GetWorldPositionFromMousePosition(ignorePlaceableObjectLayer: false);
        }
    }

    private Vector3 movementOffset;
    private void HandleObjectMovement()
    {
        var obj = PlaceableObjectBase.CurrentlySelected;
        if (obj is null) return;

        var currentPosition = UI.GetWorldPositionFromMousePosition() + movementOffset;

        if (obj is PlaceableGround)
        {
            obj.transform.position = new Vector3(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y), (obj as PlaceableProb).ZPostion);
            return;
        }

        if (obj is PlaceableProb)
        {
            obj.transform.position = new Vector3(currentPosition.x, currentPosition.y, (obj as PlaceableProb).ZPostion);
            return;
        }

        // If (obj is PlaceableCharacter)
        obj.transform.position = currentPosition;
    }

    private void HandleObjectRotation()
    {
        if (PlaceableObjectBase.CurrentlySelected is null) return;
        if (Mode != PlayMode.Placing && Mode != PlayMode.Editing) return;

        if (Input.GetKeyDown(reverseRotation))
        {
            PlaceableObjectBase.CurrentlySelected.InverseRotation();
        }
    }
}