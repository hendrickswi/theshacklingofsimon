using System.Collections.Generic;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Class
{
    public sealed class RoomFileData
    {
        public string Id { get; set; } = "";
        public List<TileData> Tiles { get; set; } = new();
        public List<EntityData> Entities { get; set; } = new();
        public List<DoorData> Doors { get; set; } = new();
    }

    public sealed class TileData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Type { get; set; } = "";
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
        public string ToRoom { get; set; } = "";
        public int SpawnX { get; set; }
        public int SpawnY { get; set; }
    }
}