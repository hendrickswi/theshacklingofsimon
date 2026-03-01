using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TheShacklingOfSimon.Level_Handler.Tiles;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Class
{
    // Sparse tilemap: only stores non-floor tiles (walls, hazards, obstacles, doors, etc.)
    public sealed class TileMap
    {
        // Key = (gridX, gridY). Value = the tile at that grid cell.
        // If a cell is NOT in this dictionary, that cell is normal floor.
        private readonly Dictionary<Point, ITile> tiles = new();

        public int Width { get; }
        public int Height { get; }

        public TileMap()
        {
            Width = RoomConstants.GridWidth;
            Height = RoomConstants.GridHeight;
        }

        public bool InBounds(Point gridPos) =>
            gridPos.X >= 0 && gridPos.X < Width &&
            gridPos.Y >= 0 && gridPos.Y < Height;

        public Point WorldToGrid(Vector2 worldPos) =>
            new Point(
                (int)(worldPos.X / RoomConstants.TileSize),
                (int)(worldPos.Y / RoomConstants.TileSize)
            );

        public Vector2 GridToWorld(Point gridPos) =>
            new Vector2(
                gridPos.X * RoomConstants.TileSize,
                gridPos.Y * RoomConstants.TileSize
            );

        public bool TryGetTile(Point gridPos, out ITile tile) =>
            tiles.TryGetValue(gridPos, out tile);

        public IEnumerable<ITile> PlacedTiles => tiles.Values;

        public void PlaceTile(Point gridPos, ITile tile)
        {
            if (tile == null || !InBounds(gridPos)) return;
            tiles[gridPos] = tile;
        }

        public bool RemoveTile(Point gridPos)
        {
            if (!InBounds(gridPos)) return false;
            return tiles.Remove(gridPos);
        }

        public void Update(GameTime gameTime)
        {
            var keys = new List<Point>(tiles.Keys);

            foreach (var key in keys)
            {
                if (!tiles.TryGetValue(key, out var tile)) continue;

                if (!tile.IsActive)
                {
                    tiles.Remove(key);
                    continue;
                }

                tile.Update(gameTime);

                if (!tile.IsActive)
                    tiles.Remove(key);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var tile in tiles.Values)
                tile.Draw(spriteBatch);
        }

        // Collision helper:
        // Given a world-space rectangle, return tiles in grid cells that overlap it.
        // Note: Rectangle.Right/Bottom are exclusive, so we use (Right-1)/(Bottom-1)
        // to avoid off-by-one inclusion of adjacent cells.
        public IEnumerable<ITile> GetTilesIntersecting(Rectangle rect)
        {
            int minX = Math.Max(0, rect.Left / RoomConstants.TileSize);

            int rightInclusive = Math.Max(rect.Left, rect.Right - 1);
            int maxX = Math.Min(Width - 1, rightInclusive / RoomConstants.TileSize);

            int minY = Math.Max(0, rect.Top / RoomConstants.TileSize);

            int bottomInclusive = Math.Max(rect.Top, rect.Bottom - 1);
            int maxY = Math.Min(Height - 1, bottomInclusive / RoomConstants.TileSize);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var p = new Point(x, y);
                    if (tiles.TryGetValue(p, out var tile))
                        yield return tile;
                }
            }
        }
    }
}