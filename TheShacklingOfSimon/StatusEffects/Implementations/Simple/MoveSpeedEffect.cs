#region

using System;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations.Simple;

public class MoveSpeedEffect : SimpleStatusEffect
{

    /// <summary>
    /// Represents a status effect that additively modifies the movement speed of the given
    /// object of type <c>IDamageableEntity</c> for a specific duration.
    /// </summary>
    /// <param name="name">The name of the effect</param>"
    /// <param name="type">The matching <c>EffectType</c> of the effect.</param>
    /// <param name="owner">The object of type <c>IDamageableEntity</c> to which the effect is to be applied</param>
    /// <param name="strength">The amount of movement speed to be added or removed</param>
    /// <param name="duration">The duration of the effect in seconds</param>
    public MoveSpeedEffect(string name, EffectType type, IDamageableEntity owner, float strength, float duration) 
        : base(name, type, owner, strength, duration)
    {
    }

    public override void OnApply()
    {
        Timer = 0.0f;
        float currentSpeed = Owner.GetStat(StatType.MoveSpeed);
        float newSpeed = Math.Max(1f, currentSpeed + Strength);

        Difference = currentSpeed - newSpeed;
        Owner.SetStat(StatType.MoveSpeed, newSpeed);
    }

    public override void OnRemove()
    {
        Owner.SetStat(StatType.MoveSpeed, Owner.GetStat(StatType.MoveSpeed) + Difference);
    }
    
    public override void Merge(IStatusEffect other)
    {
        if (other is not MoveSpeedEffect castedOther) return; 

        Strength += castedOther.Strength;
        Duration = Math.Max(Duration, castedOther.Duration);
    }
    
    public override IStatusEffect Clone(IDamageableEntity newTarget)
    {
        return new MoveSpeedEffect(Name, Type, newTarget, Strength, Duration);
    }
}