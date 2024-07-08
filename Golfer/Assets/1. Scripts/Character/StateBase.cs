using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBase
{
    protected CharacterBase character;
    protected PlayerCharacter playerCharacter;
    protected Rigidbody rigidBody;
    protected Animator animator;

    public StateBase(CharacterBase character, Rigidbody rigidBody, Animator animator)
    {
        this.character = character;
        this.rigidBody = rigidBody;
        this.animator = animator;

        playerCharacter = character.AsPlayer();
    }

    public bool isComplete { get; protected set; }

    float startTime;
    float time => Time.time - startTime;

    public virtual void EnterState() 
    {
        isComplete = false;
        startTime = Time.time;
    }
    public virtual void UpdateState() { }
    public virtual void FixedUpdateState() { }
    public virtual void ExitState() { }
}
