using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase
{
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
