using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ZombieAttackState : ZombieStateBase
{
    public ZombieAttackState(ZombieCharacter zombieCharacter, ZombieBlackboard zomebieBlackboard) : base(zombieCharacter, zomebieBlackboard) { }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void MeleeAttack(CharacterBase character)
    {
        //character.TakeDamage(new DamageEvent { damage = 10 });
    }
}
