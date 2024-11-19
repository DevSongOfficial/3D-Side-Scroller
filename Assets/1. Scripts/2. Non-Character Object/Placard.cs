using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;
using static GameSystem;

public class Placard : MonoBehaviour
{
    private byte par;
    [SerializeField] private Text text_ParIndicator;
    [SerializeField] private InputField inputField;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
    }

    private void Start()
    {
        canvas.worldCamera = GameObject.FindWithTag(Tag.EditorCamera.ToString()).GetComponent<Camera>();
    }

    private void OnEnable()
    {
        StageMaker.OnLoadComplete += SetTextToPar;
        LevelEditorManager.OnEditorModeToggled += SetInputfieldActivation;
        inputField.onValueChanged.AddListener(SetText);
    }

    private void OnDisable()
    {
        StageMaker.OnLoadComplete -= SetTextToPar;
        LevelEditorManager.OnEditorModeToggled -= SetInputfieldActivation;
        inputField.onValueChanged.RemoveAllListeners();
    }

    private void SetText(string text)
    {
        if (String.IsNullOrEmpty(text)) return;
        
        try 
        { 
            par = byte.Parse(inputField.text);
            text_ParIndicator.text = text;
        }
        catch 
        { 
            par = (int)ScoreType.Par;
            text_ParIndicator.text = par.ToString();
        }

        GameManager.SetPar(par);
    }

    private void SetTextToPar()
    {
        par = GameManager.Par;
        text_ParIndicator.text = par.ToString();
        inputField.text = par.ToString();
    }

    private void SetInputfieldActivation(bool isOn)
    {
        inputField.gameObject.SetActive(isOn);
    }
}
