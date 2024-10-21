using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieAttackType", menuName = "Scriptable Object/Zombie/Attack 1")]
public sealed class ZombieAttack1 : ZombieAttackBase
{
    [SerializeField] private DamageEvent damageEvent;

    public override void Execute(ZombieCharacter attacker, CharacterBase target)
    {
        var direction = attacker.MovementController.GetDirectionFrom(target);
        target.TakeDamage(damageEvent.ApplyDirection(direction));
    }
}
