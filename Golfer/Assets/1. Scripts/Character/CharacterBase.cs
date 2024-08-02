using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AnimationController))]
public abstract class CharacterBase : MonoBehaviour
{
    // Movement Controller
    public MovementController movementController { get; private set; }

    // Animation Controller
    public AnimationController animationController { get; private set; }

    // State (Each character runs as a single state machine)
    // todo: 나중에 독립적인 클래스로 분리?
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
        movementController = GetComponent<MovementController>();
        animationController = GetComponent<AnimationController>();
    }

    protected virtual void Start()
    {
        gameObject.layer = (int)Layer.Character;
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

    public virtual void FadeOut()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public virtual void FadeIn()
    {
        transform.GetChild(0).gameObject.SetActive(true);
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