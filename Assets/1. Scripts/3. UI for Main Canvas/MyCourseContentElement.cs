using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class MyCourseContentElement : MonoBehaviour
{
    public static MyCourseContentElement CurrentlySelected { get; private set; }

    private Button button;
    public event Action<string, string, string> OnButtonClicked;

    private string title;
    private string description;
    private string mapData;


    [SerializeField] private Text text_Title;


    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(RaiseEvent);
    }

    public string GetStageName()
    {
        return $"Stage_{title}";
    }

    public Dictionary<string, object> GetStageData()
    {
        return new Dictionary<string, object>
        {
            { "Map", mapData },
            { "Title", title },
            { "Description", description }
        };
    }
    public MyCourseContentElement SetTitle(string title)
    {
        text_Title.text = title;

        this.title = title;
        return this;
    }

    public MyCourseContentElement SetDescription(string description)
    {
        this.description = description;
        return this;
    }

    public MyCourseContentElement SetMapData(string mapData)
    {
        this.mapData = mapData;
        return this;
    }

    private void RaiseEvent()
    {
        CurrentlySelected = this;

        OnButtonClicked?.Invoke(title, description, mapData);
    }
}