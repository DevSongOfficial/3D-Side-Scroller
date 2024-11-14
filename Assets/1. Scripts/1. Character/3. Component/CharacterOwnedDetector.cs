using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
public class CharacterOwnedDetector : Detector
{
    protected override void Awake()
    {
        base.Awake();

        collider = GetComponent<CharacterController>();
    }

    public bool DetectCliffAhead()
    {
        var origin = collider.bounds.center;
        origin.x = collider.bounds.center.x + (int)movementController.Direction * 0.5f;
        origin.y = collider.bounds.max.y;
        float distance = 1.5f + Mathf.Abs(transform.eulerAngles.x) * 0.1f; // For some reason, the value of collider.bounds.center keeps changing.

        Debug_DrawRay(origin, Vector3.down * distance, Color.black);
        return (!Physics.Raycast(origin, Vector3.down, distance, Layer.Ground.GetMask())) ;
    }
}