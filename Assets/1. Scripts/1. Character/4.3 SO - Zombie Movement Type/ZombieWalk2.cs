using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieMovementType", menuName = "Scriptable Object/Zombie/Walk 2")]
public sealed class ZombieWalk2 : ZombieMovementBase
{
    public override void Execute(MovementController movementController, CharacterInfo info, EMovementDirection wishDirection)
    {
        movementController.ChangeMovementDirection(wishDirection);
        movementController.SetVelocity(info.MovementSpeed * velocityMultiplier * 2 * Time.deltaTime);
    }
}
