using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableProb : PlaceableObjectBase
{
    [SerializeField] private float wishZPosition = 0;
    public float ZPostion => wishZPosition;
}
