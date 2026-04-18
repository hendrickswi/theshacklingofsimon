#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Items.Passive_Items;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass;
using TheShacklingOfSimon.Sprites.Factory;

#endregion

namespace TheShacklingOfSimon.Entities.Pickup;

public class PickupFactory
{
    public IPickup CreatePickup(PickupData data, IPlayer player, TileMap tileMap)
    {
        if (data == null) throw new ArgumentNullException(nameof(data));
        if (player == null) throw new ArgumentNullException(nameof(player));
        if (tileMap == null) throw new ArgumentNullException(nameof(tileMap));

        Vector2 worldPos = tileMap.GridToWorld(new Point(data.X, data.Y));

        string spriteName = string.IsNullOrWhiteSpace(data.Sprite)
            ? GetDefaultSpriteName(data.ItemType)
            : data.Sprite;

        IItem item = CreateItem(data, player);

        if (data.Price > 0)
        {
            return new ShopPickup(
                worldPos,
                SpriteFactory.Instance.CreateStaticSprite(spriteName),
                item,
                data.Price
            );
        }

        return item switch
        {
            IInventoryItem inventoryItem => new InventoryPickup(
                worldPos,
                SpriteFactory.Instance.CreateStaticSprite(spriteName),
                inventoryItem
            ),

            IConsumableItem consumableItem => new ConsumablePickup(
                worldPos,
                SpriteFactory.Instance.CreateStaticSprite(spriteName),
                consumableItem
            ),

            _ => throw new InvalidOperationException(
                $"Unsupported pickup item runtime type: {item.GetType().Name}")
        };
    }

    private static IItem CreateItem(PickupData data, IPlayer player)
    {
        return data.ItemType switch
        {
            "Key" => new KeyItem(player, amt: data.Amount),
            "Coin" => new CoinItem(player, amt: data.Amount),
            "Heart" => new HealingItem(player),

            _ => throw new InvalidOperationException(
                $"Unknown pickup item type in room json: {data.ItemType}")
        };
    }

    private static string GetDefaultSpriteName(string itemType)
    {
        return itemType switch
        {
            "Key" => "images/key",
            "Coin" => "images/Coin",
            "Heart" => "images/Red_Heart",
            _ => "images/8Ball"
        };
    }
}