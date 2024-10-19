using System.Xml.Schema;
using UnityEngine;
using static AnimationController;
using static GameSystem;

public class GolfCart : MonoBehaviour, IInteractable
{
    // The character who currently has access to or is interacting with the bag.
    private Interactor driver;
    public bool IsTaken => driver != null;

    private CartMovementController movementController;

    [Header("Golf Cart Information")]
    [SerializeField] private ObjectInfo info;
    [SerializeField] private DamageEvent damageEvent;

    // Collision
    private Detector detector;
    private Vector3 offset_CollisionPosition => new Vector3((int)movementController.FacingDirection, -0.3f, 0f);
    private readonly float collisionRadius = 1;

    // Cargo Tray
    private IPickupable ObjectOnTheTray;
    [SerializeField] private Transform cargoTray;

    private void Awake()
    {
        movementController = GetComponent<CartMovementController>();
        detector = GetComponent<Detector>();
    }

    private void Start()
    {
        movementController.ChangeMovementDirection(MovementDirection.Right, smoothRotation: false);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        AttackOnCollide();
    }

    private void LateUpdate()
    {
        HandleXRotation();

        if (!IsTaken) return;

        driver.AsDriver.InvokeEvent_OnDrive(this, transform.position, transform.eulerAngles);
    }

    private void GetInTheCart(Interactor driver)
    {
        this.driver = driver;
        driver.AsDriver.InvokeEvent_OnEnterVehicle(this); // MoveState -> OnVehicleState

        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate);
        GameManager.Input_OnChangeDirection += OnChangeDirection;

        movementController.ToggleHorizontalMovement(true);

        ObjectOnTheTray?.OnPickedUp(cargoTray, shouldAlignToCenter: false);
    }

    private void GetOutOfTheCart()
    {
        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.SmartUpdate);
        GameManager.Input_OnChangeDirection -= OnChangeDirection;

        driver.AsDriver.InvokeEvent_OnExitVehicle(this); // OnVehicleState -> MoveState
        driver = null;

        movementController.ToggleHorizontalMovement(false);
        velocityMultiplier = 0;

        ObjectOnTheTray?.OnDropedOff();
    }

    public void Interact(Interactor newInteractor)
    {
        if (IsTaken)
        {
            GetOutOfTheCart();
        }
        else
        {
            GetInTheCart(newInteractor);
        }
    }

    private int velocityMultiplier;
    private void OnChangeDirection(MovementDirection newDirection)
    {
        velocityMultiplier = newDirection == MovementDirection.None ? 0 : 1;
        movementController.ChangeDirectionSmooth(newDirection);
    }

    private void HandleMovement()
    {
        movementController.ApplyHorizontalVelocity(info.MovementSpeed * velocityMultiplier, info.Acceleration);
    }

    private void HandleXRotation()
    {
        Physics.Raycast(movementController.RaycastPosition_ForwardWheel, Vector3.down, out RaycastHit hitForward, 1.2f, Layer.Default.GetMask());
        Physics.Raycast(movementController.RaycastPosition_RealWheel, Vector3.down, out RaycastHit hitRear, 1.2f, Layer.Default.GetMask());

        Vector3 normalVector = Vector3.zero;
        normalVector.x = (hitRear.normal.x + hitForward.normal.x) * 0.5f;
        normalVector.y = (hitRear.normal.y + hitForward.normal.y) * 0.5f;

        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, normalVector) * transform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
    }

    void AttackOnCollide()
    {
        if (!IsTaken) return;
        if (movementController.Velocity.x < 3.5f) return;

        if (detector.CharactersDetected(transform.position + offset_CollisionPosition, collisionRadius, out Collider[] colliders))
        {
            foreach (Collider collider in colliders)
            {
                var character = collider.GetComponentInParent<ZombieCharacter>();
                if (character is null) continue;

                var newDamageEvent = damageEvent.
                    MultiplyKnockback(movementController.Velocity.x * 0.5f).
                    ApplyDirection(movementController.FacingDirection);
                character.TakeDamage(newDamageEvent);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var pickupable = other.transform.parent?.GetComponent<IPickupable>();
        if (pickupable == null) return;

        ObjectOnTheTray = pickupable;

        Debug.Log("Enter");
    }

    private void OnTriggerExit(Collider other)
    {
        var pickupable = other.transform.parent?.GetComponent<IPickupable>();
        if (pickupable != ObjectOnTheTray) return;

        ObjectOnTheTray = null;

        Debug.Log("Exit");
    }
}