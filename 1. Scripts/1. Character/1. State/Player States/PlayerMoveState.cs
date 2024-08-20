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

    private float velocityToBeSet;

    private const float animationTransitionTime = 0.1f;

    public override void EnterState()
    {
        base.EnterState();

        player.AnimationController.SetSpeed(AnimationController.Speed.Normal);
        player.AnimationController.ChangeState(AnimationController.Player.Movement.BT_1, animationTransitionTime);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        HandleAnimation();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        MoveTowards(sharedData.directionToMove);
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void MoveTowards(EMovementDirection direction)
    {
        if (direction == EMovementDirection.None) return;
        if (direction != player.MovementController.Direction) return;

        if (Math.Abs(velocityToBeSet) < player.Info.MovementSpeed)
        {
            velocityToBeSet += player.Info.Acceleration * Time.deltaTime;
            player.MovementController.SetVelocity(velocityToBeSet);
        }
        else
        {
            player.MovementController.SetVelocity(player.Info.MovementSpeed);
        }
    }

    private void HandleAnimation()
    {
        //if (time < animationTransitionTime) return;

        var param_MoveSpeed = AnimationController.Parameter.MoveSpeed;
        var velocity = MathF.Abs(player.MovementController.GetVelocity());

        player.AnimationController.SetFloat(param_MoveSpeed, velocity);
    }

    private void OnMove(EMovementDirection newDirection)
    {
        velocityToBeSet = 0;
        sharedData.directionToMove = newDirection;

        if (!player.CurrenState.CompareState(player.MoveState)) return;

        player.MovementController.ChangeMovementDirection(sharedData.directionToMove);
    }

    private void OnLand(EMovementDirection direction)
    {
        if (sharedData.directionToMove == EMovementDirection.None) return;
        if (sharedData.directionToMove == direction) return;

        player.MovementController.ChangeMovementDirection(sharedData.directionToMove);
        player.MovementController.SetVelocity(velocityToBeSet = 0);
    }
}