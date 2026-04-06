using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects.Templates;

namespace TheShacklingOfSimon.StatusEffects;

public abstract class RecurringStatusEffect : IStatusEffect
{
    public string Name { get; protected set; }
    public EffectType Type { get; protected set; }
    public bool IsFinished { get; private set; }
    public IDamageableEntity Owner { get; private set; }
    
    protected float Strength { get; set; }
    protected float Duration { get; set; }
    protected float NumTicks { get; set; }
    
    protected float Timer;
    protected float PreviousApplicationTime;
    protected float TickDuration;

    protected RecurringStatusEffect(string name, EffectType type, IDamageableEntity owner, float strength, float duration, float numTicks)
    {
        Name = name;
        Type = type;
        IsFinished = false;
        Owner = owner;
        Strength = strength;
        Duration = duration;
        NumTicks = numTicks;
        TickDuration = Duration / NumTicks;
    }

    public virtual void OnApply()
    {
        // Default implementation is a no-op
    }

    public virtual void OnRemove()
    {
        // Default implementation is a no-op
    }

    public virtual void Update(GameTime delta)
    {
        float diff = (float) delta.ElapsedGameTime.TotalSeconds;
        Timer += diff;
        PreviousApplicationTime += diff;
        
        if (Timer >= Duration)
        {
            IsFinished = true;
        }

        // Delegate rest of Update() logic to subclasses
    }

    public abstract void Merge(IStatusEffect other);
}