using UnityEngine;

public class PlayerJumpState : PlayerStateBase
{
    public PlayerJumpState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    private MovementDirection directionOnJump;

    public override void EnterState()
    {
        base.EnterState();

        blackBoard.Input_ChangeDirection += OnChangeDirection;

        player.MovementController.SetVelocity(player.MovementController.Velocity.x, player.Info.JumpPower);
        player.AnimationController.ChangeState(AnimationController.Player.Movement.Jump, 0);

        directionOnJump = player.MovementController.FacingDirection;
    }
    public override void UpdateState()
    {
        base.UpdateState();

        if (!player.MovementController.IsGrounded) return;
        if (time < 0.5f) return;

        player.ChangeState(player.MoveState);

    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();

        blackBoard.Input_ChangeDirection -= OnChangeDirection;

        if(directionOnJump != player.MovementController.FacingDirection)
            player.MovementController.SetVelocity(0, 0);
    }

    private void OnChangeDirection(MovementDirection newDirection)
    {
        player.MovementController.ChangeMovementDirection(newDirection, Space.Self);
    }

}
