using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public sealed class GolfBall : PlaceableObject, IDamageable
{
    public static readonly float FixedZPosition = -0.85f;

    public void TakeDamage(DamageEvent damageEvent)
    {
        if(damageEvent.CompareSenderTypeWith(EventSenderType.Character))
        {
            rigidBody.AddForce(new Vector3(100 , 350, 0));
        }
    }
}