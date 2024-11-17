using Unity.VisualScripting;
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
        // Get ray information.
        RayInfo rayInfo = new RayInfo().
                SetDirection(zombie.transform.forward.normalized).
                SetDistance(zombie.Info.DetectionDistance);

        // Detect character.
        if (zombie.Detector.DetectCharacter(rayInfo, out PlayerCharacter target))
        {
            blackBoard.targetCharacter = target;
            zombie.ChangeState(zombie.ChaseState);
            return;
        }

        // Detect wall or cliff to set direction.
        movementDirection = zombie.MovementController.FacingDirection;
        var wallOrCliffAhead = zombie.Detector.DetectWithLayer(rayInfo.SetDistance(zombie.Info.WallDetectionDistance), Layer.Ground) || zombie.Detector.DetectCliffAhead();
        if (wallOrCliffAhead && !zombie.MovementController.IsChangingDirection)
        {
            movementDirection = movementDirection.GetFlippedDirection();
        }

        // Handle rotation X.
        zombie.MovementController.AlignToGround();

        // For Debugging
        if (Input.GetKeyDown(KeyCode.X)) { movementDirection = MovementDirection.Right; }
        if (Input.GetKeyDown(KeyCode.Z)) { movementDirection = MovementDirection.Left; }

        movement.Execute(zombie.MovementController, zombie.AnimationController, zombie.Info, movementDirection);
    }
}