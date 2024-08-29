using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwingState : PlayerStateBase
{
    public PlayerSwingState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    // Steps to swing
    private enum SwingStep { None, BackSwing, DownSwing }
    private SwingStep swingStep;

    // Animation Frame
    private const int maxFrame = 119;
    private const int maxFrameOnBackSwing = 34;
    private const int maxFrameOnDownSwing = 110;
    private float currentFrame;

    public override void EnterState()
    {
        base.EnterState();

        sharedData.Input_MouseUp += StartDownSwing;

        player.AnimationController.ChangeState(AnimationController.Player.Attack.Swing, 0.01f);

        swingStep = SwingStep.BackSwing;
        currentFrame = 0;
        player.MovementController.ChangeMovementDirection(EMovementDirection.Right, smoothRotation: false);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (time < 0.1f) return;

        BackSwing();
        HandleSwingAnimation(currentFrame);
    }

    public override void ExitState()
    {
        base.ExitState();

        sharedData.Input_MouseUp -= StartDownSwing;

        player.AnimationController.SetSpeed(AnimationController.Speed.Normal);
        downSwingCoroutine = null;
        Cursor.lockState = CursorLockMode.None;

        player.MovementController.StopMovement();
    }

    private void BackSwing()
    {
        if (swingStep != SwingStep.BackSwing) return;

        if (currentFrame < maxFrameOnBackSwing) currentFrame += 60 * Time.deltaTime;
        if (currentFrame > maxFrameOnBackSwing) currentFrame = maxFrameOnBackSwing;
    }

    private void StartDownSwing()
    {
        if (!player.CurrenState.CompareState(player.SwingState)) return;
        if (swingStep != SwingStep.BackSwing) return;

        if (downSwingCoroutine == null)
        {
            downSwingCoroutine = player.StartCoroutine(DownSwingRoutine());
        }
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


        var swingPosition = player.transform.position + player.Info.LocalPosition_Swing;
        var damageables = player.Detector.ComponentsDetected<IDamageable>(swingPosition, player.Info.SwingRadius, Utility.GetLayerMask(Layer.Character, Layer.PlaceableObject), Tag.Player);

        foreach (var damageable in damageables)
        {
            damageable?.TakeDamage(new DamageEvent(EventSenderType.Character, player.Info.SwingDamage));
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
        player.AnimationController.SetSpeed(AnimationController.Speed.Pause);
        player.AnimationController.Play(frame / maxFrame);
    }
}