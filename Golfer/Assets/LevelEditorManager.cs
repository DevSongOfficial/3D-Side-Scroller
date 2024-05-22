using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum EditorMode
{
    None,
    Edit
}

public class LevelEditorManager : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] public static GameObject TEST_OBJECT;

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

    public EditorMode Mode { get; private set; }

    private void Awake()
    {
        #region Singleton - LevelEditorMain
        main = GetComponent<LevelEditorMain>();
        #endregion

        SetEditorMode(EditorMode.Edit);
    }


    // Main routine for the Level Editor
    private void Update()
    {
        if (TEST_OBJECT != null) TEST_OBJECT.transform.position = GetPositionFromMousePosition();
        // Canvas.CameraMovement(); // 마우스를 가장자리로 움직이면 화면도 움직이는 코드
    }

    public void SetEditorMode(EditorMode editorMode)
    {
        Mode = editorMode;

        if(Mode == EditorMode.None)
        {
            Main.LevelEditorUI.gameObject.SetActive(false);
        }
        else if(Mode == EditorMode.Edit)
        {
            Main.LevelEditorUI.gameObject.SetActive(true);

        }

        // Swift Camera
    }    

    private Vector3 GetPositionFromMousePosition()
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
