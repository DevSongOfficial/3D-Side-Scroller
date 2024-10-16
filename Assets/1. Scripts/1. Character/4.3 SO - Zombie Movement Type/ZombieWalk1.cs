using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieMovementType", menuName = "Scriptable Object/Zombie/Walk 1")]
public sealed class ZombieWalk1 : ZombieMovementBase
{
    public override void Execute(MovementController movementController, AnimationController animationController, ObjectInfo info, EMovementDirection wishDirection)
    {
        movementController.ChangeMovementDirection(wishDirection);
        
        var velocity = movementController.CalculateVelocity(info.MovementSpeed, info.Mass);

        var frame = animationController.GetCurrentFrame(maxFrame: 120);
        float multiplier = ((frame >= 15 && frame < 35) || (frame >= 75 && frame < 95)) ? 0.5f : 1f;

        movementController.Move(velocity * multiplier * Time.fixedDeltaTime);
    }


}