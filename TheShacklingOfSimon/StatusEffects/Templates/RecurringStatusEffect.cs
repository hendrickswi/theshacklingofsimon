#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

#endregion

namespace TheShacklingOfSimon.StatusEffects.Templates;

public abstract class RecurringStatusEffect : IStatusEffect
{
    public string Name { get; protected set; }
    public EffectType Type { get; protected set; }
    public bool IsFinished { get; private set; }
    public IDamageableEntity Owner { get; private set; }
    
    protected float Strength { get; set; }
    protected float Duration { get; set; }
    protected float NumTicks { get; set; }
    
    protected float PreviousApplicationTime;
    protected float TickDuration;
    
    private float _timer;

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
    }

    public virtual void OnRemove()
    {
    }

    public virtual void Update(GameTime delta)
    {
        float diff = (float) delta.ElapsedGameTime.TotalSeconds;
        _timer += diff;
        PreviousApplicationTime += diff;
        
        if (_timer >= Duration)
        {
            IsFinished = true;
        }

        // Delegate rest of Update() logic to subclasses
    }

    public abstract void Merge(IStatusEffect other);
    public abstract IStatusEffect Clone(IDamageableEntity newTarget);
}