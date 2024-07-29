using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour
{
    // Basic components
    protected Rigidbody rigidBody;
    protected Animator animator;

    // Character Movement & Direction
    public enum EMovementDirection { Left = -1, None = 0, Right = 1 }

    // Character Animation
    protected Animation.State CurrentAnimationState { get; private set; }

    // State
    public StateBase CurrenState { get; private set; }

    public void ChangeState(StateBase newState)
    {
        CurrenState?.ExitState();
        CurrenState = newState;
        CurrenState.EnterState();
    }
    public void SelectState()
    {
        
    }

    protected virtual void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        if (CurrenState == null) return;
        
        if (CurrenState.isComplete)
        {
            SelectState();
        }

        CurrenState.UpdateState();
    }

    protected virtual void FixedUpdate()
    {
        CurrenState?.FixedUpdateState();
    }

    public virtual void TakeDamage(DamageEvent damageEvent)
    {
        //characterInfo.TakeDamage(damageEvent.damage);
    }

    public virtual bool ChangeAnimationState(Animation.State state)
    {
        if (CurrentAnimationState == state) return false;

        CurrentAnimationState = state;
        animator.Play(CurrentAnimationState.ToString());

        return true;
    }

    public virtual void FadeOut()
    {

    }

    public virtual void FadeIn()
    {

    }


    public virtual PlayerCharacter AsPlayer()
    {
        return null;
    }

    public virtual ZombieCharacter AsZombie()
    {
        return null;
    }
}