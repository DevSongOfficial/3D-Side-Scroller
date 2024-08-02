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

        if(player.IsOnGround)
        {
            if (time < 10 * Time.deltaTime) return;

            player.ChangeState(player.playerMoveState);
            sharedData.OnPlayerLand?.Invoke(directionBeforeJump);
        }
        else
        {
            player.movementController.SetVelocity(velocityBeforeJump);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void Jump()
    {
        velocityBeforeJump = player.movementController.GetVelocity();
        directionBeforeJump = player.movementController.Direction;

        player.movementController.Jump(player.Info.JumpPower);
    }
}
