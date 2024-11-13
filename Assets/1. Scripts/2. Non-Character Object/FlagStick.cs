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

        var offset = new Vector3(1, 0, -2.8f);
        transform.position = carryPoint.position;
        transform.GetChild(0).localPosition = offset;
        transform.eulerAngles = carryPoint.eulerAngles;

        rigidBody.isKinematic = true;
        rigidBody.useGravity = false;
    }

    public void OnDropedOff()
    {
        GameManager.AttachToMap(transform);

        var position = carryPoint.transform.root.position;
        position.y += 0.35f;
        position.z = fixedZPositionOnPlaced;

        transform.eulerAngles = Vector3.zero;
        transform.position = position;

        rigidBody.isKinematic = false;
        rigidBody.useGravity = true;
    }
}
