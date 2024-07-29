using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ZombieStateBase : StateBase
{
    protected ZombieCharacter zombie;
    protected ZombieBlackboard data; // Data that shared with the other zombie states

    protected ZombieStateBase(ZombieCharacter zombieCharacter, ZombieBlackboard zomebieBlackboard) 
    {
        zombie = zombieCharacter;
        data = zomebieBlackboard;
    }
}
