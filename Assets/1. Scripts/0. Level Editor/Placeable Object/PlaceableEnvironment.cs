using UnityEngine;

public sealed class PlaceableEnvironment : PlaceableProb
{
    private float origninalZPosition;
    static private float[] zPositionMultipliers = new float[] { -1, -0.75f, -0.5f, -0.25f, 0, 0.25f, 0.5f, 0.75f, 1 };
    private int index = 0;

    protected override void Start()
    {
        base.Start();

        ActualObject.gameObject.SetLayer(Layer.Environment);

        origninalZPosition = fixedZPosition;
    }

    public override void InverseRotation()
    {
        fixedZPosition = origninalZPosition * zPositionMultipliers[++index % zPositionMultipliers.Length];
    }

    protected override void OnTriggerEnter(Collider other) { }

    protected override void OnTriggerExit(Collider other) { }
}
