using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;

namespace TheShacklingOfSimon.Items.Passive_Items;

public class HealingItem : PassiveItem
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
    
    public override void ApplyEffect()
    {
        Entity.Heal(_amt);
    }
}