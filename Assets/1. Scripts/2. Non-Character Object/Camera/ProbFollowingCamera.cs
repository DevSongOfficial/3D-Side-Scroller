using System;
using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(Camera))]
public class ProbFollowingCamera : MonoBehaviour
{
    private enum AngleType { Horizontal = 0, Vertical = 90 }

    [Header("Assign prefab here to follow its clone.")]
    [SerializeField] private GameObject probPrefab;
    private Prefab.PO probType;
    protected PlaceableProb target;
    private bool hasTarget;

    [SerializeField] private AngleType angleType = AngleType.Horizontal;
    [SerializeField] private float distanceFromTarget;

    public Camera Camera { get; protected set; }
    [SerializeField] private int priorityOnStart;

    protected virtual void Awake()
    {
        Camera = GetComponent<Camera>();
        Camera.depth = priorityOnStart;

        probType = Enum.Parse<Prefab.PO>(probPrefab.name);

        POFactory.OnPORegistered += SetTarget;
        POFactory.OnPOUnregistered += ClearTarget;
    }

    protected virtual void LateUpdate()
    {
        FollowTarget();
    }

    protected virtual void SetTarget(PlaceableObjectBase po)
    {
        if (po.Type != probType) return;
 
        transform.eulerAngles = Vector3.right * (int)angleType;
        target = po as PlaceableProb;
        hasTarget = true;
    }

    protected virtual void ClearTarget(PlaceableObjectBase po)
    {
        if (!hasTarget) return;
        if (po != target) return;

        hasTarget = false;
    }

    protected virtual void FollowTarget()
    {
        if (!hasTarget) return;

        var direction = angleType == AngleType.Horizontal ? Vector3.back : Vector3.up;
        transform.position = target.ActualObject.position + direction * distanceFromTarget;
    }

    protected virtual void OnDestroy()
    {
        POFactory.OnPORegistered -= SetTarget;
        POFactory.OnPOUnregistered -= ClearTarget;
    }
}