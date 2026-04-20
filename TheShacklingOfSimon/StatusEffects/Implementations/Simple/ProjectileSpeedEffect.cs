#region

using System;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations.Simple;

public class ProjectileSpeedEffect : SimpleStatusEffect
{
    /// <summary>
    /// Represents a status effect that additively modifies the speed of projectiles for the given object of
    /// type <c>IDamageableEntity</c>. The effect modifies the speed of projectiles by a specified strength
    /// and for a specific duration.
    /// </summary>
    /// <remarks>
    /// Simply changes the underlying stat of <c>owner</c>.
    /// </remarks>
    /// <param name="name">The name of the effect.</param>
    /// <param name="type">The matching <c>EffectType</c> of the effect.</param>
    /// <param name="owner">The object of type <c>IDamageableEntity</c> to which the effect is applied.</param>
    /// <param name="strength">The amount of projectile speed to be added or removed.</param>
    /// <param name="duration">The duration of the effect in seconds.</param>
    public ProjectileSpeedEffect(string name, EffectType type, IDamageableEntity owner, float strength, float duration)
        : base(name, type, owner, strength, duration)
    {
    }

    public override void OnApply()
    {
        Timer = 0.0f;
        float currentSpeed = Owner.GetStat(StatType.ProjectileSpeedMultiplier);
        float newSpeed = currentSpeed + Strength;

        Difference = currentSpeed - newSpeed;
        Owner.SetStat(StatType.ProjectileSpeedMultiplier, newSpeed);
    }

    public override void OnRemove()
    {
        Owner.SetStat(StatType.ProjectileSpeedMultiplier, Owner.GetStat(StatType.ProjectileSpeedMultiplier) + Difference);
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not ProjectileSpeedEffect castedOther) return;

        Strength += castedOther.Strength;
        Duration = Math.Max(Duration, castedOther.Duration);
    }
    
    public override IStatusEffect Clone(IDamageableEntity newTarget)
    {
        return new ProjectileSpeedEffect(Name, Type, newTarget, Strength, Duration);
    }
}