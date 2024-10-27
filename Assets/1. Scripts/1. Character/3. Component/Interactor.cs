using System;
using UnityEngine;


// Interface for gameobjects that are interatcted by [Interactor] objects.
public interface IInteractable 
{
    void Interact(Interactor interactor);
}

public interface IPickupable 
{
    void OnPickedUp(Transform parent, bool shouldAlignToCenter);
    void OnDropedOff();
}

// Exposes only necessary [CharacterBase]'s interaction info.
public class Interactor
{
    // General
    private CharacterBase interactorCharacter;
    private IInteractable currentInteractableObject;

    public Interactor(CharacterBase interactor)
    {
         interactorCharacter = interactor;
    }

    public bool FindAndInteractWithinRange(float distance = 1.5f)
    {
        var components = interactorCharacter.Detector.ComponentsDetected<IInteractable>(interactorCharacter.Detector.ColliderCenter, distance, Layer.Interactable.GetMask());
        foreach (var component in components)
        {
            if(component == null) continue;

            currentInteractableObject = component;
            currentInteractableObject.Interact(this);
            return true;
        }

        return false;
    }

    public Vector3 GetPosition()
    {
        return interactorCharacter.Detector.ColliderCenter;
    }
    public float GetInteractionRange()
    {
        return interactorCharacter.Info.InteractionRange;
    }

    // Golfer
    public Golfer AsGolfer { get; private set; }
    public Interactor AddGolfer(ItemHolder itemHolder) // Builder for further chaining.
    {
        AsGolfer = new Golfer(this, itemHolder);
        return this;
    }

    // Driver
    public Driver AsDriver { get; private set; }
    public Interactor AddDriver()
    {
        AsDriver = new Driver(this);
        return this;
    }
}

public class Golfer
{
    private Interactor golfer;
    private ItemHolder itemHolder;
    public Golfer(Interactor golfer, ItemHolder itemHolder)
    {
        this.golfer = golfer;
        this.itemHolder = itemHolder;
    }

    // Golf Club
    public GolfClub CurrentClub { get; private set; }

    // Events
    public event Action OnClubSwitched;
    public void InvokeEvent_OnClubSwitched(Interactor sender) // [event] keyword and sender parameter for dependency injection.
    {
        if (sender != golfer) return;
        OnClubSwitched?.Invoke(); 
    }
    
    public void EquipClub(GolfClub newClub)
    {
        if (CurrentClub != null) UnequipClub();

        CurrentClub = GameObject.Instantiate(newClub, itemHolder.GetSlotTransform(newClub.SlotType));
        CurrentClub.gameObject.SetActive(true);
    }
    public void UnequipClub()
    {
        CurrentClub.gameObject.SetActive(false);
        CurrentClub = null;
    }
}

public class Driver 
{
    private Interactor driver;

    public Driver(Interactor driver)
    {
        this.driver = driver;
    }

    // Events
    public event Action OnEnterVehicle;
    public event Action<Vector3, Vector3> OnDrive;
    public event Action OnExitVehicle;
    public void InvokeEvent_OnEnterVehicle(IInteractable sender)
    {
        //if(sender != vehicle) return;
        OnEnterVehicle?.Invoke();
    }
    public void InvokeEvent_OnDrive(IInteractable sender, Vector3 poisition, Vector3 eulerAngles)
    {
        OnDrive?.Invoke(poisition, eulerAngles);
    }
    public void InvokeEvent_OnExitVehicle(IInteractable sender)
    {
        //if (sender != vehicle) return;
        OnExitVehicle?.Invoke();
    }

}