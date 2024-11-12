using UnityEngine;
using static GameSystem;

public sealed class PlayerCharacter : CharacterBase
{
    // Player Info
    public new PlayerInfo Info => info.AsPlayerInfo();

    // States
    // Must excute the contructor in Awake() after declaring new state here.
    public StateBase MoveState      { get; private set; } // Player Walk & Jump & Idle.
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

        // Initialize Inputs
        GameManager.Input_OnChangeDirection += OnChangeDirection;
        GameManager.Input_OnJump            += OnJump;
        GameManager.Input_OnClick           += OnClick;
        GameManager.Input_OnDrag            += OnDrag;
        GameManager.Input_OnInteract        += OnInteract;
        GameManager.Input_OnSwitchClub      += OnSwitchClub;
        GameManager.Input_OnTogglePickup    += OnTogglePickup;

        // Initialize behaviour states
        blackboard      = new PlayerBlackboard();
        MoveState       = new PlayerMoveState(this, blackboard);
        JumpState       = new PlayerJumpState(this, blackboard);
        SwingState      = new PlayerSwingState(this, blackboard);
        AttackState     = new PlayerAttackState(this, blackboard);
        OnVehiclState   = new PlayerOnVehicleState(this, blackboard);

        // Initialize Interactor
        Interactor.AddGolfer(itemHolder_1)
                  .AddCarrier(itemHolder_2)
                  .AddDriver();
        Interactor.AsDriver.OnEnterVehicle += () => ChangeState(OnVehiclState);
        Interactor.AsDriver.OnExitVehicle  += () => ChangeState(MoveState);

        GameManager.OnLoadComplete += Reposition;
    }

    protected override void Start()
    {
        base.Start();

        Interactor.AsGolfer.EquipClub(clubOnStart);

        AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, AnimationController.UpperLayer.Off);

        ChangeState(MoveState);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.M))
        {
            Reposition();
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    #region Input Actions
    private void OnChangeDirection(MovementDirection directionToMove)
    {
        blackboard.Input_ChangeDirection?.Invoke(directionToMove);
        blackboard.InputDirection = directionToMove;
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
        Interactor.FindAndInteractWithinRange(info.InteractionRange);
    }

    private void OnSwitchClub()
    {
        Interactor.AsGolfer.InvokeEvent_OnClubSwitched();
    }

    private void OnTogglePickup()
    {
        if (!CurrenState.CompareState(MoveState)) return;

        if (Interactor.Toggle_FindAndLoadCart())
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

    // Transfer player to a spawnpoint.
    private void Reposition()
    {
        var po = FindObjectOfType<PlaceableSpawnPoint>();
        if (po == null) return;

        var newPosition = po.transform.position;
        MovementController.SetPosition(newPosition);
    }

    public override PlayerCharacter AsPlayer()
    {
        return this;
    }
}