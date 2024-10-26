using UnityEngine;
using static AnimationController;

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

        if (!zombie.MovementController.IsGrounded)
        {
            zombie.ChangeState(zombie.StunnedState);
            return;
        }
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
            SetDirection(zombie.transform.forward.normalized).
            SetDistance(zombie.Info.DetectionDistance);

        if (!zombie.Detector.CharacterDetected(rayInfo, out CharacterBase target) || !zombie.MovementController.IsGrounded)
        {
            zombie.ChangeState(zombie.PatrolState);
            return;
        }
        
        if (zombie.IsTargetWithInDistance(blackBoard.targetCharacter, zombie.Info.AttackRange))
        {
            zombie.ChangeState(zombie.AttackState);
            return;
        }

        // Handle rotation X
        zombie.MovementController.AlignToGround();

        var wishDirection = zombie.MovementController.GetDirectionFrom(blackBoard.targetCharacter);
        movement.Execute(zombie.MovementController, zombie.AnimationController, zombie.Info, wishDirection);
    }
}