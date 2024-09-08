using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class GolfBag : MonoBehaviour, IInteractable
{
    // Golf clubs
    private GolfClub currentClub => clubs[clubIndex];
    [SerializeField] private GolfClub[] clubs;
    private int clubIndex;

    // Timer where the bag automatically closes when time's up.
    private float timeLeft;
    private const float Duration = 15;


    // The character who currently has access to or is interacting with the bag.
    private InteractionInfo interactorInfo;
    public bool IsOpen => interactorInfo != null;

    public static readonly float InteractionRange = 1.5f;

    public void Interact(InteractionInfo info)
    {
        if (info.Golfer == null) return;

        if (IsOpen)
        {
            CloseTheBag();
            return;
        }

        interactorInfo = info;
        OpenTheBag();
    }

    private void Update()
    {
        CheckDistance();
        UpdateTimer();
    }

    private void OpenTheBag()
    {
        interactorInfo.AddListener_OnClubSwitched(SwitchToNextClub);

        // Start timer
        timeLeft = 0;

        UIManager.PopupUI(UIManager.GetUI.Panel_ClubSelection);
    }

    private void CloseTheBag() 
    {
        interactorInfo.RemoveListener_OnClubSwitched(SwitchToNextClub);
        interactorInfo = null;

        UIManager.CloseUI(UIManager.GetUI.Panel_ClubSelection);
    }

    private void UpdateTimer()
    {
        if (!IsOpen) return;

        if (timeLeft < Duration)
        {
            timeLeft += Time.deltaTime;
        }
        else
        {
            CloseTheBag();
        }
    }

    private void SwitchToNextClub()
    {
        if (!IsOpen) return;

        clubIndex++;
        clubIndex %= clubs.Length;
        interactorInfo.Golfer.EquipClub(currentClub);
    }

    private void CheckDistance()
    {
        if (!IsOpen) return;

        var distance = Vector3.Distance(transform.position, interactorInfo.GetPosition());
        if(distance > InteractionRange)
        {
            CloseTheBag();
        }
    }
}