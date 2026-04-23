using System;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Templates;

namespace TheShacklingOfSimon.StatusEffects.Implementations.Simple;

public class MoveSpeedMultiplierEffect : SimpleStatusEffect
{
    /// <summary>
    /// Represents a status effect additively that modifies the move speed
    /// multiplier stat of the given object of type <c>IDamageableEntity</c>
    /// for a specific duration.
    /// </summary>
    /// <param name="name">The name of the effect</param>
    /// <param name="owner">The object of type <c>IDamageableEntity</c> to which the effect is to be applied</param>
    /// <param name="strength">The amount of move speed multiplier to be added or removed</param>
    /// <param name="duration">The duration of the effect in seconds</param>
    public MoveSpeedMultiplierEffect(string name, IDamageableEntity owner, float strength, float duration)
        : base(name, EffectType.MoveSpeedMultiplier, owner, strength, duration)
    {
    }

    public override void OnApply()
    {
        Timer = 0.0f;
        float currentSpeed = Owner.GetStat(StatType.MoveSpeedMultiplier);
        float newSpeed = currentSpeed + Strength;

        Difference = currentSpeed - newSpeed;
        Owner.SetStat(StatType.MoveSpeedMultiplier, newSpeed);
        Console.WriteLine($"Current speed: {currentSpeed}, new speed: {newSpeed}");
    }

    public override void OnRemove()
    {
        Owner.SetStat(StatType.MoveSpeedMultiplier, Owner.GetStat(StatType.MoveSpeedMultiplier) + Difference);
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not MoveSpeedMultiplierEffect castedOther) return;
        Strength += castedOther.Strength;
        Duration = Math.Max(Duration, castedOther.Duration);
    }
    
    public override IStatusEffect Clone(IDamageableEntity newTarget)
    {
        return new MoveSpeedMultiplierEffect(Name, newTarget, Strength, Duration);
    }
}