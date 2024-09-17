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
    private Vector3 offset_CollisionPosition => new Vector3((int)movementController.FacingDirection * 1.5f, -0.3f, 0f);
    private readonly float collisionRadius = 1;


    private void Awake()
    {
        movementController = GetComponent<MovementController>();
        detector = GetComponent<Detector>();

        movementController.FreezePosition();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        AttackOnCollide();
    }

    private void LateUpdate()
    {
        if (!IsTaken) return;

        driver.AsDriver.InvokeEvent_OnDrive(this, transform.position);
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

        movementController.UnfreezePosition();
    }

    private void GetOutOfTheCart()
    {
        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.SmartUpdate);

        GameManager.Input_OnMove -= SetWishDirection;
        movementController.OnDirectionChange -= ChangeDirection;

        driver.AsDriver.InvokeEvent_OnExitVehicle(this);

        driver = null;

        movementController.FreezePosition();
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
            movementController.SetVelocity(wishVelocity);
        }
        else
        {
            movementController.SetVelocity(info.MovementSpeed);
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

                var knockBackVector = damageEvent.knockBackVector * movementController.GetVelocity() * 0.5f;
                character.TakeDamage(new DamageEvent(damageEvent.senderType, damageEvent.damage, knockBackVector));
            }
        }
    }
}