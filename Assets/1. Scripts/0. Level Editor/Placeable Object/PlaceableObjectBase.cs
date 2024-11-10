using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
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
    public Transform ActualObject { get; protected set; }
    protected Rigidbody actualObject_rigidBody;
    protected bool actualObject_IsKinematic;

    public bool CanBePlaced { get { return overlappedObjectsCount == 0; } }
    private int overlappedObjectsCount;

    // Events
    public static event Action<PlaceableObjectBase> OnObjectRegistered;
    public static event Action<PlaceableObjectBase> OnObjectUnregistered;
    public static event Action<PlaceableObjectBase> OnObjectCreatedFromButton;

    public static PlaceableObjectBase CurrentlySelected { get; private set; } // The object player's dealing with at the moment.
    public static PlaceableObjectBase PreviouslyPlaced { get; private set; }
    public static void SetCurrentObject(PlaceableObjectBase po) { CurrentlySelected = po; }
    public static void SetPreviouslyPlacedObject(PlaceableObjectBase po) {  PreviouslyPlaced = po; }

    // This function is called when player click a button in the level editor in order to create new object.
    public void CreateIfSelectedPleaceableObject()
    {
        if (CurrentlySelected != null) return;
        
        var selectedObject = CreatePlaceableObject();
        SetCurrentObject(selectedObject);
        
        RegisterPlaceableObject(selectedObject);

        OnObjectCreatedFromButton.Invoke(selectedObject);
    }

    public virtual PlaceableObjectBase CreatePlaceableObject()
    {
        var newObject = Instantiate(this);
        newObject.SetType(Type);

        return newObject;
    }

    #region Registration of Object Placement
    public static List<PlaceableObjectBase> PlaceableObjectsInTheScene;
    public static void RegisterPlaceableObject(PlaceableObjectBase po) 
    {
        PlaceableObjectsInTheScene.Add(po);
        GameManager.AttachToMap(po.transform);
        GameManager.AttachToMap(po.ActualObject.transform);

        OnObjectRegistered.Invoke(po);
    }

    public static void UnregisterPlaceableObject(PlaceableObjectBase po)
    {
        if (!PlaceableObjectsInTheScene.Contains(po)) return;

        OnObjectUnregistered.Invoke(po);

        PlaceableObjectsInTheScene.Remove(po);
        GameManager.MoveToLagacy(po.transform);
        GameManager.MoveToLagacy(po.ActualObject.transform);
    }
    #endregion

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        ActualObject = transform.GetChild(0);
        actualObject_rigidBody = ActualObject.GetComponent<Rigidbody>();
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
        ActualObject.position = transform.position;
    }

    public virtual void InverseRotation()
    {
        ActualObject.transform.Rotate(0, 180, 0);
    }

    protected virtual void OnLevelEditorToggled(bool isOn)
    {
        isEditorMode = isOn;

        if (isOn)
        {
            transform.position = ActualObject.position;
            transform.eulerAngles = ActualObject.eulerAngles;
        }

        if (actualObject_rigidBody == null) return;

        if (!actualObject_IsKinematic) actualObject_rigidBody.isKinematic = isOn;
    }

    public void SetActive(bool active)
    {
        ActualObject.gameObject.SetActive(active);
        gameObject.SetActive(active);
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