#region

using System;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations.Simple;

public class DamageMultiplierEffect : SimpleStatusEffect
{
    /// <summary>
    /// Represents a status effect that additively modifies the damage multiplier of an object of
    /// type IDamageableEntity. This effect increases the damage multiplier of the targeted entity
    /// for a specified duration.
    /// </summary>
    /// <param name="name">The name of the effect.</param>"
    /// <param name="type">The matching <c>EffectType</c> of the effect.</param>
    /// <param name="owner">The object of type IDamageableEntity to which the effect is applied.</param>
    /// <param name="strength">The amount of damage multiplier to be added or removed.</param>
    /// <param name="duration">The duration of the effect in seconds.</param>
    /// 
    public DamageMultiplierEffect(string name, EffectType type, IDamageableEntity owner, float strength, float duration) 
        : base(name, type, owner, strength, duration)
    {
    }

    public override void OnApply()
    {
        Timer = 0.0f;
        float currentMultiplier = Owner.GetStat(StatType.DamageMultiplier);
        float newMultiplier = Math.Max(1, currentMultiplier + Strength);

        Difference = currentMultiplier - newMultiplier;
        Owner.SetStat(StatType.DamageMultiplier, newMultiplier);
    }

    public override void OnRemove()
    {
        int currentMultiplier = (int) Owner.GetStat(StatType.DamageMultiplier);
        Owner.SetStat(StatType.DamageMultiplier, currentMultiplier + Difference);
    }
    
    public override void Merge(IStatusEffect other)
    {
        if (other is not DamageMultiplierEffect castedOther) return; 

        Strength += castedOther.Strength;
        Duration = Math.Max(Duration, castedOther.Duration);
    }

    public override IStatusEffect Clone(IDamageableEntity newTarget)
    {
        return new DamageMultiplierEffect(Name, Type, newTarget, Strength, Duration);
    }
}