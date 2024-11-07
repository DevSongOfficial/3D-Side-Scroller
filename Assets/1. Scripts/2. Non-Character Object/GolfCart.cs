using System;
using UnityEngine;
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

    // Physics
    private Rigidbody rigidBody;
    private Detector detector;
    private Vector3 offset_CollisionPosition => new Vector3((int)movementController.FacingDirection, -0.3f, 0f);
    private readonly float collisionRadius = 1;

    // Cargo Tray
    private IPickupable carryingItem;
    [SerializeField] private Transform carryPoint;
    public Transform CarryPoint => carryPoint;

    private void Awake()
    {
        movementController = GetComponent<CartMovementController>();
        detector = GetComponent<Detector>();
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        movementController.ChangeMovementDirection(MovementDirection.Right, smoothRotation: false);

        // Initialize cart
        rigidBody.isKinematic = true;
        movementController.ToggleHorizontalMovement(false);
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

        driver.AsDriver.InvokeEvent_OnDrive(transform.position, transform.eulerAngles);
    }

    private void GetInTheCart(Interactor driver)
    {
        this.driver = driver;
        driver.AsDriver.InvokeEvent_OnEnterVehicle();

        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.FixedUpdate);
        GameManager.Input_OnChangeDirection += OnChangeDirection;

        movementController.ToggleHorizontalMovement(true);
        rigidBody.isKinematic = false;
    }

    private void GetOutOfTheCart()
    {
        GameManager.SetCameraUpdateMethod(Cinemachine.CinemachineBrain.UpdateMethod.SmartUpdate);
        GameManager.Input_OnChangeDirection -= OnChangeDirection;

        driver.AsDriver.InvokeEvent_OnExitVehicle();
        driver = null;

        rigidBody.isKinematic = true;
        movementController.ToggleHorizontalMovement(false);
        velocityMultiplier = 0;
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

    public new InteractableType GetType() => InteractableType.Vehicle;


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

    private void AttackOnCollide()
    {
        if (!IsTaken) return;
        if (movementController.Velocity.x < 2.9f) return;

        if (detector.DetectCharacters(transform.position + offset_CollisionPosition, collisionRadius, out CharacterBase[] characters))
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

    public void LoadTheTrunk(IPickupable pickupable)
    {
        carryingItem = pickupable;
        carryingItem?.OnPickedUp(carryPoint);
    }
}