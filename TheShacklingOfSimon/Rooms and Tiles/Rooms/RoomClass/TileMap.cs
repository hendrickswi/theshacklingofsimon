#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TheShacklingOfSimon.LevelHandler.Tiles;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Rooms.RoomClass
{
    // Sparse tilemap: only stores non-floor tiles (walls, hazards, obstacles, doors, etc.)
    public sealed class TileMap
    {
        private readonly Dictionary<Point, ITile> tiles = new();

        public int Width { get; }
        public int Height { get; }

        // Where the room's grid (0,0) is drawn in world/screen space.
        public Vector2 Origin { get; set; } = Vector2.Zero;

        // Full room bounds in world/screen space INCLUDING the border ring.
        public Rectangle RoomBoundsWorld => new Rectangle(
            (int)MathF.Round(Origin.X),
            (int)MathF.Round(Origin.Y),
            Width * RoomConstants.TileSize,
            Height * RoomConstants.TileSize
        );

        // Full room bounds in local (room) space.
        public Rectangle RoomBoundsLocal => new Rectangle(
            0, 0,
            Width * RoomConstants.TileSize,
            Height * RoomConstants.TileSize
        );

        public TileMap()
        {
            Width = RoomConstants.GridWidth;
            Height = RoomConstants.GridHeight;
        }

        public bool InBounds(Point gridPos) =>
            gridPos.X >= 0 && gridPos.X < Width &&
            gridPos.Y >= 0 && gridPos.Y < Height;

        public Point WorldToGrid(Vector2 worldPos)
        {
            Vector2 local = worldPos - Origin;

            return new Point(
                (int)(local.X / RoomConstants.TileSize),
                (int)(local.Y / RoomConstants.TileSize)
            );
        }

        public Vector2 GridToWorld(Point gridPos)
        {
            return Origin + new Vector2(
                gridPos.X * RoomConstants.TileSize,
                gridPos.Y * RoomConstants.TileSize
            );
        }

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

        public IEnumerable<ITile> GetTilesIntersecting(Rectangle rect)
        {
            // Convert query rect into room-local by subtracting Origin
            Rectangle local = new Rectangle(
                (int)(rect.X - Origin.X),
                (int)(rect.Y - Origin.Y),
                rect.Width,
                rect.Height
            );

            int minX = Math.Max(0, local.Left / RoomConstants.TileSize);

            int rightInclusive = Math.Max(local.Left, local.Right - 1);
            int maxX = Math.Min(Width - 1, rightInclusive / RoomConstants.TileSize);

            int minY = Math.Max(0, local.Top / RoomConstants.TileSize);

            int bottomInclusive = Math.Max(local.Top, local.Bottom - 1);
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