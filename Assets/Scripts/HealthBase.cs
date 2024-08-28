using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthBase : MonoBehaviour, IHealth
{

    [SerializeField] protected float startHealth;
    protected float currentHealth;
    [SerializeField] protected float maxHealth = 100;

    [SerializeField] protected HealthChangeEvent onHealthChange;
    [SerializeField] protected ZeroHealthEvent onZeroHealth;

    protected IDamageInflictor lastDamageSource;
    protected bool isDead = false;

    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float PercentHealth => currentHealth / maxHealth;

    public HealthChangeEvent OnHealthChange => onHealthChange;
    public ZeroHealthEvent OnZeroHealth => onZeroHealth;


    protected virtual void Start()
    {
        SetHealth(startHealth, true);
    }

    private void Update()
    {
        if (currentHealth <= 0 && !isDead)
            Die();
    }

    public virtual void SetHealth(float newAmount) => SetHealth(newAmount, false);

    public virtual void SetHealth(float newAmount, bool suppressEvent = false)
    {
        if (newAmount > maxHealth) 
        {
            Debug.LogError("Cannot set health beyond max! ");
            newAmount = maxHealth;
        }

        if(newAmount == currentHealth)
        {
            Debug.LogError("Health already at this level, no change made" + newAmount + " " + currentHealth);
            return;
        }

        float previousHealth = currentHealth;
        currentHealth = newAmount;

        if (!suppressEvent)
            OnHealthChange.Invoke(new HealthChangeEventArgs(this, newAmount - previousHealth));
    }

    public virtual void AdjustHealth(float adjustBy) => SetHealth(currentHealth + adjustBy);
    public virtual void RestoreFullHealth() => SetHealth(maxHealth);
  
    public virtual void TakeDamage(IDamageInflictor inflictor, float damageAmount)
    {
        if(damageAmount <= 0)
        {
            Debug.LogError("Damage inflicted cannot be 0 or less");
            return;
        }

        lastDamageSource = inflictor;

        Debug.Log("damaging by " + damageAmount);

        AdjustHealth(-damageAmount);
    }

    protected virtual void Die()
    {
        isDead = true;
        OnZeroHealth.Invoke(new ZeroHealthArgs(this, lastDamageSource));
    }

    public virtual void ForceKill()
    {
        currentHealth = 0;
    }

    protected virtual void Reset()
    {
        //Start off with default values that are non-zero!
        maxHealth = 100f;
        startHealth = maxHealth;
    }

    protected virtual void OnValidate()
    {
        //Would be better to use custom inspector but this'll do
        if (startHealth < 0)
        {
            Debug.LogError("Start health must non-negative!");
            startHealth = 0;
        } else if (startHealth > maxHealth)
        {
            Debug.LogError("Start health cannot be greater than max health");
            startHealth = maxHealth;
        }    
    }
}
