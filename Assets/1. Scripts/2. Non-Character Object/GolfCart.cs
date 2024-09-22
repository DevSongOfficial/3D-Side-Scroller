using System;
using UnityEngine;
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
        movementController.FreezePosition(MovementController.FreezeRotation);
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

        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate);

        GameManager.Input_OnMove += SetWishDirection;
        movementController.StopMovement();

        driver.AsDriver.InvokeEvent_OnEnterVehicle(this);

        movementController.UnfreezePosition(MovementController.FreezeRotationYandZ);
        
        ObjectOnTheTray?.OnPickedUp(cargoTray, shouldAlignToCenter: false);
    }

    private void GetOutOfTheCart()
    {
        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.SmartUpdate);

        GameManager.Input_OnMove -= SetWishDirection;

        driver.AsDriver.InvokeEvent_OnExitVehicle(this);

        driver = null;

        movementController.FreezePosition(MovementController.FreezeRotation);

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
        if (!IsTaken) return;

        if (wishDirection == EMovementDirection.None)
        {
            wishVelocity = 0;
            return;
        }

        if (Math.Abs(wishVelocity) < info.MovementSpeed)
        {
            wishVelocity += info.Acceleration * Time.deltaTime;
        }
        else
        {
            wishVelocity = info.MovementSpeed;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f, Layer.Default.GetMask()))
        {
            // If surface is flat or downhill, apply only x velocity.
            if(hit.normal.y > 0.99f || (int)movementController.FacingDirection * hit.normal.x > 0)
            {
                movementController.SetVelocity(wishVelocity);
            }
            else // If not, apply both x and y with the normal vector.
            {
                Vector3 moveDirection = new Vector3(wishVelocity, 0, 0);
                Vector3 projectedMoveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
                movementController.SetVelocity(projectedMoveDirection.x, projectedMoveDirection.y);
            }
        }
    }

    void AttackOnCollide()
    {
        if (!IsTaken) return;
        if (movementController.GetVelocity() < 3.5f) return;

        if (detector.CharactersDetected(transform.position + offset_CollisionPosition, collisionRadius, out Collider[] colliders))
        {
            foreach (Collider collider in colliders)
            {
                var character = collider.GetComponentInParent<ZombieCharacter>();
                if (character is null) continue;

                var newDamageEvent = damageEvent.
                    MultiplyKnockback(movementController.GetVelocity() * 0.5f).
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
    }

    private void OnTriggerExit(Collider other)
    {
        var pickupable = other.transform.parent?.GetComponent<IPickupable>();
        if (pickupable != ObjectOnTheTray) return;

        ObjectOnTheTray = null;
    }
}