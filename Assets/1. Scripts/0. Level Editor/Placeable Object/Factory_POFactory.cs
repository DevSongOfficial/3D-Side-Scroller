using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static GameSystem;
      
public class Factory_POFactory : MonoBehaviour
{
    // Events
    public event Action<PlaceableObjectBase> OnPORegistered;
    public event Action<PlaceableObjectBase> OnPOUnregistered;

    public List<PlaceableObjectBase> RegistedPOs { get; private set; } = new List<PlaceableObjectBase>();
    public Dictionary<Type, SingletonPO> RegisteredSingletonPOs { get; private set; } = new Dictionary<Type, SingletonPO>();

    // Handle POs.
    public PlaceableObjectBase CreatePO(Prefab.PO type)
    {
        var po = Instantiate(AssetManager.GetPrefab(type).GetComponent<PlaceableObjectBase>());
        po.SetType(type);
        RegisterPO(po);

        return po;
    }

    public void RemovePO(PlaceableObjectBase po)
    {
        POFactory.UnregisterPO(po);
        po.SetActive(false);
    }

    public void RemoveEveryRegisterdPO()
    {
        for (int i = POFactory.RegistedPOs.Count - 1; i >= 0; i--)
            RemovePO(POFactory.RegistedPOs[i]);
    }

    private void RegisterPO(PlaceableObjectBase po)
    {
        RegistedPOs.Add(po);
        AttachToMap(po.transform);
        AttachToMap(po.ActualObject.transform);

        OnPORegistered?.Invoke(po);
    }

    private void UnregisterPO(PlaceableObjectBase po)
    {
        if (!RegistedPOs.Contains(po)) return;

        OnPOUnregistered?.Invoke(po);

        RegistedPOs.Remove(po);
        MoveToLagacy(po.transform);
        MoveToLagacy(po.ActualObject.transform);
    }

    // Handle Singleton POs.
    public void RegisterAsSingletonPO(MonoBehaviour component, SingletonPO singletonPO)
    {
        RegisteredSingletonPOs.Add(component.GetType(), singletonPO);
    }
    public void UnregisterAsSingletonPO(MonoBehaviour component)
    {
        RegisteredSingletonPOs.Remove(component.GetType());
    }

    public T GetRegisteredSingletonPO<T>() where T : MonoBehaviour
    {
        return RegisteredSingletonPOs[typeof(T)].GetConnectedComponent<T>();
    }

    public bool HasRegisteredSingletonPO<T>()
    {
        return RegisteredSingletonPOs.ContainsKey(typeof(T));
    }


    /// <summary>
    /// Handle transform of Registered POs.
    /// </summary>
    public void AttachToMap(Transform objectToBeAttached)
    {
        objectToBeAttached.SetParent(transform);
    }

    [SerializeField] private Transform lagacy;
    private void MoveToLagacy(Transform objectToBeRemoved)
    {
        objectToBeRemoved.SetParent(lagacy);
    }
}