using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum Layer
{
    Default = 0,
    Character = 8,
    Ground = 9,
    Wall = 10,
    PlaceableObject = 20,
    InteractableObject = 25,
    RaycastBlockerForLevelEditor = 31,
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance => instance;
    private static GameManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        
    }
}
