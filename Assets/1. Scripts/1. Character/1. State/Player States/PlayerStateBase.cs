public abstract class PlayerStateBase : StateBase
{
    protected PlayerCharacter player;
    protected PlayerBlackboard blackBoard;

    protected PlayerStateBase(PlayerCharacter player, PlayerBlackboard data)
    {
        this.player = player;
        blackBoard = data;
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        // Apply gravity to player.
        player.MovementController.ApplyVerticalVelocity(player.Info.Mass);
    }
}
