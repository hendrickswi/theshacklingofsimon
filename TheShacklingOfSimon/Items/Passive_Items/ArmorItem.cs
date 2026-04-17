#region

using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.StatusEffects;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.Items.Passive_Items;

public class ArmorItem : PassiveItem, IInventoryItem
{
    private readonly float _amt;
    private readonly float _duration;
    
    public ArmorItem(
        IDamageableEntity entity, 
        string name = "Trusty Armor", 
        string description = "Allows you to take more hits", 
        float amt = 0.25f,
        float duration = float.MaxValue) 
        : base(entity)
    {
        Name = name;
        Description = description;
        _amt = amt;
        _duration = duration;
    }
    public override bool ApplyEffect()
    {
        IStatusEffect maxHealthEffect = new MaxHealthEffect(
            Name, 
            EffectType.MaxHealth, 
            Entity, 
            _amt * Entity.GetStat(StatType.MaxHealth),
            _duration
        );
        IStatusEffect invulnerabilityDurationEffect = new InvulnerabilityDurationEffect(
            Name, 
            EffectType.InvulnerabilityDuration, 
            Entity, 
            _amt * Entity.GetStat(StatType.InvulnerabilityDuration), 
            _duration
        );
        
        Entity.EffectManager.AddEffect(maxHealthEffect);
        Entity.EffectManager.AddEffect(invulnerabilityDurationEffect);
        return true;
    }
}