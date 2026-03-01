using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TheShacklingOfSimon.Entities;
using TheShacklingOfSimon.Level_Handler.Rooms.Room_Class;
using TheShacklingOfSimon.Level_Handler.Tiles.Border;
using TheShacklingOfSimon.Level_Handler.Tiles.Tile_Constructor;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Constructor
{
    public sealed class RoomFactory
    {
        // Builds a Room from JSON data (interior coords) + auto border (walls/doors).
        public Room Create(RoomFileData data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            var tileMap = new TileMap();
            var spriteFactory = SpriteFactory.Instance;
            var tileFactory = new TileFactory(spriteFactory);

            BuildBorderWalls(tileMap, tileFactory);
            PlaceInteriorTiles(tileMap, tileFactory, data.Tiles);
            PlaceDoors(tileMap, spriteFactory, data.Doors);

            // Entities are created elsewhere for now.
            IEnumerable<IEntity> entities = new List<IEntity>();
            return new Room(data.Id, tileMap, entities, data.Doors);
        }

        // Fills the outer ring with wall tiles.
        private static void BuildBorderWalls(TileMap tileMap, TileFactory tileFactory)
        {
            int w = RoomConstants.GridWidth;
            int h = RoomConstants.GridHeight;

            for (int x = 0; x < w; x++)
            {
                var top = new Point(x, 0);
                var bottom = new Point(x, h - 1);

                tileMap.PlaceTile(top, tileFactory.Create(TileType.Rock, top));
                tileMap.PlaceTile(bottom, tileFactory.Create(TileType.Rock, bottom));
            }

            for (int y = 1; y < h - 1; y++)
            {
                var left = new Point(0, y);
                var right = new Point(w - 1, y);

                tileMap.PlaceTile(left, tileFactory.Create(TileType.Rock, left));
                tileMap.PlaceTile(right, tileFactory.Create(TileType.Rock, right));
            }
        }

        // Places JSON tiles into the interior (offset by +1,+1 into the full grid).
        private static void PlaceInteriorTiles(TileMap tileMap, TileFactory tileFactory, List<TileData> tiles)
        {
            if (tiles == null) return;

            var used = new HashSet<Point>();
            Point off = RoomConstants.InteriorOffset;

            foreach (var t in tiles)
            {
                if (t.X < 0 || t.X >= RoomConstants.InteriorWidth ||
                    t.Y < 0 || t.Y >= RoomConstants.InteriorHeight)
                    throw new InvalidOperationException($"Tile out of interior bounds: ({t.X},{t.Y}).");

                var fullPos = new Point(t.X + off.X, t.Y + off.Y);

                if (!used.Add(fullPos))
                    throw new InvalidOperationException($"Duplicate tile entry at interior ({t.X},{t.Y}).");

                tileMap.PlaceTile(fullPos, tileFactory.Create(t.Type, fullPos));
            }
        }

        // Replaces border wall cells with door tiles based on DoorData.
        private static void PlaceDoors(TileMap tileMap, SpriteFactory spriteFactory, List<DoorData> doors)
        {
            if (doors == null) return;

            var used = new HashSet<Point>();

            foreach (var d in doors)
            {
                if (d.X < 0 || d.X >= RoomConstants.InteriorWidth ||
                    d.Y < 0 || d.Y >= RoomConstants.InteriorHeight)
                    throw new InvalidOperationException($"Door out of interior bounds: ({d.X},{d.Y}).");

                if (d.To == null)
                    throw new InvalidOperationException($"Door destination missing for door at ({d.X},{d.Y}).");

                if (string.IsNullOrWhiteSpace(d.To.Room))
                    throw new InvalidOperationException($"Door destination room missing for door at ({d.X},{d.Y}).");

                if (d.To.Spawn == null)
                    throw new InvalidOperationException($"Door destination spawn missing for door at ({d.X},{d.Y}).");

                if (d.To.Spawn.X < 0 || d.To.Spawn.X >= RoomConstants.InteriorWidth ||
                    d.To.Spawn.Y < 0 || d.To.Spawn.Y >= RoomConstants.InteriorHeight)
                    throw new InvalidOperationException(
                        $"Door spawn out of interior bounds: ({d.To.Spawn.X},{d.To.Spawn.Y}) for door at ({d.X},{d.Y}).");

                var (borderPos, side) = DoorToBorderCell(d);

                if (!used.Add(borderPos))
                    throw new InvalidOperationException($"Duplicate door placement on border at ({borderPos.X},{borderPos.Y}).");

                // Replace with your real door sprite key when available.
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

        // Converts interior-adjacent coords to a border cell + which side it is on.
        private static (Point borderPos, DoorSide side) DoorToBorderCell(DoorData d)
        {
            int iw = RoomConstants.InteriorWidth;
            int ih = RoomConstants.InteriorHeight;

            bool onPerimeter = (d.X == 0 || d.X == iw - 1 || d.Y == 0 || d.Y == ih - 1);
            if (!onPerimeter)
                throw new InvalidOperationException($"DoorData must be on interior perimeter: ({d.X},{d.Y}).");

            if (d.Y == 0) return (new Point(d.X + 1, 0), DoorSide.North);
            if (d.Y == ih - 1) return (new Point(d.X + 1, RoomConstants.GridHeight - 1), DoorSide.South);
            if (d.X == 0) return (new Point(0, d.Y + 1), DoorSide.West);

            return (new Point(RoomConstants.GridWidth - 1, d.Y + 1), DoorSide.East);
        }
    }
}