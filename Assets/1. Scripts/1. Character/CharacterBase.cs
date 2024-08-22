using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Detector))]
[RequireComponent(typeof(MovementController))]
[RequireComponent(typeof(AnimationController))]
public abstract class CharacterBase : MonoBehaviour, IDamageable
{
    // Movement Controller
    public MovementController MovementController { get; private set; }

    // Animation Controller
    public AnimationController AnimationController { get; private set; }

    // Detector for detecting wall, ground, other characters, and anything else.
    // This component also handles [Collider] of the character.
    public Detector Detector { get; private set; }

    // State (Each character runs as a single state machine)
    public StateBase CurrenState { get; private set; }
    public StateBase PreviousState { get; private set; }

    public virtual void ChangeState(StateBase newState)
    {
        CurrenState?.ExitState();
        PreviousState = CurrenState;
        CurrenState = newState;
        CurrenState.EnterState();
    }

    protected virtual void Awake()
    {
        Detector = GetComponent<Detector>();
        MovementController = GetComponent<MovementController>();
        AnimationController = GetComponent<AnimationController>();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        CurrenState?.UpdateState();
    }

    protected virtual void FixedUpdate()
    {
        CurrenState?.FixedUpdateState();
    }

    public virtual void TakeDamage(DamageEvent damageEvent)
    {
        Debug.Log($"피격 데미지: {damageEvent.damage}");
    }

    public virtual void FadeOut()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public virtual void FadeIn()
    {
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public bool IsTargetWithInDistance(CharacterBase targetCharacter, float range)
    {
        var distance = Vector3.Distance(transform.position, targetCharacter.transform.position);
        return distance <= range;
    }



    // Downcast for CharacterBase-Derived objects that are being upcast

    public virtual PlayerCharacter AsPlayer()
    {
        return null;
    }

    public virtual ZombieCharacter AsZombie()
    {
        return null;
    }
}