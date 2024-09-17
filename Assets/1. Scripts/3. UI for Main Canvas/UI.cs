using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private Image panel_ClubSelection;

    [Header("UI Elements Floating Above Player")]
    [SerializeField] GameObject group_FloatingAbovePlayer;
    [SerializeField] Image image_SwingChargeIndicator;

    public Image Panel_ClubSelection => panel_ClubSelection;
    public GameObject Group_FloatingAbovePlayer => group_FloatingAbovePlayer;
    public Image Image_SwingChargeIndicator => image_SwingChargeIndicator;
}
