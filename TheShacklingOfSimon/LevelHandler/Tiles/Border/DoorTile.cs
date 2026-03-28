using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Collisions;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
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
			if (player == null || roomManager == null) return;
			
			roomManager.RequestRoomSwitch(this, player);
		}

		public override void OnCollision(IEnemy enemy)
		{
			if (enemy == null || !IsActive) return;

			Vector2 mtv = CollisionDetector.CalculateMinimumTranslationVector(enemy.Hitbox, this.Hitbox);
			if (mtv.LengthSquared() < 0.0001f) return;
            
			// Handles position, velocity, and hitbox
			enemy.SetPosition(enemy.Position + mtv);
		}
	}
}