using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static GameSystem;

public class LevelEditorUI : MonoBehaviour
{
    // Screen Movement with Mouse Cursor Position
    public enum MouseSectionType { Middle, Left, Right, Up, Down }
    private Dictionary<MouseSectionType, Image> mouseCursorDetector = new Dictionary<MouseSectionType, Image>(); // Assign this variable in inspector window 
    [SerializeField] private Image[] mouseCursorDetectorImages;

    // Object Selection Button
    public Image objectSelectionButtonsPanel;
    private ObjectSelectionButton[] objectSelectionButtons; // Buttons in LevelEditorUI
    [SerializeField] private GameObject objectSelectionGroup;

    private void Awake()
    {
        LevelEditorManager.OnEditorModeToggled += (bool isOn) => objectSelectionGroup.SetActive(isOn);

        mouseCursorDetector.Add(MouseSectionType.Left, mouseCursorDetectorImages[0]);
        mouseCursorDetector.Add(MouseSectionType.Right, mouseCursorDetectorImages[1]);
        mouseCursorDetector.Add(MouseSectionType.Up, mouseCursorDetectorImages[2]);
        mouseCursorDetector.Add(MouseSectionType.Down, mouseCursorDetectorImages[3]);

        // Creates buttons as many as the number of general prefabs.
        #region Set up buttons
        int count = Enum.GetValues(typeof(Prefab.General)).Length;
        objectSelectionButtons = new ObjectSelectionButton[count];
        // Buttons Settings
        for (int i = 0; i < count; i++)
        {
            var button = Instantiate(AssetManager.GetPrefab(Prefab.UI.SpawnButton_1), objectSelectionButtonsPanel.transform).
                GetComponent<ObjectSelectionButton>();

            var prefab = AssetManager.GetPrefab((Prefab.General)i).GetComponent<PlaceableObjectBase>();
            prefab.SetType((Prefab.General)i);

            button.Initialize(prefab);
            objectSelectionButtons[i] = button;
        }
        #endregion
    }


    public void MoveScreenDependingOnMousePosition(int speed)
    {
        if (LevelEditorManager.Mode == PlayMode.Playing) return;

        switch (GetScreenMovementDirectionFromMousePosition())
        {
            case MouseSectionType.Left:
                LevelEditorManager.Camera.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
                break;
            case MouseSectionType.Right:
                LevelEditorManager.Camera.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
                break;
            case MouseSectionType.Up:
                LevelEditorManager.Camera.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
                break;
            case MouseSectionType.Down:
                LevelEditorManager.Camera.transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
                break;
        }
    }

    // Get mouse position depending on the resolution of the screen
    public Vector3 GetMousePositionFromTheCanvas()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -Camera.main.transform.position.z));

        return position;
    }

    public bool IsMouseCursorOnTheArea(RectTransform area)
    {
        var cursorPosition = Input.mousePosition;
        if (cursorPosition.x >= area.position.x - area.rect.width* 0.5f &&
            cursorPosition.x <= area.position.x + area.rect.width* 0.5f &&
            cursorPosition.y >= area.position.y - area.rect.height* 0.5f &&
            cursorPosition.y <= area.position.y + area.rect.height* 0.5f)
        {
            return true;
        }

        return false;
    }

    public bool TryGetComponentFromMousePosition<T>(out T component, Layer layerMask /* For Optimization */, Camera cameraForRay = null) where T : Component
    {
        component = null;

        var camera = cameraForRay ?? LevelEditorManager.Camera;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask.GetMask()))
        {
            if(raycastHit.collider.TryGetComponent(out T _component))
            {
                component = _component;
            }
        }

        return component != null;
    }

    public bool TryGetComponentFromMousePosition<T>(out T component, Camera cameraForRay = null) where T : Component
    {
        component = null;

        var camera = cameraForRay ?? LevelEditorManager.Camera;
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue))
        {
            if (raycastHit.collider.TryGetComponent(out T _component))
            {
                component = _component;
            }
        }

        return component != null;
    }

    public Vector3 GetWorldPositionFromMousePosition(bool ignorePlaceableObjectLayer = true)
    {
        var position = Vector3.zero;
        Ray ray = LevelEditorManager.Camera.ScreenPointToRay(Input.mousePosition);

        if (!ignorePlaceableObjectLayer)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                position = new Vector3(hit.point.x, hit.point.y, 0);
                return position;
            }
        }

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, Layer.RaycastBlockerForLevelEditor.GetMask()))
        {
            position = new Vector3(raycastHit.point.x, raycastHit.point.y, 0);
        }
        return position;
    }

    public MouseSectionType GetScreenMovementDirectionFromMousePosition()
    {
        var direction = MouseSectionType.Middle;
        if (IsMouseCursorOnTheArea(mouseCursorDetector[MouseSectionType.Left].rectTransform))
        {
            return MouseSectionType.Left;
        }
        if (IsMouseCursorOnTheArea(mouseCursorDetector[MouseSectionType.Right].rectTransform))
        {
            return MouseSectionType.Right;
        }
        if (IsMouseCursorOnTheArea(mouseCursorDetector[MouseSectionType.Up].rectTransform))
        {
            return MouseSectionType.Up;
        }
        if (IsMouseCursorOnTheArea(mouseCursorDetector[MouseSectionType.Down].rectTransform))
        {
            return MouseSectionType.Down;
        }


        return direction;
    }    
}
