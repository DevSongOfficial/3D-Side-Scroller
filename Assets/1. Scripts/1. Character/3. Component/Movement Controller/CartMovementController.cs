using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(Rigidbody))]
public sealed class CartMovementController : MovementController
{
    [SerializeField] private Collider wheelRear;
    [SerializeField] private Collider wheelForward;

    public Vector3 RaycastPosition_RealWheel => new Vector3(wheelRear.bounds.min.x, wheelRear.bounds.center.y, wheelRear.bounds.center.z);
    public Vector3 RaycastPosition_ForwardWheel => new Vector3(wheelForward.bounds.max.x, wheelForward.bounds.center.y, wheelForward.bounds.center.z);

    public override bool IsGrounded => IsGroundedDetectedWithIn(0.1f);

    private bool IsGroundedDetectedWithIn(float range)
    {
        float extraLength = range;
        float rayRightDistance = wheelForward.bounds.extents.y + extraLength;
        float rayLeftDistance = wheelRear.bounds.extents.y + extraLength;

        bool forward = Physics.Raycast(RaycastPosition_ForwardWheel, Vector3.down, rayRightDistance, Layer.Ground.GetMask());
        bool rear = Physics.Raycast(RaycastPosition_RealWheel, Vector3.down, rayLeftDistance, Layer.Ground.GetMask());

        return forward || rear;
    }

    // [CartMovementController] uses [Rigidbody] for movement.
    private Rigidbody rigidBody;

    protected override void Awake()
    {
        base.Awake();

        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        rigidBody.useGravity = true;
    }

    private void LateUpdate()
    {
        ClampRotationXOnDriving();
    }

    protected override void Move()
    {
        if (rigidBody.isKinematic) return;

        velocity.y = rigidBody.velocity.y;
        rigidBody.velocity = Velocity;
    }

    // Cart basically has rigidbody so doesn't need to call this unless kinematic.
    public override Quaternion AlignToGround()
    {
        Physics.Raycast(RaycastPosition_ForwardWheel, Vector3.down, out RaycastHit hitForward, 1.5f, Layer.Ground.GetMask());
        Physics.Raycast(RaycastPosition_RealWheel, Vector3.down, out RaycastHit hitRear, 1.5f, Layer.Ground.GetMask());

        Vector3 normalVector = Vector3.zero;
        normalVector.x = (hitRear.normal.x + hitForward.normal.x) * 0.5f;
        normalVector.y = (hitRear.normal.y + hitForward.normal.y) * 0.5f;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, normalVector) * transform.rotation;
        return transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
    }

    private void ClampRotationXOnDriving()
    {
        if (LevelEditorManager.IsEditorActive)
        {
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            return;
        }

        float x = transform.eulerAngles.x;
        x = (x > 180f) ? x - 360f : x;
        x = Mathf.Clamp(x, -45f, 45f);
        
        transform.eulerAngles = new Vector3(x, transform.eulerAngles.y, IsChangingDirection ? transform.eulerAngles.z : 0);
        
        if ((x > 40 || x < -40) || !IsGroundedDetectedWithIn(0.3f))
            ToggleHorizontalMovement(false);
        else
            ToggleHorizontalMovement(true, Velocity.x);
    }
}