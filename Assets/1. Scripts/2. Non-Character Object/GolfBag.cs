using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using static GameSystem;

public sealed class GolfBag : MonoBehaviour, IInteractable, IPickupable
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

    public void Interact(Interactor newInteractor)
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

    private void Update()
    {
        CheckDistance();
        UpdateTimer();
    }

    private void OpenTheBag()
    {
        interactor.AsGolfer.OnClubSwitched += SwitchToNextClub;

        // Start timer
        timeLeft = 0;

        UIManager.PopupUI(UIManager.GetUI.Panel_ClubSelection);
    }

    private void CloseTheBag() 
    {
        interactor.AsGolfer.OnClubSwitched -= SwitchToNextClub;
        interactor = null;

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

    private bool centeredPosition;
    // todo: needs to be refactored. (class extraction?)
    public void OnPickedUp(Transform itemHolder, bool centeredPosition)
    {
        this.centeredPosition = centeredPosition;

        transform.SetParent(itemHolder);

        if (centeredPosition)
        {
            var offset = itemHolder.position - bodyCollider.bounds.center;
            transform.position += offset;
        }

        bodyCollider.enabled = false;
        rigidBody.isKinematic = true;
    }

    public void OnDropedOff()
    {
        transform.SetParent(null);

        if (centeredPosition)
        {
            transform.eulerAngles = Vector3.zero;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        rigidBody.isKinematic = false;
        bodyCollider.enabled = true;
    }
}