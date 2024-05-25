using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlaceableObject : MonoBehaviour
{
    public static event EventHandler<PlaceableObject> OnObjectSelected;

    public static PlaceableObject Current { get; private set; } // The object player's dealing with at the moment
    public static void SetCurrentObjectTo(PlaceableObject newPlaceableObject) { Current = newPlaceableObject; }

    // This function is called only when player click a button in the level editor in order to create new object
    public void OnSelectObject()
    {
        var selectedObject = Instantiate(gameObject).GetComponent<PlaceableObject>();
        
        OnObjectSelected.Invoke(this, selectedObject);
        SetCurrentObjectTo(selectedObject);
        RegisterPlaceableObject(selectedObject);
    }


    private static List<PlaceableObject> PlaceableObjectsInTheScene;
    private static List<Collider> Colliders;
    private static void RegisterPlaceableObject(PlaceableObject newPlaceableObject) 
    {
        PlaceableObjectsInTheScene.Add(newPlaceableObject);
        Colliders.Add(newPlaceableObject.GetComponent<Collider>());
    }
    public static void SetActiveAll(bool active) 
    {
        for(int i = 0; i < PlaceableObjectsInTheScene.Count; i++)
        {
            PlaceableObjectsInTheScene[i].gameObject.SetActive(active);
        }
    }
    public static void SetTriggerAll(bool enabled)
    {
        for (int i = 0; i < PlaceableObjectsInTheScene.Count; i++)
        {
            Colliders[i].isTrigger = enabled;
        }
    }

    public static void Initialization()
    {
        PlaceableObjectsInTheScene = new List<PlaceableObject>();
        Colliders = new List<Collider>();
    }
}