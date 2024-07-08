using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static CharacterBase;

public class PlayerMoveState : StateBase
{
    public PlayerMoveState(CharacterBase character, Rigidbody rigidBody, Animator animator) : base(character, rigidBody, animator) { }

    private float Xspeed;
    private float YAngle;
    private bool canRotate;
    private EMovementDirection targetDirection;
    private EMovementDirection MovementDirection { get { return (EMovementDirection)movementValue; } }
    private int movementValue;

    public override void EnterState()
    {
        base.EnterState();

        animator.speed = 1;
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
        if (targetDirection != EMovementDirection.None)
        {
            Xspeed = (int)MovementDirection * 300 * Time.deltaTime;
            rigidBody.velocity = new Vector3(Xspeed, rigidBody.velocity.y, 0);
            return;
        }

        int targetSpeed = (int)MovementDirection * playerCharacter.characterInfo.MovementSpeed;

        if (Math.Abs(Xspeed) < Math.Abs(targetSpeed))
        {
            Xspeed += (int)MovementDirection * 10 * Time.deltaTime;
        }
        else
        {
            Xspeed = targetSpeed;
        }


        rigidBody.velocity = new Vector3(Xspeed, rigidBody.velocity.y, 0);
    }

    private void UpdateRotation()
    {
        if (!canRotate) return;

        int targetYAngle = targetDirection == EMovementDirection.Left ? 270 : 90;

        if (targetYAngle == 270)
        {
            if (playerCharacter.transform.eulerAngles.y < targetYAngle)
            {
                YAngle += playerCharacter.characterInfo.RotationSpeed * Time.deltaTime;
            }
            else
            {
                YAngle = targetYAngle;
                targetDirection = EMovementDirection.None;
                canRotate = false;
            }
        }
        else if (targetYAngle == 90)
        {
            if (playerCharacter.transform.eulerAngles.y > targetYAngle)
            {
                YAngle -= playerCharacter.characterInfo.RotationSpeed * Time.deltaTime;
            }
            else
            {
                YAngle = targetYAngle;
                targetDirection = EMovementDirection.None;
                canRotate = false;
            }
        }

        playerCharacter.transform.eulerAngles = new Vector3(playerCharacter.transform.rotation.x, YAngle, playerCharacter.transform.rotation.z);
    }

    private void HandleMoveAnimation()
    {
        var param_MoveSpeed = PlayerCharacterAnimation.Parameter.MoveSpeed.ToString();
        animator.SetFloat(param_MoveSpeed, MathF.Abs(Xspeed), 0.1f, Time.deltaTime);
    }

    public void OnMove(InputValue value)
    {
        movementValue = (int)value.Get<float>();
        Xspeed = 0;

        if (movementValue == 0) return;

        targetDirection = MovementDirection;
        canRotate = true;
    }
}