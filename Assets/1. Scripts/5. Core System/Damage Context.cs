using System;
using UnityEngine;

[Serializable]
public struct DamageEvent
{
    public EventSenderType senderType;

    public int damage;
    public Vector3 knockBackVector;

    public DamageEvent(EventSenderType senderType, int damage)
    {
        this.senderType = senderType;
        this.damage = damage;
        knockBackVector = Vector3.zero;
    }

    public DamageEvent(EventSenderType senderType, int damage, Vector3 knockBackVector) 
    {
        this.senderType = senderType;
        this.damage = damage;
        this.knockBackVector = knockBackVector;
    }

    public DamageEvent MultiplyDamage(int multiplier)
    {
        return new DamageEvent(senderType, damage * multiplier, knockBackVector);
    }

    public DamageEvent MultiplyKnockback(float multiplier)
    {
        return new DamageEvent(senderType, damage, knockBackVector * multiplier);
    }

    public DamageEvent ApplyDirection(EMovementDirection direction)
    {
        return new DamageEvent(senderType, damage, new Vector3((int)direction * knockBackVector.x, knockBackVector.y, knockBackVector.z));
    }

    public bool CompareSenderTypeWith(EventSenderType eventSenderType)
    {
        return senderType == eventSenderType;
    }
}

public enum EventSenderType
{
    None, Default, Club, Obstacle, Item
}

interface IDamageable
{
    public void TakeDamage(DamageEvent damageEvent);
}