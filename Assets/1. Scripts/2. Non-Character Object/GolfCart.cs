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
    private Vector3 offset_CollisionPosition => new Vector3((int)movementController.FacingDirection * 1.5f, -0.3f, 0f);
    private readonly float collisionRadius = 1;

    // Cargo Tray
    private Transform ObjectOnTheTray;
    [SerializeField] private Transform cargoTray;

    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        detector = GetComponent<Detector>();
    }

    private void Start()
    {
        movementController.FreezePosition(MovementController.FreezeRotationYandZ);
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

        driver.AsDriver.InvokeEvent_OnDrive(this, transform.position, transform.eulerAngles.x);
    }

    private void GetInTheCart(Interactor driver)
    {
        this.driver = driver;

        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate);

        GameManager.Input_OnMove += SetWishDirection;
        movementController.OnDirectionChange += ChangeDirection;
        movementController.StopMovement();

        driver.AsDriver.InvokeEvent_OnEnterVehicle(this);
        driver.AsDriver.InvokeEvent_OnChangeDirection(this, movementController.FacingDirection);

        movementController.UnfreezePosition(MovementController.FreezeRotationYandZ);
    }

    private void GetOutOfTheCart()
    {
        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.SmartUpdate);

        GameManager.Input_OnMove -= SetWishDirection;
        movementController.OnDirectionChange -= ChangeDirection;

        driver.AsDriver.InvokeEvent_OnExitVehicle(this);

        driver = null;

        movementController.FreezePosition(MovementController.FreezeRotationYandZ);
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

        movementController.ChangeMovementDirection(wishDirection);
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

    private void ChangeDirection(EMovementDirection direction) => driver.AsDriver.InvokeEvent_OnChangeDirection(this, direction);

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
        if (other.transform.parent?.GetComponent<IPickupable>() == null) return;

        ObjectOnTheTray = other.transform.parent;
        ObjectOnTheTray.SetParent(cargoTray);
    }

    private void OnTriggerExit(Collider other)
    {
        if (ObjectOnTheTray != other.transform.parent) return;

        ObjectOnTheTray.SetParent(null);
        ObjectOnTheTray = null;
    }
}