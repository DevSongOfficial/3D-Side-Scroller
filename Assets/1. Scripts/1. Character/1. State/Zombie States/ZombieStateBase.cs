public abstract class ZombieStateBase : StateBase
{
    protected ZombieCharacter zombie;
    protected ZombieBlackboard blackBoard; // Data that shared with the other zombie states

    protected ZombieStateBase(ZombieCharacter zombieCharacter, ZombieBlackboard zomebieBlackboard) 
    {
        zombie = zombieCharacter;
        blackBoard = zomebieBlackboard;
    }

    public override void FixedUpdateState()
    {
        base.FixedUpdateState();

        zombie.MovementController.ApplyVerticalVelocity(zombie.Info.Mass);
    }
}