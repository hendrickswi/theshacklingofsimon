#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Items.Passive_Items;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities.Pickup;

public class InventoryPickup : BasePickup
{
    private IInventoryItem _item;
    
    public override IItem Item
    {
        get {
            return _item;
        }

        protected set
        {
            if (value == null) return;
            if (value is not IInventoryItem castedItem) return;
            _item = castedItem;
        }
    }
    
    public InventoryPickup(Vector2 position, ISprite sprite, IInventoryItem item) 
        : base(position, sprite)
    {
        Item = item;
    }

    public override void OnCollision(IPlayer player)
    {
        if (Item == null) return;

        Item.Entity = player;

        /*
         * Logic: inventory pickups are always added to the
         * inventory, and if the item is a passive item,
         * it should be automatically applied to the player.
         */
        if (Item is IPassiveItem)
        {
            Item.ApplyEffect();
        }
        player.Inventory.Add(Item);
        Discontinue();
    }
}