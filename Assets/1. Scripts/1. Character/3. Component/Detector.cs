using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.UI.Image;

public class Detector : MonoBehaviour
{
    private CharacterMovementController movementController;
    [Tooltip("Collider for calculating precise position and bound.")]
    [SerializeField] protected new Collider collider;

    public Vector3 ColliderCenter
    {
        get
        {
            if (collider == null) return transform.position;

            Vector3 vector;
            if (collider.enabled == false)
            {
                collider.enabled = true;
                vector = collider.bounds.center;
                collider.enabled = false;
                return vector;
            }
            return collider.bounds.center;
        }
    }

    [Header("Debug Detection Ray / Sphere (ONLY FOR DEBUGGING)")]
    [SerializeField] private bool Detection_Ray = false;
    [SerializeField] private bool Detection_Sphere = false;

    protected virtual void Awake()
    {
        movementController = GetComponent<CharacterMovementController>();
    }

    public bool DetectCharacter<T>(RayInfo rayInfo, out T character) where T : CharacterBase
    {
        character = null;
        var startingPosition = rayInfo.origin ?? collider.bounds.center;
        var direction = rayInfo.direction ?? movementController.Direction.ConvertToVector3();
        var distance = rayInfo.distance;

        if (!Physics.Raycast(startingPosition, direction, out RaycastHit hit, distance))
            return false;

        Debug_DrawRay(startingPosition, direction * distance, Color.cyan);

        character = hit.collider.GetComponent<T>();

        return character != null;
    }

    public bool DetectCharacters<T>(Vector3 center, float radius, out T[] characters) where T : CharacterBase
    {
        characters = null;

        Debug_DrawSphere(center, radius);

        if (!Physics.CheckSphere(center, radius)) return false;

        var colliders = Physics.OverlapSphere(center, radius);
        characters = new T[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            characters[i] = colliders[i].GetComponent<T>();
        }

        return true;
    }

    public List<T> DetectComponents<T>(Vector3 center, float radius, int layerMask, bool putClosestInFirst)
    {
        var colliders = Physics.OverlapSphere(center, radius, layerMask);

        Debug_DrawSphere(center, radius);

        var components = new List<T>();
        float closestDistance = 100f;

        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];
            T componentToAdd;
            
            if (collider.TryGetComponent(out T component))
            {
                componentToAdd = component;
            }
            else
            {
                var componentInParent = collider.transform.GetComponentInParent<T>();
                if (componentInParent != null)
                {
                    componentToAdd = componentInParent;
                }
                else
                {
                    continue;
                }
            }

            var distance = Vector3.Distance(transform.position, collider.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                components.Insert(0, componentToAdd);
            }
            else
            {
                components.Add(componentToAdd);
            }
        }

        return components;
    }


    public List<T> DetectComponents<T>(Vector3 center, float radius, int layerMask)
    {
        var colliders = Physics.OverlapSphere(center, radius, layerMask);

        Debug_DrawSphere(center, radius);

        var components = new List<T>();

        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];

            if (collider.TryGetComponent(out T component))
            {
                components.Add(component);
            }
            else
            {
                var componentInParent = collider.transform.GetComponentInParent<T>();
                if (componentInParent != null)
                {
                    components.Add(componentInParent);
                }
            }
        }

        return components;
    }

    public List<T> DetectComponents<T>(Vector3 center, float radius, int layerMask, Tag ignoreTag = Tag.Untagged)
    {
        var colliders = Physics.OverlapSphere(center, radius, layerMask);

        Debug_DrawSphere(center, radius);

        var components = new List<T>();

        for (int i = 0; i < colliders.Length; i++)
        {
            var collider = colliders[i];

            if (collider.CompareTag(ignoreTag)) continue;

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

    public bool DetectWithLayer(RayInfo rayInfo, Layer layer)
    {
        var origin = rayInfo.origin ?? collider.bounds.center;
        var direction = rayInfo.direction ?? movementController.Direction.ConvertToVector3();
        var distance = rayInfo.distance;
        
        Debug_DrawRay(origin, direction * distance, Color.yellow);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            if (hit.collider.CompareLayer(layer))
            {
                return true;
            }
        }

        return false;
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
}

public struct RayInfo
{
    public Vector3? origin;
    public Vector3? direction;
    public float distance;

    public RayInfo(float distance, Vector3? direction = null, Vector3? origin = null)
    {
        this.origin = origin;
        this.direction = direction;
        this.distance = distance;
    }

    public RayInfo(MovementDirection direction, float distance, Vector3? origin = null)
    {
        this.origin = origin;
        this.distance = distance;
        this.direction = direction.ConvertToVector3();
    }

    
    // Builder functions
    #region Builder Functions

    public RayInfo SetOrigin(Vector3 origin)
    {
        this.origin = origin;

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