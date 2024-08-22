using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Detector : MonoBehaviour
{
    private MovementController movementController;
    private new Collider collider;

    private List<IInteractable> interactables;

    [Header("Debug Detection Ray (ONLY FOR DEBUGGING)")]
    [SerializeField] private bool ground = false;
    [SerializeField] private bool wall = false;
    [SerializeField] private bool character = false;


    private void LateUpdate()
    {
        if (ground) DebugGroundDetectionRay();
        if (wall) { /* DebugWallDetectionRay(); */ }
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

    public bool CharacterDetected<T>(RayInfo rayInfo, out T character) where T : CharacterBase
    {
        character = null;

        var startingPosition = rayInfo.startingPosition ?? collider.bounds.center;
        var direction = rayInfo.direction ?? movementController.Direction.ConvertToVector3();
        var distance = rayInfo.distance;

        if (!Physics.Raycast(startingPosition, direction, out RaycastHit hit, distance, Layer.Character.GetMask()))
            return false;

        if (this.character) Debug.DrawRay(startingPosition, direction * distance, Color.cyan);

        character = hit.collider?.GetComponent<T>();

        return character != null;
    }

    public static bool CharacterDetected(Vector3 center, float radius, out Collider[] characters)
    {
        characters = null;

        if (!Physics.CheckSphere(center, radius, Layer.Character.GetMask())) return false;

        characters = Physics.OverlapSphere(center, radius, Layer.Character.GetMask());
        
        return true;
    }

    public class PlayerOnly
    {
        public static void ObjectDetected(Vector3 center, float radius, int layerMask)
        {
            var colliders = Physics.OverlapSphere(center, radius, (int)layerMask);

            for(int i = 0; i < colliders.Length; i++)
            {
                var collider = colliders[i];
            
            }

            //T[] objects = new T[colliders.Length];

            //return true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareLayer(Layer.InteractableObject))
        {
            var interactableObject = other.GetComponent<IInteractable>();
        }
    }

    #region Debug Detection Ray
    private void DebugGroundDetectionRay()
    {
        float extraLength = 0.1f;
        var rayDistance = collider.bounds.extents.y + extraLength;

        bool rightSide;
        {
            var startingPosition = new Vector3(collider.bounds.max.x, collider.bounds.center.y, collider.bounds.center.z);
            rightSide = Physics.Raycast(startingPosition, Vector3.down, rayDistance);
            Debug.DrawRay(startingPosition, Vector3.down * rayDistance, Color.yellow);
        }
        bool leftSide;
        {
            var startingPosition = new Vector3(collider.bounds.min.x, collider.bounds.center.y, collider.bounds.center.z);
            leftSide = Physics.Raycast(startingPosition, Vector3.down, rayDistance);
            Debug.DrawRay(startingPosition, Vector3.down * rayDistance, Color.green);
        }
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