using UnityEngine;
using static GameSystem;
   
[RequireComponent(typeof(PlaceableObjectBase))]
public class SingletonPO : MonoBehaviour
{
    private PlaceableObjectBase currentPO;

    [Header("Component to access")]
    [SerializeField] private MonoBehaviour component;

    private void Awake()
    {
        currentPO = GetComponent<PlaceableObjectBase>();

        POFactory.OnPORegistered += RegisterSingletonPO;
        POFactory.OnPOUnregistered += UnregisterSingletonPO;
    }

    private void RegisterSingletonPO(PlaceableObjectBase po)
    {
        if (po.Type != currentPO.Type) return;

        if (po == currentPO)
            POFactory.RegisterAsSingletonPO(component ?? currentPO, this);
        else
            POFactory.RemovePO(currentPO);
    }

    private void UnregisterSingletonPO(PlaceableObjectBase po)
    {
        if (po.Type != currentPO.Type) return;

        POFactory.UnregisterAsSingletonPO(component ?? currentPO);
    }

    public T GetConnectedComponent<T>() where T : MonoBehaviour
    {
        return component as T;
    }

    private void OnDestroy()
    {
        POFactory.OnPORegistered -= RegisterSingletonPO;
        POFactory.OnPOUnregistered -= UnregisterSingletonPO;
    }
}