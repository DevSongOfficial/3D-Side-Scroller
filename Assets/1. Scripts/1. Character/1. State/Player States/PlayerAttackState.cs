using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TextCore.Text;

public sealed class PlayerAttackState : PlayerStateBase
{
    public PlayerAttackState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    public bool IsAttacking =>  attackCoroutine != null;

    // Delay before attacking
    private const float attackDelay = 0.2f;
    private float attackDelayLeft;

    public override void EnterState()
    {
        base.EnterState();

        blackBoard.Input_MouseUp += TryAttack;

        player.MovementController.ToggleHorizontalMovement(false);

        attackDelayLeft = attackDelay;
    }


    public override void UpdateState()
    {
        base.UpdateState();

        HandleSwingPreparation();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();

        blackBoard.Input_MouseUp -= TryAttack;

        player.MovementController.ToggleHorizontalMovement(true);

        if (IsAttacking)
        {
            player.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }
    }

    private void HandleSwingPreparation()
    {
        if (IsAttacking) return;

        attackDelayLeft -= Time.deltaTime;
        
        if(attackDelayLeft <= 0)
        {
            attackDelayLeft = 0;

            player.ChangeState(player.SwingState);
        }
    }

    private void TryAttack()
    {
        if (attackCoroutine != null) return;

        attackCoroutine = player.StartCoroutine(Attack_1_Routine());
    }

    private Coroutine attackCoroutine;
    private IEnumerator Attack_1_Routine()
    {
        player.AnimationController.ChangeState(AnimationController.Player.Attack.Downward_1, 0);

        yield return new WaitForSeconds(player.Info.DamageDelay);

        var attackPosition = player.transform.position;
        attackPosition += player.Info.LocalPosition_Attack.ChangeVectorXWithDirection(player.MovementController.FacingDirection);

        if (player.Detector.DetectCharacters(attackPosition, player.Info.AttackRadius, out ZombieCharacter[] characters))
        {
            foreach (var character in characters)
            {
                var damageEvent = player.Interactor.AsGolfer.CurrentClub.DamageEvent;
                character?.TakeDamage(damageEvent.ApplyDirection(player.MovementController.FacingDirection));
            }
        }

        yield return new WaitForSeconds(0.3f);

        player.ChangeState(player.MoveState);
    }

    private IEnumerator Attack_2_Routine()
    {
        yield return null;
    }
}