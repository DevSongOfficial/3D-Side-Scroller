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
    private byte par_Default = 3;
    [SerializeField] private Text text_ParIndicator;
    [SerializeField] private InputField inputField;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
    }

    private void Start()
    {
        if (SceneLoader.IsMakerScene)
        {
            par = par_Default;
            GameManager.SetPar(par);

            canvas.worldCamera = GameObject.FindWithTag(Tag.EditorCamera.ToString()).GetComponent<Camera>();
        }
    }

    private void OnEnable()
    {
        GameManager.OnStageSetup += SetTextToPar;
        LevelEditorManager.OnEditorModeToggled += SetInputfieldActive;
        inputField.onValueChanged.AddListener(SetText);
    }

    private void OnDisable()
    {
        GameManager.OnStageSetup -= SetTextToPar;
        LevelEditorManager.OnEditorModeToggled -= SetInputfieldActive;
        inputField.onValueChanged.RemoveAllListeners();
    }

    private void SetText(string text)
    {
        if (String.IsNullOrEmpty(text)) return;
        
        try 
        { 
            par = byte.Parse(inputField.text);
            if (par > 7 || par < 3) throw new Exception();
            text_ParIndicator.text = text;
        }
        catch 
        {
            par = par_Default;
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

    private void SetInputfieldActive(bool isOn)
    {
        inputField.gameObject.SetActive(isOn);
    }
}
