#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities.Pickup;

public class ConsumablePickup : BasePickup
{
    private IConsumableItem _item;
    
    public override IItem Item
    {
        get {
            return _item;
        }

        protected set
        {
            if (value == null) return;
            if (value is not IConsumableItem castedItem) return;
            _item = castedItem;
        }
    }
    
    public ConsumablePickup(Vector2 position, ISprite sprite, IConsumableItem item) 
        : base(position, sprite)
    {
        Item = item;
    }

    public override void OnCollision(IPlayer player)
    { 
        if (Item == null) return;

        Item.Entity = player;
        if (Item.ApplyEffect())
        {
            Discontinue();
        }
    }
}