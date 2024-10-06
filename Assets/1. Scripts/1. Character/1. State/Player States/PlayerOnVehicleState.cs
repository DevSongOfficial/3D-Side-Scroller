using UnityEngine;

public class PlayerOnVehicleState : PlayerStateBase
{
    public PlayerOnVehicleState(PlayerCharacter playerCharacter, PlayerBlackboard playerBlackboard) : base(playerCharacter, playerBlackboard) { }

    private readonly Vector3 originalLocalPosition = new Vector3(0, 0.483f, 0);
    private readonly Vector3 originalLocalEulerAngles = Vector3.zero;
    private readonly Vector3 carLocalPosition = new Vector3(0, -1.269f + 0.483f, 0.2f);
    private readonly Vector3 carLocalEulerAngles = Vector3.up * 17;
    private readonly Vector3 originalEulerAngles = Vector3.up * 90;

    
    public override void EnterState()
    {
        base.EnterState();

        player.Interactor.AsDriver.OnDrive += Drive;

        player.Detector.DisableCollider();
        player.MovementController.EnableKinematic();

        player.MovementController.SetBodyLocalEulerAngles(carLocalEulerAngles);
        player.MovementController.SetBodyLocalPosition(carLocalPosition);

        player.AnimationController.ChangeState(AnimationController.Player.Movement.Drive, transitionDuration: 0);
    }

    public override void ExitState()
    {
        base.ExitState();

        player.Interactor.AsDriver.OnDrive -= Drive;
        
        player.Detector.EnableCollider();
        player.MovementController.DisableKinematic();

        player.MovementController.SetBodyLocalEulerAngles(originalLocalEulerAngles);
        player.MovementController.SetBodyLocalPosition(originalLocalPosition);
        player.transform.eulerAngles = originalEulerAngles;

    }

    private void Drive(Vector3 newPosition, Vector3 eulerAngles)
    {
        player.MovementController.SetPosition(newPosition);
        player.transform.eulerAngles = eulerAngles;
    }
}
