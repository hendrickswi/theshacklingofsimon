using Microsoft.Xna.Framework.Audio;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Sounds;


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
        SFX = SoundManager.Instance.AddSFX("items", "keypickup");
        _amt = amt;
    }
    
    public override bool ApplyEffect()
    {
        // Temporary cast
        if (Entity is not IPlayer player) return false;
        player.Inventory.NumKeys += _amt;
        SoundManager.Instance.PlaySFX(SFX);
        return true;
    }
}