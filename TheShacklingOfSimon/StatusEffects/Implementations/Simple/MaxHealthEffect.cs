#region

using System;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations.Simple;

public class MaxHealthEffect : SimpleStatusEffect
{
    /// <summary>
    /// Represents a status effect that additively modifies the maximum health of
    /// the given object of type <c>IDamageableEntity</c>. The effect modifies the maximum
    /// health for a specified duration and with a specific strength.
    /// </summary>
    /// <param name="name">The name of the effect.</param>"
    /// <param name="type">The matching <c>EffectType</c> of the effect.</param>
    /// <param name="owner">The object of type <c>IDamageableEntity</c> to which
    /// the effect is applied.</param>
    /// <param name="strength">The amount of health to be added or removed.</param>
    /// <param name="duration">The duration of the effect in seconds.</param>
    /// 
    public MaxHealthEffect(string name, EffectType type, IDamageableEntity owner, float strength, float duration) 
        : base(name, type, owner, strength, duration)
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
    
    public override IStatusEffect Clone(IDamageableEntity newTarget)
    {
        return new MaxHealthEffect(Name, Type, newTarget, Strength, Duration);
    }
}