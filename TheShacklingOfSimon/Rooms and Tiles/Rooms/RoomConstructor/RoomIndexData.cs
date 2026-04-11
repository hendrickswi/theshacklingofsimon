#region

using System.Collections.Generic;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomConstructor
{
    public sealed class RoomIndexData
    {
        public string StartingRoom { get; set; } = "";
        public List<string> Rooms { get; set; } = new();
    }
}