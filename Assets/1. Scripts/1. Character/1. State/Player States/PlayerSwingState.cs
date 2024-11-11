using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using static GameSystem;


public class PlayerSwingState : PlayerStateBase
{
    public PlayerSwingState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    // Steps to swing
    private enum SwingStep { None, BackSwing, DownSwing }
    private SwingStep swingStep;
    

    // Animation Frame
    // Default Swing
    private ClubType swingType;
    private int MaxFrame => swingType == ClubType.Putter ? 81 : 119;
    private int MaxFrameOnBackSwing => swingType == ClubType.Putter? 29 : 34; 
    private int MaxFrameOnDownSwing => swingType == ClubType.Putter ? 81 : 110;
    private int HitFrame => swingType == ClubType.Putter ? 37 : 48;

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

        blackBoard.Input_MouseUp += StartDownSwing;

        player.AnimationController.ChangeState(player.Interactor.AsGolfer.CurrentClub.AnimationType, 0.01f);
        player.MovementController.ChangeMovementDirection(MovementDirection.Right, Space.Self , smoothRotation: false);

        swingType = player.Interactor.AsGolfer.CurrentClub.ClubType;
        currentFrame = 0;
        powerCharged = powerDefault;

        hasHit = false;

        backSwingCoroutine = player.StartCoroutine(BackSwing());
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        // Handle velocity X
        if (!player.MovementController.IsStunned)
            player.MovementController.SetVelocity(0, player.MovementController.Velocity.y);
    }

    public override void ExitState()
    {
        base.ExitState();

        blackBoard.Input_MouseUp -= StartDownSwing;

        Cursor.lockState = CursorLockMode.None;
        player.AnimationController.SetSpeed(AnimationController.Speed.Normal);

        player.AuraVFX.SmoothDecreaseFloat(target: 0, speed: 3);

        if (downSwingCoroutine != null)
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

    private Coroutine backSwingCoroutine;
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
                UIManager.PopupUI(UIManager.GetUI.Image_SwingChargeIndicator);
                UIManager.FillImage(UIManager.GetUI.Image_SwingChargeIndicator, powerCharged / powerMaximum);
            }

            PlaySwingAnimation(currentFrame);

            yield return new WaitForFixedUpdate();
        }

        powerCharged = powerMaximum;

        float alpha = 0;
        while (alpha < 2)
        {
            alpha += Time.fixedDeltaTime;
            player.AuraVFX.SetFloat(alpha);
            yield return new WaitForFixedUpdate();
        }
    }

    private void StartDownSwing()
    {
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

            var localSwingPosition = player.Info.LocalPosition_Swing;
            {
                localSwingPosition.x -= player.transform.rotation.x * 0.04f;
                localSwingPosition.y -= player.transform.rotation.x * 0.01f;
            }
            var swingPosition = player.transform.position + player.Info.LocalPosition_Swing;
            var damageables = player.Detector.DetectComponents<IDamageable>(swingPosition, player.Interactor.AsGolfer.CurrentClub.SwingRadius, Utility.GetLayerMask(Layer.Character, Layer.Damageable), Tag.Player);

            var damageEvent = player.Interactor.AsGolfer.CurrentClub.DamageEvent.
                ApplyDirection(player.MovementController.FacingDirection).
                MultiplyVelocity(powerCharged).
                MultiplyDamage(powerCharged > 2 ? 3 : 2);

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
        if (player.Interactor.AsGolfer.CurrentClub.ClubType == ClubType.Putter) return 1;
        if (frame > 80) return 4;
        else if (frame > 60) return 2;
        return 1;
    }

    private void PlaySwingAnimation(float frame)
    {
        player.AnimationController.SetSpeed(AnimationController.Speed.Pause);
        player.AnimationController.SetFrame(frame / MaxFrame);
    }
}