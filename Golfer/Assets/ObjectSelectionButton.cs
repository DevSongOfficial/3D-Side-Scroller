using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ObjectSelectionButton : Button
{
    public PlaceableObject placeableObject;
    public void SetPlaceableObject(PlaceableObject newObjectPrefab)
    {
        placeableObject = newObjectPrefab;
    }

    public void SetPlaceableObject(GameObject newObjectPrefab)
    {
        SetPlaceableObject(newObjectPrefab.GetComponent<PlaceableObject>());
    }
}
