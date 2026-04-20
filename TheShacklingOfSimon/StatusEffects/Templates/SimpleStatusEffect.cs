#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Templates;

public abstract class SimpleStatusEffect : IStatusEffect
{
    public string Name { get; protected set; }
    public EffectType Type { get; protected set; }
    public bool IsFinished { get; private set; }
    public IDamageableEntity Owner { get; private set; }
    
    protected float Strength { get; set; }
    protected float Duration { get; set; }
    protected float Timer;
    protected float Difference;

    protected SimpleStatusEffect(string name, EffectType type, IDamageableEntity owner, float strength, float duration)
    {
        Name = name;
        Type = type;
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
    public abstract IStatusEffect Clone(IDamageableEntity newTarget);
}