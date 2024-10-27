using System;
using System.Collections.Generic;
using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(Collider))]
public abstract class PlaceableObjectBase : MonoBehaviour
{
    protected bool isOn;

    [Tooltip("The name displayed in UI or editor for this object.")]
    [SerializeField] private string displayName;
    public string DisplayName => displayName;

    // [editorCollider] is only for checking if placeableObjects are overlapped with other GameObjects.
    protected Collider editorCollider;

    // Child's Information
    protected Transform child;
    private Rigidbody child_rigidBody;
    private bool isKinematic;

    public bool CanBePlaced { get { return overlappedObjectsCount == 0; } }
    private int overlappedObjectsCount;

    // Events
    public static event Action<PlaceableObjectBase> OnObjectSelectedForPlacing;

    public static PlaceableObjectBase CurrentlySelected { get; private set; } // The object player's dealing with at the moment.
    public static void SelectCurrentObject(PlaceableObjectBase newPlaceableObject) { CurrentlySelected = newPlaceableObject; }

    // This function is called only when player click a button in the level editor in order to create new object.
    public void OnSelectObjectWhenPlacing()
    {
        if (CurrentlySelected != null) return;

        var selectedObject = CreatePlaceableObject();

        OnObjectSelectedForPlacing.Invoke(selectedObject);
        SelectCurrentObject(selectedObject);
        RegisterPlaceableObject(selectedObject);
    }

    #region Registration of Object Placement
    private static List<PlaceableObjectBase> PlaceableObjectsInTheScene;
    private static void RegisterPlaceableObject(PlaceableObjectBase newPlaceableObject) 
    {
        PlaceableObjectsInTheScene.Add(newPlaceableObject);
    }

    public static void UnregisterPlaceableObject(PlaceableObjectBase newPlaceableObject)
    {
        if (PlaceableObjectsInTheScene.Contains(newPlaceableObject))
        {
            PlaceableObjectsInTheScene.Remove(newPlaceableObject);
        }
        else Debug.LogWarning("No Placeable Object Matches in the List");
    }
    #endregion

    protected virtual void Awake()
    {
        editorCollider = GetComponent<Collider>();
        editorCollider.isTrigger = true;

        child = transform.GetChild(0);
        child.SetParent(null);
        child_rigidBody = child.GetComponent<Rigidbody>();
        isKinematic = child_rigidBody.isKinematic;

        gameObject.SetLayer(Layer.PlaceableObject);
    }

    protected virtual void Start() { }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialization()
    {
        PlaceableObjectsInTheScene = new List<PlaceableObjectBase>();
    }

    private void OnEnable()
    {
        LevelEditorManager.OnEditorModeToggled += OnLevelEditorToggled;
    }

    private void OnDisable()
    {
        LevelEditorManager.OnEditorModeToggled -= OnLevelEditorToggled;
    }

    protected virtual void LateUpdate()
    {
        if (!isOn) return;
        child.position = transform.position;
    }

    public virtual void InverseRotation()
    {
        transform.Rotate(0, 180, 0);
    }

    protected virtual void OnLevelEditorToggled(bool isOn)
    {
        this.isOn = isOn;
        editorCollider.enabled = isOn;

        if (isOn) transform.position = child.position;

        if (child_rigidBody == null) return;
        if (!isKinematic) child_rigidBody.isKinematic = isOn;
    }

    public void SetActive(bool active)
    {
        child.gameObject.SetActive(active);
        gameObject.SetActive(active);
    }

    private PlaceableObjectBase CreatePlaceableObject()
    {
        var newObject = Instantiate(gameObject).GetComponent<PlaceableObjectBase>();

        return newObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareLayer(Layer.PlaceableObject)) return;

        overlappedObjectsCount++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareLayer(Layer.PlaceableObject)) return;

        overlappedObjectsCount--;
    }
}