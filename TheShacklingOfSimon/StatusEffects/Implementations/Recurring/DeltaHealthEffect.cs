#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations.Recurring;

public class DeltaHealthEffect : RecurringStatusEffect
{
    public DeltaHealthEffect(string name, EffectType type, IDamageableEntity owner, float tickStrength, float duration, float numTicks) 
        : base(name, type, owner, tickStrength, duration, numTicks)
    {
    }

    public override void Update(GameTime delta)
    {
        base.Update(delta);
        if (PreviousApplicationTime < TickDuration) return;

        if (Strength >= 0)
        {
            Owner.Heal((int)Strength);
        }
        else
        {
            Owner.TakeDamage((int)-Strength, true);
                    
        }
        PreviousApplicationTime -= TickDuration;
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not DeltaHealthEffect otherCasted) return;

        Strength += otherCasted.Strength;
        Duration = Math.Max(Duration, otherCasted.Duration);
    }

    public override IStatusEffect Clone(IDamageableEntity newTarget)
    {
        return new DeltaHealthEffect(Name, Type, newTarget, Strength, Duration, NumTicks);
    }
}