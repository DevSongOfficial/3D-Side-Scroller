using System.Collections;
using UnityEngine;

public sealed class PlaceableGround : PlaceableObjectBase
{
    protected override void Start()
    {
        base.Start();

        actualObject_rigidBody.isKinematic = true;
        actualObject_IsKinematic = true;

        actualObject.gameObject.SetLayer(Layer.Ground);
        try { actualObject.GetChild(0).gameObject.SetLayer(Layer.Ground); }
        catch { }
    }

    protected override void OnTriggerEnter(Collider other) { }

    protected override void OnTriggerExit(Collider other) { }
}