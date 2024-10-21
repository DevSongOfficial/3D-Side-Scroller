using System;
using UnityEngine;

[Serializable]
public struct DamageEvent
{
    public EventSenderType senderType;

    public int damage;
    public Vector3 knockBackVelocity;

    public DamageEvent(EventSenderType senderType, int damage)
    {
        this.senderType = senderType;
        this.damage = damage;
        knockBackVelocity = Vector3.zero;
    }

    public DamageEvent(EventSenderType senderType, int damage, Vector3 knockBackVelocity) 
    {
        this.senderType = senderType;
        this.damage = damage;
        this.knockBackVelocity = knockBackVelocity;
    }

    public DamageEvent MultiplyDamage(int multiplier)
    {
        return new DamageEvent(senderType, damage * multiplier, knockBackVelocity);
    }

    public DamageEvent MultiplyVelocity(float multiplier)
    {
        return new DamageEvent(senderType, damage, knockBackVelocity * multiplier);
    }

    public DamageEvent ApplyDirection(MovementDirection direction)
    {
        return new DamageEvent(senderType, damage, new Vector3((int)direction * knockBackVelocity.x, knockBackVelocity.y, knockBackVelocity.z));
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