using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerJumpState : PlayerStateBase
{
    public PlayerJumpState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }
    
    private float velocityBeforeJump;
    private EMovementDirection directionBeforeJump;

    public override void EnterState()
    {
        base.EnterState();

        Jump();
    }
    public override void UpdateState()
    {
        base.UpdateState();

        if(player.Detector.GroundDetected())
        {
            if (time < 10 * Time.deltaTime) return;

            player.ChangeState(player.MoveState);
            sharedData.OnLand.Invoke(directionBeforeJump);
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
        directionBeforeJump = player.MovementController.Direction;

        player.MovementController.Jump(player.Info.JumpPower);
    }
}
