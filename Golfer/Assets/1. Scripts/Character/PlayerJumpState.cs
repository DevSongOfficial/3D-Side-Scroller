using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerJumpState : StateBase
{
    public PlayerJumpState(CharacterBase character, Rigidbody rigidBody, Animator animator) : base(character, rigidBody, animator) { }


    public override void EnterState()
    {
        base.EnterState();

        Jump();
    }
    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void Jump()
    {
        rigidBody.velocity = new Vector3(rigidBody.velocity.x, 4, rigidBody.velocity.z);
    }
}
