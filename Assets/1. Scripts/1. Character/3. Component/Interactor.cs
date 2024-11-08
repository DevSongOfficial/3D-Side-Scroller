using System;
using UnityEngine;


// Interface for gameobjects that are interatcted by [Interactor] objects.
public enum InteractableType { Vehicle = 0, Equipment = 1} // Interaction Priority 
public interface IInteractable 
{
    void Interact(Interactor interactor);
    public InteractableType GetType();
}

public interface IPickupable 
{
    void OnPickedUp(Transform parent);
    void OnDropedOff();
}

// Exposes only necessary [CharacterBase]'s interaction info.
public class Interactor
{
    // General
    private CharacterBase interactorCharacter;
    private IInteractable currentlyInteractingObject;

    public Interactor(CharacterBase interactor)
    {
         interactorCharacter = interactor;
    }

    public bool FindAndInteractWithinRange(float distance = 1.5f)
    {
        var interactables = interactorCharacter.Detector.DetectComponents<IInteractable>(GetPosition(), distance, Layer.Interactable.GetMask(), putClosestInFirst: true);

        if (interactables.Count == 0) return false;
        IInteractable interactable = interactables[0];

        if (AsCarrier.IsCarryingItem)
        {
            currentlyInteractingObject = interactable;
            currentlyInteractingObject.Interact(this);
            return true;
        }

        if (interactables.Count > 1)
        {
            for(int i = 1; i < interactables.Count; i++)
            {
                // Check each's priority.
                if ((int)interactables[i].GetType() < (int)interactable.GetType())
                {
                    interactable = interactables[i];
                }
            }
        }

        currentlyInteractingObject = interactable;
        currentlyInteractingObject.Interact(this);
        return true;
    }

    public bool Toggle_FindAndPickupWithinRange(float distance = 1.5f)
    {
        if(AsCarrier == null) return false;

        if (AsCarrier.IsCarryingItem)
        {
            AsCarrier.DropOff();
            return true;
        }

        var pickupables = interactorCharacter.Detector.DetectComponents<IPickupable>(GetPosition(), distance, Layer.Interactable.GetMask());
        foreach (var pickupable in pickupables)
        {
            AsCarrier.PickUp(pickupable);
            return true;
        }

        return false;
    }

    private const float detectionDistance = 0.85f;
    public bool Toggle_FindAndLoadCart(float radius = 0.65f)
    {
        if (AsCarrier == null || !AsCarrier.IsCarryingItem) return false;

        var position = GetPosition() + detectionDistance * interactorCharacter.MovementController.FacingDirection.ConvertToVector3();
        var carts = interactorCharacter.Detector.DetectComponents<GolfCart>(position, radius, Layer.Interactable.GetMask());
        foreach(var cart in carts)
        {
            if (Vector3.Distance(cart.CarryPoint.position, position) > detectionDistance * 1.5f) continue;

            var item = AsCarrier.carryingItem;
            AsCarrier.DropOff();
            cart.LoadTheTrunk(item);

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
    public Interactor AddGolfer(ItemHolder itemHolder)
    {
        AsGolfer = new Golfer(itemHolder);
        return this;
    }

    // Driver
    public Driver AsDriver { get; private set; }
    public Interactor AddDriver()
    {
        AsDriver = new Driver();
        return this;
    }

    // Carrier
    public Carrier AsCarrier { get; private set; }
    public Interactor AddCarrier(Transform carryPoint)
    {
        AsCarrier = new Carrier(carryPoint);
        return this;
    }
}

public class Carrier
{
    private Transform carryPoint;
    public IPickupable carryingItem;
    public bool IsCarryingItem => carryingItem != null;
    
    public Carrier(Transform carryPoint)
    {
        this.carryPoint = carryPoint;
    }

    public void PickUp(IPickupable pickupable)
    {
        carryingItem = pickupable;
        carryingItem.OnPickedUp(carryPoint);
    }

    public void DropOff()
    {
        carryingItem.OnDropedOff();
        carryingItem = null;
    }
}
public class Golfer
{
    private ItemHolder itemHolder;
    public Golfer(ItemHolder itemHolder)
    {
        this.itemHolder = itemHolder;
    }

    // Golf Club
    public GolfClub CurrentClub { get; private set; }

    // Events
    public event Action OnClubSwitched;
    public void InvokeEvent_OnClubSwitched()
    {
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
    public bool IsDriving { get; private set; }

    // Events
    public event Action OnEnterVehicle;
    public event Action<Vector3, Vector3> OnDrive;
    public event Action OnExitVehicle;
    public void InvokeEvent_OnEnterVehicle()
    {
        OnEnterVehicle?.Invoke();
        IsDriving = true;
    }
    public void InvokeEvent_OnDrive(Vector3 poisition, Vector3 eulerAngles)
    {
        OnDrive?.Invoke(poisition, eulerAngles);
    }
    public void InvokeEvent_OnExitVehicle()
    {
        OnExitVehicle?.Invoke();
        IsDriving = false;
    }
}