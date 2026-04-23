#region

using System.Collections.Generic;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations.Complex;

public class StunEffect : ComplexStatusEffect
{
    public StunEffect(string name, IDamageableEntity owner, float moveStunDuration, float disarmDuration) 
        : base(name, EffectType.Stun, owner)
    {
        ComponentEffects.Add(
            new StunCountEffect(
                name, 
                owner,
                moveStunDuration
            )
        );
        ComponentEffects.Add(
            new PrimaryCooldownEffect(
                Name,
                Owner,
                disarmDuration,
                disarmDuration
            )
        );
        ComponentEffects.Add(
            new SecondaryCooldownEffect(
                Name,
                Owner, 
                disarmDuration, 
                disarmDuration
            )
        );
    }

    // Clone constructor
    private StunEffect(string name, EffectType type, IDamageableEntity owner, List<IStatusEffect> components) 
        : base(name, type, owner)
    {
        // Assumes components is already a deep copy, not a reference
        ComponentEffects = components;
    }

    public override void Merge(IStatusEffect other)
    {
        if (other is not StunEffect castedOther) return;
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
        
        return new StunEffect(Name, Type, newTarget, components);
    }
}