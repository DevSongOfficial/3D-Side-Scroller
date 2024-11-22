using UnityEngine;
using static GameSystem;

public class PlaceableProb : PlaceableObjectBase
{
    [SerializeField] protected float fixedZPosition = 0;
    [SerializeField] protected bool disableRotation;
 
    public override void InverseRotation()
    {
        if (disableRotation) return;

        base.InverseRotation();
    }

    public float ZPostion => fixedZPosition;
}