using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieMovementType", menuName = "Scriptable Object/Zombie/Walk 2")]
public sealed class ZombieWalk2 : ZombieMovementBase
{
    public override void Execute(CharacterMovementController movementController, AnimationController animationController, ObjectInfo info, MovementDirection wishDirection)
    {
        movementController.ChangeMovementDirection(wishDirection);

        var frame = animationController.GetCurrentFrame(maxFrame: 120);
        float speedMultiplier = ((frame >= 15 && frame < 35) || (frame >= 75 && frame < 95)) ? 0f : 2f;
        float accelerationMultiplier = 2;

        movementController.ApplyHorizontalVelocity(info.MovementSpeed * speedMultiplier, info.Acceleration * accelerationMultiplier);
    }
}
