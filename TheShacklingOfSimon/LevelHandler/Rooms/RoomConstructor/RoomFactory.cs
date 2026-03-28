using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;
using TheShacklingOfSimon.LevelHandler.Tiles.Border;
using TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Enemies.Managers;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Weapons;

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomConstructor
{
    public sealed class RoomFactory
    {
		public RoomManager.RoomManager RoomManager { get; set; }
		// TEMPORARY
		public Action<IProjectile> OnProjectileCreated { get; set; }
        
        // Builds a Room from JSON data (FULL GRID coords) + border walls/doors on the edges.
        public Room Create(RoomFileData data, int viewportWidth, int viewportHeight)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var tileMap = new TileMap();
            var spriteFactory = SpriteFactory.Instance;
            var tileFactory = new TileFactory(spriteFactory);

            // Center the room in the viewport by setting TileMap.Origin.
            int roomW = RoomConstants.GridWidth * RoomConstants.TileSize;
            int roomH = RoomConstants.GridHeight * RoomConstants.TileSize;

            tileMap.Origin = new Vector2(
                (viewportWidth - roomW) * 0.5f,
                (viewportHeight - roomH) * 0.5f
            );

            // Make the room background image
            var background = spriteFactory.CreateStaticSprite("images/RoomBackground");
            
            // Border is the TRUE edge of the room: (x=0, x=w-1, y=0, y=h-1)
            BuildBorderWalls(tileMap, tileFactory);

            // Tiles are FULL grid coords
            PlaceTiles(tileMap, tileFactory, data.Tiles);

            // Doors are FULL grid coords on the border
            PlaceDoors(tileMap, spriteFactory, data.Doors);

            IList<IEntity> entities = new List<IEntity>();

            //Enemies placement
            PlaceEnemies(tileMap, entities, data.Enemies);

            IEnumerable<IEntity> exposedEntities = entities;
            return new Room(data.Id, tileMap, exposedEntities, data.Doors, background);
        }

        private static void BuildBorderWalls(TileMap tileMap, TileFactory tileFactory)
        {
            int w = RoomConstants.GridWidth;
            int h = RoomConstants.GridHeight;

            for (int x = 0; x < w; x++)
            {
                var top = new Point(x, 0);
                var bottom = new Point(x, h - 1);

                tileMap.PlaceTile(top, tileFactory.Create(TileType.Wall, tileMap, top));
                tileMap.PlaceTile(bottom, tileFactory.Create(TileType.Wall, tileMap, bottom));
            }

            for (int y = 1; y < h - 1; y++)
            {
                var left = new Point(0, y);
                var right = new Point(w - 1, y);

                tileMap.PlaceTile(left, tileFactory.Create(TileType.Wall, tileMap, left));
                tileMap.PlaceTile(right, tileFactory.Create(TileType.Wall, tileMap, right));
            }
        }

        private static void PlaceTiles(TileMap tileMap, TileFactory tileFactory, List<TileData> tiles)
        {
            if (tiles == null) return;

            var used = new HashSet<Point>();

            foreach (var t in tiles)
            {
                if (t.X < 0 || t.X >= RoomConstants.GridWidth ||
                    t.Y < 0 || t.Y >= RoomConstants.GridHeight)
                    throw new InvalidOperationException($"Tile out of room bounds: ({t.X},{t.Y}).");

                var pos = new Point(t.X, t.Y);

                if (!used.Add(pos))
                    throw new InvalidOperationException($"Duplicate tile entry at ({t.X},{t.Y}).");

                tileMap.PlaceTile(pos, tileFactory.Create(t.Type, tileMap, pos));
            }
        }

        private void PlaceDoors(TileMap tileMap, SpriteFactory spriteFactory, List<DoorData> doors)
        {
            if (doors == null) return;

            var used = new HashSet<Point>();

            foreach (var d in doors)
            {
                // DoorData coords are FULL grid coords now
                if (d.X < 0 || d.X >= RoomConstants.GridWidth ||
                    d.Y < 0 || d.Y >= RoomConstants.GridHeight)
                    throw new InvalidOperationException($"Door out of room bounds: ({d.X},{d.Y}).");

                if (d.To == null)
                    throw new InvalidOperationException($"Door destination missing for door at ({d.X},{d.Y}).");

                if (string.IsNullOrWhiteSpace(d.To.Room))
                    throw new InvalidOperationException($"Door destination room missing for door at ({d.X},{d.Y}).");

                if (d.To.Spawn == null)
                    throw new InvalidOperationException($"Door destination spawn missing for door at ({d.X},{d.Y}).");

                // Spawn is FULL grid coords (consistent with door coords)
                if (d.To.Spawn.X < 0 || d.To.Spawn.X >= RoomConstants.GridWidth ||
                    d.To.Spawn.Y < 0 || d.To.Spawn.Y >= RoomConstants.GridHeight)
                    throw new InvalidOperationException(
                        $"Door spawn out of room bounds: ({d.To.Spawn.X},{d.To.Spawn.Y}) for door at ({d.X},{d.Y}).");

                var (borderPos, side) = DoorToBorderCell(d);

                if (!used.Add(borderPos))
                    throw new InvalidOperationException(
                        $"Duplicate door placement on border at ({borderPos.X},{borderPos.Y}).");

                var sprite = spriteFactory.CreateStaticSprite("images/Rocks");

                var door = new DoorTile(
	                sprite,
	                tileMap.GridToWorld(borderPos),
	                d.To.Room,
	                new Point(d.To.Spawn.X, d.To.Spawn.Y),
	                side
                );

				tileMap.PlaceTile(borderPos, door);
            }
        }

        // DoorData must specify a BORDER cell in FULL grid coords.
        // TODO: change this back to static once enemy weapon JSON loading is implemented
        private (Point borderPos, DoorSide side) DoorToBorderCell(DoorData d)
        {
            int maxX = RoomConstants.GridWidth - 1;
            int maxY = RoomConstants.GridHeight - 1;

            bool onBorder = (d.X == 0 || d.X == maxX || d.Y == 0 || d.Y == maxY);
            if (!onBorder)
                throw new InvalidOperationException($"DoorData must be on room border: ({d.X},{d.Y}).");

            if (d.Y == 0) return (new Point(d.X, 0), DoorSide.North);
            if (d.Y == maxY) return (new Point(d.X, maxY), DoorSide.South);
            if (d.X == 0) return (new Point(0, d.Y), DoorSide.West);

            return (new Point(maxX, d.Y), DoorSide.East);
        }

        private void PlaceEnemies(TileMap tileMap, IList<IEntity> entities, List<EnemyData> enemies)
        {
            if (enemies == null) return;

            foreach (var e in enemies)
            {
                Vector2 worldPos = tileMap.GridToWorld(new Point(e.X, e.Y));

                IWeapon weapon = EnemyWeaponFactory.CreateWeapon(e.Weapon);

                IEnemy enemy = e.Type switch
                {
                    EnemyTypeList.ProjectileEnemy => new ProjectileEnemy(worldPos, weapon, e.Name),
                    EnemyTypeList.ChaseEnemy => new ChaseEnemy(worldPos, weapon, e.Name),
                    _ => throw new InvalidOperationException($"Unknown enemy type: {e.Type}")
                };

                enemy.OnProjectileCreated += proj => OnProjectileCreated?.Invoke(proj);
                
                entities.Add(enemy);
            }
        }
    }
}