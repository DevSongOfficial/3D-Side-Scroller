using System.Collections.Generic;
using UnityEngine;

public sealed class ZombieCharacter : CharacterBase
{
    // Zombie Info
    public new ZombieInfo Info => info.AsZombieInfo();

    [Header("Zomebie Movement Type")]
    // Needs to be set from the inspector
    [SerializeField] private ZombieMovementBase movementOnPatrol;
    [SerializeField] private ZombieMovementBase movementOnChase;

    [Header("Zomebie Attack Type")]
    // Needs to be set from the inspector
    [SerializeField] private ZombieAttackBase attackDefault;

    [Header("Renderer")]
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;

    // States
    // Must excute the contructor in Awake() after adding new state.
    public ZombieStateBase PatrolState { get; private set; }
    public ZombieStateBase ChaseState { get; private set; }
    public ZombieStateBase AttackState { get; private set; }
    public ZombieStateBase StunnedState { get; private set; }

    // Black Board
    private ZombieBlackboard blackboard;

    protected override void Awake()
    {
        base.Awake();
        
        transform.GetChild(0).gameObject.SetLayer(Layer.Character);
        transform.GetChild(0).gameObject.SetTag(Tag.Enemy);

        blackboard = new ZombieBlackboard();
        blackboard.skinnedMeshRenderer = skinnedMeshRenderer;

        PatrolState = new ZombiePatrolState(this, blackboard, movementOnPatrol);
        ChaseState = new ZombieChaseState(this, blackboard, movementOnChase);
        AttackState = new ZombieAttackState(this, blackboard, attackDefault);
        StunnedState = new ZombieStunnedState(this, blackboard);

        HealthSystem.OnCharacterDie += OnDie;
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

    public override void TakeDamage(DamageEvent damageEvent)
    {
        base.TakeDamage(damageEvent);
        ChangeState(StunnedState);
    }

    [SerializeField] private Material mat1;
    [SerializeField] private Material mat2;
    private void OnDie()
    {
        // Modify materials
        blackboard.skinMaterials = new List<Material> { mat1, mat2 };
        blackboard.skinnedMeshRenderer.SetMaterials(blackboard.skinMaterials);

        // Get it stunned
        blackboard.isDead = true;
        ChangeState(StunnedState);

        // Inactivate & destroy
        MovementController.SetActive(false);
        StartCoroutine(DestroyRoutine(3));
    }

    public override ZombieCharacter AsZombie()
    {
        return this;
    }
}