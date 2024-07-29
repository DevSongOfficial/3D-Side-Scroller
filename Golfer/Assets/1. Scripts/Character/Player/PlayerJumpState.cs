using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerJumpState : PlayerStateBase
{
    public PlayerJumpState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }
    

    private Vector3 velocityBeforeJump;
    private CharacterBase.EMovementDirection directionBeforeJump;
    private CharacterBase.EMovementDirection directionAfterJump;

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
            player.playerMoveState.OnLand(directionBeforeJump, velocityBeforeJump.x);
        }
        else
        {
            data.rigidBody.velocity = new Vector3(velocityBeforeJump.x, data.rigidBody.velocity.y, data.rigidBody.velocity.z);
        }
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void Jump()
    {
        velocityBeforeJump = data.rigidBody.velocity;
        directionBeforeJump = player.transform.eulerAngles.y == 90 ? CharacterBase.EMovementDirection.Right 
            : CharacterBase.EMovementDirection.Left;

        data.rigidBody.velocity = new Vector3(data.rigidBody.velocity.x, 4, data.rigidBody.velocity.z);
    }
}
