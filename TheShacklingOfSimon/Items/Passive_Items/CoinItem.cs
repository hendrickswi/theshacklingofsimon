#region

using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Sounds;

#endregion

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
        SFX = SoundManager.Instance.AddSFX("items","coinpickup");
        _amt = amt;
    }
    
    public override bool ApplyEffect()
    {
        if (Entity is not IPlayer player) return false;
        player.Inventory.NumCoins += _amt;
        SoundManager.Instance.PlaySFX(SFX);
        return true;
    }
}