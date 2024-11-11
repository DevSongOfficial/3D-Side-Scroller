using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ProbFollowingCamera : MonoBehaviour
{
    private enum AngleType { Horizontal = 0, Vertical = 90 }

    [Header("Assign prefab here to follow its clone.")]
    [SerializeField] private GameObject probPrefab;
    private Prefab.General probType;
    protected PlaceableProb target;
    private bool hasTarget;

    [SerializeField] private AngleType angleType = AngleType.Horizontal;
    [SerializeField] private float distanceFromTarget;

    public Camera Camera { get; protected set; }
    [SerializeField] private int priorityOnStart;

    protected virtual void Awake()
    {
        Camera = GetComponent<Camera>();
        probType = probPrefab.GetComponent<PlaceableProb>().Type;
        
        PlaceableObjectBase.OnObjectRegistered += SetTarget;
        PlaceableObjectBase.OnObjectUnregistered += ClearTarget;
    }

    protected virtual void Start()
    {
        Camera.depth = priorityOnStart;
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
        PlaceableObjectBase.OnObjectRegistered -= SetTarget;
        PlaceableObjectBase.OnObjectUnregistered -= ClearTarget;
    }
}