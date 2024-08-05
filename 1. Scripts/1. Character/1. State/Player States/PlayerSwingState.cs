using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwingState : PlayerStateBase
{
    public PlayerSwingState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) 
    {
        sharedData.OnClick += StartDownSwing;
        sharedData.OnDrag += BackSwing;
    }

    // Steps to swing
    private enum SwingStep { None, BackSwing, DownSwing }
    private SwingStep swingStep;

    // Animation Frame
    private const int maxFrame = 119;
    private const int maxFrameOnBackSwing = 34;
    private const int maxFrameOnDownSwing = 110;
    private float currentFrame;

    private Vector2 mousePosition;

    public override void EnterState()
    {
        base.EnterState();

        player.AnimationController.SetSpeed(AnimationController.Speed.Pause);
        player.AnimationController.ChangeState(AnimationController.State.Swing);

        swingStep = SwingStep.BackSwing;
        currentFrame = 0;
        player.MovementController.SetRotation(90);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        HandleSwingAnimation(currentFrame);
    }

    public override void ExitState()
    {
        base.ExitState();

        player.AnimationController.SetSpeed(AnimationController.Speed.Normal);
        downSwingCoroutine = null;
        Cursor.lockState = CursorLockMode.None;
    }


    private void BackSwing(Vector2 newMousePosition)
    {
        if (swingStep != SwingStep.BackSwing) return;

        if (newMousePosition.x < mousePosition.x)
        {
            if (currentFrame < maxFrameOnBackSwing) currentFrame += 120 * Time.deltaTime;
            if (currentFrame > maxFrameOnBackSwing) currentFrame = maxFrameOnBackSwing;
        }

        mousePosition = newMousePosition;
    }

    private void StartDownSwing()
    {
        if (!player.CurrenState.CompareState(player.SwingState)) return;
        if (swingStep != SwingStep.BackSwing) return;
        if (currentFrame > 34 || currentFrame == 0) return;
        if (downSwingCoroutine != null) return;

        downSwingCoroutine = player.StartCoroutine(DownSwingRoutine());
    }

    private Coroutine downSwingCoroutine;
    private IEnumerator DownSwingRoutine()
    {
        swingStep = SwingStep.DownSwing;
        Cursor.lockState = CursorLockMode.Locked;

        var swingSpeed = GetProperSwingPowerDependingOnTheFrame(currentFrame);

        while (currentFrame < maxFrameOnDownSwing)
        {
            if (currentFrame < 20)
            {
                currentFrame += swingSpeed * Time.deltaTime;
            }
            else
            {
                currentFrame += swingSpeed * 2 * Time.deltaTime;
            }

            yield return null;
        }

        player.ChangeState(player.MoveState);
    }

    private float GetProperSwingPowerDependingOnTheFrame(float frame)
    {
        var swingPower = frame * 7;
        if (swingPower > 200) swingPower = 200;    // Max value: 200
        if (swingPower < 200) swingPower *= 0.75f; // Default Multiplier: 0.75
        if (swingPower < 50) swingPower = 50;      // Min value: 200
        return swingPower;
    }

    private void HandleSwingAnimation(float frame)
    {
        player.AnimationController.Play(frame / maxFrame);
    }
}