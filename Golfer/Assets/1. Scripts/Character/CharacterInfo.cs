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

    // ================================================================================

    [Header("Movement")]
    [SerializeField] private int movementSpeed = 5;
    public int MovementSpeed { get { return movementSpeed; } }

    [SerializeField] private int rotationSpeed = 1000;
    public int RotationSpeed { get { return rotationSpeed; } }

    public void SetMovementSpeed(int speed)
    {
        movementSpeed = speed;
    }
}