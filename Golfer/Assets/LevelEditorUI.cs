using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEditorUI : MonoBehaviour
{
    public ObjectSelectionButton[] ObjectSelectionButtons;

    private void Awake()
    {
        for (int i = 0; i < PlaceableObject.Prefabs_PlaceableObject.Length; i++)
        {
            // (1) 버튼 초기화
            ObjectSelectionButtons[i].placeableObject = PlaceableObject.Prefabs_PlaceableObject[i];

            // (2) 이벤트 연결
            ObjectSelectionButtons[i].onClick.AddListener(delegate { PlaceableObject.SelectObject(ObjectSelectionButtons[i].placeableObject); });
        }

    }

    // Get mouse position depending on the resolution. 
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
