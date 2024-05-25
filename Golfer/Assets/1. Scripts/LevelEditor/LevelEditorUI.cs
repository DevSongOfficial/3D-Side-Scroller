using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    public Image ObjectSelectionButtonsPanel;

    private ObjectSelectionButton[] ObjectSelectionButtons; // Buttons in LevelEditorUI
    private enum ObjectSelectionButtonType : Int16 { Cube = 0,  } // 기획 실수 방지용, 로직에 직접적으로 작용하지는 않음.

    private void Awake()
    {
        int count = Enum.GetValues(typeof(ObjectSelectionButtonType)).Length;
        ObjectSelectionButtons = new ObjectSelectionButton[count];

        if(count != ObjectSelectionButtonsPanel.transform.childCount) Debug.LogWarning("Enum Length and Button Counts don't match", transform);

        // Buttons Setting
        for (int i = 0; i < count; i++)
        {
            var button = ObjectSelectionButtonsPanel.transform.GetChild(i).GetComponent<ObjectSelectionButton>();
            ObjectSelectionButtons[i] = button;
            var prefab = AssetsManager.GetPrefab(AssetsManager.PrefabType.Cube).GetComponent<PlaceableObject>();

            // (1) Initialize the button
            button.SetConnectedPlaceableObject(prefab);

            // (2) connect it with the event
            button.onClick.AddListener(delegate { prefab.OnSelectObject(); }); // TODO: 나중에 ObjectButton클래스에서 간단히 추가하는 함수 만들어서 대체
        }
    }


    // Get mouse position depending on the resolution of the screen
    public Vector3 GetMousePositionFromTheCanvas()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -Camera.main.transform.position.z));

        return position;
    }
}
