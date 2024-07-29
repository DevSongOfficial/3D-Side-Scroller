using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieMovementType", menuName = "Scriptable Object/Zombie Movement/Walk 1")]
public sealed class ZomebieMovement_Walk1 : ZombieMovementBase
{
    public override void SetAnimationType()
    {
        SetAnimationType(Animation.State.BT_ZombieMove1);
    }

    public override void Execute(ZombieCharacter zombie, Vector3 targetPosition, Rigidbody rigidbody)
    {
        var direction = targetPosition - zombie.transform.position;
        rigidbody.velocity += direction * zombie.info.MovementSpeed * Time.deltaTime;
    }
}