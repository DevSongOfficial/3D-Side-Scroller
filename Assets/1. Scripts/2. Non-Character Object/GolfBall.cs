using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public sealed class GolfBall : PlaceableObject, IDamageable
{
    public void TakeDamage(DamageEvent damageEvent)
    {
        Debug.Log("Golfball's got damaged");

        if(damageEvent.senderType == EventSenderType.Character)
        {
            rigidBody.AddForce(new Vector3(100 , 350, 0));
        }
    }
}