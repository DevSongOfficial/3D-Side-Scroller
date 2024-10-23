using UnityEngine;

[RequireComponent(typeof(CharacterBase))]
public class CharacterOwnedDetector : Detector
{
    protected override void Awake()
    {
        base.Awake();

        collider = GetComponent<CharacterController>();
    }
}