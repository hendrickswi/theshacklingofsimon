#region

using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Sounds;
using TheShacklingOfSimon.StatusEffects;
using TheShacklingOfSimon.StatusEffects.Implementations.Simple;
using TheShacklingOfSimon.StatusEffects.Templates;

#endregion

namespace TheShacklingOfSimon.Items.Passive_Items.Inventory_Items;

public class ArmorItem : PassiveItem, IInventoryItem
{
    private readonly IStatusEffect _maxHealthEffect;
    private readonly IStatusEffect _invulnerabilityDurationEffect;
    private readonly string SFX;
    
    public ArmorItem(
        IDamageableEntity entity, 
        string name = "Trusty Armor", 
        string description = "Allows you to take more hits", 
        float amt = 0.25f,
        float duration = float.MaxValue
        ) 
        : base(name, description, entity)
    {
        SFX = SoundManager.Instance.AddSFX("items", "equip_armor");
        _maxHealthEffect = new MaxHealthEffect(
            Name, 
            Entity, 
            amt * Entity.GetStat(StatType.MaxHealth),
            duration
        );
        
        _invulnerabilityDurationEffect = new InvulnerabilityDurationEffect(
            Name, 
            Entity, 
            amt * Entity.GetStat(StatType.InvulnerabilityDuration),
            duration
        );
    }
    
    public override bool ApplyEffect()
    {
        Entity.EffectManager.AddPermanentEffect(_maxHealthEffect);
        Entity.EffectManager.AddPermanentEffect(_invulnerabilityDurationEffect);
        SoundManager.Instance.PlaySFX(SFX);
        return true;
    }

    public override void ClearEffect()
    {
        Entity.EffectManager.ClearPermanentEffect(_maxHealthEffect);
        Entity.EffectManager.ClearPermanentEffect(_invulnerabilityDurationEffect);
    }
}