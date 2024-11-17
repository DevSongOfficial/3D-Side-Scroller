using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public sealed class PlayerBlackboard : BlackboardBase
{
    // Properties
    public MovementDirection InputDirection;

    // Events
    public Action<MovementDirection> Input_ChangeDirection;
    public Action<CharacterMovementController.ZAxisMovementDirection> Input_ChangeZDirection;
    public Action Input_OnJump;
    public Action<Vector2> Input_Drag;
    public Action Input_MouseDown;
    public Action Input_MouseUp;
}