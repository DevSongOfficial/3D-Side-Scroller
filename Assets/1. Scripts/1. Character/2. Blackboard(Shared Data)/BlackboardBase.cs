using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BlackboardBase
{
    // The same as UE5's Blackboard System in Behaviour Tree.
    // States 간 공용으로 사용할 Properties, Delegate 등 Key values 관리.
    // Every property must be public.

    // Constructors
    public BlackboardBase() { }
}
