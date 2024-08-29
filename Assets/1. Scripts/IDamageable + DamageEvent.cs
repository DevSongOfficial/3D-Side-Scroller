using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 데미지 입었을 때 PE, Sound, Damage 등을 정의

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

    public DamageEvent(EventSenderType sender, int damage, Vector3 knockBackVector) 
    {
        this.senderType = sender;
        this.damage = damage;
        this.knockBackVector = knockBackVector;
    }
}

public enum EventSenderType
{
    None, Character, Obstacle, Item
}

interface IDamageable
{
    public void TakeDamage(DamageEvent damageEvent);
}