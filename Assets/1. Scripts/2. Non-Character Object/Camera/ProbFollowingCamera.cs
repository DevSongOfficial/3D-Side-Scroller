using System;
using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(Camera))]
public class ProbFollowingCamera : MonoBehaviour
{
    private enum AngleType { Horizontal = 0, Vertical = 90 }

    [Header("Prob to follow its clone")]
    [SerializeField] private Prefab.PO targetType;
    protected PlaceableProb target;
    private bool hasTarget;
    [Space]
    [SerializeField] private AngleType angleType = AngleType.Horizontal;
    [SerializeField] private float distanceFromTarget;

    public Camera Camera { get; protected set; }
    [SerializeField] private int priorityOnStart;

    private void Awake()
    {
        if (IsMakerScene)
        {
            LevelEditorManager.OnEditorModeToggled += SetActive;
            SetActive(false);
        }
    }


    protected virtual void OnEnable()
    {
        Camera = GetComponent<Camera>();
        Camera.depth = priorityOnStart;

        POFactory.OnPORegistered += SetTarget;
        POFactory.OnPOUnregistered += ClearTarget;
    }
    protected virtual void OnDisable()
    {
        POFactory.OnPORegistered -= SetTarget;
        POFactory.OnPOUnregistered -= ClearTarget;
    }

    protected virtual void LateUpdate()
    {
        FollowTarget();
    }

    protected virtual void SetTarget(PlaceableObjectBase po)
    {
        if (po.Type != targetType) return;
 
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

    private void SetActive(bool isOn)
    {
        gameObject.SetActive(!isOn);
    }
}