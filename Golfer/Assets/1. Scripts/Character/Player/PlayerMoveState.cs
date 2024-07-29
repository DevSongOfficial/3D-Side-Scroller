using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static CharacterBase;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    private float velocityX;
    private float angleY;
    public bool canRotate;
    private EMovementDirection targetDirection;
    public EMovementDirection MovementDirection => (EMovementDirection)movementValue;
    private int movementValue;
    public void SetMovementValue(int value)
    {
        movementValue = value;
    }

    public override void EnterState()
    {
        base.EnterState();

        data.animator.speed = 1;
        player.ChangeAnimationState(Animation.State.BT_Move);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        UpdateMovement();
        UpdateRotation();
        HandleMoveAnimation();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    public void UpdateMovement()
    {
        /*if (targetDirection != EMovementDirection.None)
        {
            velocityX = (int)MovementDirection * playerCharacter.characterInfo.MovementSpeedOnRotate * Time.deltaTime;
            rigidBody.velocity = new Vector3(velocityX, rigidBody.velocity.y, 0);
            return;
        }*/

        int targetSpeed = (int)MovementDirection * player.playerInfo.MovementSpeed;

        if (Math.Abs(velocityX) < Math.Abs(targetSpeed))
        {
            velocityX += (int)MovementDirection * player.playerInfo.Acceleration * Time.deltaTime;
        }
        else
        {
            velocityX = targetSpeed;
        }


        data.rigidBody.velocity = new Vector3(velocityX, data.rigidBody.velocity.y, 0);
    }

    private void UpdateRotation()
    {
        if (!canRotate) return;

        int targetYAngle = targetDirection == EMovementDirection.Left ? 270 : 90;

        if (targetYAngle == 270)
        {
            if (player.transform.eulerAngles.y < targetYAngle)
            {
                angleY += player.playerInfo.RotationSpeed * Time.deltaTime;
            }
            else
            {
                angleY = targetYAngle;
                targetDirection = EMovementDirection.None;
                canRotate = false;
            }
        }
        else if (targetYAngle == 90)
        {
            if (player.transform.eulerAngles.y > targetYAngle)
            {
                angleY -= player.playerInfo.RotationSpeed * Time.deltaTime;
            }
            else
            {
                angleY = targetYAngle;
                targetDirection = EMovementDirection.None;
                canRotate = false;
            }
        }

        player.transform.eulerAngles = new Vector3(player.transform.rotation.x, angleY, player.transform.rotation.z);
    }

    private void HandleMoveAnimation()
    {
        var param_MoveSpeed = Animation.Parameter.MoveSpeed;
        data.animator.SetFloat(param_MoveSpeed, MathF.Abs(velocityX), 0.1f, Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        velocityX = 0;

        if (movementValue == 0) return;

        targetDirection = MovementDirection;
        canRotate = true;
    }

    public void OnLand(EMovementDirection direction, float velocityX)
    {
        if (MovementDirection == EMovementDirection.None) return;
        if (MovementDirection == direction) return;

        this.velocityX = 0;
        canRotate = true;
        targetDirection = direction == EMovementDirection.Left? EMovementDirection.Right : EMovementDirection.Left;
        data.rigidBody.velocity = new Vector3(this.velocityX, data.rigidBody.velocity.y, 0);
    }
}