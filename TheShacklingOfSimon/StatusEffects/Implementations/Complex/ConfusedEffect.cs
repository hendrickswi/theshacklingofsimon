using System.Collections.Generic;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;

namespace TheShacklingOfSimon.StatusEffects.Implementations.Complex;

public class ConfusedEffect : ComplexStatusEffect
{
    public ConfusedEffect(string name, IDamageableEntity owner, float confusedDuration)
        : base(name, EffectType.Confusion, owner)
    {
        // Effectively, don't allow multiple confused effects at once
        float str = owner.GetStat(StatType.MoveSpeedMultiplier) < float.Epsilon 
            ? 0f 
            : owner.GetStat(StatType.MoveSpeedMultiplier) * -2f;
        ComponentEffects.Add(
            new MoveSpeedMultiplierEffect(
                name,
                owner,
                str,
                confusedDuration
            )
        );
    }

    // Clone constructor
    private ConfusedEffect(string name, EffectType type, IDamageableEntity owner, List<IStatusEffect> components) 
        : base(name, type, owner)
    {
        // Assumes components is already a deep copy, not a reference
        ComponentEffects = components;
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not ConfusedEffect castedOther) return;
        for (int i = 0; i < ComponentEffects.Count; i++)
        {
            if (i >= castedOther.ComponentEffects.Count) break; // safety, although likely unneeded.
            ComponentEffects[i].Merge(castedOther.ComponentEffects[i]);
        }
    }

    public override IStatusEffect Clone(IDamageableEntity newTarget)
    {
        List<IStatusEffect> components = new List<IStatusEffect>();
        foreach (var effect in ComponentEffects)
        {
            components.Add(effect.Clone(newTarget));
        }

        return new ConfusedEffect(Name, Type, newTarget, components);
    }
}