using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) 
    {
        sharedData.OnPlayerLand += OnLand;
    }

    private float velocityX;
    public EMovementDirection MovementDirection => (EMovementDirection)movementValue;
    private int movementValue;
    public void SetMovementValue(int value)
    {
        movementValue = value;
    }

    public override void EnterState()
    {
        base.EnterState();

        player.animationController.SetSpeed(1);
        player.animationController.ChangeState(AnimationController.State.PlayerMove1);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        HandleAnimation();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        HandleMovement();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void HandleMovement()
    {
        if (movementValue == 0) return;

        if (Math.Abs(velocityX) < player.Info.MovementSpeed)
        {
            velocityX += player.Info.Acceleration * Time.deltaTime;
            player.movementController.SetVelocity(velocityX);
        }
        else
        {
            player.movementController.SetVelocity(player.Info.MovementSpeed);
        }
    }

    private void HandleAnimation()
    {
        var param_MoveSpeed = AnimationController.Parameter.MoveSpeed;
        var velocity = MathF.Abs(player.movementController.GetVelocity());

        player.animationController.SetFloat(param_MoveSpeed, velocity);
    }

    public void OnMove(InputValue value)
    {
        velocityX = 0;

        if ((int)value.Get<float>() == 0)
        {
            return;
        }

        player.movementController.ChangeMovementDirection(MovementDirection);
    }

    private void OnLand(EMovementDirection direction)
    {
        if (MovementDirection == EMovementDirection.None) return;
        if (MovementDirection == direction) return;

        player.movementController.ChangeMovementDirection(MovementDirection);
        player.movementController.SetVelocity(velocityX = 0);
    }
}