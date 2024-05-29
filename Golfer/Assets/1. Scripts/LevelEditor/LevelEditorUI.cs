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
    [SerializeField] private Dictionary<ScreenMovementDirection, Image> mouseCursorDetector; // Assign this variable in inspector window

    // Object Selection Button
    private enum ObjectSelectionButtonType : Int16 { Cube = 0,  } // ��ȹ �Ǽ� ������, ������ ���������� �ۿ������� ����, ���߿� ����.
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
            button.onClick.AddListener(delegate { prefab.OnSelectObjectForPlacing(); }); // TODO: ���߿� ObjectButtonŬ�������� ������ �߰��ϴ� �Լ� ���� ��ü
        }
    }

    private void Update()
    {
        if(GetScreenMovementDirectionFromMousePosition() == ScreenMovementDirection.Left)
        {
            Debug.Log("Left");
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
