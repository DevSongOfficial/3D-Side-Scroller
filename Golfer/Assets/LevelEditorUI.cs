using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    [SerializeField] private Image ObjectSelectionButtonsPanel;

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

            // Initialize the button and connect it with the event
            button.SetPlaceableObject(prefab);
            button.onClick.AddListener(delegate { prefab.SelectObject(); });
        }
    }

    private void Start()
    {
        Debug.Log(ObjectSelectionButtons[0].placeableObject);
    }

    // Get mouse position depending on the resolution of the screen
    public Vector3 GetMousePositionFromTheCanvas()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                Input.mousePosition.y, -Camera.main.transform.position.z));

        return position;
    }

    private void Update()
    {
        //print(GetMousePositionFromTheCanvas());
    }
}
