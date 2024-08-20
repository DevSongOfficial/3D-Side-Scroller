using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZombieStateBase : StateBase
{
    protected ZombieCharacter zombie;
    protected ZombieBlackboard sharedData; // Data that shared with the other zombie states

    protected ZombieStateBase(ZombieCharacter zombieCharacter, ZombieBlackboard zomebieBlackboard) 
    {
        zombie = zombieCharacter;
        sharedData = zomebieBlackboard;
    }

    // For zombies walking strangely
    protected float GetProperMultiplierOnMove(ZombieMovementBase movement)
    {
        float multiplier = 1f; // Default value

        // For Zombie Walk 1 & Zombie Walk 2, zombie walks slowly between the frames below.
        // 1) 15 ~ 34,
        // 2) 75 ~ 95.
        // FYI: Max frame of the animation clip is 120.
        if (movement.GetAnimationType() == AnimationController.Zombie.Movement.ZombieMove1)
        {
            var frame = zombie.AnimationController.GetCurrentFrame(maxFrame: 120);
            multiplier = ((frame >= 15 && frame < 35) || (frame >= 75 && frame < 95)) ? 0.5f : 1f;
        }

        if (movement.GetAnimationType() == AnimationController.Zombie.Movement.ZombieMove2)
        {
            var frame = zombie.AnimationController.GetCurrentFrame(maxFrame: 120);
            multiplier = ((frame >= 15 && frame < 35) || (frame >= 75 && frame < 95)) ? 0.25f : 1f;
        }

        return multiplier;
    }
}