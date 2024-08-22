using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerJumpState : PlayerStateBase
{
    public PlayerJumpState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }
    
    private float velocityBeforeJump;

    public override void EnterState()
    {
        base.EnterState();

        Jump();

        player.AnimationController.ChangeState(AnimationController.Player.Movement.Jump, 0);
    }
    public override void UpdateState()
    {
        base.UpdateState();

        if(player.Detector.GroundDetected())
        {
            if (time < 0.5f) return;

            player.ChangeState(player.MoveState);
        }
        else
        {
            player.MovementController.SetVelocity(velocityBeforeJump);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void Jump()
    {
        velocityBeforeJump = player.MovementController.GetVelocity();

        player.MovementController.Jump(player.Info.JumpPower);
    }
}
