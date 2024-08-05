using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Detector : MonoBehaviour
{
    private new Collider collider;

    private List<IInteractable> interactables;

    [Space(1)]
    [Header("Debug Detection Ray (ONLY FOR DEBUGGING)")]
    [SerializeField] private bool ground = false;
    [SerializeField] private bool wall = false;
    [SerializeField] private bool character = false;


    private void LateUpdate()
    {
        if (ground) DebugGroundDetectionRay();
        if (wall) { }
    }

    private void Awake()
    {
        collider = GetComponent<Collider>();
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
        var startingPosition = rayInfo.startingPosition == null ? collider.bounds.center : rayInfo.startingPosition.Value;
        var direction = rayInfo.direction;
        var distance = rayInfo.distance;

        Physics.Raycast(startingPosition, direction, out RaycastHit hit, distance);

        if (this.character) Debug.DrawRay(startingPosition, rayInfo.direction * rayInfo.distance, Color.cyan);

        character = hit.collider?.GetComponent<T>();

        return character != null;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collider = other.gameObject;

        if (GameManager.LayerCheck(collider, Layer.InteractableObject))
        {
            var interactableObject = collider.GetComponent<IInteractable>();
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
    public Vector3 direction;
    public float distance;

    public RayInfo(Vector3 direction, float distance, Vector3? startingPosition = null)
    {
        this.startingPosition = startingPosition;
        this.direction = direction;
        this.distance = distance;
    }

    public RayInfo(EMovementDirection direction, float distance, Vector3? startingPosition = null)
    {
        this.startingPosition = startingPosition;
        this.distance = distance;

        switch (direction)
        {
            case EMovementDirection.Right:
                this.direction = Vector3.right; break;
            case EMovementDirection.Left:
                this.direction = Vector3.left; break;
            default:
                this.direction = Vector3.zero; break;
        }
    }
}