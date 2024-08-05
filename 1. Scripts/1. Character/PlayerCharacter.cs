using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public sealed class PlayerCharacter : CharacterBase
{
    [Header("Player Information")]
    [SerializeField] private PlayerInfo info;
    public PlayerInfo Info => info;

    // States
    // Must excute the contructor in Awake() after adding new state.
    public StateBase MoveState { get; private set; } // Player Walk & Jump & >> Idle <<
    public StateBase SwingState { get; private set; } // Player golf swing
    public StateBase JumpState { get; private set; }

    // Black Board
    private PlayerBlackboard playerBlackboard;

    protected override void Awake()
    {
        base.Awake();

        playerBlackboard = new PlayerBlackboard();

        MoveState = new PlayerMoveState(this, playerBlackboard);
        SwingState = new PlayerSwingState(this, playerBlackboard);
        JumpState = new PlayerJumpState(this, playerBlackboard);
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
        if (!CurrenState.CompareState(MoveState)) return;
        
        var valueConverted = (int)value.Get<float>();
        var directionToMove = (EMovementDirection)valueConverted;

        playerBlackboard.OnMove.Invoke(directionToMove);
    }

    private void OnJump() // [Space Bar] pressed
    {
        if (!CurrenState.CompareState(MoveState)) return;
        if (!Detector.GroundDetected()) return;
        
        ChangeState(JumpState);
    }

    private void OnClick() // [Mouse 0] pressed
    {
        playerBlackboard.OnClick.Invoke();

        if (CurrenState.CompareState(MoveState)) ChangeState(SwingState);
    }

    private void OnDrag(InputValue value) // [Cursor position] changed
    {
        var mousePosition = value.Get<Vector2>();
        playerBlackboard.OnDrag.Invoke(mousePosition);
    }

    private void OnInteract() // [E] pressed
    {
        //if (interactableObject == null) return;
        //interactableObject.Interact(this);
    }
    #endregion

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