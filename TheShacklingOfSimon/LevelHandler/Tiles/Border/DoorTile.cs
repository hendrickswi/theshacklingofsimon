using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomManager;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.LevelHandler.Tiles.Border
{
	public sealed class DoorTile : Tile
	{
		private RoomManager roomManager;

		public override bool BlocksGround => false;
		public override bool BlocksFly => false;
		public override bool BlocksProjectiles => true;

		public string ToRoom { get; }
		public Point SpawnGrid { get; }
		public DoorSide Side { get; }

		public DoorTile(
			ISprite sprite,
			Vector2 position,
			string toRoom,
			Point spawnGrid,
			DoorSide side)
			: base(sprite, position)
		{
			ToRoom = toRoom ?? "";
			SpawnGrid = spawnGrid;
			Side = side;
		}

		public void BindRoomManager(RoomManager roomManager)
		{
			this.roomManager = roomManager;
		}

		public override void OnCollision(IPlayer player)
		{
			if (player == null || roomManager == null)
				return;

			roomManager.RequestRoomSwitch(this, player);
		}
	}
}