using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
        (MovementController movementController, AnimationController animationController, ObjectInfo characterInfo, EMovementDirection wishDirection);
}