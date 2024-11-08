using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum PopupType
{
    Static, SlideUp, SlideHorizontal, MoveAndFadeOut
}

[RequireComponent(typeof(UI))]
public class System_UIManager : MonoBehaviour
{
    public UI GetUI => instance_UI;
    private UI instance_UI;
    public Canvas Canvas { get; private set; }

    private List<TMP_Text> texts_MoveAndFadeOut = new List<TMP_Text>();

    public Vector3 floatingPosition => Camera.main.WorldToScreenPoint(playerTransform.position);
    private Transform playerTransform;

    private void Awake()
    {
        Canvas = GetComponent<Canvas>();
        instance_UI = GetComponent<UI>();
        playerTransform = FindObjectOfType<PlayerCharacter>().transform;
    }

    private void FixedUpdate()
    {
        UpdateUIAlphaToBeZero();
    }

    private void LateUpdate()
    {
        UpdateFloatingUIPositions();
    }

    private void UpdateUIAlphaToBeZero()
    {
        foreach (var text in texts_MoveAndFadeOut)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - 0.5f * Time.deltaTime);
            text.rectTransform.localPosition += new Vector3(30 * Time.deltaTime, 90 * Time.deltaTime, 0);

            if (text.color.a <= 0) CloseUI(text);
        }
    }

    private void UpdateFloatingUIPositions()
    {
        GetUI.Group_FloatingAbovePlayer.transform.position = floatingPosition;
    }

    public void PopupUI(MaskableGraphic graphic)
    {
        graphic.gameObject.SetActive(true);
    }

    public void PopupUI(TMP_Text text, Vector3 position, PopupType popupType)
    {
        PopupUI(text);
        text.transform.position = position;

        if(popupType == PopupType.MoveAndFadeOut) texts_MoveAndFadeOut.Add(text);
    }

    public void CloseUI(MaskableGraphic graphic)
    {
        graphic.gameObject.SetActive(false);
    }

    public void FillImage(Image image, float fillAmount /* 0 ~ 1 */)
    {
        image.fillAmount = fillAmount;
    }
}