using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieMovementType", menuName = "Scriptable Object/Movement/Zombie Walk 2")]
public sealed class Movement_ZombieWalk2 : ZombieMovementBase
{
    public override void SetAnimationType()
    {
        SetAnimationType(AnimationController.State.ZombieMove2);
    }

    public override void Execute(MovementController movementController, CharacterInfo info, EMovementDirection wishDirection)
    {
        movementController.ChangeMovementDirection(wishDirection);
        movementController.SetVelocity(info.MovementSpeed * 2 * Time.deltaTime);
    }
}
