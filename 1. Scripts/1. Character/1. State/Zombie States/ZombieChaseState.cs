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

        zombie.AnimationController.ChangeState(movement.GetAnimationType());
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        Chase();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void Chase()
    {
        RayInfo rayInfo = new RayInfo().
            SetDirection(zombie.MovementController.Direction.ConvertToVector3()).
            SetDistance(zombie.Info.DetectionDistance * 2);

        if (!zombie.Detector.CharacterDetected(rayInfo, out CharacterBase target))
        {
            zombie.ChangeState(zombie.PatrolState);
            return;

        }
        
        if (zombie.IsTargetWithInDistance(sharedData.targetCharacter, zombie.Info.AttackRange))
        {
            zombie.ChangeState(zombie.AttackState);
            return;
        }

        var wishDirection = zombie.MovementController.GetDirectionFrom(sharedData.targetCharacter);
        movement.ApplyVelocityMultiplier(GetProperMultiplierOnMove(movement));
        movement.Execute(zombie.MovementController, zombie.Info, wishDirection);
    }
}