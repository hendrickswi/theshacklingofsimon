using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class ProjectileSpeedEffect : IStatusEffect
{
    // TODO: Implement instance variables
    
    public bool IsFinished { get; private set; }
    public IDamageableEntity Owner { get; private set; }

    public ProjectileSpeedEffect(IDamageableEntity owner, float strength, float duration)
    {
        IsFinished = false;
        Owner = owner;
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