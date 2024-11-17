using System;
using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    public PlayerMoveState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    private const float animationTransitionTime = 0.1f;

    public override void EnterState()
    {
        base.EnterState();

        // Add listeners.
        blackBoard.Input_ChangeDirection += OnChangeDirection;
        blackBoard.Input_ChangeZDirection += OnChangeZDirection;
        blackBoard.Input_OnJump += SwithToJumpState;
        blackBoard.Input_MouseDown += SwitchToAttackState;
        
        // Set up animation.
        player.AnimationController.SetSpeed(AnimationController.Speed.Normal);
        player.AnimationController.ChangeState(AnimationController.Player.Movement.BT_1, animationTransitionTime);

        // Check if player required to change direction during another state's routine, and it hasn't applied.
        if (blackBoard.InputDirection != player.MovementController.Direction)
            player.MovementController.StopAndChangeDirection(blackBoard.InputDirection);
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

        // Remove listeners.
        blackBoard.Input_ChangeDirection -= OnChangeDirection;
        blackBoard.Input_ChangeZDirection -= OnChangeZDirection;
        blackBoard.Input_OnJump -= SwithToJumpState;
        blackBoard.Input_MouseDown -= SwitchToAttackState;
    }

    private void OnChangeDirection(MovementDirection newDirection)
    {
        player.MovementController.StopAndChangeDirection(newDirection);
    }

    private void OnChangeZDirection(CharacterMovementController.ZAxisMovementDirection direction)
    {
        player.MovementController.MoveOnZAxis(direction);
    }

    private void HandleMovement()
    {
        if (player.MovementController.IsStunned) return;

        // Handle velocity X
        float velocityX = blackBoard.InputDirection == MovementDirection.None ? 0 : player.Info.MovementSpeed;
        player.MovementController.ApplyHorizontalVelocity(velocityX, player.Info.Acceleration);

        // Handle rotation X
        player.MovementController.AlignToGround();

        // Handle character controller settings
        player.MovementController.SetStepOffset(player.MovementController.IsGrounded ? 0.3f : 0);
    }

    private void HandleAnimation()
    {
        var param_MoveSpeed = AnimationController.Parameter.MoveSpeed;

        if(player.MovementController.IsMovingOnZAxis)
        {
            var speedZ = Math.Abs(player.MovementController.Velocity.z);
            player.AnimationController.SetFloat(param_MoveSpeed, speedZ);
            return;
        }

        var speedX = Math.Abs(player.MovementController.Velocity.x);
        speedX = blackBoard.InputDirection == MovementDirection.None ? 0 : speedX;
        player.AnimationController.SetFloat(param_MoveSpeed, speedX);
    }

    private void SwithToJumpState()
    {
        if (!player.MovementController.IsGrounded) return;
        if (player.MovementController.IsMovingOnZAxis) return;

        player.ChangeState(player.JumpState);
    }

    private void SwitchToAttackState()
    {
        if (player.MovementController.IsMovingOnZAxis) return;

        player.ChangeState(player.AttackState);
    }
}