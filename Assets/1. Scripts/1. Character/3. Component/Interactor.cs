using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GameSystem;


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
    void OnDroppedOff();
}

// Exposes only necessary [CharacterBase]'s interaction info.
public class Interactor
{
    private readonly CharacterBase interactorCharacter;
    private IInteractable currentlyInteractingObject;

    public Interactor(CharacterBase character)
    {
        interactorCharacter = character;
    }

    public bool TryInteract(float range = 1.5f)
    {
        var interactable = FindHighestPriorityInteractable(range);
        if (interactable == null) return false;

        currentlyInteractingObject = interactable;
        currentlyInteractingObject.Interact(this);
        return true;
    }

    public bool TryLoadToCart(float radius = 0.65f)
    {
        if (AsCarrier == null || !AsCarrier.IsCarryingItem) return false;

        var cart = FindClosestCart(radius);
        if (cart == null) return false;

        cart.LoadTheTrunk(AsCarrier.DropOffItem());
        return true;
    }


    public bool Toggle_FindAndPickupWithinRange(Vector3 range)
    {
        if (AsCarrier == null) return false;

        if (AsCarrier.IsCarryingItem)
        {
            AsCarrier.DropOff();
            return true;
        }

        var position = GetPosition() + interactorCharacter.MovementController.Direction.ConvertToVector3() * interactorCharacter.Info.PickupRange.z * 0.5f;
        var pickupables = interactorCharacter.Detector.DetectComponentsWithBox<IPickupable>(position, range, interactorCharacter.transform.rotation, Layer.Interactable.GetMask());
        foreach (var pickupable in pickupables)
        {
            AsCarrier.PickUp(pickupable);
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

    private IInteractable FindHighestPriorityInteractable(float range)
    {
        var position = GetAdjustedPosition(0.5f);
        var interactables = interactorCharacter.Detector.DetectComponentsWithClosestInFirst<IInteractable>(position, range, Layer.Interactable.GetMask());
        return interactables.OrderBy(x => (int)x.GetType()).FirstOrDefault();
    }

    private GolfCart FindClosestCart(float radius)
    {
        var position = GetAdjustedPosition(0.85f);
        return interactorCharacter.Detector.DetectComponentsWithSphere<GolfCart>(position, radius, Layer.Interactable.GetMask())
                                   .FirstOrDefault(cart => Vector3.Distance(cart.CarryPoint.position, position) <= 1.275f);
    }

    private Vector3 GetAdjustedPosition(float offset)
    {
        return interactorCharacter.Detector.ColliderCenter + interactorCharacter.MovementController.Direction.ConvertToVector3() * offset;
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
    private readonly Transform carryPoint;
    public IPickupable CarryingItem { get; private set; }
    public bool IsCarryingItem => CarryingItem != null;

    public Carrier(Transform carryPoint)
    {
        this.carryPoint = carryPoint;
    }

    public void PickUp(IPickupable pickupable)
    {
        CarryingItem = pickupable;
        CarryingItem.OnPickedUp(carryPoint);
    }

    public IPickupable DropOffItem()
    {
        var item = CarryingItem;
        CarryingItem?.OnDroppedOff();
        CarryingItem = null;
        return item;
    }

    public void DropOff()
    {
        DropOffItem();
    }
}
public class Golfer
{
    private readonly ItemHolder itemHolder;
    public GolfClub CurrentClub { get; private set; }
    public event Action OnClubSwitched;

    public Golfer(ItemHolder holder)
    {
        itemHolder = holder;
    }

    public void InvokeEvent_OnClubSwitched()
    {
        OnClubSwitched?.Invoke();
    }

    public void EquipClub(GolfClub newClub)
    {
        UnequipClub();

        CurrentClub = GameObject.Instantiate(newClub, itemHolder.GetSlotTransform(newClub.ClubType));
        CurrentClub.gameObject.SetActive(true);

        UpdateClubUI();
    }

    public void UnequipClub()
    {
        if (CurrentClub == null) return;
        CurrentClub.gameObject.SetActive(false);
        CurrentClub = null;
    }

    private void UpdateClubUI()
    {
        var images = UIManager.UI.Images_GolfClub;
        foreach (var image in images)
            image.color = Color.yellow;

        images[(int)CurrentClub.ClubType].color = Color.green;
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