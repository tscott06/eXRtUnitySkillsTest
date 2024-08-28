using System;
using UnityEngine.Events;

public interface IHealth
{
    public float CurrentHealth { get; }
    public float MaxHealth { get; }
    public float PercentHealth { get; }

    public HealthChangeEvent OnHealthChange { get; }
    public ZeroHealthEvent OnZeroHealth { get; }


    public void SetHealth(float newAmount);
    public void TakeDamage(IDamageInflictor inflictor, float damageAmount);

}

[Serializable]
public struct HealthChangeEventArgs
{

    private IHealth _healthHaver;
    private float _healthChange;

    public HealthChangeEventArgs(IHealth healthHaver, float healthChange)
    {
        _healthHaver = healthHaver;
        _healthChange = healthChange;
    }

    public IHealth HealthHaver { get => _healthHaver; }
    public float HealthChange { get => _healthChange; }
    public float PreviousHealth => _healthHaver.CurrentHealth + _healthChange;
}

[Serializable]
public class HealthChangeEvent : UnityEvent<HealthChangeEventArgs> { }

[Serializable]
public struct ZeroHealthArgs
{

    private IHealth victim;
    private IDamageInflictor causeOfDeath;

    public IHealth Victim { get => victim; }
    public IDamageInflictor CauseOfDeath { get => causeOfDeath; }

    public ZeroHealthArgs(IHealth victim, IDamageInflictor causeOfDeath)
    {
        this.victim = victim;
        this.causeOfDeath = causeOfDeath;
    }
}

[Serializable]
public class ZeroHealthEvent : UnityEvent<ZeroHealthArgs> { }

