using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class ZombiePatrolState : ZombieStateBase
{
    private ZombieMovementBase movement;
    private Vector3 targetPosition;

    public ZombiePatrolState(ZombieCharacter zombieCharacter , ZombieBlackboard zomebieBlackboard, ZombieMovementBase movementType) : base(zombieCharacter, zomebieBlackboard)
    {
        movement = movementType;
    }

    public override void EnterState()
    {
        base.EnterState();

        zombie.ChangeAnimationState(movement.GetAnimationType());
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

    private void Patrol()
    {
        movement.Execute(zombie, targetPosition, data.rigidBody);

        Debug.DrawRay(zombie.transform.position, zombie.transform.forward * zombie.info.DetectionDistance, Color.yellow);


        if (Physics.Raycast(zombie.transform.position, zombie.transform.forward * zombie.info.DetectionDistance, out RaycastHit hit))
        {
            var target = hit.collider.GetComponent<CharacterBase>();
            if (target == null) return;

            data.targetCharacter = target;
            zombie.ChangeState(zombie.zombieChaseState);
        }
    }
}