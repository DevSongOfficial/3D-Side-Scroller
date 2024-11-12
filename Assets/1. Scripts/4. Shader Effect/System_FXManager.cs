using Cinemachine;
using System;
using System.Collections;
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

    public void SetCameraScreenX(float position = 0.5f)
    {
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenX = position;
    }

    public void SetCameraScreenY(float position = 0.5f)
    {
        virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_ScreenY = position;
    }

    public void SetCameraFOB(float value = 40)
    {
        virtualCamera.m_Lens.FieldOfView = value;
    }


    // VFX SECTION
    public VFX CreateEffect(Prefab.VFX effectPrefab, Transform parent, bool isSharedMaterial = false)
    {
        var effect = Instantiate(AssetManager.GetPrefab(effectPrefab), parent).GetComponent<VFX>();
        effect.Initialize(isSharedMaterial);

        return effect;
    }


    // TIME SCALE SECTION
    public bool IsSlowMotioning => slowMotionCoroutine != null;
    public void StartSlowMotion(float minTimeScale, float slowMultiplier, float resetDelay = -1, Action OnFXEnded = null)
    {
        if(slowMotionCoroutine != null)
        {
            StopCoroutine(slowMotionCoroutine);
            slowMotionCoroutine = null;
        }
        slowMotionCoroutine = StartCoroutine(SlowMotionRoutine(minTimeScale, slowMultiplier, resetDelay, OnFXEnded));
    }

    private Coroutine slowMotionCoroutine;
    private IEnumerator SlowMotionRoutine(float minTimeScale, float slowMultiplier, float resetDelay, Action OnFXEnded)
    {
        float defaultFixedDeltaTime = 0.02f;
        int defaultTimeScale = 1;
        float timeScale = defaultTimeScale;

        while(timeScale > minTimeScale)
        {
            timeScale = Mathf.Clamp(timeScale - Time.unscaledDeltaTime * slowMultiplier, minTimeScale, defaultTimeScale);

            Time.timeScale = timeScale;
            Time.fixedDeltaTime = Time.timeScale * defaultFixedDeltaTime;

            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }

        if(resetDelay > 0) yield return new WaitForSecondsRealtime(resetDelay);
        Time.timeScale = defaultTimeScale;
        Time.fixedDeltaTime = defaultFixedDeltaTime;

        OnFXEnded?.Invoke();

        slowMotionCoroutine = null;
    }

}