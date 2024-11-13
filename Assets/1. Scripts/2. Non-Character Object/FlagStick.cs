using UnityEngine;
using static GameSystem;

public class FlagStick : MonoBehaviour, IPickupable
{
    private Transform carryPoint;
    private Rigidbody rigidBody;

    private const float fixedZPositionOnPlaced = 1;


    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void OnPickedUp(Transform carryPoint)
    {
        this.carryPoint = carryPoint;
        transform.SetParent(carryPoint);

        transform.position = carryPoint.position;
        transform.eulerAngles = carryPoint.eulerAngles;

        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
    }

    public void OnDropedOff()
    {
        GameManager.AttachToMap(transform);

        var position = carryPoint.transform.root.position;
        position.z = fixedZPositionOnPlaced;

        transform.eulerAngles = Vector3.zero;
        transform.position = position;

        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
    }
}
