#region

using System;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class PrimaryCooldownEffect : SimpleStatusEffect
{
    public PrimaryCooldownEffect(IDamageableEntity owner, float strength, float duration) 
        : base(owner, strength, duration)
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
}