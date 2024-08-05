using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) 
    {
        sharedData.OnLand += OnLand;
        sharedData.OnMove += OnMove;
    }

    private float velocityX;

    // [MovementController.Direction] refers to the direction the character is facing at tho moment,
    // On the other hand, [directionToMove] refers to the way to be aksed to go at the moment.
    private EMovementDirection directionToMove;

    public override void EnterState()
    {
        base.EnterState();

        player.AnimationController.SetSpeed(AnimationController.Speed.Normal);
        player.AnimationController.ChangeState(AnimationController.State.PlayerMove1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        HandleAnimation();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        MoveTowards(directionToMove);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void MoveTowards(EMovementDirection direction)
    {
        if (direction == EMovementDirection.None) return;
        if (direction != player.MovementController.Direction) return;

        if (Math.Abs(velocityX) < player.Info.MovementSpeed)
        {
            velocityX += player.Info.Acceleration * Time.deltaTime;
            player.MovementController.SetVelocity(velocityX);
        }
        else
        {
            player.MovementController.SetVelocity(player.Info.MovementSpeed);
        }
    }

    private void HandleAnimation()
    {
        var param_MoveSpeed = AnimationController.Parameter.MoveSpeed;
        var velocity = MathF.Abs(player.MovementController.GetVelocity());

        player.AnimationController.SetFloat(param_MoveSpeed, velocity);
    }

    private void OnMove(EMovementDirection newDirection)
    {
        velocityX = 0;
        directionToMove = newDirection;
        player.MovementController.ChangeMovementDirection(directionToMove);
    }

    private void OnLand(EMovementDirection direction)
    {
        if (directionToMove == EMovementDirection.None) return;
        if (directionToMove == direction) return;

        player.MovementController.ChangeMovementDirection(directionToMove);
        player.MovementController.SetVelocity(velocityX = 0);
    }
}