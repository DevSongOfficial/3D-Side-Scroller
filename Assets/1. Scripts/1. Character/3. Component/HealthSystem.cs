using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem
{
    // Events
    public event Action<int> OnTakeDamage;
    public event Action<int> OnHealthChanged;
    public event Action OnCharacterDie;

    public HealthSystem(int maxHealth)
    {
        health = maxHealth;
    }

    public int Health => health;
    private int health;

    public void SetHealth(int health)
    {
        if (Health == 0) return;
        if (health == Health) return;

        this.health = health;
        OnHealthChanged?.Invoke(health);

        if (Health <= 0)
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
    protected virtual void Die()
    {
        OnCharacterDie?.Invoke();
    }
}
