using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(Rigidbody))]
public sealed class GolfBall : PlaceableObject, IDamageable
{
    public void TakeDamage(DamageEvent damageEvent)
    {
        Debug.Log("Golfball's got damaged");
    }
}