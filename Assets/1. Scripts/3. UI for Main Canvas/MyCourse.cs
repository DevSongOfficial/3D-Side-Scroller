using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static GameSystem;

public sealed class MyCourse : MonoBehaviour
{
    [SerializeField] private Text text_CourseCount;

    [Header("Content of Scroll View")]
    [SerializeField] private RectTransform panel_Content;
    private List<MyCourseContentElement> elements = new List<MyCourseContentElement>();

    [Header("Course Info")]
    [SerializeField] private Text text_Title;
    [SerializeField] private Text text_Description;
    [SerializeField] private Text text_Par;


    private void OnEnable()
    {
        LoadMyCourses();
    }

    private void OnDisable()
    {
        ClearContentPanel();
    }

    // Going to be executed when the panel activated by user clicking the button.
    private async void LoadMyCourses()
    {
        SetupPage(await LoadCoursesDataAsync());
    }


    private async Task<Dictionary<string, object>> LoadCoursesDataAsync()
    {
        // Before data loaded.
        var loadingText = Instantiate(AssetManager.GetPrefab(Prefab.UI.Canvas_LoadingText));

        // Load data async.
        var rawData = await SaveManager.DownloadStageDataAsync("000000_ADMIN");

        // After data loaded.
        loadingText.SetActive(false);

        // Convert data.
        return rawData.ToStagesData();
    }

    private void SetupPage(Dictionary<string, object> stageData)
    {
        var elementPrefab = AssetManager.GetPrefab(Prefab.UI.Content_CourseInfo);

        if (stageData == null)
        {
            text_CourseCount.text = "0 / 10";
            return;
        }

        foreach (var stage in stageData)
        {
            var convertedData = JsonConvert.DeserializeObject<Dictionary<string, object>>(stage.Value.ToString());
            var element = Instantiate(elementPrefab, panel_Content).GetComponent<MyCourseContentElement>();
            elements.Add(element);

            element.SetTitle(convertedData["Title"].ToString())
                   .SetDescription(convertedData["Description"].ToString())
                   .SetMapData(convertedData["Map"].ToString());

            element.OnButtonClicked += ShowCourseInfo;
        }

        text_CourseCount.text = $"{stageData.Count} / 10";
    }

    private void ClearContentPanel()
    {
        if (elements.Count == 0) return;

        for (int i = elements.Count - 1; i >= 0; i--)
            Destroy(elements[i].gameObject);

        elements.Clear();
    }

    private async void DeleteElement(MyCourseContentElement element)
    {
        elements.Remove(element);
        Destroy(element.gameObject);

        await CloudManager.DeleteStageDataAsnyc("000000_ADMIN", element.GetStageName());
    }

    public void DeleteCurrentlySelectedElement()
    {
        DeleteElement(MyCourseContentElement.CurrentlySelected);
        ShowCourseInfo(string.Empty, string.Empty, string.Empty);
    }

    private void ShowCourseInfo(string title, string description, string mapDataHandler)
    {
        text_Title.text = title;

        text_Description.text = description;
        
        var mapData = JsonUtility.FromJson<StageDataHandler>(mapDataHandler);
        text_Par.text = mapDataHandler == string.Empty ? string.Empty : $"Par {mapData.par}";
    }
}
