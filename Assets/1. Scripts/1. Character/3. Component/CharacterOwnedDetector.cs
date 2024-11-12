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
        origin.x = collider.bounds.center.x + (int)movementController.Direction;
        origin.y = collider.bounds.max.y;
        float distance = collider.bounds.center.y;

        Debug_DrawRay(origin, Vector3.down * distance, Color.black);
        return (!Physics.Raycast(origin, Vector3.down, distance, Layer.Ground.GetMask())) ;
    }
}