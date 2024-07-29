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
    private enum ObjectSelectionButtonType : Int16 { Cube = 0,  } // 기획 실수 방지용, 로직에 직접적으로 작용하지는 않음, 나중에 삭제.
    public Image objectSelectionButtonsPanel;
    private ObjectSelectionButton[] objectSelectionButtons; // Buttons in LevelEditorUI

    private void Awake()
    {
        mouseCursorDetector.Add(MouseSectionType.Left, mouseCursorDetectorImages[0]);
        mouseCursorDetector.Add(MouseSectionType.Right, mouseCursorDetectorImages[1]);
        mouseCursorDetector.Add(MouseSectionType.Up, mouseCursorDetectorImages[2]);
        mouseCursorDetector.Add(MouseSectionType.Down, mouseCursorDetectorImages[3]);

        int count = Enum.GetValues(typeof(ObjectSelectionButtonType)).Length;
        objectSelectionButtons = new ObjectSelectionButton[count];

        if(count != objectSelectionButtonsPanel.transform.childCount) Debug.LogWarning("Enum Length and Button Counts don't match", transform);

        // Buttons Setting
        for (int i = 0; i < count; i++)
        {
            var button = objectSelectionButtonsPanel.transform.GetChild(i).GetComponent<ObjectSelectionButton>();
            objectSelectionButtons[i] = button;
            var prefab = AssetsManager.GetPrefab(AssetsManager.PrefabType.Cube).GetComponent<PlaceableObject>();

            // (1) Initialize the button
            button.SetConnectedPlaceableObject(prefab);

            // (2) connect it with the event
            button.onClick.AddListener(delegate { prefab.OnSelectObjectForPlacing(); }); // todo: 나중에 ObjectButton클래스에서 간단히 추가하는 함수 만들어서 대체
        }
    }


    public void MoveScreenDependingOnMousePosition(int speed)
    {
        if (LevelEditorManager.Mode == EditorMode.None) return;

        switch (GetScreenMovementDirectionFromMousePosition())
        {
            case MouseSectionType.Left:
                LevelEditorManager.Main.Camera.transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
                break;
            case MouseSectionType.Right:
                LevelEditorManager.Main.Camera.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
                break;
            case MouseSectionType.Up:
                LevelEditorManager.Main.Camera.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
                break;
            case MouseSectionType.Down:
                LevelEditorManager.Main.Camera.transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
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

    public Vector3 GetWorldPositionFromMousePosition(bool ignorePlaceableObjectLayer = true)
    {
        var position = Vector3.zero;
        Ray ray = LevelEditorManager.Main.Camera.ScreenPointToRay(Input.mousePosition);

        if (!ignorePlaceableObjectLayer)
        {
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue))
            {
                position = new Vector3(hit.point.x, hit.point.y, 0);
                return position;
            }
        }

        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, 1 << (int)Layer.RaycastBlockerForLevelEditor))
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
