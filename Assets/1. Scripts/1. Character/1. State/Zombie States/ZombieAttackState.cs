using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using System.Collections;

public sealed class ZombieAttackState : ZombieStateBase
{
    private ZombieAttackBase attackBase;
    private float cooldownLeft = 0;
    public bool IsAttacking => delayedAttackRoutine != null;

    public ZombieAttackState(ZombieCharacter zombieCharacter, ZombieBlackboard zomebieBlackboard, ZombieAttackBase attackType) : base(zombieCharacter, zomebieBlackboard) 
    {
        attackBase = attackType;
    }

    public override void EnterState()
    {
        base.EnterState();

        zombie.MovementController.SetVelocity(0, zombie.MovementController.Velocity.y);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (IsAttacking) return;

        if (!zombie.IsTargetWithInDistance(blackBoard.targetCharacter, zombie.Info.AttackRange))
        {
            zombie.ChangeState(zombie.PatrolState);
            return;
        }

        HandleAttack();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();

        blackBoard.targetCharacter = null;

        if(delayedAttackRoutine != null)
        {
            zombie.StopCoroutine(delayedAttackRoutine);
            delayedAttackRoutine = null;
        }
    }

    private void HandleAttack()
    {
        if (cooldownLeft > 0)
        {
            cooldownLeft -= Time.deltaTime;

            zombie.AnimationController.ChangeState(AnimationController.Zombie.Attack.ZombieThreaten);
        }
        else 
        {
            cooldownLeft = attackBase.CooldownTime;

            delayedAttackRoutine = zombie.StartCoroutine(DelayedAttackRoutine());
            zombie.AnimationController.ChangeState(attackBase.AnimationType);
        }
    }

    private Coroutine delayedAttackRoutine;
    private IEnumerator DelayedAttackRoutine()
    {
        yield return new WaitForSeconds(0.85f);

        if(zombie.IsTargetWithInDistance(blackBoard.targetCharacter, zombie.Info.AttackRange))
            attackBase.Execute(zombie, blackBoard.targetCharacter);

        yield return new WaitForSeconds(0.75f);

        delayedAttackRoutine = null;
    }
}