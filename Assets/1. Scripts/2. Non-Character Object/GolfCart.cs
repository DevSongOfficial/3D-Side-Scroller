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
        movementController.AlignToGround();

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
        movementController.StopAndChangeDirection(newDirection);
    }

    private void HandleMovement()
    {
        movementController.ApplyHorizontalVelocity(info.MovementSpeed * velocityMultiplier, info.Acceleration);
    }

    void AttackOnCollide()
    {
        if (!IsTaken) return;
        if (movementController.Velocity.x < 3.5f) return;

        if (detector.CharactersDetected(transform.position + offset_CollisionPosition, collisionRadius, out CharacterBase[] characters))
        {
            foreach (var character in characters)
            {
                var newDamageEvent = damageEvent.
                    MultiplyVelocity(movementController.Velocity.x * 0.5f).
                    ApplyDirection(movementController.FacingDirection);

                character?.TakeDamage(newDamageEvent);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var pickupable = other.transform.parent?.GetComponent<IPickupable>();
        if (pickupable == null) return;

        ObjectOnTheTray = pickupable;

    }
}