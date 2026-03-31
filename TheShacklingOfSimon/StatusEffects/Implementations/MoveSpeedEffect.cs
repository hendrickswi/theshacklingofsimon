using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.StatusEffects.Implementations;

public class MoveSpeedEffect : IStatusEffect
{
    /*
     * Keep track of how much speed was taken away or added
     * by *this* so it can be restored once *this* is removed.
     */
    private float _speedAmount;
    private float _timer;
    private float Strength { get; set; }
    private float Duration { get; set; }
    
    public bool IsFinished { get; private set; }
    public IDamageable Owner { get; private set; }
    
    public MoveSpeedEffect(IDamageable owner, float strength, float duration)
    {
        _speedAmount = 0.0f;
        Strength = strength;
        Duration = duration;
        IsFinished = false;
        Owner = owner;
    }

    public void OnApply()
    {
        _timer = 0.0f;
        float currentSpeed = Owner.GetStat(StatType.MoveSpeed);
        float newSpeed = Math.Max(1f, currentSpeed * Strength);

        _speedAmount = currentSpeed - newSpeed;
        Owner.SetStat(StatType.MoveSpeed, newSpeed);
    }

    public void OnRemove()
    {
        float currentSpeed = Owner.GetStat(StatType.MoveSpeed);
        Owner.SetStat(StatType.MoveSpeed, currentSpeed + _speedAmount);
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
        /*
         * Double dispatch/vistor pattern here doesn't make much sense
         *      Would cause lots of bloating in IStatusEffect
         *      The manager already ensures Merge() is only called with the same GetType() result
         * 
         */
        if (other is not MoveSpeedEffect castedOther) return; 
        
        // For now just "add" the multipliers
        // and make the duration the average of the two
        Strength = 1.0f - Strength - castedOther.Strength;
        Duration = (Duration + castedOther.Duration) / 2;
    }
}