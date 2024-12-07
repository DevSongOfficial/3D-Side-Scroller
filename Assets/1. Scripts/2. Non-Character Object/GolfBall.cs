using System;
using Unity.VisualScripting;
using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
// This class is singleton managed by PleaceableProb.
public sealed class GolfBall : MonoBehaviour, IDamageable
{
    // Physics
    private Rigidbody rigidBody;
    private new Collider collider;
    
    private const float knockBackMultiplier = 35;
    private const float torqueMultiplier    = 1000;

    public event Action OnHit;

    public bool OnGreen { get; private set; }
    public event Action OnEnterGreen;
    public event Action OnExitGreen;

    public struct Drag
    {
        public static float OffGround   = 0f;
        public static float OnGreen     = 0.1f;
        public static float OnGrass     = 2.0f;
        public static float OnDirt      = 3.5f;
        public static float OnSand      = 9.5f;
        public static float UnderWater  = 4;    // Water area is way larger than sand area so drag should be less.
    }
    public struct AngularDrag
    {
        public static float OffGround   = 0f;
        public static float OnGreen     = 0.1f;
        public static float OnGrass     = 2.5f;
        public static float OnDirt      = 3.5f;
        public static float OnSand      = 20f;
        public static float UnderWater  = 25;
    }

    // VFX
    private ParticleInfo particleInfo;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void OnEnable()
    {
        OnHit += GameManager.Player.IncrementStroke;
    }

    private void OnDisable()
    {
        OnHit -= GameManager.Player.IncrementStroke;
    }

    private void FixedUpdate()
    {
        HandleBallAttributesBasedOnGroundType();
    }

    private void LateUpdate()
    {
        HandleProbCameraOutputUI();
    }

    public void TakeDamage(DamageEvent damageEvent)
    {
        if (!damageEvent.CompareSenderTypeWith(EventSenderType.Club)) return;

        rigidBody.AddForce(damageEvent.knockBackVelocity * knockBackMultiplier);
        rigidBody.AddTorque(Vector3.forward * torqueMultiplier, ForceMode.VelocityChange);

        // Make an effect.
        if(damageEvent.knockBackVelocity.y < 1) particleInfo.lifeTime = 0;
        var effect = FXManager.CreateParticleFX(Prefab.VFX.Dirt, null, particleInfo);
        effect.transform.position = new Vector3(transform.position.x, collider.bounds.min.y, transform.position.z);

        OnHit?.Invoke();
    }

    private const float velocityThreshold = 0.025f;
    private const float visibilityTimer = 1;
    private float visibilityTimeLeft;
    private void HandleProbCameraOutputUI()
    {
        if (FXManager.IsSlowMotioning)
        {
            UIManager.CloseUI(UIManager.UI.RawImage_ProbCameraOutput);
            return;
        }

        // Handle height text.
        UIManager.SetText(UIManager.UI.Text_ballHeight, $"{Math.Truncate(rigidBody.velocity.magnitude * 100) / 100}m/s");

        // Handle rendered image visibility.
        if (rigidBody.velocity.magnitude > velocityThreshold)
        {
            UIManager.PopupUI(UIManager.UI.RawImage_ProbCameraOutput);
            visibilityTimeLeft = visibilityTimer;
        }
        else if (visibilityTimeLeft <= 0)
        {
            UIManager.CloseUI(UIManager.UI.RawImage_ProbCameraOutput);
        }
        else
        {
            visibilityTimeLeft -= Time.fixedDeltaTime;
        }
    }    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag.HoleCup))
            GameManager.BallInTheCup();

        if (other.CompareTag(Tag.Green))
        {
            OnGreen = true;
            OnEnterGreen?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tag.Green))
        {
            OnGreen = false;
            OnExitGreen?.Invoke();
        }
    }

    private const float distance = 1;
    private void HandleBallAttributesBasedOnGroundType()
    {
        if (!Physics.Raycast(collider.bounds.center + Vector3.up * distance, Vector3.down, out RaycastHit hit, collider.bounds.extents.y + distance + 0.1f , Layer.Ground.GetMask()))
        {
            particleInfo.color = Vector4.zero;
            particleInfo.lifeTime = 0f;

            rigidBody.drag = Drag.OffGround;
            rigidBody.angularDrag = AngularDrag.OffGround;

            return;
        }

        if (hit.collider.CompareTag(Tag.Grass))
        {
            particleInfo.color = new Vector4(0.5f, 0.4f, 0.25f, 0.8f); // Color: Dirt/Grass Brown
            particleInfo.lifeTime = 0.15f;

            rigidBody.drag = Drag.OnGrass;
            rigidBody.angularDrag = AngularDrag.OnGrass;

            return;
        }

        if (hit.collider.CompareTag(Tag.Green))
        {
            particleInfo.color = new Vector4(0.1f, 0.8f, 0.25f, 0.8f); // Color: Grass Green
            particleInfo.lifeTime = 0.15f;

            rigidBody.drag = Drag.OnGreen;
            rigidBody.angularDrag = AngularDrag.OnGreen;

            return;
        }

        if (hit.collider.CompareTag(Tag.Dirt))
        {
            particleInfo.color = new Vector4(0.4f, 0.2f, 0, 1); // Color: Dirt Brown
            particleInfo.lifeTime = 0.25f;

            rigidBody.drag = Drag.OnDirt;
            rigidBody.angularDrag = AngularDrag.OnDirt;

            return;
        }

        if (hit.collider.CompareTag(Tag.Sand))
        {
            particleInfo.color = new Vector4(0.8f, 0.8f, 0.6f, 1); // Color: Sand Yellow
            particleInfo.lifeTime = 1f;

            rigidBody.drag = Drag.OnSand;
            rigidBody.angularDrag = AngularDrag.OnSand;

            return;
        }

        if (hit.collider.CompareTag(Tag.Water))
        {
            particleInfo.color = new Vector4(0.2f, 0.2f, 0.9f, 0.9f); // Color: Water Blue
            particleInfo.lifeTime = 1.2f;

            rigidBody.drag = Drag.UnderWater;
            rigidBody.angularDrag = AngularDrag.UnderWater;

            return;
        }
    }
}