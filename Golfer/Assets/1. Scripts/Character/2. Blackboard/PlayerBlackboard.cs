using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerBlackboard : BlackboardBase
{
    public PlayerBlackboard() { }

    // Events
    public Action<EMovementDirection> OnPlayerLand;
}
