using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieAttackType", menuName = "Scriptable Object/Zombie/Attack 1")]
public sealed class ZombieAttack1 : ZombieAttackBase
{
    [SerializeField] private DamageEvent damageEvent;

    public override void Execute(CharacterBase target)
    {
        target.TakeDamage(damageEvent) ;
    }
}
