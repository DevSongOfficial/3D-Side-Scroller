using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionButton : Button
{
    public PlaceableObject connectedPlaceableObject;
    public void SetConnectedPlaceableObject(PlaceableObject newObjectPrefab)
    {
        connectedPlaceableObject = newObjectPrefab;
    }
}
