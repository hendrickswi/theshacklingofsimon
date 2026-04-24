#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;
using TheShacklingOfSimon.Entities.Enemies.Managers;
using TheShacklingOfSimon.Entities.Pickup;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Entities.Projectiles;
using TheShacklingOfSimon.Items;
using TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomClass;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Border.Doors;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Factory;
using TheShacklingOfSimon.Weapons;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Rooms.RoomConstructor
{
    public sealed class RoomFactory
    {
        // we can set this once from Game1 after loading the two upright door textures.
        public DoorTextureSet DoorTextures { get; set; }

        // Events for wiring managers
        public Action<IProjectile> OnProjectileCreated { get; set; }
        public Action<IPickup> OnItemDropped { get; set; }

        // I left this overridable so we can add special puzzle/boss/key door rules later
        // without rewriting DoorTile.
        public Func<DoorData, IDoorUnlockCondition> DoorConditionFactory { get; set; }

        // Assigned by Game1 after player creation and before RoomManager creation.
        public Func<IPlayer> PlayerProvider { get; set; }

        private readonly PickupFactory pickupFactory = new PickupFactory();

        public Room Create(RoomFileData data, int viewportWidth, int viewportHeight)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (DoorTextures == null)
                throw new InvalidOperationException("DoorTextures must be assigned before creating rooms.");

            if (PlayerProvider == null)
                throw new InvalidOperationException("PlayerProvider must be assigned before creating rooms.");

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
            PlacePickups(tileMap, entities, data.Pickups);

            IEnumerable<IEntity> exposedEntities = entities;
            return new Room(data.Id, data.IsBossRoom, tileMap, exposedEntities, data.Doors, background);
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

                var door = new DoorTile(
                    SpriteFactory.Instance.CreateStaticSprite("images/Rocks"),
                    tileMap.GridToWorld(borderPos),
                    d.To.Room,
                    new Point(d.To.Spawn.X, d.To.Spawn.Y),
                    side,
                    DoorTextures,
                    unlockCondition
                );

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
                DoorUnlockType.KeyRequired => new KeyRequiredDoorCondition(),
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

            IPlayer player = PlayerProvider();
            IPathfindingService pathfindingService = new GridPathfindingService(tileMap);

            foreach (var e in enemies)
            {
                Vector2 worldPos = tileMap.GridToWorld(new Point(e.X, e.Y));

                IWeapon weapon = EnemyWeaponFactory.CreateWeapon(e.Weapon);

                IEnemy enemy = e.Type switch
                {
                    EnemyTypeList.ProjectileEnemy => new ProjectileEnemy(worldPos, weapon, e.Name),
                    EnemyTypeList.ChaseEnemy => new ChaseEnemy(worldPos, weapon, e.Name),
                    EnemyTypeList.FlyingEnemy => new FlyingEnemy(worldPos, weapon, e.Name),
                    EnemyTypeList.SpawnerEnemy => new SpawnerEnemy(worldPos, weapon, e.Name),
                    _ => throw new InvalidOperationException($"Unknown enemy type: {e.Type}")
                };

                if (enemy is BaseEnemy baseEnemy)
                {
                    baseEnemy.SetTargetPlayer(player);
                    baseEnemy.SetPathfindingService(pathfindingService);
                }

                enemy.OnProjectileCreated += proj => OnProjectileCreated?.Invoke(proj);
                enemy.OnItemDropped += (item, pos) =>
                {
                    IPickup p;
                    switch (item)
                    {
                        case IInventoryItem inventoryItem:
                            p = new InventoryPickup(
                                pos,
                                SpriteFactory.Instance.CreateStaticSprite("images/8Ball"),
                                inventoryItem);
                            break;

                        case IConsumableItem consumableItem:
                            p = new ConsumablePickup(
                                pos,
                                SpriteFactory.Instance.CreateStaticSprite("images/Red_Heart"),
                                consumableItem);
                            break;

                        default:
                            throw new InvalidOperationException($"Unknown item type: {item.GetType()}");
                    }

                    OnItemDropped?.Invoke(p);
                };

                entities.Add(enemy);
            }
        }

        private void PlacePickups(TileMap tileMap, IList<IEntity> entities, List<PickupData> pickups)
        {
            if (pickups == null || pickups.Count == 0) return;

            IPlayer player = PlayerProvider();

            foreach (var p in pickups)
            {
                IPickup pickup = pickupFactory.CreatePickup(p, player, tileMap);
                entities.Add((IEntity)pickup);
            }
        }
    }
}