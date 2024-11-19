using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [Header("Prob Camera Stuff")]
    [SerializeField] private RawImage rawImage_ProbCameraOutput;
    [SerializeField] private TMP_Text text_ballHeight;
    
    [Space(10)]
    [SerializeField] private Image panel_ClubSelection;

    [Header("UI Elements Floating Above Player")]
    [SerializeField] GameObject group_FloatingAbovePlayer;
    [SerializeField] Image image_SwingChargeIndicator;

    [Header("Stage Clear")]
    [SerializeField] private Text text_StageClear;
    [SerializeField] private Image panel_StageClear;

    public RawImage RawImage_ProbCameraOutput => rawImage_ProbCameraOutput;
    public TMP_Text Text_ballHeight => text_ballHeight;
    public Image Panel_ClubSelection => panel_ClubSelection;
    public GameObject Group_FloatingAbovePlayer => group_FloatingAbovePlayer;
    public Image Image_SwingChargeIndicator => image_SwingChargeIndicator;
    
    public Image Panel_StageClear => panel_StageClear;
    public Text Text_StageClear => text_StageClear;
}
