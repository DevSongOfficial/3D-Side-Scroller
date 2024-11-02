using System;
using System.Collections.Generic;
using UnityEngine;

public enum PlayMode
{
    Playing,
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
    public PlayMode Mode { get; private set; }
    public bool IsEditorActive => Mode == PlayMode.Playing ? false : true; 

    public event Action<bool> OnEditorModeToggled;

    private void Awake()
    {
        PlaceableObjectBase.OnObjectSelectedForPlacing += delegate { SetPlayMode(PlayMode.Placing); };
    }

    private void Start()
    {
        SetPlayMode(PlayMode.Playing);
    }


    // Main routine for the Level Editor
    private void Update()
    {
        SwitchMode();

        if (Mode != PlayMode.Placing && Mode != PlayMode.Editing) return;

        HandleObjectMovement();
        HandleObjectRotation();
        HandleObjectSelection();
        HandleObjectPlacement();
        HandleObjectRemovement();

        UI.MoveScreenDependingOnMousePosition(speed: 5);
    }

    public void SetPlayMode(PlayMode editorMode)
    {
        Mode = editorMode;
        OnEditorModeToggled.Invoke(IsEditorActive);

        // Switch Camera
        Camera.main.depth = IsEditorActive ? -1 : 0;
        Camera.gameObject.SetActive(IsEditorActive);

        switch (Mode)
        {
            case PlayMode.Playing:
                PlaceableObjectBase.SelectCurrentObject(null);
                break;
            case PlayMode.Editing:
                PlaceableObjectBase.SelectCurrentObject(null);
                break;
            case PlayMode.Placing:
                break;
        }
    }    

    private void SwitchMode()
    {
        if (Input.GetKeyDown(switchMode))
        {
            if (Mode == PlayMode.Playing) SetPlayMode(PlayMode.Editing);
            else if (Mode == PlayMode.Editing) SetPlayMode(PlayMode.Playing);
        }
    }

    private void PlaceSelectedObject()
    {
        if (PlaceableObjectBase.CurrentlySelected is null) return;
        if (!PlaceableObjectBase.CurrentlySelected.CanBePlaced) return;
        SetPlayMode(PlayMode.Editing);
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
        if(PlaceableObjectBase.CurrentlySelected is null) return;

        if (Input.GetKeyDown(removeObject))
        {
            RemoveSelectedObject();
        }
    }

    private void HandleObjectSelection()
    {
        if (Mode != PlayMode.Editing) return;
        
        if(Input.GetKeyDown(selectObject))
        {
            if(PlaceableObjectBase.CurrentlySelected != null)
            {
                if (!PlaceableObjectBase.CurrentlySelected.CanBePlaced) return;

                PlaceableObjectBase.SelectCurrentObject(null);
                movementOffset = Vector3.zero;
                return;
            }

            if(!UI.TryGetComponentFromMousePosition(out PlaceableObjectBase selectedObject, Layer.Placeable))
            {
                 //Debug.Log("Detected Object: NOTHING");
                 return;
            }
            else //.Log($"Detected Object: {selectedObject}");

            PlaceableObjectBase.SelectCurrentObject(selectedObject);
            movementOffset = selectedObject.transform.position - UI.GetWorldPositionFromMousePosition(ignorePlaceableObjectLayer: false);
        }
    }

    private Vector3 movementOffset;
    private void HandleObjectMovement()
    {
        var obj = PlaceableObjectBase.CurrentlySelected;

        if (obj is null) return;

        var currentPosition = UI.GetWorldPositionFromMousePosition() + movementOffset;

        if (obj is PlaceableProb)
        {
            obj.transform.position = new Vector3(currentPosition.x, currentPosition.y, (obj as PlaceableProb).ZPostion);
            return;
        }

        if (obj is PlaceableGround)
        {
            obj.transform.position = new Vector3(Mathf.Round(currentPosition.x), Mathf.Round(currentPosition.y), currentPosition.z);
            return;
        }

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