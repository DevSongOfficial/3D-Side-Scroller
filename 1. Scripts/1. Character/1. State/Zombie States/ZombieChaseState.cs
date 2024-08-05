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

        zombie.AnimationController.ChangeState(movement.GetAnimationType());
    }

    public override void EnterState()
    {
        base.EnterState();
    }

    public override void UpdateState()
    {
        base.UpdateState();

        Chase();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void Chase()
    {
        RayInfo rayInfo = new RayInfo(zombie.MovementController.Direction, zombie.Info.DetectionDistance * 2);
        if (zombie.Detector.CharacterDetected(rayInfo, out CharacterBase target))
        {
            var wishDirection = zombie.MovementController.GetDirectionFrom(sharedData.targetCharacter);
            movement.Execute(zombie.MovementController, zombie.Info, wishDirection);

            if(Vector3.Distance(zombie.transform.position, target.transform.position) < zombie.Info.AttackRange)
                zombie.ChangeState(zombie.AttackState);
        }
        else
        {
            zombie.ChangeState(zombie.PatrolState);
        }
    }
}
