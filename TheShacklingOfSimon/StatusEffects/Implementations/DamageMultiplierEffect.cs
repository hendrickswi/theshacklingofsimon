using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class DamageMultiplierEffect : IStatusEffect
{
    /*
     * Keep track of how much speed was taken away by *this*
     * so it can be restored once *this* is removed.
     */
    private int _damageAmount;
    private float _timer;
    private int Strength { get; set; }
    private float Duration { get; set; }
    
    public bool IsFinished { get; private set; }
    public IDamageableEntity Owner { get; private set; }
    
    public DamageMultiplierEffect(IDamageableEntity owner, int strength, float duration)
    {
        _damageAmount = 0;
        Strength = strength;
        Duration = duration;
        IsFinished = false;
        Owner = owner;
    }

    public void OnApply()
    {
        _timer = 0.0f;
        int currentMultiplier = (int) Owner.GetStat(StatType.DamageMultiplier);
        int newMultiplier = Math.Max(1, currentMultiplier * Strength);

        _damageAmount = currentMultiplier - newMultiplier;
        Owner.SetStat(StatType.DamageMultiplier, newMultiplier);
    }

    public void OnRemove()
    {
        int currentMultiplier = (int) Owner.GetStat(StatType.DamageMultiplier);
        Owner.SetStat(StatType.DamageMultiplier, currentMultiplier + _damageAmount);
    }

    public void Update(GameTime delta)
    {
        _timer += (float) delta.ElapsedGameTime.TotalSeconds;
        if (_timer >= Duration)
        {
            IsFinished = true;
        }
    }
    
    public void Merge(IStatusEffect other)
    {
        if (other is not DamageMultiplierEffect castedOther) return; 
        
        // For now just "add" the multipliers
        // and make the duration the average of the two
        Strength = Strength + castedOther.Strength;
        Duration = (Duration + castedOther.Duration) / 2;
    }
}