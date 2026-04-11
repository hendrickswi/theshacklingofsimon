#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomConstructor;
using TheShacklingOfSimon.LevelHandler.Tiles.Border.Doors;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomManager
{
    public sealed class RoomManager : IRoomNavigator
    {
        private readonly RoomIndexReader indexReader;
        private readonly RoomLoader roomLoader;

        private readonly Dictionary<string, RoomFileData> dataCache = new();
        private readonly Dictionary<string, Room> roomCache = new();

        private readonly bool preserveRoomState;

        // I make this nullable so pendingSwitch can cleanly be "none".
        private PendingRoomSwitch? pendingSwitch;

        private readonly List<string> roomIds = new();
        private int currentIndex;
        private string startingRoomId;

        public Room CurrentRoom { get; private set; }
        public IReadOnlyList<string> RoomIds => roomIds;

        /// Fired whenever CurrentRoom is changed through GoTo/NextRoom/PrevRoom.
        /// RoomManager does not handle collision, it only announces room transitions.
        public event Action<Room> RoomChanged;

        public RoomManager(
            JsonRoomReader roomReader,
            RoomIndexReader indexReader,
            RoomFactory factory,
            GraphicsDevice graphicsDevice,
            bool preserveRoomState = true)
        {
            this.indexReader = indexReader ?? throw new ArgumentNullException(nameof(indexReader));
            roomLoader = new RoomLoader(
                roomReader ?? throw new ArgumentNullException(nameof(roomReader)),
                factory ?? throw new ArgumentNullException(nameof(factory)),
                graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice)));

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
            if (idx >= 0)
            {
                currentIndex = idx;
            }

            RaiseRoomChanged();
        }

        public void RequestRoomSwitch(string roomId, Point spawnGrid, IPlayer player)
        {
            if (string.IsNullOrWhiteSpace(roomId) || player == null)
            {
                return;
            }

            if (pendingSwitch.HasValue)
            {
                return;
            }

            pendingSwitch = new PendingRoomSwitch(roomId, spawnGrid, player);
        }

        public void ResolvePendingRoomSwitch()
        {
            if (!pendingSwitch.HasValue)
            {
                return;
            }

            PendingRoomSwitch request = pendingSwitch.Value;

            //Debug.WriteLine($"SWITCH TO {request.RoomId} spawn={request.SpawnGrid}");
            GoTo(request.RoomId);
            request.Player.SetPosition(CurrentRoom.TileMap.GridToWorld(request.SpawnGrid));

            pendingSwitch = null;
        }

        public void NextRoom()
        {
            if (roomIds.Count == 0) return;

            currentIndex = (currentIndex + 1) % roomIds.Count;
            CurrentRoom = Load(roomIds[currentIndex]);

            RaiseRoomChanged();
        }

        public void PrevRoom()
        {
            if (roomIds.Count == 0) return;

            currentIndex = (currentIndex - 1 + roomIds.Count) % roomIds.Count;
            CurrentRoom = Load(roomIds[currentIndex]);

            RaiseRoomChanged();
        }

        private void RaiseRoomChanged()
        {
            RoomChanged?.Invoke(CurrentRoom);
        }

        private void InitializeIndex()
        {
            RoomIndexData idx = indexReader.ReadIndex();

            if (idx.Rooms == null || idx.Rooms.Count == 0)
                throw new InvalidOperationException("room_index.json has no rooms.");

            roomIds.Clear();
            roomIds.AddRange(idx.Rooms);

            startingRoomId = string.IsNullOrWhiteSpace(idx.StartingRoom) ? roomIds[0] : idx.StartingRoom;
            currentIndex = Math.Max(0, roomIds.IndexOf(startingRoomId));
            CurrentRoom = Load(roomIds[currentIndex]);

            // I do not raise RoomChanged here to avoid side effects during construction.
            // Game1 should register collidables once after subscribing.
        }

        private Room Load(string roomId)
        {
            if (preserveRoomState && roomCache.TryGetValue(roomId, out var existing))
            {
                return existing;
            }

            if (!dataCache.TryGetValue(roomId, out var data))
            {
                data = roomLoader.ReadData(roomId);
                dataCache[roomId] = data;
            }

            Room room = roomLoader.CreateRoom(data);

            foreach (var tile in room.TileMap.PlacedTiles)
            {
                if (tile is DoorTile door)
                {
                    door.BindNavigator(this);
                    //Debug.WriteLine(
                    //    $"BOUND DOOR in {roomId}: side={door.Side} -> {door.ToRoom} spawn={door.SpawnGrid}");
                }
            }

            if (preserveRoomState)
            {
                roomCache[roomId] = room;
            }

            //Debug.WriteLine($"LOAD ROOM {roomId}");
            return room;
        }

        public void ResetToGameStart()
        {
            dataCache.Clear();
            roomCache.Clear();
            pendingSwitch = null;

            RoomIndexData idx = indexReader.ReadIndex();

            if (idx.Rooms == null || idx.Rooms.Count == 0)
                throw new InvalidOperationException("room_index.json has no rooms.");

            roomIds.Clear();
            roomIds.AddRange(idx.Rooms);

            startingRoomId = string.IsNullOrWhiteSpace(idx.StartingRoom) ? roomIds[0] : idx.StartingRoom;
            currentIndex = Math.Max(0, roomIds.IndexOf(startingRoomId));

            CurrentRoom = Load(roomIds[currentIndex]);

            RaiseRoomChanged();
        }

        private readonly struct PendingRoomSwitch
        {
            public string RoomId { get; }
            public Point SpawnGrid { get; }
            public IPlayer Player { get; }

            public PendingRoomSwitch(string roomId, Point spawnGrid, IPlayer player)
            {
                RoomId = roomId;
                SpawnGrid = spawnGrid;
                Player = player;
            }
        }
    }
}