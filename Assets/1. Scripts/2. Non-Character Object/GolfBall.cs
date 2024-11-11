using System;
using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public sealed class GolfBall : MonoBehaviour, IDamageable
{
    private Rigidbody rigidBody;
    private bool isGrounded;

    private const float knockBackMultiplier = 35;
    private const float torqueMultiplier = 100;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleAngularDrag();
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
    }

    private const float velocityThreshold = 0.025f;
    private const float visibilityTimer = 1;
    private float timeLeft;
    private void HandleProbCameraOutputUI()
    {
        // Handle height text.
        UIManager.SetText(UIManager.GetUI.Text_ballHeight, $"{Math.Truncate(rigidBody.velocity.magnitude * 100) / 100}m/s");

        // Handle render image visibility.
        if (rigidBody.velocity.magnitude > velocityThreshold)
        {
            UIManager.PopupUI(UIManager.GetUI.RawImage_ProbCameraOutput);
            timeLeft = visibilityTimer;
        }
        else if(timeLeft <= 0)
        {
            UIManager.CloseUI(UIManager.GetUI.RawImage_ProbCameraOutput);
        }
        else
        {
            timeLeft -= Time.fixedDeltaTime;
        }
    }

    private const float angularDragOnGround = 0;
    private const float angularDragOffGround = 2.9f;
    private void HandleAngularDrag()
    {
        rigidBody.angularDrag = isGrounded ? angularDragOffGround : angularDragOnGround;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.CompareLayer(Layer.Ground))
            isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.CompareLayer(Layer.Ground))
            isGrounded = false;
    }
}