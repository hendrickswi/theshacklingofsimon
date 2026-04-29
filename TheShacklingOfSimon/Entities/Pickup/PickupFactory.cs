#region

using System;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Items.Passive_Items.Consumables;
using TheShacklingOfSimon.Items.Passive_Items.Inventory_Items;
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

        IItem item = CreateItem(data, player);

        string spriteName = string.IsNullOrWhiteSpace(data.Sprite)
            ? GetSpriteNameForItem(item)
            : NormalizeSpriteName(data.Sprite, item);

        return CreatePickupFromItem(worldPos, item, spriteName, data.Price);
    }

    public IPickup CreateDroppedPickup(IItem item, Vector2 worldPos)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        string spriteName = GetSpriteNameForItem(item);
        return CreatePickupFromItem(worldPos, item, spriteName, 0);
    }

    private static IPickup CreatePickupFromItem(Vector2 worldPos, IItem item, string spriteName, int price)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        if (string.IsNullOrWhiteSpace(spriteName))
        {
            spriteName = GetSpriteNameForItem(item);
        }

        var sprite = SpriteFactory.Instance.CreateStaticSprite(spriteName);

        if (price > 0)
        {
            return new ShopPickup(worldPos, sprite, item, price);
        }

        return item switch
        {
            IInventoryItem inventoryItem => new InventoryPickup(worldPos, sprite, inventoryItem),
            IConsumableItem consumableItem => new ConsumablePickup(worldPos, sprite, consumableItem),

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
            "Armor" => new ArmorItem(player),
            "Damage" => new DamageItem(player),
            "Speed" => new SpeedItem(player),

            _ => throw new InvalidOperationException(
                $"Unknown pickup item type in room json: {data.ItemType}")
        };
    }

    private static string GetSpriteNameForItem(IItem item)
    {
        return item switch
        {
            KeyItem => "key",
            CoinItem => "Coin",
            HealingItem => "images/Red_Heart",
            ArmorItem => "images/chestplate",
            DamageItem => "images/fireball",
            SpeedItem => "images/feather",

            _ => "images/8Ball"
        };
    }

    private static string NormalizeSpriteName(string spriteName, IItem item)
    {
        if (string.IsNullOrWhiteSpace(spriteName))
        {
            return GetSpriteNameForItem(item);
        }

        // The SpriteFactory looks up the sprite name from the JSON file,
        // not always the image path. Coin.json uses "Coin" and key.json uses "key".
        return spriteName switch
        {
            "images/key" => "key",
            "images/Key" => "key",
            "Key" => "key",

            "images/Coin" => "Coin",
            "images/coin" => "Coin",
            "coin" => "Coin",

            "Heart" => "images/Red_Heart",
            "Red_Heart" => "images/Red_Heart",

            "chestplate" => "images/chestplate",

            "feather" => "images/feather",

            "8Ball" => "images/8Ball",

            _ => spriteName
        };
    }
}