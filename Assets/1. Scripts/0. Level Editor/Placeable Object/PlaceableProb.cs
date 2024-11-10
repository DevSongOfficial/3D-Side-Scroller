using UnityEngine;
using static GameSystem;

public class PlaceableProb : PlaceableObjectBase
{
    [SerializeField] private float fixedZPosition = 0;
    [SerializeField] private bool isSingleton = false;
    private PlaceableObjectBase previousPO;

    public override PlaceableObjectBase CreatePlaceableObject()
    {
        if(!isSingleton) return base.CreatePlaceableObject();

        if (previousPO != null)
            LevelEditorManager.RemovePlaceableObject(previousPO);
        return previousPO = base.CreatePlaceableObject();

    }

    public float ZPostion => fixedZPosition;
    public bool IsSingleton => isSingleton;
}