#region

using System;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class MaxHealthEffect : SimpleStatusEffect
{
    /// <summary>
    /// Represents a status effect that additively modifies the maximum health of
    /// an object of type <c>IDamageableEntity</c>. The effect modifies the maximum
    /// health for a specified duration and with a specific strength.
    /// <param name="owner">The object of type <c>IDamageableEntity</c> to which
    /// the effect is applied.</param>
    /// <param name="strength">The amount of health to be added or removed.</param>
    /// <param name="duration">The duration of the effect in seconds.</param>
    /// </summary>
    public MaxHealthEffect(IDamageableEntity owner, float strength, float duration) 
        : base(owner, strength, duration)
    {
    }
    
    public override void OnApply()
    {
        Timer = 0.0f;
        int newHealth = (int) Owner.GetStat(StatType.MaxHealth) + (int) Strength;

        Difference = (int) Owner.GetStat(StatType.MaxHealth) - newHealth;
        Owner.SetStat(StatType.MaxHealth, newHealth);
    }

    public override void OnRemove()
    {
        Owner.SetStat(StatType.MaxHealth, Owner.GetStat(StatType.MaxHealth) + Difference);
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not MaxHealthEffect castedOther) return;
        
        Strength += castedOther.Strength;
        Duration = Math.Max(Duration, castedOther.Duration);
    }
}