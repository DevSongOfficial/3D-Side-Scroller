using System;
using System.Collections.Generic;
using UnityEngine;
using static GameSystem;

// Only for checking if placeableObjects are overlapped with other GameObjects.
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class PlaceableObjectBase : MonoBehaviour
{
    protected bool isEditorMode;

    [Tooltip("The name displayed in UI or editor for this object.")]
    [SerializeField] private string displayName;
    public string DisplayName => displayName;

    // Prefab Type
    public Prefab.General Type { get; private set; }
    public void SetType(Prefab.General type) { Type = type; }

    private Rigidbody rigidBody;
    private new Collider collider;

    // The actual object connected to PO.
    protected Transform actualObject;
    protected Rigidbody actualObject_rigidBody;
    protected bool actualObject_IsKinematic;

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
    public static List<PlaceableObjectBase> PlaceableObjectsInTheScene;
    public static void RegisterPlaceableObject(PlaceableObjectBase newPlaceableObject) 
    {
        PlaceableObjectsInTheScene.Add(newPlaceableObject);
        GameManager.AttachToMap(newPlaceableObject.transform);
        GameManager.AttachToMap(newPlaceableObject.actualObject.transform);
    }

    public static void UnregisterPlaceableObject(PlaceableObjectBase newPlaceableObject)
    {
        if (!PlaceableObjectsInTheScene.Contains(newPlaceableObject)) return;
            
        PlaceableObjectsInTheScene.Remove(newPlaceableObject);
        GameManager.MoveToLagacy(newPlaceableObject.transform);
        GameManager.MoveToLagacy(newPlaceableObject.actualObject.transform);
    }
    #endregion

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        actualObject = transform.GetChild(0);
        actualObject_rigidBody = actualObject.GetComponent<Rigidbody>();
        if(actualObject_rigidBody != null) actualObject_IsKinematic = actualObject_rigidBody.isKinematic;
        
        collider.isTrigger = true;
        rigidBody.isKinematic = true;
    }

    protected virtual void Start() 
    {
        gameObject.SetLayer(Layer.Placeable);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad()
    {
        PlaceableObjectsInTheScene = new List<PlaceableObjectBase>();
    }

    protected virtual void OnEnable()
    {
        LevelEditorManager.OnEditorModeToggled += OnLevelEditorToggled;
    }

    protected virtual void OnDisable()
    {
        LevelEditorManager.OnEditorModeToggled -= OnLevelEditorToggled;
    }

    protected virtual void LateUpdate()
    {
        if (!isEditorMode) return;
        actualObject.position = transform.position;
    }

    public virtual void InverseRotation()
    {
        actualObject.transform.Rotate(0, 180, 0);
    }

    protected virtual void OnLevelEditorToggled(bool isOn)
    {
        isEditorMode = isOn;
        rigidBody.isKinematic = isOn;

        if (isOn)
        {
            transform.position = actualObject.position;
            transform.eulerAngles = actualObject.eulerAngles;
        }

        if (actualObject_rigidBody == null) return;

        if (!actualObject_IsKinematic) actualObject_rigidBody.isKinematic = isOn;
    }

    public void SetActive(bool active)
    {
        actualObject.gameObject.SetActive(active);
        gameObject.SetActive(active);
    }

    private PlaceableObjectBase CreatePlaceableObject()
    {
        var newObject = Instantiate(this);
        newObject.SetType(Type);

        return newObject;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareLayer(Layer.Placeable)) return;
        overlappedObjectsCount++;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (!other.CompareLayer(Layer.Placeable)) return;

        overlappedObjectsCount--;
    }
}