using System;
using System.Collections.Generic;
using UnityEngine;
using static GameSystem;

public class Factory_POFactory : MonoBehaviour
{
    // Events
    public event Action<PlaceableObjectBase> OnPORegistered;
    public event Action<PlaceableObjectBase> OnPOUnregistered;

    public List<PlaceableObjectBase> RegistedPOs = new List<PlaceableObjectBase>();

    private void RegisterPO(PlaceableObjectBase po)
    {
        RegistedPOs.Add(po);
        GameManager.AttachToMap(po.transform);
        GameManager.AttachToMap(po.ActualObject.transform);

        OnPORegistered?.Invoke(po);
    }

    private void UnregisterPO(PlaceableObjectBase po)
    {
        if (!RegistedPOs.Contains(po)) return;

        OnPOUnregistered?.Invoke(po);

        RegistedPOs.Remove(po);
        GameManager.MoveToLagacy(po.transform);
        GameManager.MoveToLagacy(po.ActualObject.transform);
    }

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
}