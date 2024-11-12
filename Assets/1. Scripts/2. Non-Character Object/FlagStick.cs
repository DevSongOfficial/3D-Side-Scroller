using UnityEngine;
using static GameSystem;

public class FlagStick : MonoBehaviour, IPickupable
{
    private GolfClub club;

    private void Awake()
    {
        club = GetComponent<GolfClub>();
    }

    public void OnPickedUp(Transform carryPoint)
    {
        transform.SetParent(carryPoint);

        transform.position = carryPoint.position;
        transform.eulerAngles = carryPoint.eulerAngles;

    }

    public void OnDropedOff()
    {
        GameManager.AttachToMap(transform);

        transform.eulerAngles = Vector3.zero;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.25f, 1f);
    }
}
