using UnityEngine;

public class PlayerOnVehicleState : PlayerStateBase
{
    public PlayerOnVehicleState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    private readonly Vector3 originalLocalPosition = new Vector3(0, 0.483f, 0);
    private readonly Vector3 originalLocalEulerAngles = Vector3.zero;
    private readonly Vector3 offset_Position = new Vector3(0, -1.269f + 0.483f, 0.2f);
    private readonly Vector3 offset_Rotation = new Vector3(0, 17, 0);

    
    public override void EnterState()
    {
        base.EnterState();

        player.MovementController.SetBodyLocalEulerAngles(offset_Rotation);
        player.MovementController.SetBodyLocalPosition(offset_Position);

        player.Interactor.AsDriver.OnDrive += Drive;
        player.Interactor.AsDriver.OnChangeDirection += ChangeDirection;

        player.Detector.DisableCollider();
        player.MovementController.EnableKinematic();

        player.AnimationController.ChangeState(AnimationController.Player.Movement.Drive, transitionDuration: 0);
    }

    public override void ExitState()
    {
        base.ExitState();
        
        player.MovementController.SetBodyLocalEulerAngles(originalLocalEulerAngles);
        player.MovementController.SetBodyLocalPosition(originalLocalPosition);

        player.Interactor.AsDriver.OnDrive -= Drive;
        player.Interactor.AsDriver.OnChangeDirection -= ChangeDirection;

        player.Detector.EnableCollider();
        player.MovementController.DisableKinematic();
    }

    private void Drive(Vector3 newPosition, float rotationX)
    {
        player.MovementController.SetPosition(newPosition);
        player.transform.eulerAngles = new Vector3(rotationX, player.transform.eulerAngles.y, 0);
    }

    private void ChangeDirection(EMovementDirection direction)
    {
        player.MovementController.ChangeMovementDirection(direction);
    }
}
