using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Scriptable Object/Character Info")]
public class CharacterInfo : ScriptableObject
{
    // Events
    public event EventHandler<int> OnTakeDamage;
    public event EventHandler<int> OnHealthChanged;
    public event EventHandler OnCharacterDie;

    
    // Health
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


    // Movement
    [Header("Movement")]
    [SerializeField] private int movementSpeed = 5;
    [SerializeField] private int acceleration = 10;

    public int MovementSpeed => movementSpeed;
    public int Acceleration => acceleration;
}