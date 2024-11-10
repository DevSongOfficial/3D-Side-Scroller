using UnityEngine;
using static GameSystem;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(Rigidbody))]
public sealed class GolfBall : MonoBehaviour, IDamageable
{
    private Rigidbody rigidBody;

    private const float knockBackMultiplier = 35;

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void LateUpdate()
    {
        HandleProbCameraOutputUI();
    }

    public void TakeDamage(DamageEvent damageEvent)
    {
        if(damageEvent.CompareSenderTypeWith(EventSenderType.Club))
            rigidBody.AddForce(damageEvent.knockBackVelocity * knockBackMultiplier);

        if(damageEvent.CompareSenderTypeWith(EventSenderType.Skill))
        {
            rigidBody.AddForce(damageEvent.knockBackVelocity * knockBackMultiplier);
            Debug.Log("Skilled Shot");
        }
    }

    private void HandleProbCameraOutputUI()
    {
        // Handle height text.
        UIManager.SetText(UIManager.GetUI.Text_ballHeight, $"{Mathf.Round(transform.position.y)}m");

        // Handle render image.
        if (rigidBody.velocity.magnitude > 0.1f)
            UIManager.PopupUI(UIManager.GetUI.RawImage_ProbCameraOutput);
        else
            UIManager.CloseUI(UIManager.GetUI.RawImage_ProbCameraOutput);
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
}