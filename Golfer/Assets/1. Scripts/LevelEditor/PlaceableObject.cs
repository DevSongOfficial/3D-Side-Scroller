using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlaceableObject : MonoBehaviour
{
    // PlacebleObject의 Collider는 오브젝트 자신에 필수적으로 메인 콜라이더 하나 + (필요하다면) 자식으로 여러개를 가진다.
    // mainCollider is only for checking if placeableObjects are overlapped with MeshCollider Component.
    // Actual collider when playing game is List - childColliders.
    private MeshCollider mainCollider;
    private List<Collider> childColliders = new List<Collider>();
    private bool isOverlapped;
    [HideInInspector] public bool CanBePlaced { get { return !isOverlapped; } }

    public static event EventHandler<PlaceableObject> OnObjectSelected;

    public static PlaceableObject Current { get; private set; } // The object player's dealing with at the moment
    public static void SetCurrentObjectTo(PlaceableObject newPlaceableObject) { Current = newPlaceableObject; }

    // This function is called only when player click a button in the level editor in order to create new object
    public void OnSelectObject()
    {
        var selectedObject = CreatePlaceableObject();

        OnObjectSelected.Invoke(this, selectedObject);
        SetCurrentObjectTo(selectedObject);
        RegisterPlaceableObject(selectedObject);
    }


    private static List<PlaceableObject> PlaceableObjectsInTheScene;
    private static void RegisterPlaceableObject(PlaceableObject newPlaceableObject) 
    {
        PlaceableObjectsInTheScene.Add(newPlaceableObject);
    }
    public static void SetActiveAll(bool active) 
    {
        for(int i = 0; i < PlaceableObjectsInTheScene.Count; i++)
        {
            PlaceableObjectsInTheScene[i].gameObject.SetActive(active);
        }
    }
    public void SetConvex(bool enabled) 
    {
        mainCollider.convex = enabled;
        mainCollider.isTrigger = enabled;
    }
    public static void SetConvexAll(bool enabled)
    {
        for (int i = 0; i < PlaceableObjectsInTheScene.Count; i++)
        {
            PlaceableObjectsInTheScene[i].SetConvex(enabled);
        }
    }
    public void SetCollision(bool enabled) 
    {
        for(int i = 0; i < childColliders.Count; i++)
        {
            childColliders[i].enabled = enabled;
        }
    }
    public static void SetCollisionAll(bool enabled)
    {
        for (int i = 0; i < PlaceableObjectsInTheScene.Count; i++)
        {
            PlaceableObjectsInTheScene[i].SetCollision(enabled);
        }
    }
    public static void Initialization()
    {
        PlaceableObjectsInTheScene = new List<PlaceableObject>();
    }

    private PlaceableObject CreatePlaceableObject()
    {
        var newObject = Instantiate(gameObject).GetComponent<PlaceableObject>();
        newObject.InitializeOnInstantiated();
        return newObject;
    }

    private void InitializeOnInstantiated()
    {
        gameObject.layer = (int)Layer.PlaceableObject;

        mainCollider = GetComponent<MeshCollider>();
        SetConvex(true);

        for (int i = 0; i < transform.childCount; i++) 
        {
            var childCollider = transform.GetChild(i).GetComponent<Collider>();
            if (childCollider != null)
            {
                childColliders.Add(childCollider);
            }
        }
        SetCollision(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != (int)Layer.PlaceableObject) return;

        isOverlapped = true;
        // 경고 띄우기
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != (int)Layer.PlaceableObject) return;

        isOverlapped = !isOverlapped;
        // 경고 제거
    }
}