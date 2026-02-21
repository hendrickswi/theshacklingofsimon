// Room_Manager/Rooms/Room_Constructor/RoomManager.cs
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TheShacklingOfSimon.Level_Handler.Rooms.Room_Class;
using TheShacklingOfSimon.Level_Handler.Rooms.Room_Constructor;

namespace TheShacklingOfSimon.Level_Handler.Rooms.RoomManager
{
    // Owns the current room and handles loading/switching
    public sealed class RoomManager
    {
        private readonly JsonRoomReader reader;
        private readonly RoomFactory factory;

        private readonly Dictionary<string, Room> cache = new();

        public Room CurrentRoom { get; private set; }

        public RoomManager(JsonRoomReader reader, RoomFactory factory, string startingRoomId)
        {
            this.reader = reader;
            this.factory = factory;

            CurrentRoom = Load(startingRoomId);
        }

        // Switch to a different room id (debug keys can call this)
        public void GoTo(string roomId)
        {
            CurrentRoom = Load(roomId);
        }

        public void Update(GameTime gameTime)
        {
            CurrentRoom.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentRoom.Draw(spriteBatch);
        }

        private Room Load(string roomId)
        {
            if (cache.TryGetValue(roomId, out var room))
                return room;

            var data = reader.Read(roomId);
            room = factory.Create(data);

            cache[roomId] = room;
            return room;
        }
    }
}