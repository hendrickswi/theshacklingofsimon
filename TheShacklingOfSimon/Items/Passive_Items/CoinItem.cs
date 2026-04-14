using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;

namespace TheShacklingOfSimon.Items.Passive_Items;

public class CoinItem : PassiveItem, IConsumableItem
{
    private readonly int _amt;
    
    public CoinItem(
        IDamageableEntity entity, 
        string name = "Coin", 
        string description = "A shiny gold coin that can be used to buy stuff!", 
        int amt = 1) 
        : base(entity)
    {
        Name = name;
        Description = description;
        _amt = amt;
    }
    
    public override bool ApplyEffect()
    {
        if (Entity is not IPlayer player) return false;
        player.Inventory.NumCoins += _amt;
        return true;
    }
}