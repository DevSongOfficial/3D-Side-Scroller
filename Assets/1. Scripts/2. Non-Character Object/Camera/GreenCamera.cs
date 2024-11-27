using static GameSystem;
public sealed class GreenCamera : ProbFollowingCamera
{
    private void Start()
    {
        SetUp();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

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

    protected override void OnDisable()
    {
        base.OnDisable();

        if (!POFactory.HasRegisteredSingletonPO<GolfBall>()) return;

        var golfBall = POFactory.GetRegisteredSingletonPO<GolfBall>();
        golfBall.OnEnterGreen -= RenderGreenCamera;
        golfBall.OnExitGreen -= HideGreenCamera;
    }
}