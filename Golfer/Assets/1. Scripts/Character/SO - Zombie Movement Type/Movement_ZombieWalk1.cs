using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieMovementType", menuName = "Scriptable Object/Movement/Zombie Walk 1")]
public sealed class Movement_ZombieWalk1 : ZombieMovementBase
{
    public override void SetAnimationType()
    {
        SetAnimationType(AnimationController.State.ZombieMove1);
    }

    public override void Execute(MovementController movementController, CharacterInfo info, EMovementDirection wishDirection)
    {
        movementController.ChangeMovementDirection(wishDirection);
        movementController.SetVelocity(info.MovementSpeed * Time.deltaTime);
    }
}