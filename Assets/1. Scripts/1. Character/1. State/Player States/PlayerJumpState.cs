using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerJumpState : PlayerStateBase
{
    public PlayerJumpState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    public override void EnterState()
    {
        base.EnterState();

        blackBoard.Input_ChangeDirection += OnChangeDirection;

        player.MovementController.Jump(player.Info.JumpPower);
        player.AnimationController.ChangeState(AnimationController.Player.Movement.Jump, 0);
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
    }

    private void OnChangeDirection(MovementDirection newDirection)
    {
        player.MovementController.ChangeDirectionSmooth(newDirection);
    }

}
