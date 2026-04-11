#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;
using TheShacklingOfSimon.Entities.Enemies.Managers;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;
using TheShacklingOfSimon.LevelHandler.Tiles.Border.Doors;
using TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomConstructor
{
    public sealed class RoomFactory
    {
        // we can set this once from Game1 after loading the two upright door textures.
        public DoorTextureSet DoorTextures { get; set; }

        // TEMPORARY
        public Action<IProjectile> OnProjectileCreated { get; set; }

        // I left this overridable so we can add special puzzle/boss/key door rules later
        // without rewriting DoorTile.
        public Func<DoorData, IDoorUnlockCondition> DoorConditionFactory { get; set; }

        public Room Create(RoomFileData data, int viewportWidth, int viewportHeight)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (DoorTextures == null)
                throw new InvalidOperationException("DoorTextures must be assigned before creating rooms.");

            var tileMap = new TileMap();
            var spriteFactory = SpriteFactory.Instance;
            var tileFactory = new TileFactory(spriteFactory);

            int roomW = RoomConstants.GridWidth * RoomConstants.TileSize;
            int roomH = RoomConstants.GridHeight * RoomConstants.TileSize;

            tileMap.Origin = new Vector2(
                (viewportWidth - roomW) * 0.5f,
                (viewportHeight - roomH) * 0.5f
            );

            var background = spriteFactory.CreateStaticSprite("images/RoomBackground");

            BuildBorderWalls(tileMap, tileFactory);
            PlaceTiles(tileMap, tileFactory, data.Tiles);
            PlaceDoors(tileMap, data.Doors);

            IList<IEntity> entities = new List<IEntity>();
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
                {
                    throw new InvalidOperationException($"Tile out of room bounds: ({t.X},{t.Y}).");
                }

                var pos = new Point(t.X, t.Y);

                if (!used.Add(pos))
                {
                    throw new InvalidOperationException($"Duplicate tile entry at ({t.X},{t.Y}).");
                }

                tileMap.PlaceTile(pos, tileFactory.Create(t.Type, tileMap, pos));
            }
        }

        private void PlaceDoors(TileMap tileMap, List<DoorData> doors)
        {
            if (doors == null) return;

            var used = new HashSet<Point>();

            foreach (var d in doors)
            {
                if (d.X < 0 || d.X >= RoomConstants.GridWidth ||
                    d.Y < 0 || d.Y >= RoomConstants.GridHeight)
                {
                    throw new InvalidOperationException($"Door out of room bounds: ({d.X},{d.Y}).");
                }

                if (d.To == null)
                {
                    throw new InvalidOperationException($"Door destination missing for door at ({d.X},{d.Y}).");
                }

                if (string.IsNullOrWhiteSpace(d.To.Room))
                {
                    throw new InvalidOperationException($"Door destination room missing for door at ({d.X},{d.Y}).");
                }

                if (d.To.Spawn == null)
                {
                    throw new InvalidOperationException($"Door destination spawn missing for door at ({d.X},{d.Y}).");
                }

                if (d.To.Spawn.X < 0 || d.To.Spawn.X >= RoomConstants.GridWidth ||
                    d.To.Spawn.Y < 0 || d.To.Spawn.Y >= RoomConstants.GridHeight)
                {
                    throw new InvalidOperationException(
                        $"Door spawn out of room bounds: ({d.To.Spawn.X},{d.To.Spawn.Y}) for door at ({d.X},{d.Y}).");
                }

                var (borderPos, side) = DoorToBorderCell(d);

                if (!used.Add(borderPos))
                {
                    throw new InvalidOperationException(
                        $"Duplicate door placement on border at ({borderPos.X},{borderPos.Y}).");
                }

                IDoorUnlockCondition unlockCondition = CreateDoorCondition(d);

                // DoorTile still inherits from Tile, so we pass a dummy sprite to the base class.
                // Real drawing comes from the locked/unlocked upright textures in DoorTextureSet.
                var door = new DoorTile(
                    SpriteFactory.Instance.CreateStaticSprite("images/Rocks"),
                    tileMap.GridToWorld(borderPos),
                    d.To.Room,
                    new Point(d.To.Spawn.X, d.To.Spawn.Y),
                    side,
                    DoorTextures,
                    unlockCondition
                );

                //Debug.WriteLine(
                //    $"CREATE DOOR in room build at ({borderPos.X},{borderPos.Y}) side={side} -> {d.To.Room} spawn=({d.To.Spawn.X},{d.To.Spawn.Y})");
                tileMap.PlaceTile(borderPos, door);
            }
        }

        private IDoorUnlockCondition CreateDoorCondition(DoorData doorData)
        {
            if (DoorConditionFactory != null)
            {
                return DoorConditionFactory(doorData);
            }

            return doorData.UnlockType switch
            {
                DoorUnlockType.AlwaysUnlocked => new AlwaysUnlockedDoorCondition(),
                DoorUnlockType.Custom => new CustomDoorCondition(),
                _ => new ClearEnemiesDoorCondition()
            };
        }

        private static (Point borderPos, DoorSide side) DoorToBorderCell(DoorData d)
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

        private void PlacePickups(TileMap tileMap, IList<IEntity> entities, List<PickupData> pickups)
        {
            if (pickups == null) return;

            foreach (var p in pickups)
            {
                Vector2 worldPos = tileMap.GridToWorld(new Point(p.X, p.Y));
                //IPickup pickup = new Pickup(worldPos, p.Item, p.Item.sprite);
                //entities.Add(pickup);
            }
        }
    }
}