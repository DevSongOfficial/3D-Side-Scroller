using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ZombieChaseState : ZombieStateBase
{
    private ZombieMovementBase movement;

    public ZombieChaseState(ZombieCharacter zombieCharacter, ZombieBlackboard zomebieBlackboard, ZombieMovementBase movementType) : base(zombieCharacter, zomebieBlackboard)
    {
        movement = movementType;
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }
}
