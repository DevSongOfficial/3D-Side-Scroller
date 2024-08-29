using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class PlayerCharacter : CharacterBase
{
    [Header("Player Information")]
    [SerializeField] private PlayerInfo info;
    public PlayerInfo Info => info;

    // States
    // Must excute the contructor in Awake() after declaring new state here.
    public StateBase MoveState { get; private set; } // Player Walk & Jump & Idle.
    public StateBase SwingState { get; private set; } // Player golf swing which can be a normal swing or a powerful attack.
    public StateBase AttackState { get; private set; } // Player basic attack.
    public StateBase JumpState { get; private set; }

    // Black Board
    private PlayerBlackboard sharedData;

    // Input Action
    private PlayerInput input;

    protected override void Awake()
    {
        base.Awake();

        LevelEditorManager.OnEditorModeTriggered += (bool active) => input.enabled = !active;

        gameObject.SetLayer(Layer.Character);

        input = GetComponent<PlayerInput>();

        sharedData = new PlayerBlackboard();

        MoveState = new PlayerMoveState(this, sharedData);
        SwingState = new PlayerSwingState(this, sharedData);
        AttackState = new PlayerAttackState(this, sharedData);
        JumpState = new PlayerJumpState(this, sharedData);
    }

    protected override void Start()
    {
        base.Start();

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

    // Automatically called by Player Input Action
    #region Input Action
    private void OnMove(InputValue value) // [A/D], [LeftArrow/RightArrow] key down & up
    {
        var valueConverted = (int)value.Get<float>();
        var directionToMove = (EMovementDirection)valueConverted;

        sharedData.Input_ChangeDirection.Invoke(directionToMove);
    }

    private void OnJump() // [Space Bar] pressed
    {
        if (!CurrenState.CompareState(MoveState)) return;
        if (!Detector.GroundDetected()) return;
        
        ChangeState(JumpState);
    }

    private void OnClick(InputValue value) // [Mouse 0]
    {
        var mouseDown = (int)value.Get<float>() == 1;

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


    private void OnDrag(InputValue value) // [Cursor position] changed
    {
        var mousePosition = value.Get<Vector2>();
        sharedData.Input_Drag?.Invoke(mousePosition);
    }
    
    private void OnInteract() // [E] pressed
    {
        //if (interactableObject is null) return;
        //interactableObject.Interact(this);
    }
    #endregion

    public override void TakeDamage(DamageEvent damageEvent)
    {
        base.TakeDamage(damageEvent);

        Info.TakeDamage(damageEvent.damage);
    }

    // This wrapping function is for letting states be able to start coroutine cuz they are not monobehaviour.
    public new Coroutine StartCoroutine(IEnumerator routine)
    {
        return base.StartCoroutine(routine);
    }

    public override PlayerCharacter AsPlayer()
    {
        return this;
    }
}