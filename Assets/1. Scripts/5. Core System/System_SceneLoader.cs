using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static GameSystem;

public enum Scene { Menu = 0, Tutorial = 1, Main = 2, Maker = 3 };
public enum TransitionEffect { Nothing, FadeToBlack }
public class System_SceneLoader : MonoBehaviour
{
    public Scene CurrentScene { get; private set; }
    public bool IsLoading { get; private set; }
    public bool IsMakerScene => CurrentScene == Scene.Maker;
    public bool IsMainScene => CurrentScene == Scene.Main;
    public bool IsMenuScene => CurrentScene == Scene.Menu;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void LoadScene(Scene scene, TransitionEffect effect = TransitionEffect.Nothing)
    {
        if (IsLoading) return;
        IsLoading = true;

        previousTransitionEffect = effect;

        switch (effect)
        {
            case TransitionEffect.Nothing:
                StartCoroutine(LoadSceneRoutine(scene));
                break;
            case TransitionEffect.FadeToBlack: 
                StartCoroutine(LoadSceneWithFadeToBlack(scene)); 
                break;
        }
        
    }

    private Image blackScreen;
    private TransitionEffect previousTransitionEffect;
    private IEnumerator LoadSceneWithFadeToBlack(Scene scene)
    {
        if (blackScreen == null)
            blackScreen = Instantiate(AssetManager.GetPrefab(Prefab.UI.CanvasGroup_BlackScreen), transform)
                          .GetComponentInChildren<Image>();
        

        yield return StartCoroutine(Utility.FadeInRoutine(blackScreen));

        StartCoroutine(LoadSceneRoutine(scene));
    }
    
    private IEnumerator LoadSceneRoutine(Scene scene)
    {
        CurrentScene = scene;
        
        POFactory?.RemoveEveryRegisterdPO();

        var mAsyncOperation = SceneManager.LoadSceneAsync(scene.ToString());
        yield return mAsyncOperation;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (blackScreen == null) return;
        if (previousTransitionEffect != TransitionEffect.FadeToBlack) return;

        StartCoroutine(OnSceneLoadedRoutine());
    }

    private IEnumerator OnSceneLoadedRoutine()
    {
        yield return new WaitForEndOfFrame();

        IsLoading = false;
        StartCoroutine(Utility.FadeOutRoutine(blackScreen));
    }
}