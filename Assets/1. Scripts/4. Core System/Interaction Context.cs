using System;
using UnityEngine;

public interface IInteractable
{
    void Interact(InteractionInfo info);
}

public class InteractionInfo
{
    // General
    private CharacterBase interactor;
    public InteractionInfo(CharacterBase interactor)
    {
        this.interactor = interactor;
    }
    public Vector3 GetPosition()
    {
        return interactor.Detector.ColliderCenter;
    }

    // Golfer
    public IGolfer Golfer { get; private set; }
    public InteractionInfo WithGolfer(IGolfer golfer)
    {
        Golfer = golfer;
        return this;
    }
    public void AddListener_OnClubSwitched(Action action)
    {
        var player = interactor.AsPlayer();
        if(player != null) player.OnClubSwitched += action;
    }
    public void RemoveListener_OnClubSwitched(Action action)
    {
        var player = interactor.AsPlayer();
        if (player != null) player.OnClubSwitched -= action;
    }
}

public interface IGolfer
{
    void EquipClub(GolfClub club);
    void UnequipClub();
}