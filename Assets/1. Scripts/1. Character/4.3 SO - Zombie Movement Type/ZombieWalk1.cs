using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieMovementType", menuName = "Scriptable Object/Zombie/Walk 1")]
public sealed class ZombieWalk1 : ZombieMovementBase
{
    public override void Execute(CharacterMovementController movementController, AnimationController animationController, ObjectInfo info, MovementDirection wishDirection)
    {
        movementController.ChangeMovementDirection(wishDirection);
        
        var frame = animationController.GetCurrentFrame(maxFrame: 120);
        float multiplier = ((frame >= 15 && frame < 35) || (frame >= 75 && frame < 95)) ? 0f : 1f;

        movementController.ApplyHorizontalVelocity(info.MovementSpeed * multiplier, info.Acceleration);
    }
}