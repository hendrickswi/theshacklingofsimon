#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomConstructor;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Border.Doors;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomManager
{
    public enum MapDirection
    {
        North,
        South,
        West,
        East
    }

    // Simple read-only map edge so UI can ask for room adjacency
    // without depending on full room file details.
    public readonly struct RoomConnection
    {
        public string ToRoomId { get; }
        public MapDirection Direction { get; }

        public RoomConnection(string toRoomId, MapDirection direction)
        {
            ToRoomId = toRoomId;
            Direction = direction;
        }
    }

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
        public string CurrentRoomId => CurrentRoom?.Id ?? string.Empty;
        public string StartingRoomId => startingRoomId;
        public bool HasPendingRoomSwitch => pendingSwitch.HasValue;

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

        public IReadOnlyList<RoomConnection> GetConnections(string roomId)
        {
            if (string.IsNullOrWhiteSpace(roomId))
            {
                return Array.Empty<RoomConnection>();
            }

            RoomFileData data = GetOrReadRoomData(roomId);

            if (data.Doors == null || data.Doors.Count == 0)
            {
                return Array.Empty<RoomConnection>();
            }

            var connections = new List<RoomConnection>(data.Doors.Count);

            foreach (DoorData door in data.Doors)
            {
                if (door?.To == null || string.IsNullOrWhiteSpace(door.To.Room))
                {
                    continue;
                }

                connections.Add(new RoomConnection(
                    door.To.Room,
                    GetDirectionFromDoorPosition(door.X, door.Y)));
            }

            return connections;
        }

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

        // I keep this helper here so clients like the minimap can ask RoomManager
        // for connectivity without needing to know how room files are cached/read.
        private RoomFileData GetOrReadRoomData(string roomId)
        {
            if (!dataCache.TryGetValue(roomId, out RoomFileData data))
            {
                data = roomLoader.ReadData(roomId);
                dataCache[roomId] = data;
            }

            return data;
        }

        // Door positions are stored on the room border, so I derive map direction
        // directly from the border cell instead of duplicating direction data in JSON.
        private static MapDirection GetDirectionFromDoorPosition(int x, int y)
        {
            int maxX = RoomConstants.GridWidth - 1;
            int maxY = RoomConstants.GridHeight - 1;

            if (y == 0) return MapDirection.North;
            if (y == maxY) return MapDirection.South;
            if (x == 0) return MapDirection.West;
            if (x == maxX) return MapDirection.East;

            throw new InvalidOperationException(
                $"Door at ({x},{y}) is not on the room border.");
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