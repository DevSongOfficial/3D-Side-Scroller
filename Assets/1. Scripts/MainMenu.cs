using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static GameSystem;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Text text_StageNumber;

    [Space]
    [SerializeField] private Transform golfCart;
    [SerializeField] private Animator golfCartAnimator;


    private void Awake()
    {
        GameManager.OnStageNumberChanged += SetStageNumberText;
    }

    private void Start()
    {
        if (SceneLoader.CurrentScene != Scene.Menu)
            SceneLoader.LoadScene(Scene.Menu);

        ShowMenuCoroutine = StartCoroutine(ShowMenuRoutine());
    }

    private Coroutine ShowMenuCoroutine;

    private IEnumerator ShowMenuRoutine()
    {
        yield return new WaitForEndOfFrame();

        GameManager.SetStageNumber(1);

        golfCart.position = new Vector3(-5.35f, 0.98f, 3.53f);
        golfCartAnimator.enabled = true;
        
        while (golfCart.position.x < -1.2f)
        {
            golfCart.Translate(Vector3.right * 2 * Time.fixedDeltaTime, Space.World);
            yield return new WaitForFixedUpdate();
        }

        golfCartAnimator.enabled = false;
        ShowMenuCoroutine = null;
    }


    private Coroutine SceneChangeCoroutine;
    #region Scene Change into Main Game
    public void StartGame()
    {
        if (SceneChangeCoroutine == null) 
            SceneChangeCoroutine = StartCoroutine(StartGameRoutine());
    }


    private IEnumerator StartGameRoutine()
    {
        yield return ShowMenuCoroutine;

        golfCartAnimator.enabled = true;

        while (golfCart.position.x < 3.5f)
        {
            golfCart.Translate(Vector3.right * 2 * Time.fixedDeltaTime, Space.World);
            yield return new WaitForFixedUpdate();
        }

        SceneLoader.LoadScene(Scene.Main, TransitionEffect.FadeToBlack);
    }
    #endregion
    #region Scene Change into Maker
    public void StartMaker()
    {
        if(SceneChangeCoroutine == null)
            SceneChangeCoroutine = StartCoroutine(StartMakerRoutine());
    }

    private IEnumerator StartMakerRoutine()
    {
        yield return ShowMenuCoroutine;

        golfCartAnimator.enabled = true;

        while (golfCart.position.x < 3.5f)
        {
            golfCart.Translate(Vector3.right * 2 * Time.fixedDeltaTime, Space.World);
            yield return new WaitForFixedUpdate();
        }

        SceneLoader.LoadScene(Scene.Maker, TransitionEffect.FadeToBlack);
    }
    #endregion

    private void SetStageNumberText(int stageNumber) 
    {
        text_StageNumber.text = stageNumber.ToString();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 어플리케이션 종료
#endif
    }
}
