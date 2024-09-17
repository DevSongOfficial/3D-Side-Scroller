using System;
using UnityEditor;
using UnityEngine;

public enum EditorMode
{
    None,
    Editing,
    Placing,
}

public class System_LevelEditorManager : MonoBehaviour
{
    [Header("Key Binding")]
    [SerializeField] private KeyCode switchMode = KeyCode.P;
    [SerializeField] private KeyCode reverseRotation = KeyCode.R;
    [SerializeField] private KeyCode selectObject = KeyCode.Mouse0;
    [SerializeField] private KeyCode placeObject = KeyCode.Mouse0;
    [SerializeField] private KeyCode removeObject = KeyCode.Mouse1;

    public LevelEditorUI UI => levelEditorUI;
    [SerializeField] private LevelEditorUI levelEditorUI;

    public Camera Camera => levelEditorCamera;
    [SerializeField] private Camera levelEditorCamera;

    // Current Editor Mode
    public EditorMode Mode { get; private set; }
    public bool IsEditorActive => Mode == EditorMode.None ? false : true; 

    public event Action<bool> OnEditorModeToggled;

    private void Awake()
    {
        PlaceableObject.OnObjectSelectedForPlacing += delegate { SetEditorMode(EditorMode.Placing); };
    }

    private void Start()
    {
        SetEditorMode(EditorMode.None);
    }


    // Main routine for the Level Editor
    private void Update()
    {
        SwitchMode();

        if (Mode != EditorMode.Placing && Mode != EditorMode.Editing) return;

        HandleObjectMovement();
        HandleObjectRotation();
        HandleObjectSelection();
        HandleObjectPlacement();
        HandleObjectRemovement();

        UI.MoveScreenDependingOnMousePosition(speed: 5);
    }

    public void SetEditorMode(EditorMode editorMode)
    {
        Mode = editorMode;
        OnEditorModeToggled.Invoke(IsEditorActive);

        // Switch Camera
        Camera.main.depth = IsEditorActive ? -1 : 0;
        Camera.gameObject.SetActive(IsEditorActive);

        switch (Mode)
        {
            case EditorMode.None:
                PlaceableObject.SelectCurrentObject(null);
                break;
            case EditorMode.Editing:
                PlaceableObject.SelectCurrentObject(null);
                break;
            case EditorMode.Placing:
                break;
        }
    }    

    private void SwitchMode()
    {
        if (Input.GetKeyDown(switchMode))
        {
            if (Mode == EditorMode.None) SetEditorMode(EditorMode.Editing);
            else if (Mode == EditorMode.Editing) SetEditorMode(EditorMode.None);
        }
    }

    private void PlaceSelectedObject()
    {
        if (PlaceableObject.CurrentlySelected is null) return;
        if (!PlaceableObject.CurrentlySelected.CanBePlaced) return;

        SetEditorMode(EditorMode.Editing);
    }

    private void RemoveSelectedObject()
    {
        if (PlaceableObject.CurrentlySelected is null) return;

        PlaceableObject.UnregisterPlaceableObject(PlaceableObject.CurrentlySelected);
        PlaceableObject.CurrentlySelected.SetActive(false);

        SetEditorMode(EditorMode.Editing);
    }

    private void HandleObjectPlacement()
    {
        if (Mode != EditorMode.Placing) return;
        if (UI.IsMouseCursorOnTheArea(UI.objectSelectionButtonsPanel.rectTransform)) return;

        if (Input.GetKeyDown(placeObject))
        {
            PlaceSelectedObject();
            movementOffset = Vector3.zero;
            return;
        }
    }

    private void HandleObjectRemovement()
    {
        if(PlaceableObject.CurrentlySelected is null) return;

        if (Input.GetKeyDown(removeObject))
        {
            RemoveSelectedObject();
        }
    }

    private void HandleObjectSelection()
    {
        if (Mode != EditorMode.Editing) return;
        
        if(Input.GetKeyDown(selectObject))
        {
            if(PlaceableObject.CurrentlySelected != null)
            {
                if (!PlaceableObject.CurrentlySelected.CanBePlaced) return;

                PlaceableObject.SelectCurrentObject(null);
                movementOffset = Vector3.zero;
                return;
            }

            if(!UI.TryGetComponentFromMousePosition(out PlaceableObject selectedObject, Layer.PlaceableObject))
            {
                 Debug.Log("Detected Object: NOTHING");
                 return;
            }
            else Debug.Log($"Detected Object: {selectedObject}");

            PlaceableObject.SelectCurrentObject(selectedObject);
            movementOffset = selectedObject.transform.position - UI.GetWorldPositionFromMousePosition(ignorePlaceableObjectLayer: false);
        }
    }

    private Vector3 movementOffset;
    private void HandleObjectMovement()
    {
        var obj = PlaceableObject.CurrentlySelected;

        if (obj is null) return;
        obj.transform.position = UI.GetWorldPositionFromMousePosition() + movementOffset;

        if (obj is GolfBall)
            obj.transform.position = new Vector3(obj.transform.position.x, obj.transform.position.y, GolfBall.FixedZPosition);
    }

    private void HandleObjectRotation()
    {
        if (PlaceableObject.CurrentlySelected is null) return;
        if (Mode != EditorMode.Placing && Mode != EditorMode.Editing) return;

        if (Input.GetKeyDown(reverseRotation))
        {
            PlaceableObject.CurrentlySelected.InverseRotation();
        }
    }
}