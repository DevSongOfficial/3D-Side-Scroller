using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Layer
{
    PlaceableObject = 20,
    RaycastBlockerForLevelEditor = 31,
}

public class GameManager : MonoBehaviour
{
    public int GetLayerMask(Layer layer)
    {
        return (int)layer;
    }
}
