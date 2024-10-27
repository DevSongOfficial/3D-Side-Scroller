using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public sealed class GolfBall : MonoBehaviour, IDamageable
{
    private Rigidbody rigidBody;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    public void TakeDamage(DamageEvent damageEvent)
    {
        if(damageEvent.CompareSenderTypeWith(EventSenderType.Club))
        {
            rigidBody.AddForce(damageEvent.knockBackVelocity * 35);
        }
    }
}