using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Switch;
using UnityEngine.TextCore.Text;
using static CharacterBase;

public sealed class PlayerCharacter : CharacterBase
{
    [Header("Zombie Information")]
    public PlayerInfo playerInfo;

    // States
    public PlayerMoveState playerMoveState { get; private set; } // Player Walk & Jump & >> Idle <<
    public PlayerSwingState playerSwingState { get; private set; } // Player golf swing
    public PlayerJumpState playerJumpState { get; private set; }

    // Black Board
    private PlayerBlackboard playerBlackboard;

    // Ground Check
    #region Ground Check
    private float groundCheckDistance = 1.05f;
    [SerializeField] private Transform groundCheckStartPoint;
    private readonly float PlayerColliderLength = 0.4f;
    public bool IsOnGround
    {
        get
        {
            bool groundLeft = Physics.Raycast(groundCheckStartPoint.position + new Vector3(PlayerColliderLength / 2f, 0, 0), 
                Vector3.down, out RaycastHit hitLeft, groundCheckDistance);
            bool groundRight = Physics.Raycast(groundCheckStartPoint.position - new Vector3(PlayerColliderLength / 2f, 0, 0),
                Vector3.down, out RaycastHit hitRight, groundCheckDistance);

            return groundLeft || groundRight;
        }
    }
    #endregion

    // Interactables
    private List<IInteractable> interactableObjectsInDistance;
    private IInteractable interactableObject;

    protected override void Awake()
    {
        base.Awake();

        playerBlackboard = new PlayerBlackboard(rigidBody, animator);

        playerMoveState = new PlayerMoveState(this, playerBlackboard);
        playerSwingState = new PlayerSwingState(this, playerBlackboard);
        playerJumpState = new PlayerJumpState(this, playerBlackboard);
    }

    protected override void Start()
    {
        base.Start();

        ChangeState(playerMoveState);
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
    private void OnMove(InputValue value)
    {
        playerMoveState.SetMovementValue((int)value.Get<float>());
        if (CurrenState == playerMoveState) playerMoveState.OnMove(value);
    }

    private void OnJump()
    {
        if(CurrenState == playerMoveState && IsOnGround) ChangeState(playerJumpState);
    }

    private void OnClick()
    {
        if (CurrenState == playerMoveState) ChangeState(playerSwingState);

        playerSwingState.OnClick();
    }

    private void OnDrag(InputValue value)
    {
        playerSwingState.OnDrag(value);
    }

    private void OnInteract()
    {
        if (interactableObject == null) return;

        interactableObject.Interact(this);
    }
    #endregion

    public override void FadeOut()
    {
        base.FadeOut();

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public override void FadeIn()
    {
        base.FadeIn();

        transform.GetChild(0).gameObject.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject collider = other.gameObject;

        if (GameManager.LayerCheck(collider, Layer.InteractableObject))
        {
            interactableObject = collider.GetComponent<IInteractable>();
        }
    }

    public override PlayerCharacter AsPlayer()
    {
        return this;
    }
}