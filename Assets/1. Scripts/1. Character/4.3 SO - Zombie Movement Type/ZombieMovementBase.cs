using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static AnimationController;

public abstract class ZombieMovementBase : ScriptableObject
{  
    // Animation clip to play
    // [animationType] must be assigned in the inspector.
    [SerializeField] private AnimationController.Zombie.Movement animationType;
    public AnimationController.Zombie.Movement GetAnimationType() 
    {
        return animationType;
    }

    public abstract void Execute
        (MovementController movementController, CharacterInfo characterInfo, EMovementDirection wishDirection);

    // Speed effect handled by states
    protected float velocityMultiplier = 1;
    public void ApplyVelocityMultiplier(float multiplier)
    {
        velocityMultiplier = multiplier;
    }
}