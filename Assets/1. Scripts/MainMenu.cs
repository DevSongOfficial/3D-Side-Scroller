using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using static GameSystem;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Settings")]
    [SerializeField] private Text text_StageNumber;
    [SerializeField] private Text text_StageScore;
    [SerializeField] private Text text_PlayerButton;

    [Header("Course Settings")]
    [SerializeField] private Image panel_CoursePanelBackground;
    [SerializeField] private Image panel_CoureInfo;
    [SerializeField] private Text text_Title;
    [SerializeField] private Text text_Description;
    [SerializeField] private Text text_Par;

    [Header("Others")]
    [SerializeField] private Transform golfCart;
    [SerializeField] private Animator golfCartAnimator;
    
    private void Awake()
    {
        GameManager.OnStageNumberChanged += SetStageNumberText;
        GameManager.OnStageNumberChanged += SetStageScoreText;
        GameManager.OnStageNumberChanged += SetPlayButtonTextColor;
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
        if (SceneChangeCoroutine != null) return;
        if (!GameManager.CanStartTheStage(GameManager.GetStageNumber())) return;

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
        {
            GameManager.SetUserStageData(null);
            SceneChangeCoroutine = StartCoroutine(StartMakerRoutine());
        }
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
    
    #region Finding Others' Courses
    public async void LoadPage()
    {
        // Before scene load

        UIManager.PopupUI(panel_CoursePanelBackground);
        var loadingText = Instantiate(AssetManager.GetPrefab(Prefab.UI.Canvas_LoadingText));


        // Load data async.
        //var rawData = await SaveManager.DownloadStageDataAsync();
        //var rawData = await SaveManager.DownloadStageDataAsync();
        var rawData = await SaveManager.DownloadStageDataAsync("000000_ADMIN", "ABC");

        // After scene load
        loadingText.SetActive(false);
        UIManager.PopupUI(panel_CoureInfo);

        // Convert data.
        var stageData = rawData.ToStageData();

        var title = stageData["Title"];
        var description = stageData["Description"];
        
        var mapDataHandler = stageData["Map"].ToString();
        var mapData = JsonUtility.FromJson<StageDataHandler>(mapDataHandler);

        var par = mapData.par;


        // Show Stage Info.
        text_Title.text = title.ToString();
        text_Description.text = description.ToString();
        text_Par.text = par.ToString();

        // Set stage to start.
        GameManager.SetUserStageData(stageData);

        Invoke("PlayUserStage", 3);
    }

    public void PlayUserStage()
    {
        if(GameManager.GetUserStageData() != null)
            SceneLoader.LoadScene(Scene.Maker, TransitionEffect.FadeToBlack);
    }
    #endregion

    private void SetStageNumberText(int stageNumber) 
    {
        text_StageNumber.text = stageNumber.ToString();
    }

    private void SetPlayButtonTextColor(int stageNumber)
    {
        text_PlayerButton.color = GameManager.CanStartTheStage(stageNumber) ? Color.white : Color.gray;
    }

    private void SetStageScoreText(int stageNumber)
    {
        if(GameManager.GetScoreOfStageByIndex(stageNumber, out ScoreType score))
            text_StageScore.text = $"[{score}]";
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

