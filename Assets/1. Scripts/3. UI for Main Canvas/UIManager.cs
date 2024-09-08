using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(UI))]
public class UIManager : MonoBehaviour
{
    public static UI GetUI => instance_UI;
    private static UI instance_UI;

    private void Awake()
    {
        instance_UI = GetComponent<UI>();
    }

    public static void PopupUI(Image graphic)
    {
        graphic.gameObject.SetActive(true);
    }

    public static void CloseUI(MaskableGraphic graphic)
    {
        graphic.gameObject.SetActive(false);
    }
}
