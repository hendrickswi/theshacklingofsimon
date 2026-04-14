using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

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
        _amt = amt;
    }
    
    public override bool ApplyEffect()
    {
        if (Entity.Health >= Entity.MaxHealth) return false;
        Entity.Heal(_amt);
        return true;
    }
}