using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Detector : MonoBehaviour
{
    private MovementController movementController;
    private new Collider collider;

    private List<IInteractable> interactables;

    [Header("Debug Detection Ray (ONLY FOR DEBUGGING)")]
    [SerializeField] private bool Detection_Ground = false;
    //[SerializeField] private bool Detection_Wall = false;
    [SerializeField] private bool Detection_Ray = false;
    [SerializeField] private bool Detection_Sphere = false;

    private void LateUpdate()
    {
        DebugGroundDetectionRay();
        //if (Detection_Wall) { /* DebugWallDetectionRay(); */ }
    }

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        collider = GetComponentInChildren<BoxCollider>();
        interactables = new List<IInteractable>();
    }

    public bool GroundDetected()
    {
        float extraLength = 0.1f;
        var rayDistance = collider.bounds.extents.y + extraLength;
        
        bool rightSide;
        {
            var startingPosition = new Vector3(collider.bounds.max.x, collider.bounds.center.y, collider.bounds.center.z);
            rightSide = Physics.Raycast(startingPosition, Vector3.down, rayDistance);
        }

        bool leftSide;
        {
            var startingPosition = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.center.z);
            leftSide = Physics.Raycast(startingPosition, Vector3.down, rayDistance);
        }

        return rightSide || leftSide;
    }

    public bool CharacterDetected<T/* T: Type of component trying to get */>(RayInfo rayInfo, out T character) where T : CharacterBase
    {
        character = null;

        var startingPosition = rayInfo.startingPosition ?? collider.bounds.center;
        var direction = rayInfo.direction ?? movementController.Direction.ConvertToVector3();
        var distance = rayInfo.distance;

        if (!Physics.Raycast(startingPosition, direction, out RaycastHit hit, distance, Layer.Character.GetMask()))
            return false;

        DrawRay(startingPosition, direction * distance, Color.cyan);

        character = hit.collider?.GetComponent<T>();

        return character != null;
    }

    public bool CharactersDetected(Vector3 center, float radius, out Collider[] characters)
    {
        characters = null;

        if (!Physics.CheckSphere(center, radius, Layer.Character.GetMask())) return false;

        DrawSphere(center, radius);

        characters = Physics.OverlapSphere(center, radius, Layer.Character.GetMask());
        
        return true;
    }

    public T[] ComponentsDetected<T>(Vector3 center, float radius, int layerMask)
    {
        var colliders = Physics.OverlapSphere(center, radius, layerMask);

        DrawSphere(center, radius);

        T[] components = new T[colliders.Length];
        int index = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out T component))
            {
                components[index] = component;
                index++;
            }
        }

        return components;
    }

    public T[] ComponentsDetected<T>(Vector3 center, float radius, int layerMask, Tag tagExcepted)
    {
        var colliders = Physics.OverlapSphere(center, radius, layerMask);

        DrawSphere(center, radius);

        T[] components = new T[colliders.Length];
        int index = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (collider.CompareTag(tagExcepted)) continue;
            UnityEngine.Debug.Log(colliders[i].name);
            if (colliders[i].TryGetComponent(out T component))
            {
                components[index] = component;
                index++;
            }
        }

        return components;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareLayer(Layer.InteractableObject))
        {
            var interactableObject = other.GetComponent<IInteractable>();
        }
    }

    #region Functions for debugging
    [Conditional("UNITY_EDITOR")]
    private void DebugGroundDetectionRay()
    {
        if (!Detection_Ground) return;

        float extraLength = 0.1f;
        var rayDistance = collider.bounds.extents.y + extraLength;

        bool rightSide;
        {
            var startingPosition = new Vector3(collider.bounds.max.x, collider.bounds.center.y, collider.bounds.center.z);
            rightSide = Physics.Raycast(startingPosition, Vector3.down, rayDistance);
            UnityEngine.Debug.DrawRay(startingPosition, Vector3.down * rayDistance, Color.yellow);
        }
        bool leftSide;
        {
            var startingPosition = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.center.z);
            leftSide = Physics.Raycast(startingPosition, Vector3.down, rayDistance);
            UnityEngine.Debug.DrawRay(startingPosition, Vector3.down * rayDistance, Color.green);
        }
    }

    [Conditional("UNITY_EDITOR")]
    private void DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        if (!Detection_Ray) return;

        UnityEngine.Debug.DrawRay(start, dir, color);
    }

    [Conditional("UNITY_EDITOR")]
    private void DrawSphere(Vector3 center, float radius, float duration = 0.1f)
    {
        if (!Detection_Sphere) return;

        var sphere = Instantiate(AssetManager.GetPrefab(Prefab.Debugger.Sphere_1));
        sphere.transform.position = center;
        sphere.transform.localScale = Vector3.one * radius * 2;
        Destroy(sphere, duration);
    }
    #endregion
}

public struct RayInfo
{
    public Vector3? startingPosition;
    public Vector3? direction;
    public float distance;

    public RayInfo(float distance, Vector3? direction = null, Vector3? startingPosition = null)
    {
        this.startingPosition = startingPosition;
        this.direction = direction;
        this.distance = distance;
    }

    public RayInfo(EMovementDirection direction, float distance, Vector3? startingPosition = null)
    {
        this.startingPosition = startingPosition;
        this.distance = distance;
        this.direction = direction.ConvertToVector3();
    }

    
    // Builder functions for builder pattern.
    #region Builder Functions

    public RayInfo SetStartingPosition(Vector3 startingPosition)
    {
        this.startingPosition = startingPosition;

        return this;
    }

    public RayInfo SetDirection(Vector3 direction)
    {
        this.direction = direction;

        return this;
    }

    public RayInfo SetDistance(float distance) 
    {
        this.distance = distance;

        return this;
    }
    #endregion 
    
}