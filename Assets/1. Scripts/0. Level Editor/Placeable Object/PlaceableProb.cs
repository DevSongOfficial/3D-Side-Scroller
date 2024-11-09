using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableProb : PlaceableObjectBase
{
    [SerializeField] private float fixedZPosition = 0;
    public float ZPostion => fixedZPosition;
}
