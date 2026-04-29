#region

using System.Collections.Generic;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.TileConstructor;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass
{
    public sealed class RoomFileData
    {
        public string Id { get; set; } = "";
        public bool IsBossRoom { get; set; }
        public List<TileData> Tiles { get; set; } = new();
        public List<EntityData> Entities { get; set; } = new();
        public List<DoorData> Doors { get; set; } = new();
        public List<EnemyData> Enemies { get; set; } = new();
        public List<PickupData> Pickups { get; set; } = new();
    }

    // Tile coordinates are full room grid coords.
    public sealed class TileData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }
    }

    public sealed class EntityData
    {
        public string Type { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
    }

    public sealed class DoorData
    {
        public int X { get; set; }
        public int Y { get; set; }

        // Base combat doors should default to enemy-clear unlock behavior.
        public DoorUnlockType UnlockType { get; set; } = DoorUnlockType.ClearEnemies;

        // can use this later if we want a condition keyed off some room event or puzzle id.
        public string UnlockTag { get; set; } = "";

        public DoorDestination To { get; set; } = new();
    }

    public sealed class DoorDestination
    {
        public string Room { get; set; } = "";
        public DoorSpawn Spawn { get; set; } = new();
    }

    public sealed class DoorSpawn
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public sealed class EnemyData
    {
        public EnemyTypeList Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; } = "";
        public WeaponTypeList Weapon { get; set; }
    }

    public sealed class PickupData
    {
        public int X { get; set; }
        public int Y { get; set; }

        // "None", "Key", "Health", "Coin", "Speed", "Armor", "Damage"
        public string ItemType { get; set; } = "";

        // Optional override. Leave blank to use defaults from code.
        public string Sprite { get; set; } = "";

        // 0 = free pickup, > 0 = shop pickup
        public int Price { get; set; } = 0;

        // For stackable pickups like keys/coins
        public int Amount { get; set; } = 1;
    }
}