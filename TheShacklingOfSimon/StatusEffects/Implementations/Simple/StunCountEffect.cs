#region

using System;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations.Simple;

public class StunCountEffect : SimpleStatusEffect
{
    public StunCountEffect(string name, IDamageableEntity owner, float duration) 
        : base(name, EffectType.Stun, owner, 1.0f, duration)
    {
    }

    public override void OnApply()
    {
        Difference = 1.0f;
        Owner.SetStat(StatType.StunCount, Owner.GetStat(StatType.StunCount) + 1.0f);
    }

    public override void OnRemove()
    {
        Owner.SetStat(StatType.StunCount, Owner.GetStat(StatType.StunCount) - 1.0f);
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not StunCountEffect castedOther) return;
        Duration = Math.Max(Duration, castedOther.Duration);
    }

    public override IStatusEffect Clone(IDamageableEntity newTarget)
    {
        return new StunCountEffect(Name, newTarget, Duration);
    }
}