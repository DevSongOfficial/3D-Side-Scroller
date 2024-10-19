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
        (CharacterMovementController movementController, AnimationController animationController, ObjectInfo characterInfo, MovementDirection wishDirection);


    // For Zombie Walk 1 & Zombie Walk 2, zombie walks slowly between the frames below.
    // 1) 15 ~ 34,
    // 2) 75 ~ 95.
    // FYI: Max frame of the animation clip is 120.
}