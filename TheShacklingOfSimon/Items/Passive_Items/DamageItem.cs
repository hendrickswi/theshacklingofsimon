#region

using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.StatusEffects;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.Items.Passive_Items;

public class DamageItem : PassiveItem
{
    private readonly float _amt;
    private readonly float _duration;
    
    public DamageItem(
        IDamageableEntity entity, 
        string name = "Brace", 
        string description = "Allows you to channel more power into shots", 
        float amt = 0.25f,
        float duration = float.MaxValue) 
        : base(entity)
    {
        Name = name;
        Description = description;
        _amt = amt * Entity.GetStat(StatType.DamageMultiplier);
        _duration = duration;
    }
    public override void ApplyEffect()
    {
        IStatusEffect effect = new DamageMultiplierEffect(
            Name, 
            EffectType.DamageMultiplier, 
            Entity, 
            _amt, 
            _duration
        );
        Entity.EffectManager.AddEffect(effect);
    }
}