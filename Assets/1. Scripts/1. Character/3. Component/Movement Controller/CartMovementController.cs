using UnityEngine;

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

            bool forward = Physics.Raycast(RaycastPosition_ForwardWheel, Vector3.down, rayRightDistance);
            bool rear = Physics.Raycast(RaycastPosition_RealWheel, Vector3.down, rayLeftDistance);

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
}