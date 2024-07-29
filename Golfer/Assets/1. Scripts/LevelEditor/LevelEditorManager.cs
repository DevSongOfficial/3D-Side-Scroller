using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PlayerLoop;

public enum EditorMode
{
    None,
    Editing,
    Placing,
}

public class LevelEditorManager : MonoBehaviour
{
    #region Singleton - LevelEditorMain
    public static LevelEditorMain Main 
    { 
        get 
        {
            if(main == null) main = GameObject.Find("Level Editor").AddComponent<LevelEditorMain>();
            return main;
        } 
    }
    private static LevelEditorMain main;
    #endregion

    public static EditorMode Mode { get; private set; }

    private void Awake()
    {
        #region Singleton - LevelEditorMain
        main = GetComponent<LevelEditorMain>();
        #endregion

        PlaceableObject.Initialization();
        PlaceableObject.OnObjectSelectedForPlacing += delegate { SetEditorMode(EditorMode.Placing); };

        SetEditorMode(EditorMode.None);
    }


    // Main routine for the Level Editor
    private void Update()
    {
        HandleObjectMovement();
        HandleObjectSelection();
        HandleObjectPlacement();
        HandleObjectRemovement();

        Main.UI.MoveScreenDependingOnMousePosition(speed: 5);
    }

    public static void SetEditorMode(EditorMode editorMode)
    {
        Debug.Log(editorMode);

        Mode = editorMode;

        switch(Mode)
        {
            case EditorMode.None:
                Camera.main.depth = 0;
                Main.UI.gameObject.SetActive(false);
                //Main.LevelEditorUI.ObjectSelectionButtonsPanel.gameObject.SetActive(true);
                PlaceableObject.SetConvexAll(false);
                PlaceableObject.SetCollisionAll(true);
                PlaceableObject.SetCurrentObjectTo(null);
                break;
            case EditorMode.Editing:
                Camera.main.depth = -1;
                Main.UI.gameObject.SetActive(true);
                //Main.LevelEditorUI.ObjectSelectionButtonsPanel.gameObject.SetActive(true);
                PlaceableObject.SetConvexAll(true);
                PlaceableObject.SetCollisionAll(false);
                PlaceableObject.SetCurrentObjectTo(null);
                break;
            case EditorMode.Placing:
                Camera.main.depth = -1;
                Main.UI.gameObject.SetActive(true);
                //Main.LevelEditorUI.ObjectSelectionButtonsPanel.gameObject.SetActive(false);
                PlaceableObject.SetConvexAll(true);
                PlaceableObject.SetCollisionAll(false);
                break;
        }
    }    

    private void PlaceSelectedObject()
    {
        if (PlaceableObject.Current == null) return;
        if (!PlaceableObject.Current.CanBePlaced) return;

        SetEditorMode(EditorMode.Editing);
    }

    private void RemoveSelectedObject()
    {
        if (PlaceableObject.Current == null) return;

        PlaceableObject.UnregisterPlaceableObject(PlaceableObject.Current);
        PlaceableObject.Current.SetActive(false);

        SetEditorMode(EditorMode.Editing);
    }

    private void HandleObjectPlacement()
    {
        if (Mode != EditorMode.Placing) return;
        if (Main.UI.IsMouseCursorOnTheArea(Main.UI.objectSelectionButtonsPanel.rectTransform)) return;

        if (Input.GetMouseButtonDown(0))
        {
            PlaceSelectedObject();
            movementOffset = Vector3.zero;
            return;
        }
    }

    private void HandleObjectRemovement()
    {
        if(PlaceableObject.Current == null) return;
        if(Mode != EditorMode.Placing && Mode != EditorMode.Editing) return;

        if(Input.GetMouseButtonDown(1))
        {
            RemoveSelectedObject();
        }
    }

    private void HandleObjectSelection()
    {
        if (Mode != EditorMode.Editing) return;
        
        if(Input.GetMouseButtonDown(0))
        {
            if(PlaceableObject.Current != null)
            {
                if (!PlaceableObject.Current.CanBePlaced) return;

                PlaceableObject.SetCurrentObjectTo(null);
                movementOffset = Vector3.zero;
                return;
            }

            var selectedObject = GetPlaceableObjectFromMousePosition();
            if (selectedObject == null)
            {
                 Debug.Log("Detected Object: NOTHING");
                 return;
            }
            else Debug.Log($"Detected Object: {selectedObject}");

            PlaceableObject.SetCurrentObjectTo(selectedObject);
            movementOffset = selectedObject.transform.position - Main.UI.GetWorldPositionFromMousePosition(ignorePlaceableObjectLayer: false);
        }
    }

    private Vector3 movementOffset;
    private void HandleObjectMovement()
    {
        if (PlaceableObject.Current == null) return;
        PlaceableObject.Current.transform.position = Main.UI.GetWorldPositionFromMousePosition() + movementOffset;
    }

    private PlaceableObject GetPlaceableObjectFromMousePosition()
    {
        PlaceableObject placeableObject = null;
        Ray ray = Main.Camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, 1 << (int)Layer.PlaceableObject))
        {
            placeableObject = raycastHit.collider.GetComponent<PlaceableObject>();
        }

        return placeableObject;
    }
}
