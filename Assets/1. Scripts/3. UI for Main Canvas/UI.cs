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
    
    [Header("UI Elements Floating Above Player")]
    [SerializeField] Image image_SwingChargeIndicator;

    [Header("Stage Clear")]
    [SerializeField] private Text text_StageClear;
    [SerializeField] private Image panel_StageClear;

    [Header("Golf Clubs")]
    [SerializeField] private Image panel_ClubSelection;
    [SerializeField] private List<Image> images_golfClub;

    public RawImage RawImage_ProbCameraOutput => rawImage_ProbCameraOutput;
    public TMP_Text Text_ballHeight => text_ballHeight;
    public Image Panel_ClubSelection => panel_ClubSelection;
    public Image Image_SwingChargeIndicator => image_SwingChargeIndicator;
    
    public Image Panel_StageClear => panel_StageClear;
    public Text Text_StageClear => text_StageClear;

    public List<Image> Images_GolfClub => images_golfClub;
}
