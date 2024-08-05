using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerBlackboard : BlackboardBase
{
    public PlayerBlackboard() { }


    // Events
    public Action<EMovementDirection> OnMove;
    public Action<EMovementDirection> OnLand;
    public Action<Vector2> OnDrag;
    public Action OnClick;
}
