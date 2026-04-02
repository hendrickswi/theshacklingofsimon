using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class SecondaryCooldownEffect : IStatusEffect
{
    public bool IsFinished { get; private set; }
    public IDamageableEntity Owner { get; private set; }
    
    private float Strength { get; set; }
    private float Duration { get; set; }

    public SecondaryCooldownEffect(IDamageableEntity owner, float strength, float duration)
    {
        IsFinished = false;
        Owner = owner;
        Strength = strength;
        Duration = duration;
    }
    
    public void OnApply()
    {
        // TODO: Implement this
    }

    public void OnRemove()
    {
        // TODO: Implement this
    }

    public void Update(GameTime delta)
    {
        // TODO: Implement this
    }

    public void Merge(IStatusEffect other)
    {
        // TODO: Implement this
    }
}