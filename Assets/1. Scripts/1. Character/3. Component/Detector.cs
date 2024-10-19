using System.Collections.Generic;
using UnityEngine;

public class Detector : MonoBehaviour
{
    private CharacterMovementController movementController;
    [SerializeField] private Collider Hitbox;

    public Vector3 ColliderCenter
    {
        get
        {
            if (Hitbox == null) return transform.position;

            Vector3 vector;
            if (Hitbox.enabled == false)
            {
                Hitbox.enabled = true;
                vector = Hitbox.bounds.center;
                Hitbox.enabled = false;
                return vector;
            }
            return Hitbox.bounds.center;
        }
    }

    [Header("Debug Detection Ray (ONLY FOR DEBUGGING)")]
    //[SerializeField] private bool Detection_Wall = false;
    [SerializeField] private bool Detection_Ray = false;
    [SerializeField] private bool Detection_Sphere = false;

    private void LateUpdate()
    {
        //if (Detection_Wall) { /* DebugWallDetectionRay(); */ }
    }

    private void Awake()
    {
        movementController = GetComponent<CharacterMovementController>();
    }

    public bool CharacterDetected<T/* T: Type of component trying to get */>(RayInfo rayInfo, out T character) where T : CharacterBase
    {
        character = null;

        var startingPosition = rayInfo.startingPosition ?? Hitbox.bounds.center;
        var direction = rayInfo.direction ?? movementController.Direction.ConvertToVector3();
        var distance = rayInfo.distance;

        if (!Physics.Raycast(startingPosition, direction, out RaycastHit hit, distance, Layer.Character.GetMask()))
            return false;

        Debug_DrawRay(startingPosition, direction * distance, Color.cyan);

        character = hit.collider?.GetComponentInParent<T>();

        return character != null;
    }

    public bool CharactersDetected(Vector3 center, float radius, out Collider[] characters)
    {
        characters = null;

        Debug_DrawSphere(center, radius);

        if (!Physics.CheckSphere(center, radius, Layer.Character.GetMask())) return false;

        characters = Physics.OverlapSphere(center, radius, Layer.Character.GetMask());
        
        return true;
    }

    public List<T> ComponentsDetected<T>(Vector3 center, float radius, int layerMask)
    {
        var colliders = Physics.OverlapSphere(center, radius, layerMask);

        Debug_DrawSphere(center, radius);

        var components = new List<T>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out T component))
            {
                components.Add(component);
            }
            else
            {
                var componentInParent = colliders[i].transform.GetComponentInParent<T>();
                if (componentInParent != null)
                {
                    components.Add(componentInParent);
                }
            }
        }

        return components;
    }

    public List<T> ComponentsDetected<T>(Vector3 center, float radius, int layerMask, Tag tagExcepted)
    {
        var colliders = Physics.OverlapSphere(center, radius, layerMask);
        
        Debug_DrawSphere(center, radius);

        var components = new List<T>();

        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];

            if (collider.CompareTag(tagExcepted)) continue;

            if (collider.TryGetComponent(out T component))
            {
                components.Add(component);
            }
            else
            {
                var componentInParent = collider.GetComponentInParent<T>();
                if (componentInParent != null)
                {
                    components.Add(componentInParent);
                }
            }
        }

        return components;
    }

    #region Functions for debugging
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Debug_DrawRay(Vector3 start, Vector3 dir, Color color)
    {
        if (!Detection_Ray) return;

        UnityEngine.Debug.DrawRay(start, dir, color);
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    private void Debug_DrawSphere(Vector3 center, float radius, float duration = 0.1f)
    {
        if (!Detection_Sphere) return;

        var sphere = Instantiate(AssetManager.GetPrefab(Prefab.Debugger.Sphere_1));
        sphere.transform.position = center;
        sphere.transform.localScale = Vector3.one * radius * 2;
        Destroy(sphere, duration);
    }
    #endregion


    public void EnableCollider()
    {
        //Hitbox.enabled = true;
    }

    public void DisableCollider()
    {
        //Hitbox.enabled = false;
    }
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

    public RayInfo(MovementDirection direction, float distance, Vector3? startingPosition = null)
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