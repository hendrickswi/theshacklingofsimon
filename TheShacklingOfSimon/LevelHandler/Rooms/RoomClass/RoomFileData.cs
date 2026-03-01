using System.Collections.Generic;
using TheShacklingOfSimon.Level_Handler.Tiles.Tile_Constructor;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Class
{
    public sealed class RoomFileData
    {
        public string Id { get; set; } = "";
        public List<TileData> Tiles { get; set; } = new();
        public List<EntityData> Entities { get; set; } = new();
        public List<DoorData> Doors { get; set; } = new();
    }

    // Tile coordinates are in INTERIOR grid space: (0..12, 0..6)
    public sealed class TileData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TileType Type { get; set; }
    }

    // Entity coordinates are in INTERIOR grid space unless the entity loader says otherwise.
    public sealed class EntityData
    {
        public string Type { get; set; } = "";
        public int X { get; set; }
        public int Y { get; set; }
    }

    // Door coordinates are the INTERIOR cell adjacent to the door.
    // The door tile itself is placed on the BORDER just outside that cell.
    public sealed class DoorData
    {
        public int X { get; set; }
        public int Y { get; set; }

        // Destination data (room + spawn tile in destination interior grid).
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
}