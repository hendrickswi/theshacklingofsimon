#region

using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Sounds;

#endregion

namespace TheShacklingOfSimon.Items.Passive_Items;

public class HealingItem : PassiveItem, IConsumableItem
{
    private readonly int _amt;
    
    public HealingItem(
        IDamageableEntity entity, 
        string name = "Healing Item", 
        string description = "Heals 1 health", 
        int amt = 1) 
        : base(entity)
    {
        Name = name;
        Description = description;
        SFX = SoundManager.Instance.AddSFX("isaac","1up");
        _amt = amt;
    }
    
    public override bool ApplyEffect()
    {
        if (Entity.Health >= Entity.MaxHealth) return false;
        Entity.Heal(_amt);
        SoundManager.Instance.PlaySFX(SFX);
        return true;
    }
}