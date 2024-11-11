using UnityEngine;
using UnityEngine.InputSystem.XR;

[RequireComponent(typeof(Rigidbody))]
public sealed class CartMovementController : MovementController
{
    [SerializeField] private Collider wheelRear;
    [SerializeField] private Collider wheelForward;

    public Vector3 RaycastPosition_RealWheel => new Vector3(wheelRear.bounds.min.x, wheelRear.bounds.center.y, wheelRear.bounds.center.z);
    public Vector3 RaycastPosition_ForwardWheel => new Vector3(wheelForward.bounds.max.x, wheelForward.bounds.center.y, wheelForward.bounds.center.z);

    public override bool IsGrounded 
    {
        get
        {
            float extraLength = 0.1f;
            float rayRightDistance = wheelForward.bounds.extents.y + extraLength;
            float rayLeftDistance = wheelRear.bounds.extents.y + extraLength;

            bool forward = Physics.Raycast(RaycastPosition_ForwardWheel, Vector3.down, rayRightDistance, Layer.Ground.GetMask());
            bool rear = Physics.Raycast(RaycastPosition_RealWheel, Vector3.down, rayLeftDistance, Layer.Ground.GetMask());

            return forward || rear;
        }
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

    protected override void Move()
    {
        if (rigidBody.isKinematic) return;

        velocity.y = rigidBody.velocity.y;
        rigidBody.velocity = Velocity;
    }

    public override Quaternion AlignToGround()
    {
        Physics.Raycast(RaycastPosition_ForwardWheel, Vector3.down, out RaycastHit hitForward, 2.7f, Layer.Ground.GetMask());
        Physics.Raycast(RaycastPosition_RealWheel, Vector3.down, out RaycastHit hitRear, 2.7f, Layer.Ground.GetMask());

        if (hitForward.collider == null && hitRear.collider == null) return transform.rotation = Quaternion.Euler(0, FacingDirection.GetYRotationValue(), 0);

        Vector3 normalVector = Vector3.zero;
        normalVector.x = (hitRear.normal.x + hitForward.normal.x) * 0.5f;
        normalVector.y = (hitRear.normal.y + hitForward.normal.y) * 0.5f;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, normalVector) * transform.rotation;
        return transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }
}