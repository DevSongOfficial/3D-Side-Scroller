using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemSlotType { Empty, Putter, Iron, Driver }

// This manages the attachment of items or weapons to specified body parts. (mostly weapon in the right hand)
// Should be attached to the correct specified [Transform]
public class ItemHolder : MonoBehaviour, IEnumerable
{
    [SerializeField] private Transform[] itemSlot;

    public Transform GetSlotTransform(ItemSlotType slotType)
    {
        switch (slotType)
        {
            case ItemSlotType.Putter:
                return itemSlot[1];
            case ItemSlotType.Iron
                : return itemSlot[2];
            case ItemSlotType.Driver:
                return itemSlot[3];
            default: return itemSlot[0];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public ItemEnum GetEnumerator()
    {
        return new ItemEnum(itemSlot);
    }
}

public class ItemEnum : IEnumerator
{
    private Transform[] ItemSlot;

    private int maxIndex;
    private int index = 0;

    public ItemEnum(Transform[] itemSlot)
    {
        ItemSlot = itemSlot;
        maxIndex = ItemSlot.Length - 1;
    }

    public bool MoveNext()
    {
        return (index++ < maxIndex);
    }

    public void Reset()
    {
        index = 0;
    }

    object IEnumerator.Current => Current;
    public Transform Current
    {
        get
        {
            try
            {
                return ItemSlot[index];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }
    }
}