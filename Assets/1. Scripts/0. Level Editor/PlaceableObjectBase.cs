using System;
using System.Collections.Generic;
using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public abstract class PlaceableObjectBase : MonoBehaviour
{
    [Tooltip("The name displayed in UI or editor for this object.")]
    [SerializeField] private string displayName;
    public string DisplayName => displayName;

    [Tooltip("This stops forces or collisions from affecting the rigidbody")]
    [SerializeField] protected bool isKinematic;

    // [editorCollider] is only for checking if placeableObjects are overlapped with other GameObjects.
    protected Collider editorCollider;
    
    // Actual collider when playing game is [gameColliders]. (In case one has more than two colliders, I used generic list.)
    // Characters basically have [CharacterController] instead of [Collider]s and [Rigidbody].
    private List<Collider> gameColliders = new List<Collider>();
    protected Rigidbody rigidBody;

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
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.isKinematic = true;

        editorCollider = GetComponent<Collider>();
        editorCollider.isTrigger = true;

        gameObject.SetLayer(Layer.PlaceableObject);

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

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialization()
    {
        PlaceableObjectsInTheScene = new List<PlaceableObjectBase>();
    }

    private void OnEnable()
    {
        OnLevelEditorToggled(true);

        LevelEditorManager.OnEditorModeToggled += OnLevelEditorToggled;
    }

    private void OnDisable()
    {
        LevelEditorManager.OnEditorModeToggled -= OnLevelEditorToggled;
    }

    public virtual void InverseRotation()
    {
        transform.Rotate(0, 180, 0);
    }

    protected virtual void OnLevelEditorToggled(bool isOn)
    {
        SetBodyCollision(!isOn);
        SetEditorCollision(isOn);        
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