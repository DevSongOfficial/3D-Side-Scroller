using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfClub : MonoBehaviour
{
    [SerializeField] private ItemSlotType slotType;
    [SerializeField] private string displayName = "Club";
    [SerializeField] private float swingRadius = 1.5f;
    [SerializeField] private float chargeSpeed = 1f;
    [SerializeField] private DamageEvent damageEvent;

    public ItemSlotType SlotType => slotType;
    public string DisplayName => displayName;
    public DamageEvent DamageEvent => damageEvent;
    public float SwingRadius => swingRadius;
    public float ChargeSpeed => chargeSpeed;
}