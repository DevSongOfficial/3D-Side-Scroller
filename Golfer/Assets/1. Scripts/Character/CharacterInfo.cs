using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Health System
// 기타 등등 넣을 예정

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Scriptable Object/Character Info")]
public class CharacterInfo : ScriptableObject
{
    // Events
    public event EventHandler<int> OnTakeDamage;
    public event EventHandler<int> OnHealthChanged;
    public event EventHandler OnCharacterDie;

    /// <summary>
    /// Health Section
    /// </summary>

    [Header("Health")]
    [SerializeField] private int health;
    public int Health { get { return health; } }

    public void SetHealth(int health)
    {
        if (health == Health) return;

        this.health = health;
        OnHealthChanged.Invoke(this, health);

        if(Health <= 0)
        {
            this.health = 0;
            Die();
        }
    }

    public void TakeDamage(int damage)
    {
        OnTakeDamage.Invoke(this, damage); // TODO: before or after changing health?

        var health = Health - damage;
        SetHealth(health);
    }

    private void Die()
    {
        OnCharacterDie.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Movement Section
    /// </summary>

    [Header("Movement")]
    [SerializeField] private int movementSpeed = 5;
    public int MovementSpeed => movementSpeed;

    [SerializeField] private int acceleration = 10;
    public int Acceleration => acceleration;

    [SerializeField] private int movementSpeedOnRotate = 100;
    public int MovementSpeedOnRotate => movementSpeedOnRotate;

    [SerializeField] private int rotationSpeed = 1000;
    public int RotationSpeed => rotationSpeed;
}