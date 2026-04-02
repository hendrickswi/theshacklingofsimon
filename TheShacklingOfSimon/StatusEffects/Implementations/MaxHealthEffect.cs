using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class MaxHealthEffect : IStatusEffect
{
    public bool IsFinished { get; private set; }
    public IDamageableEntity Owner { get; private set; }
    
    private int Strength { get; set; }
    private float Duration { get; set; }
    private float _timer;
    private int _difference;

    /// <summary>
    /// Represents a status effect that modifies the maximum health (additively) of
    /// an object of type <c>IDamageableEntity</c>. The effect modifies the maximum
    /// health for a specified duration and with a specific strength.
    /// <param name="owner">The object of type <c>IDamageableEntity</c> to which
    /// the effect is applied.</param>
    /// <param name="strength">The amount of health to be added or removed.</param>
    /// <param name="duration">The duration of the effect in seconds.</param>
    /// </summary>
    public MaxHealthEffect(IDamageableEntity owner, int strength, float duration)
    {
        IsFinished = false;
        Owner = owner;
        Strength = strength;
        Duration = duration;
    }
    
    public void OnApply()
    {
        _timer = 0.0f;
        int currentHealth = (int) Owner.GetStat(StatType.MaxHealth);
        int newHealth = currentHealth + Strength;

        _difference = newHealth - currentHealth;
        Owner.SetStat(StatType.MaxHealth, newHealth);
    }

    public void OnRemove()
    {
        Owner.SetStat(StatType.MaxHealth, Owner.GetStat(StatType.MaxHealth) + _difference);
    }

    public void Update(GameTime delta)
    {
        _timer += (float) delta.ElapsedGameTime.TotalSeconds;
        if (_timer >= Duration)
        {
            IsFinished = true;
        }
    }

    public void Merge(IStatusEffect other)
    {
        if (other is not MaxHealthEffect castedOther) return;
        
        Strength += castedOther.Strength;
        Duration = Math.Max(Duration, castedOther.Duration);
    }
}