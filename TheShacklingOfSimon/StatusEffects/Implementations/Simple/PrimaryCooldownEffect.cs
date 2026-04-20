#region

using System;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations.Simple;

public class PrimaryCooldownEffect : SimpleStatusEffect
{
    /// <summary>
    /// Represents a status effect that additively modifies the cooldown of the primary attack of the given object of type
    /// <c>IDamageableEntity</c>. This status effect modifies the primary attack's cooldown by a given strength
    /// for the specified duration.
    /// </summary>
    /// <remarks>
    /// Note that <c>this</c> does not manually implement the cooldown logic.
    /// It simply adjusts the underlying stat of <c>owner</c>.
    /// </remarks>
    /// <param name="name">The name of the effect.</param>
    /// <param name="type">The matching <c>EffectType</c> of the effect.</param>
    /// <param name="owner">The object of type <c>IDamageableEntity</c> to which the effect is applied.</param>
    /// <param name="strength">The amount of cooldown reduction to be added or removed.</param>
    /// <param name="duration">The duration of the effect in seconds.</param>
    public PrimaryCooldownEffect(string name, EffectType type, IDamageableEntity owner, float strength, float duration)
        : base(name, type, owner, strength, duration)
    {
    }

    public override void OnApply()
    {
        Timer = 0.0f;
        float currentCooldown = Owner.GetStat(StatType.PrimaryCooldown);
        float newCooldown = currentCooldown + Strength;
        
        Difference = currentCooldown - newCooldown;
        Owner.SetStat(StatType.PrimaryCooldown, newCooldown);
    }

    public override void OnRemove()
    {
        Owner.SetStat(StatType.PrimaryCooldown, Owner.GetStat(StatType.PrimaryCooldown) + Difference);
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not PrimaryCooldownEffect otherCasted) return;

        Strength += otherCasted.Strength;
        Duration = Math.Max(Duration, otherCasted.Duration);
    }
    
    public override IStatusEffect Clone(IDamageableEntity newTarget)
    {
        return new PrimaryCooldownEffect(Name, Type, newTarget, Strength, Duration);
    }
}