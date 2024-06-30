using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ZombieCharacter : CharacterBase
{
    private void MeleeAttack(CharacterBase character)
    {
        character.TakeDamage(new DamageEvent { damage = 10 });
    }
}
