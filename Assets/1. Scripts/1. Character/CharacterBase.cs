using System;
using System.Collections;
using TMPro;
using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(CharacterOwnedDetector))]
[RequireComponent(typeof(CharacterMovementController))]
public abstract class CharacterBase : MonoBehaviour, IDamageable
{
    // Movement Controller
    public CharacterMovementController MovementController { get; private set; }

    // Detector for detecting wall, ground, other characters, and anything else.
    public CharacterOwnedDetector Detector { get; private set; }

    // Interactor
    public Interactor Interactor { get; protected set; }

    // Animation Controller
    public AnimationController AnimationController { get; private set; }

    // Health System
    protected HealthSystem HealthSystem;
    public Action OnDestroy;

    // State (Each character runs as a single state machine)
    public StateBase CurrenState { get; private set; }
    public StateBase PreviousState { get; private set; }
    public event Action<StateBase> OnStateChanged;

    // Character Information
    [Header("Character Information")]
    [SerializeField] protected ObjectInfo info;
    public virtual ObjectInfo Info => info;

    // Character Location
    public bool OnGreen { get; protected set; }
    public bool InWater { get; protected set; }


    // VFX
    public ShaderFX AuraVFX { get; private set; }

    public virtual void ChangeState(StateBase newState)
    {
        CurrenState?.ExitState();
        PreviousState = CurrenState;
        CurrenState = newState;
        CurrenState.EnterState();

        OnStateChanged?.Invoke(newState);
    }

    protected virtual void Awake()
    {
        Detector = GetComponent<CharacterOwnedDetector>();
        MovementController = GetComponent<CharacterMovementController>();

        HealthSystem = new HealthSystem(info.MaxHealth);
        Interactor = new Interactor(this);

        AnimationController = new AnimationController(GetComponentInChildren<Animator>());

        AuraVFX = FXManager.CreateShaderFX(Prefab.VFX.Aura, transform);
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
        HealthSystem.TakeDamage(damageEvent.damage);

        // Generate damage effect
        var damageText = Instantiate(AssetManager.GetPrefab(Prefab.UI.DamageText).GetComponent<TMP_Text>(), UIManager.Canvas.transform);
        UIManager.PopupUI(damageText, Camera.main.WorldToScreenPoint(Detector.ColliderCenter), PopupType.MoveAndFadeOut);
        damageText.text = damageEvent.damage.ToString();
    }

    protected IEnumerator DestroyRoutine(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        OnDestroy?.Invoke();
    }

    public bool IsTargetWithInDistance(CharacterBase targetCharacter, float range)
    {
        var distance = Vector3.Distance(transform.position, targetCharacter.transform.position);
        return distance <= range;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag.Green))
        {
            OnGreen = true;
        }

        if (other.CompareTag(Tag.Water))
        {
            MovementController.SetVelocityMultiplier(0.6f);
            InWater = true;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tag.Green))
        {
            OnGreen = false;
        }

        if (other.CompareTag(Tag.Water))
        {
            MovementController.SetVelocityMultiplier(1);
            InWater = false;
        }
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