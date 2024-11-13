using UnityEngine;

public sealed class PlaceableEnvironment : PlaceableProb
{
    private float origninalZPosition;
    private int[] zPositionMultipliers = new int[3] { 1, 0, -1 };
    private int index = 0;

    protected override void Start()
    {
        base.Start();

        ActualObject.gameObject.SetLayer(Layer.Environment);

        origninalZPosition = fixedZPosition;
    }

    public override void InverseRotation()
    {
        fixedZPosition = origninalZPosition * zPositionMultipliers[++index % 3];
    }

    protected override void OnTriggerEnter(Collider other) { }

    protected override void OnTriggerExit(Collider other) { }
}
