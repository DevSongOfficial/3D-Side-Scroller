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
        blackBoard.Input_OnJump -= SwithToJumpState;
        blackBoard.Input_MouseDown -= SwitchToAttackState;
    }

    private void OnChangeDirection(MovementDirection newDirection)
    {
        player.MovementController.StopAndChangeDirection(newDirection);
    }

    private void HandleMovement()
    {
        if (player.MovementController.IsStunned) return;

        // Handle velocity X
        float velocityX = blackBoard.InputDirection == MovementDirection.None ? 0 : player.Info.MovementSpeed;
        player.MovementController.ApplyHorizontalVelocity(velocityX, player.Info.Acceleration);

        // Handle rotation X
        HandleXRotation();

        // Handle character controller settings
        player.MovementController.SetStepOffset(player.MovementController.IsGrounded ? 0.3f : 0);
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

        var speedX = Math.Abs(player.MovementController.Velocity.x);
        speedX = blackBoard.InputDirection == MovementDirection.None ? 0 : speedX;

        player.AnimationController.SetFloat(param_MoveSpeed, speedX);
    }

    private void SwithToJumpState()
    {
        if (!player.MovementController.IsGrounded) return;
        player.ChangeState(player.JumpState);
    }

    private void SwitchToAttackState()
    {
        player.ChangeState(player.AttackState);
    }
}