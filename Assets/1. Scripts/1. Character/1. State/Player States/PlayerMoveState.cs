using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) 
    {
        sharedData.Input_ChangeDirection += SetWishDirection;
    }


    private float wishVelocity;
    private EMovementDirection wishDirection; // Direction where player's about to move.
    private void SetWishDirection(EMovementDirection newDirection)
    {
        wishDirection = newDirection;

        if (newDirection != player.MovementController.Direction)
            wishVelocity = 0;

        if (player.CurrenState.CompareState(player.MoveState))
            player.MovementController.ChangeMovementDirection(wishDirection);
    }

    private const float animationTransitionTime = 0.1f;

    public override void EnterState()
    {
        base.EnterState();

        // Set up animation.
        player.AnimationController.SetSpeed(AnimationController.Speed.Normal);
        player.AnimationController.ChangeState(AnimationController.Player.Movement.BT_1, animationTransitionTime);

        player.MovementController.ChangeMovementDirection(wishDirection);

        if (!player.PreviousState.CompareState(player.JumpState)) 
            wishVelocity = 0;
    }
    
    public override void UpdateState()
    {
        base.UpdateState();

        HandleAnimation();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        if (player.IsCarryingObject) 
            HandleSlowMovement();
        else                         
            HandleMovement();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void HandleMovement()
    {
        if (wishDirection == EMovementDirection.None)
        {
            wishVelocity = 0;
            return;
        }

        if (Math.Abs(wishVelocity) < player.Info.MovementSpeed)
        {
            wishVelocity += player.Info.Acceleration * Time.deltaTime;
        }
        else
        {
            wishVelocity = player.Info.MovementSpeed;
        }

        player.MovementController.SetVelocity(wishVelocity);
    }

    private void HandleSlowMovement()
    {
        if (wishDirection == EMovementDirection.None)
        {
            wishVelocity = 0;
            return;
        }

        if (Math.Abs(wishVelocity) < player.Info.MovementSpeed * 0.5f)
        {
            wishVelocity += player.Info.Acceleration * Time.deltaTime;
            player.MovementController.SetVelocity(wishVelocity * 0.5f);
        }
        else
        {
            player.MovementController.SetVelocity(player.Info.MovementSpeed * 0.5f);
        }
    }

    private void HandleAnimation()
    {
        var param_MoveSpeed = AnimationController.Parameter.MoveSpeed;
        var velocity = MathF.Abs(player.MovementController.GetVelocity());

        player.AnimationController.SetFloat(param_MoveSpeed, velocity);
    }
}