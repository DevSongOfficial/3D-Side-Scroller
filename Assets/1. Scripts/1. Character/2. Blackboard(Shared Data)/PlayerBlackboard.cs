using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class PlayerBlackboard : BlackboardBase
{
    // Events
    public Action<EMovementDirection> Input_ChangeDirection;
    public Action Input_OnJump;
    public Action<Vector2> Input_Drag;
    public Action Input_MouseDown;
    public Action Input_MouseUp;
}