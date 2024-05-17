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
    enum ObjectSelectionButtonType : Int16 { Cube = 0,  }

    private void Awake()
    {
        int count = Enum.GetValues(typeof(ObjectSelectionButtonType)).Length;
        ObjectSelectionButtons = new ObjectSelectionButton[count];
        if(count != ObjectSelectionButtonsPanel.transform.childCount) Debug.LogWarning("Enum Length and Button Counts don't match", transform);


        for (int i = 0; i < count; i++)
        {
            var button = ObjectSelectionButtons[i];
            button = ObjectSelectionButtonsPanel.transform.GetChild(i).GetComponent<ObjectSelectionButton>();
            var prefab = AssetsManager.GetAsset().Prefab_Cube;
            // AssetManager 수정 ! 후 아래 (1) (2) 여기다 구현
        }

        for (int i = 0; i < count; i++)
        {
            // (1) Initialize the button
           // ObjectSelectionButtons[i].placeableObject = PlaceableObject.Prefabs_PlaceableObject[i];

            // (2) Connect it with the event
            //ObjectSelectionButtons[i].onClick.AddListener(delegate { PlaceableObject.SelectObject(ObjectSelectionButtons[i].placeableObject); });
        }

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
        print(GetMousePositionFromTheCanvas());
    }
}
