// Room_Manager/Rooms/Room_Constructor/RoomFactory.cs
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Level_Handler.Rooms.Room_Class;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Constructor
{
    // Builds a Room instance from JSON data
    public sealed class RoomFactory
    {
        public Room Create(RoomFileData data)
        {
            // TileMap starts empty (floor = nothing)
            var tileMap = new TileMap(Sprites.Factory.SpriteFactory.Instance);

            // Place interactive tiles from the file
            foreach (var t in data.Tiles)
            {
                var gridPos = new Point(t.X, t.Y);
                var tile = tileMap.CreateTile(t.Type, gridPos);
                tileMap.PlaceTile(gridPos, tile);
            }

            // TODO: spawn entities from data.Entities once entity creation is ready
            IEnumerable<IEntity> entities = new List<IEntity>();

            return new Room(data.Id, tileMap, entities);
        }
    }
}