using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public Prefab.PO Type { get; private set; }
    public void SetType(Prefab.PO type) { Type = type; }

    private Rigidbody rigidBody;
    private new Collider collider;

    // The actual object connected to PO.
    public Transform ActualObject { get; protected set; }
    protected Rigidbody actualObject_rigidBody;
    protected bool actualObject_IsKinematic;

    public virtual bool NotOverlapped { get { return overlappedObjectsCount == 0; } }
    private int overlappedObjectsCount;

    public static PlaceableObjectBase CurrentlySelected { get; private set; } // The object player's dealing with at the moment.
    public static PlaceableObjectBase PreviouslyPlaced { get; private set; }
    public static void SetCurrentObject(PlaceableObjectBase po) { CurrentlySelected = po; }
    public static void SetPreviouslyPlacedObject(PlaceableObjectBase po) {  PreviouslyPlaced = po; }

    // This function is called when player click a button in the level editor in order to create new object.
    public void OnCreatedFromButton()
    {
        if (CurrentlySelected != null) return;

        SetCurrentObject(POFactory.CreatePO(Type));
        LevelEditorManager.SetPlayMode(PlayMode.Placing);
    }

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        ActualObject = transform.GetChild(0);
        actualObject_rigidBody = ActualObject.GetComponent<Rigidbody>();
        if (actualObject_rigidBody != null) actualObject_IsKinematic = actualObject_rigidBody.isKinematic;

        collider.isTrigger = true;
        rigidBody.isKinematic = true;
    }

    protected virtual void Start() 
    {
        gameObject.SetLayer(Layer.Placeable);
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
        if (isEditorMode)
        {
            ActualObject.position = transform.position;
            ActualObject.rotation = transform.rotation;
        }
        else
        {
            transform.position = ActualObject.position;
            transform.eulerAngles = ActualObject.eulerAngles;
        }
    }

    public virtual void InverseRotation()
    {
        transform.Rotate(0, 180, 0);
    }

    protected virtual void OnLevelEditorToggled(bool isOn)
    {
        isEditorMode = isOn;

        if (isOn)
        {
            if (actualObject_rigidBody != null) actualObject_rigidBody.isKinematic = true;
        }
        else
        {
            ActualObject.position = transform.position;

            if (actualObject_rigidBody != null) actualObject_rigidBody.isKinematic = actualObject_IsKinematic;
        }
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

    public virtual PlaceableGround AsGround()
    {
        return null;
    }
}