using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    public static PlaceableObject[] Prefabs_PlaceableObject;
    public static PlaceableObject SelectedObject { get; private set;  }

    public static void SelectObject(PlaceableObject placeableObject)
    {
        
    }

    public static void SelectObject()
    {

    }
    
    public void Place()
    {
    }    
}