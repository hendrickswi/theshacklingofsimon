#region

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Items.Passive_Items;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Entities.Pickup;

public class ShopPickup : BasePickup
{
    private IItem _item;
    private readonly SpriteFont _priceFont;

    public int Price { get; }
    public bool IsSold { get; private set; }

    public override IItem Item
    {
        get => _item;
        protected set => _item = value;
    }

    public ShopPickup(Vector2 position, ISprite sprite, IItem item, int price)
        : base(position, sprite)
    {
        Item = item;
        Price = price;
        IsSold = false;
        _priceFont = SpriteFactory.Instance.GetFont("Upheaval32");
    }

    public override void OnCollision(IPlayer player)
    {
        if (!IsActive || IsSold || Item == null || player == null) return;
        if (player.Inventory.NumCoins < Price) return;

        player.Inventory.NumCoins -= Price;
        Item.Entity = player;

        if (Item is IConsumableItem)
        {
            Item.ApplyEffect();
        }
        else if (Item is IInventoryItem inventoryItem)
        {
            if (inventoryItem is IPassiveItem)
            {
                inventoryItem.ApplyEffect();
            }

            player.Inventory.Add(inventoryItem);
        }

        IsSold = true;
        Discontinue();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsActive || IsSold) return;

        base.Draw(spriteBatch);

        if (_priceFont == null) return;

        string priceText = $"${Price}";
        float scale = 0.5f;
        Vector2 textSize = _priceFont.MeasureString(priceText) * scale;

        Vector2 textPos = new Vector2(
            Position.X + 8f - textSize.X / 2f,
            Position.Y + 20f
        );

        spriteBatch.DrawString(
            _priceFont,
            priceText,
            textPos + new Vector2(1f, 1f),
            Color.Black,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f
        );

        spriteBatch.DrawString(
            _priceFont,
            priceText,
            textPos,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f
        );
    }
}