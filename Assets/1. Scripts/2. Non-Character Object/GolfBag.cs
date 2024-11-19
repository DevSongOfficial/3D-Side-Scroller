using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using static GameSystem;

public class GolfBag : MonoBehaviour, IInteractable, IPickupable
{
    // Golf clubs
    private GolfClub currentClub => clubs[clubIndex];
    [SerializeField] private GolfClub[] clubs;
    private int clubIndex;

    // Timer where the bag automatically closes when time's up.
    private float timeLeft;
    private const float Duration = 15;


    // The character who currently has access to or is interacting with the bag.
    private Interactor interactor;
    public bool IsOpen => interactor != null;


    // Physics
    [SerializeField] private Collider bodyCollider;
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CheckDistance();
    }

    public virtual void Interact(Interactor newInteractor)
    {
        if (newInteractor.AsGolfer == null) return;

        if (IsOpen)
        {
            CloseTheBag();
            return;
        }

        interactor = newInteractor;
        OpenTheBag();
    }

    public new InteractableType GetType() => InteractableType.Equipment;


    protected virtual void OpenTheBag()
    {
        interactor.AsGolfer.OnClubSwitched += SwitchToNextClub;

        UIManager.PopupUI(UIManager.UI.Panel_ClubSelection);
    }

    protected virtual void CloseTheBag() 
    {
        interactor.AsGolfer.OnClubSwitched -= SwitchToNextClub;
        interactor = null;

        UIManager.CloseUI(UIManager.UI.Panel_ClubSelection);
    }

    private void SwitchToNextClub()
    {
        if (!IsOpen) return;

        clubIndex++;
        clubIndex %= clubs.Length;
        interactor.AsGolfer.EquipClub(currentClub);
    }

    private void CheckDistance()
    {
        if (!IsOpen) return;

        var distance = Vector3.Distance(transform.position, interactor.GetPosition());
        if(distance > interactor.GetInteractionRange())
        {
            CloseTheBag();
        }
    }

    public virtual void OnPickedUp(Transform carryPoint)
    {
        transform.SetParent(carryPoint);

        transform.position = carryPoint.position;
        transform.eulerAngles = carryPoint.eulerAngles;

        rigidBody.isKinematic = true;
        bodyCollider.isTrigger = true;
    }

    public virtual void OnDropedOff()
    {
        GameManager.AttachToMap(transform);

        transform.eulerAngles = Vector3.zero;
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        rigidBody.isKinematic = false;
        bodyCollider.isTrigger = false;
    }
}