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

        zombie.animationController.ChangeState(movement.GetAnimationType());
    }

    public override void UpdateState()
    {
        base.UpdateState();

        Patrol();
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
        if (Input.GetKeyDown(KeyCode.X)) { movementDirection = EMovementDirection.Right; }
        if (Input.GetKeyDown(KeyCode.Z)) { movementDirection = EMovementDirection.Left; }
        movement.Execute(zombie.movementController, zombie.Info, movementDirection);

        var rayOffset = new Vector3(0, 1, 0);
        Debug.DrawRay(zombie.transform.position + rayOffset, zombie.transform.forward * zombie.Info.DetectionDistance, Color.yellow);


        if (Physics.Raycast(zombie.transform.position + rayOffset, zombie.transform.forward * zombie.Info.DetectionDistance, out RaycastHit hit))
        {
            var target = hit.collider.GetComponent<CharacterBase>();
            if (target == null) return;

            sharedData.targetCharacter = target;

            //zombie.ChangeState(zombie.zombieChaseState);
        }
    }
}