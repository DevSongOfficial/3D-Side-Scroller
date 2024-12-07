using static GameSystem;
public sealed class GreenCamera : ProbFollowingCamera
{
    protected override void Start()
    {
        base.Start();

        SetUp();
    }

    private void HideGreenCamera()
    {
        Camera.depth = -1;
    }

    private void RenderGreenCamera()
    {
        Camera.depth = 1;
    }

    private void SetUp()
    {
        if (!POFactory.HasRegisteredSingletonPO<GolfBall>()) return;

        var golfBall = POFactory.GetRegisteredSingletonPO<GolfBall>();
        golfBall.OnEnterGreen += RenderGreenCamera;
        golfBall.OnExitGreen += HideGreenCamera;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (!POFactory.HasRegisteredSingletonPO<GolfBall>()) return;

        var golfBall = POFactory.GetRegisteredSingletonPO<GolfBall>();
        golfBall.OnEnterGreen -= RenderGreenCamera;
        golfBall.OnExitGreen -= HideGreenCamera;
    }
}