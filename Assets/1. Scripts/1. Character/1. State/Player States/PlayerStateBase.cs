using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateBase : StateBase
{
    protected PlayerCharacter player;
    protected PlayerBlackboard sharedData;

    protected PlayerStateBase(PlayerCharacter player, PlayerBlackboard data)
    {
        this.player = player;
        sharedData = data;
    }
}
