using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LivingEntity : MonoBehaviour, IEntityEffect
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
        health -= damage;
        Debug.Log("damaged " + damage + health);

        if (health <= 0 && !dead)
        {
            Die();
        }
    }

    public virtual void OnBuff(string name, float value, string state)
    {

    }

    public virtual void OnStat(string name, float value, string state)
    {
        switch (name)
        {
            case "hp": health += Calculate(health, value, state); break;
            case "mp": mana += Calculate(mana, value, state); break;
            //case "exp": exp += value; break;
            //case "money": money += value; break;
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

    private float Calculate(float v, float value, string state)
    {
        switch (state)
        {
            case "none": return value;
            case "rate": return v * value;
            default: return value;
        }
    }
}
