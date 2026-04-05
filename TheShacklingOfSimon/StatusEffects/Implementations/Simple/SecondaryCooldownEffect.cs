#region

using System;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class SecondaryCooldownEffect : SimpleStatusEffect
{
    public SecondaryCooldownEffect(IDamageableEntity owner, float strength, float duration) 
        : base(owner, strength, duration)
    {
    }
    
    public override void OnApply()
    {
        Timer = 0.0f;
        float currentCooldown = Owner.GetStat(StatType.SecondaryCooldown);
        float newCooldown = currentCooldown + Strength;
        
        Difference = currentCooldown - newCooldown;
        Owner.SetStat(StatType.SecondaryCooldown, newCooldown);
    }

    public override void OnRemove()
    {
        Owner.SetStat(StatType.SecondaryCooldown, Owner.GetStat(StatType.SecondaryCooldown) + Difference);
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not SecondaryCooldownEffect otherCasted) return;

        Strength += otherCasted.Strength;
        Duration = Math.Max(Duration, otherCasted.Duration);
    }
}