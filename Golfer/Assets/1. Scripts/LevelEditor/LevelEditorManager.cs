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
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Camera LevelEditorCamera;

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
        PlaceableObject.OnObjectSelected += delegate { SetEditorMode(EditorMode.Placing); };

        SetEditorMode(EditorMode.Editing);
    }


    // Main routine for the Level Editor
    private void Update()
    {
        /* (1) */ HandlePlaceableObject();
        // (2)    오브젝트 제거, 수정하는 코드
        // (3)    Canvas.CameraMovement(); // 마우스를 가장자리로 움직이면 화면도 움직이는 코드 추가
    }

    private static void SetEditorMode(EditorMode editorMode)
    {
        Mode = editorMode;

        switch(Mode)
        {
            case EditorMode.None:
                Camera.main.depth = 0;
                Main.LevelEditorUI.gameObject.SetActive(false);
                Main.LevelEditorUI.ObjectSelectionButtonsPanel.gameObject.SetActive(true);
                PlaceableObject.SetConvexAll(false);
                PlaceableObject.SetCollisionAll(true);
                break;
            case EditorMode.Editing:
                Camera.main.depth = -1;
                Main.LevelEditorUI.gameObject.SetActive(true);
                Main.LevelEditorUI.ObjectSelectionButtonsPanel.gameObject.SetActive(true);
                PlaceableObject.SetConvexAll(true);
                PlaceableObject.SetCollisionAll(false);
                break;
            case EditorMode.Placing:
                Camera.main.depth = -1;
                Main.LevelEditorUI.gameObject.SetActive(true);
                Main.LevelEditorUI.ObjectSelectionButtonsPanel.gameObject.SetActive(false);
                PlaceableObject.SetConvexAll(true);
                PlaceableObject.SetCollisionAll(false);
                break;
        }
    }    

    private void PlaceObject()
    {
        if (!PlaceableObject.Current.CanBePlaced) return;

        PlaceableObject.SetCurrentObjectTo(null);
        SetEditorMode(EditorMode.Editing);
    }

    private void HandlePlaceableObject()
    {
        if (Mode != EditorMode.Placing) return;
        if (Input.GetMouseButtonDown(0))
        {
            PlaceObject();
            return;
        }

        PlaceableObject.Current.transform.position = GetWorldPositionFromMousePosition();

        
    }

    private Vector3 GetWorldPositionFromMousePosition()
    {
        var position = Vector3.zero;
        Ray ray = LevelEditorCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            position = new Vector3(raycastHit.point.x, raycastHit.point.y, 0);
        }

        return position;
    }
}
