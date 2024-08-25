using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfBall : MonoBehaviour, IDamageable
{
    public void TakeDamage(DamageEvent damageEvent)
    {
        Debug.Log("Golfball's got damaged");
    }
}