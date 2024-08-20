using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

public sealed class ZombieCharacter : CharacterBase
{
    [Header("Zombie Information")]
    [SerializeField] private ZombieInfo info;
    public ZombieInfo Info => info;


    [Space(10)]
    [Header("Zomebie Movement Type")]
    // Needs to be set from the inspector
    [SerializeField, RequiredMember] private ZombieMovementBase movementOnPatrol;
    [SerializeField, RequiredMember] private ZombieMovementBase movementOnChase;

    [Space(10)]
    [Header("Zomebie Attack Type")]
    // Needs to be set from the inspector
    [SerializeField, RequiredMember] private ZombieAttackBase attackDefault;

    // States
    // Must excute the contructor in Awake() after adding new state.
    public ZombieStateBase PatrolState { get; private set; }
    public ZombieStateBase ChaseState { get; private set; }
    public ZombieStateBase AttackState { get; private set; }

    // Black Board
    private ZombieBlackboard zombieBlackboard;

    protected override void Awake()
    {
        base.Awake();

        transform.GetChild(0).gameObject.SetLayer(Layer.Character);

        zombieBlackboard = new ZombieBlackboard();

        PatrolState = new ZombiePatrolState(this, zombieBlackboard, movementOnPatrol);
        ChaseState = new ZombieChaseState(this, zombieBlackboard, movementOnChase);
        AttackState = new ZombieAttackState(this, zombieBlackboard, attackDefault);
    }

    protected override void Start()
    {
        base.Start();

        ChangeState(PatrolState);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void ChangeState(StateBase newState)
    {
        base.ChangeState(newState);
    }

    public override ZombieCharacter AsZombie()
    {
        return this;
    }
}