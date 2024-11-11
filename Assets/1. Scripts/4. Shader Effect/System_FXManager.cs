using Cinemachine;
using UnityEngine;
using static GameSystem;

public sealed class System_FXManager : MonoBehaviour
{
    private void Awake()
    {
        virtualCamera = Camera.GetComponent<CinemachineVirtualCamera>();
    }

    // CAMERA SECTION
    [Header("Camera & Cinemachine")]
    [SerializeField] private CinemachineBrain cinemachineBrain;
    [SerializeField] private ProbFollowingCamera camera_Green;
    [SerializeField] private ProbFollowingCamera camera_GolfBall;

    private Camera Camera => Camera.main;
    private CinemachineVirtualCamera virtualCamera;
    public void SetCameraUpdateMethod(CinemachineBrain.UpdateMethod method)
    {
        cinemachineBrain.m_UpdateMethod = method;
    }

    public void ZoomIn()
    {
        virtualCamera.m_Lens.OrthographicSize = 40;
    }


    // VFX SECTION
    public VFX CreateEffect(Prefab.VFX effectPrefab, Transform parent, bool isSharedMaterial = false)
    {
        var effect = Instantiate(AssetManager.GetPrefab(effectPrefab), parent).GetComponent<VFX>();
        effect.Initialize(isSharedMaterial);

        return effect;
    }
}