#region

using System;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class MoveSpeedEffect : SimpleStatusEffect
{

    /// <summary>
    /// Represents a status effect that additively modifies the movement speed of an object
    /// of type <c>IDamageableEntity</c> for a specific duration.
    /// <param name="owner">The object of type <c>IDamageableEntity</c> to which the effect is to be applied.</param>
    /// <param name="strength">The amount of movement speed to be added or removed.</param>
    /// <param name="duration">The duration of the effect in seconds.</param>
    /// </summary>
    public MoveSpeedEffect(IDamageableEntity owner, float strength, float duration) 
        : base(owner, strength, duration)
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
}