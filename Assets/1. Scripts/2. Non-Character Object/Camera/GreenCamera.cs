using static GameSystem;
public sealed class GreenCamera : ProbFollowingCamera
{
    private void Start()
    {
        var golfBall = POFactory.GetRegisteredSingletonPO<GolfBall>();
        golfBall.OnEnterGreen += RenderGreenCamera;
        golfBall.OnExitGreen += HideGreenCamera;
    }

    private void HideGreenCamera()
    {
        Camera.depth = -1;
    }

    private void RenderGreenCamera()
    {
        Camera.depth = 1;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        var golfBall = POFactory.GetRegisteredSingletonPO<GolfBall>();
        golfBall.OnEnterGreen -= RenderGreenCamera;
        golfBall.OnExitGreen -= HideGreenCamera;
    }
}