using System;
using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public sealed class GolfBall : MonoBehaviour, IDamageable
{
    // Physics
    private Rigidbody rigidBody;
    private new Collider collider;
    
    private const float knockBackMultiplier = 35;
    private const float torqueMultiplier   = 100;

    public struct Drag
    {
        public static float OffGround   = 0.2f;
        public static float OnGrass     = 0.3f;
        public static float OnDirt      = 0.5f;
        public static float OnSand      = 7.5f;
    }
    public struct AngularDrag
    {
        public static float OffGround   = 2.9f;
        public static float OnGrass     = 1f;
        public static float OnDirt      = 1.5f;
        public static float OnSand      = 20f;
    }

    // VFX
    private ParticleInfo particleInfo;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private void FixedUpdate()
    {
        SetBallAttributesBasedOnGroundType();
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
    }

    private const float velocityThreshold = 0.025f;
    private const float visibilityTimer = 1;
    private float timeLeft;
    private void HandleProbCameraOutputUI()
    {
        if (FXManager.IsSlowMotioning)
        {
            UIManager.CloseUI(UIManager.GetUI.RawImage_ProbCameraOutput);
            return;
        }

        // Handle height text.
        UIManager.SetText(UIManager.GetUI.Text_ballHeight, $"{Math.Truncate(rigidBody.velocity.magnitude * 100) / 100}m/s");

        // Handle rendered image visibility.
        if (rigidBody.velocity.magnitude > velocityThreshold)
        {
            UIManager.PopupUI(UIManager.GetUI.RawImage_ProbCameraOutput);
            timeLeft = visibilityTimer;
        }
        else if (timeLeft <= 0)
        {
            UIManager.CloseUI(UIManager.GetUI.RawImage_ProbCameraOutput);
        }
        else
        {
            timeLeft -= Time.fixedDeltaTime;
        }
    }    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tag.HoleCup))
            GameManager.BallInTheCup();

        if (other.CompareTag(Tag.Green))
            GameManager.BallOnTheGreen();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tag.Green))
            GameManager.BallOutTheGreen();
    }

    private void SetBallAttributesBasedOnGroundType()
    {
        if (!Physics.Raycast(collider.bounds.center, Vector3.down, out RaycastHit hit, collider.bounds.extents.y + 0.1f, Layer.Ground.GetMask()))
        {
            particleInfo.color = Vector4.zero;
            particleInfo.lifeTime = 0f;

            rigidBody.drag = Drag.OffGround;
            rigidBody.angularDrag = AngularDrag.OffGround;
            
            return;
        }

        if (hit.collider.CompareTag(Tag.Grass))
        {
            particleInfo.color = new Vector4(0.5f, 0.4f, 0.25f, 0.8f); // Color: Grass Green
            particleInfo.lifeTime = 0.15f;

            rigidBody.drag = Drag.OnGrass;
            rigidBody.angularDrag = AngularDrag.OnGrass;

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
    }
}