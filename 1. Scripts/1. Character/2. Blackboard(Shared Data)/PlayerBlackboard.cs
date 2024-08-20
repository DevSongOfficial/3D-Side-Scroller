using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class PlayerBlackboard : BlackboardBase
{
    public PlayerBlackboard() { }


    public EMovementDirection directionToMove; // Direction where player's expected to move.
    // [MovementController.Direction] refers to the direction the character is facing at tho moment,
    // On the other hand, [directionToMove] refers to the way to be aksed to go at the moment.

    // Events
    public Action<EMovementDirection> OnMove;
    public Action<EMovementDirection> OnLand;
    public Action<Vector2> OnDrag;
    public Action OnMouseDown;
    public Action OnMouseUp;
}
