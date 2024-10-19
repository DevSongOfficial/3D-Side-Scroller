using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableProb : PlaceableObjectBase
{
    protected override void OnLevelEditorToggled(bool isOn)
    {
        base.OnLevelEditorToggled(isOn);

        rigidBody.isKinematic = isKinematic ? true : isOn;
    }
}
