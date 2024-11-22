using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(PlaceableObjectBase))]
public class SingletonPO : MonoBehaviour
{
    private PlaceableObjectBase currentPO;

    private void Awake()
    {
        currentPO = GetComponent<PlaceableObjectBase>();
        POFactory.OnPORegistered += RemoveSamePO;
    }

    private void RemoveSamePO(PlaceableObjectBase po)
    {
        if (po.Type != currentPO.Type) return;
        if (po == currentPO) return;

        POFactory.RemovePO(currentPO);
    }
}
