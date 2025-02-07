using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameSystem;

public enum PopupType
{
    Static, FadeIn, SlideUp, SlideHorizontal, MoveAndFadeOut
}

[RequireComponent(typeof(UI))]
public class System_UIManager : MonoBehaviour
{
    public UI UI => instance_UI;
    private UI instance_UI;
    public Canvas Canvas { get; private set; }

    private List<MaskableGraphic> UIs_MoveAndFadeOut = new List<MaskableGraphic>();

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
        instance_UI = GetComponent<UI>();
    }

    private void FixedUpdate()
    {
        HandleUIs_MoveAndFadeOut();
    }


    public void PopupUI(MaskableGraphic graphic)
    {
        graphic.gameObject.SetActive(true);
    }

    public void PopupUI(MaskableGraphic graphic, Vector3 position, PopupType popupType)
    {
        PopupUI(graphic);
        graphic.transform.position = position;

        switch (popupType)
        {
            case PopupType.MoveAndFadeOut:
                UIs_MoveAndFadeOut.Add(graphic);
                break;
            case PopupType.FadeIn:
                
                break;
        }
    }

    private void HandleUIs_MoveAndFadeOut()
    {
        foreach (var ui in UIs_MoveAndFadeOut)
        {
            ui.color = new Color(ui.color.r, ui.color.g, ui.color.b, ui.color.a - 0.5f * Time.deltaTime);
            ui.rectTransform.localPosition += new Vector3(30 * Time.deltaTime, 90 * Time.deltaTime, 0);

            if (ui.color.a <= 0) CloseUI(ui);
        }
    }

    public void CloseUI(MaskableGraphic graphic)
    {
        graphic.gameObject.SetActive(false);
    }

    public void SetText(TMP_Text text, string s)
    {
        text.text = s;
    }

    public void SetText(Text text, string s) 
    { 
        text.text = s; 
    }

    public string AddSpaceBeforeUppercase(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;

        StringBuilder formattedString = new StringBuilder();

        for (int i = 0; i < s.Length; i++)
        {
            char currentChar = s[i];
            if (char.IsUpper(currentChar) && i > 0)
                formattedString.Append(' ');
            formattedString.Append(currentChar);
        }

        return formattedString.ToString();
    }

    public void FillImage(Image image, float fillAmount /* 0 ~ 1 */)
    {
        image.fillAmount = fillAmount;
    }
}