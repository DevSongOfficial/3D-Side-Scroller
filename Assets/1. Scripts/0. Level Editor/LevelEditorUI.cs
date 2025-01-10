using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static GameSystem;

[Flags] public enum StretchType { None = 0, Width = 1, Height = 2}

public class LevelEditorUI : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster graphicRaycaster;

    [Space]
    // Screen Movement with Mouse Cursor Position
    private Dictionary<Vector2, Image> mouseAreas = new Dictionary<Vector2, Image>();
    [SerializeField] private Image[] mouseCursorDetectorImages;

    [Space]
    // Object Selection Button
    [SerializeField] private RectTransform contentsPanel;
    private ObjectSelectionButton[] objectSelectionButtons; // Buttons in contentsPanel.
    [SerializeField] private RectTransform objectSelectionGroup;
    public RectTransform ObjectSelectionGroup => objectSelectionGroup;
    private void ToggleObjectSelectionGroup(bool inOn) => objectSelectionGroup.gameObject.SetActive(inOn);

    [Space]
    // Upload and download data.
    [SerializeField] private InputField inputField_Title;
    [SerializeField] private InputField inputField_Description;
    [SerializeField] private Button button_OpenUploadPanel;
    [SerializeField] private Image panel_Upload;
    public InputField InputField_Title => inputField_Title;
    public InputField InputField_Description => inputField_Description;
    public Button Button_OpenUploadPanel => button_OpenUploadPanel;
    public Image Panel_Upload => panel_Upload;

    private void Awake()
    {
        LevelEditorManager.OnEditorModeToggled += ToggleObjectSelectionGroup;

        // Initialize mouse cursor detector for screen movement.
        mouseAreas.Add(Vector2.left, mouseCursorDetectorImages[0]);
        mouseAreas.Add(Vector2.right, mouseCursorDetectorImages[1]);
        mouseAreas.Add(Vector2.up, mouseCursorDetectorImages[2]);
        mouseAreas.Add(Vector2.down, mouseCursorDetectorImages[3]);

        // Creates buttons as many as the number of general prefabs.
        #region Set up creation buttons
        int count = Enum.GetValues(typeof(Prefab.PO)).Length;
        objectSelectionButtons = new ObjectSelectionButton[count];
        // Buttons Settings
        for (int i = 0; i < count; i++)
        {
            var button = Instantiate(AssetManager.GetPrefab(Prefab.UI.SpawnButton_1), contentsPanel.transform).
                GetComponent<ObjectSelectionButton>();

            var prefab = AssetManager.GetPrefab((Prefab.PO)i).GetComponent<PlaceableObjectBase>();
            prefab.SetType((Prefab.PO)i);

            button.Initialize(prefab);
            objectSelectionButtons[i] = button;
        }
        #endregion
    }

    public bool IsMouseCursorOnTheArea(RectTransform area)
    {
        Vector2 cursorPosition = Input.mousePosition;

        return cursorPosition.x >= area.position.x - area.rect.width  * 0.5f &&
                cursorPosition.x <= area.position.x + area.rect.width  * 0.5f &&
                cursorPosition.y >= area.position.y - area.rect.height * 0.5f &&
                cursorPosition.y <= area.position.y + area.rect.height * 0.5f;
    }

    public bool IsMouseCursorOnUI()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }

    public bool TryGetComponentFromMousePosition<T>(out T component, Layer layerMask, Camera cameraForRay = null) where T : Component
    {
        component = null;

        var camera = cameraForRay ?? LevelEditorManager.Camera;
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

    public Vector3 GetWorldPositionFromMousePosition(bool ignorePlaceableObjectLayer = true)
    {
        var position = Vector3.zero;
        Ray ray = LevelEditorManager.Camera.ScreenPointToRay(Input.mousePosition);

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

    public Vector2 GetMouseArea()
    {
        Vector2 mouseArea = Vector2.zero;

        foreach(var area in mouseAreas.Keys)
        {
            if (IsMouseCursorOnTheArea(mouseAreas[area].rectTransform))
                mouseArea += area;
        }

        return mouseArea;
    }    
}
