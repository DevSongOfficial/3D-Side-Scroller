using UnityEngine;

public sealed class ZombiePatrolState : ZombieStateBase
{
    private ZombieMovementBase movement;
    private MovementDirection movementDirection = MovementDirection.None;

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

        if (!zombie.MovementController.IsGrounded)
        {
            zombie.ChangeState(zombie.StunnedState);
            return;
        }

        // For Debugging
        if (Input.GetKeyDown(KeyCode.X)) { movementDirection = MovementDirection.Right; }
        if (Input.GetKeyDown(KeyCode.Z)) { movementDirection = MovementDirection.Left; }
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
        // Detect character.
        RayInfo rayInfo = new RayInfo().
            SetDirection(zombie.transform.forward.normalized).
            SetDistance(zombie.Info.DetectionDistance);

        if (zombie.Detector.CharacterDetected(rayInfo, out PlayerCharacter target))
        {
            blackBoard.targetCharacter = target;
            zombie.ChangeState(zombie.ChaseState);
            return;
        }

        // Handle rotation X
        if (Physics.Raycast(zombie.transform.position, Vector3.down, out RaycastHit hit, 1.2f, Layer.Default.GetMask()))
        {
            Quaternion targetRotation = Quaternion.FromToRotation(zombie.transform.up, hit.normal) * zombie.transform.rotation;
            zombie.transform.rotation = Quaternion.Slerp(zombie.transform.rotation, targetRotation, Time.deltaTime * 10f);
        }

        movement.Execute(zombie.MovementController, zombie.AnimationController, zombie.Info, movementDirection);
    }
}