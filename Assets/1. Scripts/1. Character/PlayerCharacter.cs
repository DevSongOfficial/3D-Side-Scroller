using UnityEngine;
using static GameSystem;

public sealed class PlayerCharacter : CharacterBase
{
    // Player Info
    public new PlayerInfo Info => info.AsPlayerInfo();

    // Golf Stroke
    public byte StrokeCount { get; private set; } = 0;
    public void IncrementStroke() => StrokeCount++;
    public void IntializeStroke() => StrokeCount = 0;

    // States
    // Must excute the contructor in Awake() after declaring new state here.
    public StateBase MoveState      { get; private set; } // Player Walk & Jump & Idle.
    public StateBase ZAxisMoveState { get; private set; } // Z axis movement only for special purpose.
    public StateBase JumpState      { get; private set; }
    public StateBase SwingState     { get; private set; } // Player golf swing which can be a normal swing or a powerful attack.
    public StateBase AttackState    { get; private set; } // Player basic attack.
    public StateBase OnVehiclState  { get; private set; } // Player driving vehicle including being controlled by other gameobject(like zombie).

    // Black Board
    private PlayerBlackboard blackboard;

    // Item Equipment
    [Header("Item Equipment")]
    [SerializeField] private ItemHolder itemHolder_1; // Clubs, weapons, ...
    [SerializeField] private Transform  itemHolder_2;  // Golf Bag, ...
    [SerializeField] private GolfClub   clubOnStart;
    public bool IsCarryingObject => currentlyCarriedObject != null;
    private IPickupable currentlyCarriedObject;

    protected override void Awake()
    {
        base.Awake();

        // Initialize inputs.
        InputManager.Input_OnChangeDirection     += OnChangeDirection;
        InputManager.Input_OnChangeZDirection    += OnChangeZDirection;
        InputManager.Input_OnJump                += OnJump;
        InputManager.Input_OnClick               += OnClick;
        InputManager.Input_OnDrag                += OnDrag;
        InputManager.Input_OnInteract            += OnInteract;
        InputManager.Input_OnSwitchClub          += OnSwitchClub;
        InputManager.Input_OnTogglePickup        += OnTogglePickup;

        // Initialize behaviour states.
        blackboard      = new PlayerBlackboard();
        MoveState       = new PlayerMoveState(this, blackboard);
        ZAxisMoveState  = new PlayerMoveOnZAxisState(this, blackboard);
        JumpState       = new PlayerJumpState(this, blackboard);
        SwingState      = new PlayerSwingState(this, blackboard);
        AttackState     = new PlayerAttackState(this, blackboard);
        OnVehiclState   = new PlayerOnVehicleState(this, blackboard);

        // Initialize Interactor.
        Interactor.AddGolfer(itemHolder_1)
                  .AddCarrier(itemHolder_2)
                  .AddDriver();
        Interactor.AsDriver.OnEnterVehicle += () => ChangeState(OnVehiclState);
        Interactor.AsDriver.OnExitVehicle  += () => ChangeState(MoveState);

        // Initialize stroke count.
        GameManager.OnGameStart                     += IntializeStroke;
        GameManager.OnGameStart                     += Reposition;
        GameManager.OnStageSetup                    += Reposition;
        LevelEditorManager.OnEditorModeToggled      += SetActiveAndReposition;

        HealthSystem.OnCharacterDie += OnDie;
    }

    protected override void Start()
    {
        base.Start();

        Interactor.AsGolfer.EquipClub(clubOnStart);

        AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, AnimationController.UpperLayer.Off);

        ChangeState(MoveState);
    }

    #region Input Actions
    private void OnChangeDirection(MovementDirection directionToMove)
    {
        blackboard.Input_ChangeDirection?.Invoke(directionToMove);
        blackboard.InputDirection = directionToMove;
    }

    private void OnChangeZDirection(ZAxisMovementDirection directionToMove)
    {
        blackboard.Input_ChangeZDirection?.Invoke(directionToMove);
    }

    private void OnJump()
    {
        blackboard.Input_OnJump?.Invoke();
    }

    private void OnClick(bool mouseDown)
    {
        if(mouseDown)
        {
            blackboard.Input_MouseDown?.Invoke();
        }
        else // if (mouseUp)
        {
            blackboard.Input_MouseUp?.Invoke();
        }
    }

    private void OnDrag(Vector2 mousePosition)
    {
        blackboard.Input_Drag?.Invoke(mousePosition);
    }
    
    private void OnInteract() 
    {
        Interactor.TryInteract(info.InteractionRange);
    }

    private void OnSwitchClub()
    {
        Interactor.AsGolfer.InvokeEvent_OnClubSwitched();
    }

    private void OnTogglePickup()
    {
        if (!CurrenState.CompareState(MoveState)) return;

        if (Interactor.TryLoadToCart())
        {
            AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, AnimationController.UpperLayer.Off);
            return;
        }

        if (Interactor.Toggle_FindAndPickupWithinRange(info.PickupRange))
        {
            var layer = Interactor.AsCarrier.IsCarryingItem ? AnimationController.UpperLayer.On : AnimationController.UpperLayer.Off;
            AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, layer);
        }
    }

    #endregion

    public override void TakeDamage(DamageEvent damageEvent)
    {
        base.TakeDamage(damageEvent);
        MovementController.StunCharacter();
    }

    private void SetActiveAndReposition(bool active)
    {
        gameObject.SetActive(!active);
        Reposition();
    }

    // Transfer player to a spawnpoint.
    private void Reposition()
    {
        if (!POFactory.HasRegisteredSingletonPO<PlaceableSpawnPoint>()) return;

        var newPosition = POFactory.GetRegisteredSingletonPO<PlaceableSpawnPoint>().GetPosition();
        MovementController.SetPosition(newPosition);
        MovementController.ChangeMovementDirection(MovementDirection.Right, smoothRotation: false);
        MovementController.SetVelocityMultiplier(1, smoothTransition: false);
    }

    private void OnDie()
    {
        Reposition();
    }

    public override PlayerCharacter AsPlayer()
    {
        return this;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (other.CompareTag(Tag.Green))
        {
            if (MovementController.CurrentZAxis == ZAxisMovementDirection.Up) return;
            blackboard.InputZDirection = ZAxisMovementDirection.Up;
            ChangeState(ZAxisMoveState);
        }
    }
}