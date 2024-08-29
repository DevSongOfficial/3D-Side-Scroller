using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo", menuName = "Scriptable Object/Character Info/Character Info")]
public class CharacterInfo : ScriptableObject
{
    // Events
    public event Action<int> OnTakeDamage;
    public event Action<int> OnHealthChanged;
    public event Action OnCharacterDie;

    
    // Health
    [Header("Health")]
    [SerializeField] private int health;
    
    public int Health { get { return health; } }
    public void SetHealth(int health)
    {
        if (Health == 0) return;
        if (health == Health) return;

        this.health = health;
        OnHealthChanged?.Invoke(health);

        if(Health <= 0)
        {
            this.health = 0;
            Die();
        }
    }
    public void TakeDamage(int damage)
    {
        OnTakeDamage?.Invoke(damage); // todo: before or after changing health?

        var health = Health - damage;
        SetHealth(health);
    }
    private void Die()
    {
        OnCharacterDie?.Invoke();
        Debug.Log("Dead");
    }


    // Movement
    [Header("Movement")]
    [SerializeField] private int movementSpeed = 5;
    [SerializeField] private int acceleration = 10;

    public int MovementSpeed => movementSpeed;
    public int Acceleration => acceleration;
}