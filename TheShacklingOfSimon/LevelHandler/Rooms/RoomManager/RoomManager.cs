using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomConstructor;
using TheShacklingOfSimon.LevelHandler.Tiles;
using TheShacklingOfSimon.LevelHandler.Tiles.Border;

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomManager
{
    public sealed class RoomManager : INavigationService
    {
        private readonly JsonRoomReader roomReader;
        private readonly RoomIndexReader indexReader;
        private readonly RoomFactory factory;

        // needed to compute centered origin from the actual viewport size
        private readonly GraphicsDevice graphicsDevice;

        private readonly Dictionary<string, RoomFileData> dataCache = new();
        private readonly Dictionary<string, Room> roomCache = new();

        private readonly bool preserveRoomState;
        private DoorTile pendingDoor;
        private IPlayer pendingPlayer;

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
            this.roomReader = roomReader ?? throw new ArgumentNullException(nameof(roomReader));
            this.indexReader = indexReader ?? throw new ArgumentNullException(nameof(indexReader));
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.graphicsDevice = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
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

            RaiseRoomChanged();
        }

		public void RequestRoomSwitch(DoorTile door, IPlayer player)
		{
			if (door == null || player == null)
				return;

			if (pendingDoor != null)
				return;

			pendingDoor = door;
			pendingPlayer = player;
		}

		public void ResolvePendingRoomSwitch()
		{
			if (pendingDoor == null || pendingPlayer == null)
				return;

			GoTo(pendingDoor.ToRoom);
			pendingPlayer.SetPosition(CurrentRoom.TileMap.GridToWorld(pendingDoor.SpawnGrid));

			pendingDoor = null;
			pendingPlayer = null;
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

			// we do NOT raise RoomChanged here to avoid surprising side-effects during construction.
			// Game1 should call RegisterRoomCollidables(CurrentRoom) once after subscribing
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

			var vp = graphicsDevice.Viewport;
			var room = factory.Create(data, vp.Width, vp.Height);

			foreach (var tile in room.TileMap.PlacedTiles)
			{
				if (tile is DoorTile door)
				{
					door.BindRoomManager(this);
				}
			}

			if (preserveRoomState)
				roomCache[roomId] = room;

			return room;
		}

		public void ResetToGameStart()
        {
	        dataCache.Clear();
	        roomCache.Clear();

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

        public Vector2 GetNextDirection(Vector2 currentPosition, Vector2 targetPosition, Func<ITile, bool> rules)
        {
	        // TODO: Implement this
	        // Calls the private calculation method below
	        return new Vector2(0, 0);
        }
        
        private Point CalculateAStarStep(TileMap map, Point start, Point end, Func<ITile, bool> rules) {
	        // Implement pathfinding here with A* pathfinding (or whatever pathfinding)
	        // Returns immediate next point enemy should step on.
	        return new Point(0, 0);
        }
	}
}