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

public sealed class PlayerCharacter : CharacterBase
{
    // States
    public PlayerMoveState playerMoveState; // Player Walk & Jump & >> Idle <<
    public PlayerSwingState playerSwingState; // Player golf swing
    public PlayerJumpState playerJumpState;

    public override PlayerCharacter AsPlayer()
    {
        return this;
    }

    protected override void Awake()
    {
        base.Awake();

        playerMoveState = new PlayerMoveState(this, rigidBody, animator);
        playerSwingState = new PlayerSwingState(this, rigidBody, animator);
        playerJumpState = new PlayerJumpState(this, rigidBody, animator);
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
    private void OnMove(InputValue value)
    {
        playerMoveState.OnMove(value);
    }

    private void OnJump()
    {
        ChangeState(playerJumpState);
    }

    private void OnClick()
    {
        if (CurrenState == playerMoveState)
        {
            ChangeState(playerSwingState);
            ChangeAnimationState(PlayerCharacterAnimation.State.Swing);
        }

        playerSwingState.OnClick();
    }

    private void OnDrag(InputValue value)
    {
        playerSwingState.OnDrag(value);
    }
}