using UnityEngine;

public class PlayerMoveOnZAxisState : PlayerStateBase
{
    public PlayerMoveOnZAxisState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    private const float animationTransitionTime = 0.1f;

    public override void EnterState()
    {
        base.EnterState();

        if (!player.MovementController.MoveOnZAxis(blackBoard.InputZDirection)) SwitchToMoveState();
        player.MovementController.OnZMovementEnded += SwitchToMoveState;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        HandleAnimation();
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

    }

    public override void ExitState()
    {
        base.ExitState();

        player.MovementController.OnZMovementEnded -= SwitchToMoveState;
    }

    private void HandleAnimation()
    {
        var param_MoveSpeed = AnimationController.Parameter.MoveSpeed;
        var speed = Mathf.Abs(player.MovementController.Velocity.z);
        player.AnimationController.SetFloat(param_MoveSpeed, speed);
    }


    private void SwitchToMoveState()
    {
        player.ChangeState(player.MoveState);
    }
}
