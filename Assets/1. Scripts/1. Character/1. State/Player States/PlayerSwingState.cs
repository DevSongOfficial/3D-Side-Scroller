using System.Collections;
using UnityEngine;
using static GameSystem;


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

    // Power of swing that affects frame increasement speed & damage & knockback.
    private float powerCharged;
    private const float powerDefault = 0;
    private const float powerMaximum = 2;

    private bool hasHit;

    public override void EnterState()
    {
        base.EnterState();

        sharedData.Input_MouseUp += StartDownSwing;

        player.AnimationController.ChangeState(AnimationController.Player.Attack.Swing, 0.01f);
        player.MovementController.ChangeMovementDirection(EMovementDirection.Right, smoothRotation: false);

        currentFrame = 0;
        powerCharged = powerDefault;
        hasHit = false;

        backSwingCoroutine = player.StartCoroutine(BackSwing());
    }

    public override void ExitState()
    {
        base.ExitState();

        sharedData.Input_MouseUp -= StartDownSwing;

        Cursor.lockState = CursorLockMode.None;
        player.AnimationController.SetSpeed(AnimationController.Speed.Normal);
        player.MovementController.StopMovement();

        if(downSwingCoroutine != null)
        {
            player.StopCoroutine(downSwingCoroutine);
            downSwingCoroutine = null;
        }

        if(backSwingCoroutine != null)
        {
            player.StopCoroutine(backSwingCoroutine);
            backSwingCoroutine = null;
        }
    }

    Coroutine backSwingCoroutine;
    private IEnumerator BackSwing()
    {
        swingStep = SwingStep.BackSwing;

        yield return new WaitForSeconds(0.1f);

        while(powerCharged < powerMaximum)
        {
            if (currentFrame < MaxFrameOnBackSwing) 
                currentFrame += player.Interactor.AsGolfer.CurrentClub.ChargeSpeed * 50 * Time.fixedDeltaTime;
            else
            {
                currentFrame = MaxFrameOnBackSwing;
                powerCharged += player.Interactor.AsGolfer.CurrentClub.ChargeSpeed * Time.fixedDeltaTime;
                powerCharged = Mathf.Clamp(powerCharged, powerDefault, powerMaximum);

                UIManager.PopupUI(UIManager.GetUI.Image_SwingChargeIndicator);
                UIManager.FillImage(UIManager.GetUI.Image_SwingChargeIndicator, powerCharged / powerMaximum);
            }

            PlaySwingAnimation(currentFrame);

            yield return new WaitForFixedUpdate();
        }
    }

    private void StartDownSwing()
    {
        if (!player.CurrenState.CompareState(player.SwingState)) return;
        if (swingStep != SwingStep.BackSwing) return;
        if (downSwingCoroutine != null) return;

        powerCharged++;

        UIManager.CloseUI(UIManager.GetUI.Image_SwingChargeIndicator);
        player.StopCoroutine(backSwingCoroutine);
        downSwingCoroutine = player.StartCoroutine(DownSwingRoutine());
    }

    private Coroutine downSwingCoroutine;
    private IEnumerator DownSwingRoutine()
    {
        swingStep = SwingStep.DownSwing;
        Cursor.lockState = CursorLockMode.Locked;

        frameOnStartDownSwing = currentFrame;

        while (currentFrame < MaxFrameOnDownSwing)
        {
            TryHit();
            CalculateAnimationFrame();
            PlaySwingAnimation(currentFrame);

            yield return new WaitForFixedUpdate();
        }

        player.ChangeState(player.MoveState);
    }

    private void TryHit()
    {
        if (!hasHit && currentFrame >= HitFrame)
        {
            hasHit = true;

            var swingPosition = player.transform.position + player.Info.LocalPosition_Swing;
            var damageables = player.Detector.ComponentsDetected<IDamageable>(swingPosition, player.Interactor.AsGolfer.CurrentClub.SwingRadius, Utility.GetLayerMask(Layer.Character, Layer.PlaceableObject), Tag.Player);

            var damageEvent = player.Interactor.AsGolfer.CurrentClub.DamageEvent;
            damageEvent.knockBackVector *= powerCharged;
            damageEvent.damage *= powerCharged > 2 ? 3 : 2;

            foreach (var damageable in damageables)
            {
                damageable.TakeDamage(damageEvent);
            }
        }
    }

    private void CalculateAnimationFrame()
    {
        var multiplier = GetProperMultiplierDependingOnTheFrame(currentFrame);
        multiplier *= 50 * powerCharged;

        if (frameOnStartDownSwing < MaxFrameOnBackSwing)
        {
            currentFrame += multiplier * Time.fixedDeltaTime;
        }
        else
        {
            currentFrame +=  2.5f * multiplier * Time.fixedDeltaTime;
        }
    }

    private float GetProperMultiplierDependingOnTheFrame(float frame)
    {
        if (frame > 80) return 4;
        else if (frame > 60) return 2;
        return 1;
    }

    private void PlaySwingAnimation(float frame)
    {
        player.AnimationController.SetSpeed(AnimationController.Speed.Pause);
        player.AnimationController.Play(frame / MaxFrame);
    }
}