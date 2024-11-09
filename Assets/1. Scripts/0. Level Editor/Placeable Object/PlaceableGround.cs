using System.Collections;
using Unity.XR.OpenVR;
using UnityEngine;

public sealed class PlaceableGround : PlaceableProb
{
    [SerializeField] private bool disableRotation;

    protected override void Start()
    {
        base.Start();

        actualObject_rigidBody.isKinematic = true;
        actualObject_IsKinematic = true;

        ActualObject.gameObject.SetLayer(Layer.Ground);
        try { ActualObject.GetChild(0).gameObject.SetLayer(Layer.Ground); }
        catch { }
    }

    public override void InverseRotation()
    {
        if (disableRotation) return;

        base.InverseRotation();
    }

    protected override void OnTriggerEnter(Collider other) { }

    protected override void OnTriggerExit(Collider other) { }
}