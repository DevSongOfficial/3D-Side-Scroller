using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase
{
    // Fo any state derived from [StateBase] class, each must be independent on one another,
    // which means each must not be allowed to access to another's functions or properties,
    // CharacterBase-Derived classes as statemachine and [Blackboard] are the only classes that are allowed to get inside states.
    
    // According to this principal above,
    // 1) Every property and method inside State class must be private.
    // 2) Every state variable in Character(statemachine) class must be read-only.
    // 3) Every action that involves (1) or (2) must be conducted through the blackboard.(You can declare event inside of it.)

    // Timer
    public bool isComplete { get; protected set; }
    private float startTime;
    protected float time => Time.time - startTime;

    public virtual void EnterState() 
    {
        isComplete = false;
        startTime = Time.time;
    }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
    public virtual void ExitState() { }
}