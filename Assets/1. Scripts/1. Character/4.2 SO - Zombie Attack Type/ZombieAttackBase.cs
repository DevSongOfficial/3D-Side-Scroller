using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class ZombieAttackBase : ScriptableObject
{
    [Tooltip("Cooldown duration after attack")]
    [SerializeField] private float cooldownTime;

    [Space(5)]
    [Tooltip("Animation clip to play on attack")]
    [SerializeField] private AnimationController.Zombie.Attack animationType;

    public AnimationController.Zombie.Attack AnimationType => animationType;
    public float CooldownTime => cooldownTime;
    

    public abstract void Execute(ZombieCharacter attacker, CharacterBase target /* Target character to attack */);
}