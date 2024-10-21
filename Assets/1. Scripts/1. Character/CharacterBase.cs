using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using static GameSystem;

[RequireComponent(typeof(Detector))]
[RequireComponent(typeof(CharacterMovementController))]
public abstract class CharacterBase : MonoBehaviour, IDamageable
{
    // Movement Controller
    public CharacterMovementController MovementController { get; private set; }


    // Detector for detecting wall, ground, other characters, and anything else.
    // This component also handles [Collider] of the character.
    public Detector Detector { get; private set; }

    // Interactor
    public Interactor Interactor { get; protected set; }

    // Animation Controller
    public AnimationController AnimationController { get; private set; }
    private Animator animator;

    // Health System
    protected HealthSystem healthSystem;

    // State (Each character runs as a single state machine)
    public StateBase CurrenState { get; private set; }
    public StateBase PreviousState { get; private set; }

    // Character Information
    [Header("Character Information")]
    [SerializeField] protected ObjectInfo info;
    public virtual ObjectInfo Info => info;

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
        MovementController = GetComponent<CharacterMovementController>();

        healthSystem = new HealthSystem(info.MaxHealth);
        Interactor = new Interactor(this);

        animator = GetComponentInChildren<Animator>();
        AnimationController = new AnimationController(animator);
    }

    protected virtual void Start() { }

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
        // Take damage
        MovementController.SetVelocity(damageEvent.knockBackVelocity);
        healthSystem.TakeDamage(damageEvent.damage);

        // Generate damage effect
        var damageText = Instantiate(AssetManager.GetPrefab(Prefab.UI.DamageText).GetComponent<TMP_Text>(), UIManager.Canvas.transform);
        UIManager.PopupUI(damageText, Camera.main.WorldToScreenPoint(Detector.ColliderCenter), PopupType.MoveAndFadeOut);
        damageText.text = damageEvent.damage.ToString();
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