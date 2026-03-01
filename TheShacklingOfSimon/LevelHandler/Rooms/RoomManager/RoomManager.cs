using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TheShacklingOfSimon.Level_Handler.Rooms.Room_Class;
using TheShacklingOfSimon.Level_Handler.Rooms.Room_Constructor;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomManager;

namespace TheShacklingOfSimon.Level_Handler.Rooms.RoomManager
{
    public sealed class RoomManager
    {
        private readonly JsonRoomReader roomReader;
        private readonly RoomIndexReader indexReader;
        private readonly RoomFactory factory;

        private readonly Dictionary<string, RoomFileData> dataCache = new();
        private readonly Dictionary<string, Room> roomCache = new();

        private readonly bool preserveRoomState;

        private readonly List<string> roomIds = new();
        private int currentIndex;

        public Room CurrentRoom { get; private set; }
        public IReadOnlyList<string> RoomIds => roomIds;

        public RoomManager(
            JsonRoomReader roomReader,
            RoomIndexReader indexReader,
            RoomFactory factory,
            bool preserveRoomState = true)
        {
            this.roomReader = roomReader ?? throw new ArgumentNullException(nameof(roomReader));
            this.indexReader = indexReader ?? throw new ArgumentNullException(nameof(indexReader));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.preserveRoomState = preserveRoomState;

            InitializeIndex();
        }

        public void Update(GameTime gameTime) => CurrentRoom.Update(gameTime);

        public void Draw(SpriteBatch spriteBatch) => CurrentRoom.Draw(spriteBatch);

        public void GoTo(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
                throw new ArgumentException("roomId cannot be null/empty", nameof(roomId));

            CurrentRoom = Load(roomId);

            int idx = roomIds.IndexOf(roomId);
            if (idx >= 0) currentIndex = idx;
        }

        public void NextRoom()
        {
            if (roomIds.Count == 0) return;
            currentIndex = (currentIndex + 1) % roomIds.Count;
            CurrentRoom = Load(roomIds[currentIndex]);
        }

        public void PrevRoom()
        {
            if (roomIds.Count == 0) return;
            currentIndex = (currentIndex - 1 + roomIds.Count) % roomIds.Count;
            CurrentRoom = Load(roomIds[currentIndex]);
        }

        private void InitializeIndex()
        {
            RoomIndexData idx = indexReader.ReadIndex();

            if (idx.Rooms == null || idx.Rooms.Count == 0)
                throw new InvalidOperationException("room_index.json has no rooms.");

            roomIds.Clear();
            roomIds.AddRange(idx.Rooms);

            string start = string.IsNullOrWhiteSpace(idx.StartingRoom) ? roomIds[0] : idx.StartingRoom;
            currentIndex = Math.Max(0, roomIds.IndexOf(start));
            CurrentRoom = Load(roomIds[currentIndex]);
        }

        private Room Load(string roomId)
        {
            if (preserveRoomState && roomCache.TryGetValue(roomId, out var existing))
                return existing;

            if (!dataCache.TryGetValue(roomId, out var data))
            {
                data = roomReader.Read(roomId);
                dataCache[roomId] = data;
            }

            var room = factory.Create(data);

            if (preserveRoomState)
                roomCache[roomId] = room;

            return room;
        }
    }
}