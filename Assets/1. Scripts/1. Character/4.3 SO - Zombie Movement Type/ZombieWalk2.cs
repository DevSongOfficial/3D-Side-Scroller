using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieMovementType", menuName = "Scriptable Object/Zombie/Walk 2")]
public sealed class ZombieWalk2 : ZombieMovementBase
{
    public override void Execute(MovementController movementController, AnimationController animationController, ObjectInfo info, EMovementDirection wishDirection)
    {
        movementController.ChangeMovementDirection(wishDirection);

        var velocity = movementController.CalculateVelocity(info.MovementSpeed, info.Mass);

        int frame = animationController.GetCurrentFrame(maxFrame: 120);
        float multiplier = ((frame >= 15 && frame < 35) || (frame >= 75 && frame < 95)) ? 0.25f : 1f;

        movementController.Move(velocity * multiplier * 2 * Time.fixedDeltaTime);
    }
}
