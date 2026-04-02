using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class DamageMultiplierEffect : IStatusEffect
{
    public bool IsFinished { get; private set; }
    public IDamageableEntity Owner { get; private set; }
    
    private float _difference;
    private float _timer;
    private float Strength { get; set; }
    private float Duration { get; set; }

    /// <summary>
    /// Represents a status effect that modifies the damage multiplier (additively) an object of
    /// type IDamageableEntity. This effect increases the damage multiplier of the targeted entity
    /// for a specified duration.
    /// <param name="owner">The object of type IDamageableEntity to which the effect is applied.</param>
    /// <param name="strength">The amount of damage multiplier to be added or removed.</param>
    /// <param name="duration">The duration of the effect in seconds.</param>
    /// </summary>
    public DamageMultiplierEffect(IDamageableEntity owner, float strength, float duration)
    {
        IsFinished = false;
        Owner = owner;
        Strength = strength;
        Duration = duration;
    }

    public void OnApply()
    {
        _timer = 0.0f;
        float currentMultiplier = Owner.GetStat(StatType.DamageMultiplier);
        float newMultiplier = Math.Max(1, currentMultiplier + Strength);

        _difference = currentMultiplier - newMultiplier;
        Owner.SetStat(StatType.DamageMultiplier, newMultiplier);
    }

    public void OnRemove()
    {
        int currentMultiplier = (int) Owner.GetStat(StatType.DamageMultiplier);
        Owner.SetStat(StatType.DamageMultiplier, currentMultiplier + _difference);
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
        if (other is not DamageMultiplierEffect castedOther) return; 
        
        // For now just "add" the multipliers
        // and make the duration the average of the two
        Strength += castedOther.Strength;
        Duration = (Duration + castedOther.Duration) / 2;
    }
}