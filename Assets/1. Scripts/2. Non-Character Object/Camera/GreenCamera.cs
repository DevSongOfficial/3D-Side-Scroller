using System.Diagnostics;
using UnityEngine.Rendering;
using static GameSystem;
public sealed class GreenCamera : ProbFollowingCamera
{
    protected override void Awake()
    {
        base.Awake();

        GameManager.OnGreen += GameManager_OnGreen;
        GameManager.OnExitGreen += GameManager_OnExitGreen;
    }

    private void GameManager_OnExitGreen()
    {
        Camera.depth = -1;
    }

    private void GameManager_OnGreen()
    {
        Camera.depth = 1;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        GameManager.OnGreen -= GameManager_OnGreen;
        GameManager.OnExitGreen -= GameManager_OnExitGreen;
    }
}