using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    [SerializeField] private KeyCode moveScreen = KeyCode.Mouse2;
    [SerializeField] private KeyCode fastMove = KeyCode.LeftShift;
    [SerializeField] private KeyCode toggleVerticalCamera = KeyCode.P;

    [Header("Editor Camera")]
    [SerializeField] private Camera levelEditorCamera;
    public Camera Camera => levelEditorCamera;
    [SerializeField] private TMP_Text text_levelEditorCameraCoordinates;
    [SerializeField] private Camera levelEditorCamera_vertical;
    [SerializeField] private RawImage rawImage_verticalCamera;

    // Current Editor Mode
    public PlayMode Mode { get; private set; }
    public bool IsEditorActive => Mode == PlayMode.Playing ? false : true; 

    public event Action<bool> OnEditorModeToggled;

    private void Start()
    {
        if (!IsMakerScene) return;

        SetPlayMode(PlayMode.Editing);
    }

    // Main routine for the Level Editor
    private void Update()
    {
        if (!IsMakerScene) return;

        HandleModeSwitch();

        if (Mode != PlayMode.Placing && Mode != PlayMode.Editing) return;

        HandleObjectMovement();
        HandleObjectRotation();
        HandleObjectSelection();
        HandleObjectPlacement();
        HandleObjectRemovement();

        HandleScreenMovementDependingOnMousePosition(speed: 5, multiplier: 3);
        HandleVerticalCameraActivation();
    }

    private void LateUpdate()
    {
        HandleScreenPositionText();
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
        rawImage_verticalCamera.gameObject.SetActive(IsEditorActive);

        GameManager.Player.gameObject.SetActive(!IsEditorActive);
        GameManager.GetReferenceToSingletonObjects();

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

    private bool PlaceSelectedObject()
    {
        if (PlaceableObjectBase.CurrentlySelected is null) return false;
        if (!PlaceableObjectBase.CurrentlySelected.NotOverlapped) return false;

        // Add the object to the Tile.
        PlaceableObjectBase.CurrentlySelected.AsGround()?.AddToTile();

        // Place the object.
        PlaceableObjectBase.SetPreviouslyPlacedObject(PlaceableObjectBase.CurrentlySelected);
        SetPlayMode(PlayMode.Editing);

        if (Input.GetKey(placeInARow))
        {
            var po = AssetManager.GetPrefab(PlaceableObjectBase.PreviouslyPlaced.Type).GetComponent<PlaceableObjectBase>();
            po.OnCreatedFromButton();
        }

        return true;
    }

    private void RemoveSelectedObject()
    {
        if (PlaceableObjectBase.CurrentlySelected is null) return;

        POFactory.RemovePO(PlaceableObjectBase.CurrentlySelected);

        SetPlayMode(PlayMode.Editing);
    }

    private void HandleObjectPlacement()
    {
        if (Mode != PlayMode.Placing) return;
        if (UI.IsMouseCursorOnTheArea(UI.ObjectSelectionGroup)) return;
        if (Input.GetKeyDown(placeObject))
        {
            if (!PlaceSelectedObject()) return;

            movementOffset = Vector3.zero;
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
        if (!Input.GetKeyDown(selectObject)) return;

        // Place if holding an object.
        if (PlaceableObjectBase.CurrentlySelected != null)
        {
            if (!PlaceableObjectBase.CurrentlySelected.NotOverlapped) return;

            // Add the object to the Tile.
            PlaceableObjectBase.CurrentlySelected.AsGround()?.AddToTile();

            PlaceableObjectBase.SetCurrentObject(null);
            movementOffset = Vector3.zero;

            return;
        }

        // If not, select the object on mouse cursor.
        if (!UI.TryGetComponentFromMousePosition(out PlaceableObjectBase selectedObject, Layer.Placeable))
        {
            //Debug.Log("Detected Object: NOTHING");
            return;
        }
        else //.Log($"Detected Object: {selectedObject}");

        PlaceableObjectBase.SetCurrentObject(selectedObject);
        movementOffset = selectedObject.transform.position - UI.GetWorldPositionFromMousePosition(ignorePlaceableObjectLayer: false);

        selectedObject.AsGround()?.RemoveFromTile();
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

    private void HandleScreenMovementDependingOnMousePosition(float speed, float multiplier = 1)
    {
        if (!Input.GetKey(moveScreen)) return;
        if (Mode == PlayMode.Playing) return;

        var delta = new Vector3(UI.GetMouseArea().x, UI.GetMouseArea().y, 0);
        delta *= Input.GetKey(fastMove) ? speed * multiplier : speed;
        Camera.transform.position += delta * Time.deltaTime;
    }

    private void HandleScreenPositionText()
    {
        text_levelEditorCameraCoordinates.text = $"({Mathf.Round(Camera.transform.position.x)}, {Mathf.Round(Camera.transform.position.y)})";
    }
}