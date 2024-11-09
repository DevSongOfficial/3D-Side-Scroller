using UnityEngine;

public enum ClubType { None, Putter, Iron, Driver }

public class GolfClub : MonoBehaviour
{
    [SerializeField] private string displayName = "Club";
    [SerializeField] private ClubType clubType;
    [SerializeField] private float swingRadius = 1.5f;
    [SerializeField] private float chargeSpeed = 1f;
    [SerializeField] private DamageEvent damageEvent;

    public string DisplayName => displayName;
    public ClubType ClubType => clubType;
    public DamageEvent DamageEvent => damageEvent;
    public float SwingRadius => swingRadius;
    public float ChargeSpeed => chargeSpeed;
    public AnimationController.Player.Attack AnimationType 
        => clubType == ClubType.Putter? AnimationController.Player.Attack.Putt : AnimationController.Player.Attack.Swing;
}