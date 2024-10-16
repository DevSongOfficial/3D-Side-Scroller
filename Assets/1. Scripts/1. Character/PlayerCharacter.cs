using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using static AnimationController;
using static GameSystem;

public sealed class PlayerCharacter : CharacterBase
{
    public CharacterController Controller { get; private set; }

    // Player Info
    public new PlayerInfo Info => info.AsPlayerInfo();

    // States
    // Must excute the contructor in Awake() after declaring new state here.
    public StateBase MoveState { get; private set; } // Player Walk & Jump & Idle.
    public StateBase SwingState { get; private set; } // Player golf swing which can be a normal swing or a powerful attack.
    public StateBase AttackState { get; private set; } // Player basic attack.
    public StateBase OnVehiclState { get; private set; } // Player driving vehicle including being controlled by other gameobject(like zombie).

    // Black Board
    private PlayerBlackboard blackboard;

    // Item Equipment
    [Header("Item Equipment")]
    [SerializeField] private ItemHolder itemHolder_1; // Clubs, weapons, ...
    [SerializeField] private Transform itemHolder_2;  // Golf Bag, ...
    [SerializeField] private GolfClub equippedClubOnStart;
    private IPickupable currentlyCarriedObject;
    public bool IsCarryingObject => currentlyCarriedObject != null;

    // Effect
    [Header("Effect")]
    [SerializeField] private Renderer auraEffect;
    public void SetAuraAlpha(float alphaMultiplier)
    {
        auraEffect.sharedMaterial.SetFloat("_AlphaMultiplier", alphaMultiplier);
    }


    protected override void Awake()
    {
        base.Awake();

        Controller = GetComponent<CharacterController>();

        // Initialize Inputs
        GameManager.Input_OnMove         += OnMove;
        GameManager.Input_OnJump         += OnJump;
        GameManager.Input_OnClick        += OnClick;
        GameManager.Input_OnDrag         += OnDrag;
        GameManager.Input_OnInteract     += OnInteract;
        GameManager.Input_OnSwitchClub   += OnSwitchClub;
        GameManager.Input_OnTogglePickup += OnTogglePickup;

        // Initialize behaviour states
        blackboard =    new PlayerBlackboard();
        MoveState =     new PlayerMoveState(this, blackboard);
        SwingState =    new PlayerSwingState(this, blackboard);
        AttackState =   new PlayerAttackState(this, blackboard);
        OnVehiclState = new PlayerOnVehicleState(this, blackboard);

        // Initialize Interactor
        Interactor.AddGolfer(itemHolder_1).AddDriver();
        Interactor.AsDriver.OnEnterVehicle += () => ChangeState(OnVehiclState);
        Interactor.AsDriver.OnExitVehicle  += () => ChangeState(MoveState);
    }

    protected override void Start()
    {
        base.Start();

        Interactor.AsGolfer.EquipClub(equippedClubOnStart);

        AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, AnimationController.UpperLayer.Off);

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
        blackboard.Input_ChangeDirection.Invoke(directionToMove);
    }

    private void OnJump()
    {
        blackboard.Input_OnJump.Invoke();
    }

    private void OnClick(bool mouseDown)
    {
        if(mouseDown)
        {
            blackboard.Input_MouseDown?.Invoke();
            
            if (CurrenState.CompareState(MoveState)) ChangeState(AttackState);
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
        Interactor.AsGolfer.InvokeEvent_OnClubSwitched(Interactor);
    }

    private void OnTogglePickup()
    {
        if (!CurrenState.CompareState(MoveState)) return;

        if (IsCarryingObject)
        {
            currentlyCarriedObject.OnDropedOff();
            currentlyCarriedObject = null;
            AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, AnimationController.UpperLayer.Off);
            return;   
        }

        var pickupables = Detector.ComponentsDetected<IPickupable>(Detector.ColliderCenter, 1.5f, Layer.InteractableObject.GetMask());
        foreach (var pickupable in pickupables)
        {
            currentlyCarriedObject = pickupable;
            currentlyCarriedObject.OnPickedUp(itemHolder_2, shouldAlignToCenter: true);
            AnimationController.SetLayerWeight(AnimationController.Layer.UpperLayer, AnimationController.UpperLayer.On);
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