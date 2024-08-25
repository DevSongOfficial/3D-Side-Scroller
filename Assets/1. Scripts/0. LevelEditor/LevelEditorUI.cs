using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    // Screen Movement with Mouse Cursor Position
    public enum MouseSectionType { Middle, Left, Right, Up, Down }
    private Dictionary<MouseSectionType, Image> mouseCursorDetector = new Dictionary<MouseSectionType, Image>(); // Assign this variable in inspector window 
    [SerializeField] private Image[] mouseCursorDetectorImages;

    // Object Selection Button
    public Image objectSelectionButtonsPanel;
    private ObjectSelectionButton[] objectSelectionButtons; // Buttons in LevelEditorUI

    private void Awake()
    {
        LevelEditorManager.OnEditorModeTriggered += (bool active) => gameObject.SetActive(active);

        mouseCursorDetector.Add(MouseSectionType.Left, mouseCursorDetectorImages[0]);
        mouseCursorDetector.Add(MouseSectionType.Right, mouseCursorDetectorImages[1]);
        mouseCursorDetector.Add(MouseSectionType.Up, mouseCursorDetectorImages[2]);
        mouseCursorDetector.Add(MouseSectionType.Down, mouseCursorDetectorImages[3]);

        int count = Enum.GetValues(typeof(AssetsManager.PrefabType)).Length;
        objectSelectionButtons = new ObjectSelectionButton[count];

        if(count != objectSelectionButtonsPanel.transform.childCount) Debug.LogWarning("Enum Length and Button Counts don't match", transform);

        // Buttons Setting
        for (int i = 0; i < count; i++)
        {
            var button = objectSelectionButtonsPanel.transform.GetChild(i).GetComponent<ObjectSelectionButton>();
            objectSelectionButtons[i] = button;
            var prefab = AssetsManager.GetPrefab((AssetsManager.PrefabType)i).GetComponent<PlaceableObject>();

            // (1) Initialize the button
            button.SetConnectedPlaceableObject(prefab);

            // (2) connect it with the event
            button.onClick.AddListener(delegate { prefab.OnSelectObjectWhenPlacing(); }); // todo: ���߿� ObjectButtonŬ�������� ������ �߰��ϴ� �Լ� ���� ��ü
        }
    }


    public void MoveScreenDependingOnMousePosition(int speed)
    {
        if (LevelEditorManager.Mode == EditorMode.None) return;

        switch (GetScreenMovementDirectionFromMousePosition())
        {
            case MouseSectionType.Left:
                LevelEditorManager.GetReferenceTo.EditorCamera.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
                break;
            case MouseSectionType.Right:
                LevelEditorManager.GetReferenceTo.EditorCamera.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
                break;
            case MouseSectionType.Up:
                LevelEditorManager.GetReferenceTo.EditorCamera.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
                break;
            case MouseSectionType.Down:
                LevelEditorManager.GetReferenceTo.EditorCamera.transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
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

        var camera = cameraForRay ?? LevelEditorManager.GetReferenceTo.EditorCamera;
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

        var camera = cameraForRay ?? LevelEditorManager.GetReferenceTo.EditorCamera;
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
        Ray ray = LevelEditorManager.GetReferenceTo.EditorCamera.ScreenPointToRay(Input.mousePosition);

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