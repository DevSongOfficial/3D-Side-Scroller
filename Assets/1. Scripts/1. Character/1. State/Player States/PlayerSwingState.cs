using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwingState : PlayerStateBase
{
    public PlayerSwingState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    // Steps to swing
    private enum SwingStep { None, BackSwing, DownSwing }
    private SwingStep swingStep;

    // Animation Frame
    private const int MaxFrame = 119;
    private const int MaxFrameOnBackSwing = 34;
    private const int MaxFrameOnDownSwing = 110;
    private const int HitFrame = 48;
    private float currentFrame;
    private float frameOnStartDownSwing;
    private float powerCharged;

    private bool hasHit;

    public override void EnterState()
    {
        base.EnterState();

        sharedData.Input_MouseUp += StartDownSwing;

        player.AnimationController.ChangeState(AnimationController.Player.Attack.Swing, 0.01f);

        swingStep = SwingStep.BackSwing;
        currentFrame = 0;
        powerCharged = 0;
        hasHit = false;
        player.MovementController.ChangeMovementDirection(EMovementDirection.Right, smoothRotation: false);
    }

    public override void FixedUpdateState()
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

        if (currentFrame < MaxFrameOnBackSwing) 
            currentFrame += 50 * Time.deltaTime;
        else
        {
            currentFrame = MaxFrameOnBackSwing;
            powerCharged += Time.deltaTime;
        }
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

        frameOnStartDownSwing = currentFrame;

        while (currentFrame < MaxFrameOnDownSwing)
        {
            yield return null;

            #region Hit
            if (!hasHit && currentFrame >= HitFrame)
            {
                hasHit = true;

                var swingPosition = player.transform.position + player.Info.LocalPosition_Swing;
                var damageables = player.Detector.ComponentsDetected<IDamageable>(swingPosition, player.Info.SwingRadius, Utility.GetLayerMask(Layer.Character, Layer.PlaceableObject), Tag.Player);

                foreach (var damageable in damageables)
                {
                    damageable?.TakeDamage(new DamageEvent(EventSenderType.Character, player.Info.SwingDamage));
                }
            }
            #endregion

            var multiplier = GetProperMultiplierDependingOnTheFrame(currentFrame);

            if (frameOnStartDownSwing < MaxFrameOnBackSwing)
            {
                currentFrame +=  50 * multiplier * Time.deltaTime;
                continue;
            }
            else
            {
                currentFrame += 120 * multiplier * Time.deltaTime;
                continue;
            }
        }

        player.ChangeState(player.MoveState);
    }

    private float GetProperMultiplierDependingOnTheFrame(float frame)
    {
        if (frame > 80) return 4;
        else if (frame > 60) return 2;
        return 1;
    }

    private void HandleSwingAnimation(float frame)
    {
        player.AnimationController.SetSpeed(AnimationController.Speed.Pause);
        player.AnimationController.Play(frame / MaxFrame);
    }
}