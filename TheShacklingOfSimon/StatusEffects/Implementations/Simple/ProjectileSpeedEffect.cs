#region

using System;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class ProjectileSpeedEffect : SimpleStatusEffect
{
    public ProjectileSpeedEffect(IDamageableEntity owner, float strength, float duration) 
        : base(owner, strength, duration)
    {
    }
    
    public override void OnApply()
    {
        Timer = 0.0f;
        float currentSpeed = Owner.GetStat(StatType.ProjectileSpeedMultiplier);
        float newSpeed = currentSpeed + Strength;

        Difference = currentSpeed - newSpeed;
        Owner.SetStat(StatType.MoveSpeed, newSpeed);
    }

    public override void OnRemove()
    {
        Owner.SetStat(StatType.MoveSpeed, Owner.GetStat(StatType.MoveSpeed) + Difference);
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not ProjectileSpeedEffect castedOther) return;

        Strength += castedOther.Strength;
        Duration = Math.Max(Duration, castedOther.Duration);
    }
}