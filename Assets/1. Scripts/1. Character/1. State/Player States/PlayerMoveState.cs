using System;
using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) 
    {
        sharedData.Input_ChangeDirection += OnChangeDirection;
        sharedData.Input_OnJump += Jump;
    }


    private const float animationTransitionTime = 0.1f;

    public override void EnterState()
    {
        base.EnterState();

        // Set up animation.
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

        HandleMovement();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    EMovementDirection wishDirection;
    private void OnChangeDirection(EMovementDirection newDirection)
    {
        wishDirection = newDirection;
        player.MovementController.ChangeDirectionSmooth(wishDirection);
    }

    private void HandleMovement()
    {
        // Handle velocity
        var velocity = player.MovementController.CalculateVelocity(player.Info.MovementSpeed, player.Info.Acceleration, player.Info.Mass);
        velocity.x = wishDirection == EMovementDirection.None ? 0 : velocity.x;
        player.MovementController.Move(velocity * Time.fixedDeltaTime);
        
        // Handle X rotation 
        HandleXRotation();

        // Handle character controller settings
        player.Controller.stepOffset = player.Controller.isGrounded ? 0.3f : 0;
        
    }

    private void Jump()
    {
        player.MovementController.Jump(player.Info.JumpPower);
    }

    private void HandleXRotation()
    {
        if (Physics.Raycast(player.transform.position, Vector3.down, out RaycastHit hit, 1.2f, Layer.Default.GetMask()))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(player.transform.up, hit.normal) * player.transform.rotation;
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    private void HandleAnimation()
    {
        var param_MoveSpeed = AnimationController.Parameter.MoveSpeed;
        var velocity = MathF.Abs(player.MovementController.CalculateVelocity(
            player.Info.MovementSpeed, player.Info.Acceleration, player.Info.Mass).x);
        velocity = wishDirection == EMovementDirection.None ? 0 : velocity;

        player.AnimationController.SetFloat(param_MoveSpeed, velocity);
    }
}