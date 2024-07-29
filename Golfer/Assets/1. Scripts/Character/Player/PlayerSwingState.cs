using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwingState : PlayerStateBase
{
    public PlayerSwingState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    private const int maxFrame = 119;
    private float frame;
    private Vector2 mousePosition;
    private bool canSwing;
    private float swingPower;

    public override void EnterState()
    {
        base.EnterState();

        data.animator.speed = 0;
        player.ChangeAnimationState(Animation.State.Swing);

        canSwing = false;
        swingPower = 0;
        frame = 0;
        player.transform.eulerAngles = new Vector3(player.transform.rotation.x, 90, player.transform.rotation.z);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        HandleGolfSwing();
        HandleGolfAnimation(frame);
    }

    public override void ExitState()
    {
        base.ExitState();

        data.animator.speed = 1;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnClick()
    {
        if (player.CurrenState != player.playerSwingState) return;
        
        if (frame <= 34 && frame > 0)
        {
            Cursor.lockState = CursorLockMode.Locked;
            canSwing = true;
            swingPower = SetSwingPower(frame);
        }
    }

    public void OnDrag(InputValue value)
    {
        if (player.CurrenState != player.playerSwingState) return;

        var newMousePosition = value.Get<Vector2>();
        ChargeGolfSwing(newMousePosition);
    }

    private void ChargeGolfSwing(Vector2 newMousePosition)
    {
        if (swingPower > 0) return;

        if (newMousePosition.x < mousePosition.x)
        {
            if (frame < 34) frame += 120 * Time.deltaTime;
            if (frame > 34) frame = 34;
        }

        mousePosition = newMousePosition;
    }

    private void HandleGolfSwing()
    {
        if (!canSwing) return;

        if (frame < 110)
        {
            if (frame < 20)
            {
                frame += swingPower * Time.deltaTime;
            }
            else
            {
                frame += swingPower * 2 * Time.deltaTime;
            }
        }
        else
        {
            player.ChangeState(player.playerMoveState);
        }
    }

    private float SetSwingPower(float frame)
    {
        swingPower = frame * 7;
        if (swingPower > 200) swingPower = 200;
        if (swingPower < 200) swingPower *= 0.75f;
        if (swingPower < 50) swingPower = 50;
        return swingPower;
    }

    private void HandleGolfAnimation(float frame)
    {
        if (player.CurrenState != player.playerSwingState) return;

        data.animator.Play(Animation.State.Swing.ToString(), 0, frame / maxFrame);
    }
}
