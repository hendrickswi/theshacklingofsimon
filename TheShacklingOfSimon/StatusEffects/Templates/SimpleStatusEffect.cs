#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public abstract class SimpleStatusEffect : IStatusEffect
{
    public bool IsFinished { get; private set; }
    public IDamageableEntity Owner { get; private set; }
    
    protected float Strength { get; set; }
    protected float Duration { get; set; }
    protected float Timer;
    protected float Difference;

    protected SimpleStatusEffect(IDamageableEntity owner, float strength, float duration)
    {
        IsFinished = false;
        Owner = owner;
        Strength = strength;
        Duration = duration;
    }

    public abstract void OnApply();

    public abstract void OnRemove();

    public virtual void Update(GameTime delta)
    {
        Timer += (float) delta.ElapsedGameTime.TotalSeconds;
        if (Timer >= Duration)
        {
            IsFinished = true;
        }
    }

    public abstract void Merge(IStatusEffect other);
}