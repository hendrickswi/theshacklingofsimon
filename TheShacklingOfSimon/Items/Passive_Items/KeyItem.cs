using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Items.Passive_Items;

public class KeyItem : PassiveItem, IConsumableItem
{
    private readonly int _amt;
    
    public KeyItem(
        IDamageableEntity entity, 
        string name = "Key", 
        string description = "Unlocks a door", 
        int amt = 1) 
        : base(entity)
    {
        Name = name;
        Description = description;
        _amt = amt;
    }
    
    public override bool ApplyEffect()
    {
        // Temporary cast
        if (Entity is not IPlayer player) return false;
        player.Inventory.NumKeys += _amt;
        return true;
    }
}