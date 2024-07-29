using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Scripting;

public sealed class ZombieCharacter : CharacterBase
{
    [Header("Zombie Information")]
    public ZombieInfo info; // this를 통해 접근

    [Space(10)]
    [Header("Zomebie Movement Type")]
    // Needs to be set from the inspector
    [SerializeField, RequiredMember] private ZombieMovementBase movementOnPatrol;
    [SerializeField, RequiredMember] private ZombieMovementBase movementOnChase;

    // States
    public ZombiePatrolState zombiePatrolState { get; private set; }
    public ZombieChaseState zombieChaseState { get; private set; }
    public ZombieAttackState zombieAttackState { get; private set; }

    // Black Board
    private ZombieBlackboard zombieBlackboard;

    protected override void Awake()
    {
        base.Awake();

        zombieBlackboard = new ZombieBlackboard();

        movementOnPatrol.SetAnimationType();
        movementOnChase.SetAnimationType();

        zombiePatrolState = new ZombiePatrolState(this, zombieBlackboard, movementOnPatrol);
        zombieChaseState = new ZombieChaseState(this, zombieBlackboard, movementOnChase);
        zombieAttackState = new ZombieAttackState(this, zombieBlackboard);
    }

    protected override void Start()
    {
        base.Start();
        
        //ChangeState(zombiePatrolState);
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }


    public override ZombieCharacter AsZombie()
    {
        return this;
    }
}