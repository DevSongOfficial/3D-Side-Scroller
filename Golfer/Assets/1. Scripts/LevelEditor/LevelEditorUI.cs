using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    // Screen Movement with Mouse Cursor Position
    public enum ScreenMovementDirection { Middle, Left, Right, Up, Down }
    private Dictionary<ScreenMovementDirection, Image> mouseCursorDetector = new Dictionary<ScreenMovementDirection, Image>(); // Assign this variable in inspector window 
    [SerializeField] private Image[] mouseCursorDetectorImages;

    // Object Selection Button
    private enum ObjectSelectionButtonType : Int16 { Cube = 0,  } // 기획 실수 방지용, 로직에 직접적으로 작용하지는 않음, 나중에 삭제.
    [SerializeField] private Image objectSelectionButtonsPanel;
    private ObjectSelectionButton[] objectSelectionButtons; // Buttons in LevelEditorUI

    private void Awake()
    {
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
            button.onClick.AddListener(delegate { prefab.OnSelectObjectForPlacing(); }); // TODO: 나중에 ObjectButton클래스에서 간단히 추가하는 함수 만들어서 대체
        }
    }

    private void Start()
    {
        mouseCursorDetector.Add(ScreenMovementDirection.Left, mouseCursorDetectorImages[0]);
        mouseCursorDetector.Add(ScreenMovementDirection.Right, mouseCursorDetectorImages[1]);
        mouseCursorDetector.Add(ScreenMovementDirection.Up, mouseCursorDetectorImages[2]);
        mouseCursorDetector.Add(ScreenMovementDirection.Down, mouseCursorDetectorImages[3]);
    }

    private void Update()
    {
        float speed = 5;

        switch(GetScreenMovementDirectionFromMousePosition())
        {
            case ScreenMovementDirection.Left:
                LevelEditorManager.Main.Camera.transform.position += new Vector3(- speed * Time.deltaTime, 0, 0);
                break;
            case ScreenMovementDirection.Right:
                LevelEditorManager.Main.Camera.transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
                break;
            case ScreenMovementDirection.Up:
                LevelEditorManager.Main.Camera.transform.position += new Vector3(0, speed * Time.deltaTime, 0);
                break;
            case ScreenMovementDirection.Down:
                LevelEditorManager.Main.Camera.transform.position += new Vector3(0, -speed * Time.deltaTime, 0);
                break;
        }
    }

    public ScreenMovementDirection GetScreenMovementDirectionFromMousePosition()
    {
        var direction = ScreenMovementDirection.Middle;
        if (IsMouseCursorOnTheArea(mouseCursorDetector[ScreenMovementDirection.Left].rectTransform))
        {
            return ScreenMovementDirection.Left;
        }
        if (IsMouseCursorOnTheArea(mouseCursorDetector[ScreenMovementDirection.Right].rectTransform))
        {
            return ScreenMovementDirection.Right;
        }
        if (IsMouseCursorOnTheArea(mouseCursorDetector[ScreenMovementDirection.Up].rectTransform))
        {
            return ScreenMovementDirection.Up;
        }
        if (IsMouseCursorOnTheArea(mouseCursorDetector[ScreenMovementDirection.Down].rectTransform))
        {
            return ScreenMovementDirection.Down;
        }


        return direction;
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
}
