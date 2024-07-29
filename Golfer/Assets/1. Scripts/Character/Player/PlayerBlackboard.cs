using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerBlackboard : BlackboardBase
{
    public PlayerBlackboard(Rigidbody rigidBody, Animator animator) : base(rigidBody, animator) { }

    public enum EMovementDirection { Left = -1, None = 0, Right = 1 }

}
