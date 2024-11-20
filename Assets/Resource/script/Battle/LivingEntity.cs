using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startingHealth = 10f;
    public float startingMana = 1f;
    public float health { get; protected set; }
    public float maxHealth { get; protected set; }
    public float mana { get; protected set; }
    public float maxMana { get; protected set; }
    public bool dead {  get; protected set; }
    public string state { get; protected set; }
    public event Action onDeath;

    protected virtual void OnEnable()
    {
        dead = false;
        //turnEnd = true;
        health = startingHealth;
        maxHealth = startingHealth;
        mana = startingMana;
        maxMana = startingMana;
        state = "normal";
    }

    public virtual void OnDamage(float damage, Vector3 hitPos, Vector3 hitSurface)
    {
        //이거 안 줄어드는데요?
        health -= damage;
        Debug.Log("damaged " + damage + health);

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void RestoreHealth(float newHealth)
    {
        if (dead) return;

        float addedHealth = health + newHealth;
        health = (addedHealth >= maxHealth ) ? maxHealth : addedHealth;
    }

    public virtual void Revive(float newHealth)
    {
        if (!dead) return;

        health = newHealth;

    }

    public virtual void Die()
    {
        if(onDeath != null) onDeath();

        dead = true;
        //turnEnd = true;
        Debug.Log("dead " + name);
    }
}
