#region

using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.StatusEffects;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.Items.Passive_Items;

public class ArmorItem : PassiveItem
{
    private readonly float _amtHealth;
    private readonly float _amtInvul;
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
        _amtHealth = amt * Entity.GetStat(StatType.MaxHealth);
        _amtInvul = amt * Entity.GetStat(StatType.InvulnerabilityDuration);
        _duration = duration;
    }
    public override void ApplyEffect()
    {
        IStatusEffect maxHealthEffect = new MaxHealthEffect(
            Name, 
            EffectType.MaxHealth, 
            Entity, 
            _amtHealth, 
            _duration
        );
        IStatusEffect invulnerabilityDurationEffect = new InvulnerabilityDurationEffect(
            Name, 
            EffectType.InvulnerabilityDuration, 
            Entity, 
            _amtInvul, 
            _duration
        );
        
        Entity.EffectManager.AddEffect(maxHealthEffect);
        Entity.EffectManager.AddEffect(invulnerabilityDurationEffect);
    }
}