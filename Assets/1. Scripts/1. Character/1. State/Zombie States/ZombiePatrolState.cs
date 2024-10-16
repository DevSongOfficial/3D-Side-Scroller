using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ZombiePatrolState : ZombieStateBase
{
    private ZombieMovementBase movement;
    private EMovementDirection movementDirection = EMovementDirection.None;

    public ZombiePatrolState(ZombieCharacter zombieCharacter, ZombieBlackboard zomebieBlackboard, ZombieMovementBase movementType) : base(zombieCharacter, zomebieBlackboard)
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

        // For Debugging
        if (Input.GetKeyDown(KeyCode.X)) { movementDirection = EMovementDirection.Right; }
        if (Input.GetKeyDown(KeyCode.Z)) { movementDirection = EMovementDirection.Left; }
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        Patrol();
    }

    public override void ExitState()
    {
        base.ExitState();
    }

    private void Patrol()
    {
        RayInfo rayInfo = new RayInfo(zombie.MovementController.Direction, zombie.Info.DetectionDistance);

        if (zombie.Detector.CharacterDetected(rayInfo, out PlayerCharacter target))
        {
            sharedData.targetCharacter = target;
            zombie.ChangeState(zombie.ChaseState);
            return;
        }
        
        movement.Execute(zombie.MovementController, zombie.AnimationController, zombie.Info, movementDirection);
    }
}