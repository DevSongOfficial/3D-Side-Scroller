using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 데미지 입었을 때 PE, Sound, Damage 등을 정의

[Serializable]
public struct DamageEvent
{
    public int damage;
}