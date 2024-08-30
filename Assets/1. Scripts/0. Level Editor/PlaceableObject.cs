using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class PlaceableObject : MonoBehaviour
{
    [Tooltip("The name displayed in UI or editor for this object.")]
    [SerializeField] private string displayName;
    public string DisplayName => displayName;

    // [editorCollider] is only for checking if placeableObjects are overlapped with other GameObjects.
    // Actual collider when playing game is [gameColliders]. (In case one has more than two colliders, I used generic list.)
    protected Collider editorCollider;
    private List<Collider> gameColliders = new List<Collider>();
    protected Rigidbody rigidBody;

    public bool CanBePlaced { get { return overlappedObjectsCount == 0; } }
    private int overlappedObjectsCount;

    // Events
    public static event Action<PlaceableObject> OnObjectSelectedForPlacing;

    public static PlaceableObject CurrentlySelected { get; private set; } // The object player's dealing with at the moment.
    public static void SelectCurrentObject(PlaceableObject newPlaceableObject) { CurrentlySelected = newPlaceableObject; }

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
    private static List<PlaceableObject> PlaceableObjectsInTheScene;
    private static void RegisterPlaceableObject(PlaceableObject newPlaceableObject) 
    {
        PlaceableObjectsInTheScene.Add(newPlaceableObject);
    }

    public static void UnregisterPlaceableObject(PlaceableObject newPlaceableObject)
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

        gameObject.SetLayer(Layer.PlaceableObject);

        // Initialize Rigidbody
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;

        // Initialize actual collider(s) of the body.
        for (int i = 0; i < transform.childCount; i++)
        {
            var childCollider = transform.GetChild(i).GetComponent<Collider>();
            if (childCollider != null)
            {
                gameColliders.Add(childCollider);
            }
        }
    }

    protected virtual void Start() { }

    private void OnEnable()
    {
        OnLevelEditorTriggered(true);

        LevelEditorManager.OnEditorModeTriggered += OnLevelEditorTriggered;
    }

    private void OnDisable()
    {
        LevelEditorManager.OnEditorModeTriggered -= OnLevelEditorTriggered;
    }

    public virtual void InverseRotation()
    {
        transform.Rotate(0, 180, 0);
    }

    protected virtual void OnLevelEditorTriggered(bool active)
    {
        SetBodyCollision(!active);
        SetEditorCollision(active);
        rigidBody.isKinematic = active;
    }

    public void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    private void SetEditorCollision(bool enabled)
    {
        editorCollider.enabled = enabled;
    }

    private void SetBodyCollision(bool enabled) 
    {
        for(int i = 0; i < gameColliders.Count; i++)
        {
            gameColliders[i].enabled = enabled;
        }
    }

    public static void Initialization()
    {
        PlaceableObjectsInTheScene = new List<PlaceableObject>();
    }

    private PlaceableObject CreatePlaceableObject()
    {
        var newObject = Instantiate(gameObject).GetComponent<PlaceableObject>();
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