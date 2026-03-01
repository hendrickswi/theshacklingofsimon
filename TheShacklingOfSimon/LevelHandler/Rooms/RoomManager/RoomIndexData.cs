using System.Collections.Generic;

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomManager
{
    public sealed class RoomIndexData
    {
        public string StartingRoom { get; set; } = "";
        public List<string> Rooms { get; set; } = new();
    }
}