using System;
using UnityEngine;
using static AnimationController;
using static GameSystem;

public class GolfCart : MonoBehaviour, IInteractable
{
    // The character who currently has access to or is interacting with the bag.
    private Interactor driver;
    public bool IsTaken => driver != null;

    private MovementController movementController;

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
        movementController = GetComponent<MovementController>();
        detector = GetComponent<Detector>();
    }

    private void Start()
    {
        movementController.ChangeMovementDirection(EMovementDirection.Right, smoothRotation: false);
    }

    private void FixedUpdate()
    {
        HandleMovement();
        AttackOnCollide();
    }

    private void LateUpdate()
    {
        if (!IsTaken) return;

        driver.AsDriver.InvokeEvent_OnDrive(this, transform.position, transform.eulerAngles);
    }

    private void GetInTheCart(Interactor driver)
    {
        this.driver = driver;
        driver.AsDriver.InvokeEvent_OnEnterVehicle(this); // MoveState -> OnVehicleState

        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate);
        GameManager.Input_OnMove += SetWishDirection;

        ObjectOnTheTray?.OnPickedUp(cargoTray, shouldAlignToCenter: false);
    }

    private void GetOutOfTheCart()
    {
        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.SmartUpdate);
        GameManager.Input_OnMove -= SetWishDirection;

        driver.AsDriver.InvokeEvent_OnExitVehicle(this); // OnVehicleState -> MoveState
        driver = null;

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

    private float wishVelocity;
    private EMovementDirection wishDirection; // Direction where player's about to move.
    private void SetWishDirection(EMovementDirection newDirection)
    {
        wishDirection = newDirection;

        if (newDirection != movementController.Direction)
            wishVelocity = 0;

        movementController.ChangeMovementDirection(wishDirection, Space.Self);
    }

    private void HandleMovement()
    {
        var velocity = movementController.CalculateVelocity(info.MovementSpeed, info.Acceleration, info.Mass);

        if (!IsTaken || wishDirection == EMovementDirection.None)
            velocity.x = 0;

        movementController.Move(velocity * Time.fixedDeltaTime);
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