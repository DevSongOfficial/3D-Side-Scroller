using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameSystem;

public sealed class PlayerCharacter : CharacterBase
{
    // Player Info
    public new PlayerInfo Info => info.AsPlayerInfo();

    // States
    // Must excute the contructor in Awake() after declaring new state here.
    public StateBase MoveState { get; private set; } // Player Walk & Jump & Idle.
    public StateBase SwingState { get; private set; } // Player golf swing which can be a normal swing or a powerful attack.
    public StateBase AttackState { get; private set; } // Player basic attack.
    public StateBase JumpState { get; private set; }
    public StateBase OnVehiclState { get; private set; } // Player driving vehicle including being controlled by other gameobject(like zombie).

    // Black Board
    private PlayerBlackboard sharedData;

    // Item Equipment
    [Header("Item Equipment")]
    [SerializeField] private ItemHolder itemHolder_1;
    [SerializeField] private Transform itemHolder_2;
    [SerializeField] private GolfClub equippedClubOnStart;
    private IPickupable currentlyCarriedObject;
    public bool IsCarryingObject => currentlyCarriedObject != null;

    protected override void Awake()
    {
        base.Awake();

        // Initialize Inputs
        GameManager.Input_OnMove += OnMove;
        GameManager.Input_OnJump += OnJump;
        GameManager.Input_OnClick += OnClick;
        GameManager.Input_OnDrag += OnDrag;
        GameManager.Input_OnInteract += OnInteract;
        GameManager.Input_OnSwitchClub += OnSwitchClub;
        GameManager.Input_OnTogglePickup += OnTogglePickup;

        // Initialize behaviour states
        sharedData = new PlayerBlackboard();
        MoveState = new PlayerMoveState(this, sharedData);
        SwingState = new PlayerSwingState(this, sharedData);
        AttackState = new PlayerAttackState(this, sharedData);
        JumpState = new PlayerJumpState(this, sharedData);
        OnVehiclState = new PlayerOnVehicleState(this, sharedData);

        // Initialize Interactor
        Interactor.AddGolfer(itemHolder_1).AddDriver();
        Interactor.AsDriver.OnEnterVehicle += () => ChangeState(OnVehiclState);
        Interactor.AsDriver.OnExitVehicle += () => ChangeState(MoveState);
    }

    protected override void Start()
    {
        base.Start();

        Interactor.AsGolfer.EquipClub(equippedClubOnStart);

        AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, AnimationController.WeightType.Off);

        ChangeState(MoveState);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    #region Input Actions
    private void OnMove(EMovementDirection directionToMove)
    {
        sharedData.Input_ChangeDirection.Invoke(directionToMove);
    }

    private void OnJump()
    {
        if (!CurrenState.CompareState(MoveState)) return;
        if (!Detector.GroundDetected()) return;
        
        ChangeState(JumpState);
    }

    private void OnClick(bool mouseDown)
    {
        if(mouseDown)
        {
            sharedData.Input_MouseDown?.Invoke();
            
            if (CurrenState.CompareState(MoveState)) ChangeState(AttackState);
        }
        else // if (mouseUp)
        {
            sharedData.Input_MouseUp?.Invoke();
        }
    }

    private void OnDrag(Vector2 mousePosition)
    {
        sharedData.Input_Drag?.Invoke(mousePosition);
    }
    
    private void OnInteract() 
    {
        Interactor.FindAndInteractWithinRange(info.InteractionRange);
    }

    private void OnSwitchClub()
    {
        Interactor.AsGolfer.InvokeEvent_OnClubSwitched(Interactor);
    }

    private void OnTogglePickup()
    {
        if (!CurrenState.CompareState(MoveState)) return;

        if (IsCarryingObject)
        {
            currentlyCarriedObject.OnDropedOff();
            currentlyCarriedObject = null;
            AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, AnimationController.WeightType.Off);
            return;   
        }

        var pickupables = Detector.ComponentsDetected<IPickupable>(Detector.ColliderCenter, 1.5f, Layer.InteractableObject.GetMask());
        foreach (var pickupable in pickupables)
        {
            currentlyCarriedObject = pickupable;
            currentlyCarriedObject.OnPickedUp(itemHolder_2);
            AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, AnimationController.WeightType.On);
            return;
        }
    }

    #endregion

    public override void TakeDamage(DamageEvent damageEvent)
    {
        base.TakeDamage(damageEvent);
    }

    public override PlayerCharacter AsPlayer()
    {
        return this;
    }
}