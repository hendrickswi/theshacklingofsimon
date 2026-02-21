using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TheShacklingOfSimon.Level_Handler.Tiles;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Level_Handler.Rooms.Room_Class
{
    // Sparse tilemap: only stores interactive tiles (floor = no entry)
    public sealed class TileMap
    {
        // Key = (gridX, gridY). Value = the tile at that grid cell
        // If a cell is NOT in this dictionary, that cell is just normal floor
        private readonly Dictionary<Point, ITile> tiles = new();

        private readonly SpriteFactory spriteFactory;

        // Room is always a fixed size in grid cells.
        public int Width { get; }
        public int Height { get; }

        public TileMap(SpriteFactory spriteFactory)
        {
            this.spriteFactory = spriteFactory;
            Width = RoomConstants.GridWidth;
            Height = RoomConstants.GridHeight;
        }

        // Returns true if a grid coordinate is inside the room
        // Example: (0,0) is top-left tile. (Width-1, Height-1) is bottom right like monogame
        public bool InBounds(Point gridPos) =>
            gridPos.X >= 0 && gridPos.X < Width &&
            gridPos.Y >= 0 && gridPos.Y < Height;

        // Converts pixel position into grid coordinates
        public Point WorldToGrid(Vector2 worldPos) =>
            new Point(
                (int)(worldPos.X / RoomConstants.TileSize),
                (int)(worldPos.Y / RoomConstants.TileSize)
            );

        // Converts grid coordinate into top-left world/pixel position of that tile cell
        public Vector2 GridToWorld(Point gridPos) =>
            new Vector2(
                gridPos.X * RoomConstants.TileSize,
                gridPos.Y * RoomConstants.TileSize
            );

        // returns true if a tile exists in that grid cell
        // If it returns false, it means "floor"
        public bool TryGetTile(Point gridPos, out ITile tile) =>
            tiles.TryGetValue(gridPos, out tile);

        // Place/replace a tile at a grid cell
        public void PlaceTile(Point gridPos, ITile tile)
        {
            if (tile == null || !InBounds(gridPos)) return;
            tiles[gridPos] = tile; // replaces existing tile if one was already there
        }

        // Remove a tile from a grid cell
        // This is how destructible tiles will be implemented
        // removing the entry turns it into "floor"
        public bool RemoveTile(Point gridPos)
        {
            if (!InBounds(gridPos)) return false;
            return tiles.Remove(gridPos);
        }

        // Convenience for loaders: create a tile instance by type string + where it goes
        // The tile gets a sprite and a position aligned to the grid cell
        public ITile CreateTile(string tileType, Point gridPos)
        {
            var position = GridToWorld(gridPos);

            return tileType switch
            {
                "Rock" => new RockTile(spriteFactory.CreateStaticSprite("images/Rocks"), position),
                "Hole" => new HoleTile(spriteFactory.CreateStaticSprite("images/Hole"), position),
                _ => throw new ArgumentException($"Unknown tile type '{tileType}'")
            };
        }

        public void Update(GameTime gameTime)
        {
            // We make a copy of the keys because tiles might be removed during update
            var keys = new List<Point>(tiles.Keys);

            foreach (var key in keys)
            {
                // Tile could have been removed already, so re-check it exists
                if (!tiles.TryGetValue(key, out var tile)) continue;

                // If tile is inactive, remove it (becomes floor)
                if (!tile.IsActive)
                {
                    tiles.Remove(key);
                    continue;
                }

                tile.Update(gameTime);

                // If the tile became inactive during Update, remove it
                if (!tile.IsActive)
                    tiles.Remove(key);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Only draw tiles that exist 
            foreach (var tile in tiles.Values)
                tile.Draw(spriteBatch);
        }

        // Collision helper:
        // Given an entity's rectangle hitbox, return only tiles in the grid cells that overlap it
        // This is fast because we can just check a small area instead of scanning the entire room
        public IEnumerable<ITile> GetTilesIntersecting(Rectangle rect)
        {
            // Convert the rectangle bounds from pixels to grid cell range
            int minX = Math.Max(0, rect.Left / RoomConstants.TileSize);
            int maxX = Math.Min(Width - 1, rect.Right / RoomConstants.TileSize);
            int minY = Math.Max(0, rect.Top / RoomConstants.TileSize);
            int maxY = Math.Min(Height - 1, rect.Bottom / RoomConstants.TileSize);

            // For each grid cell in that range, yield the tile if it exists
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    var p = new Point(x, y);

                    // Only return tiles that are actually stored 
                    if (tiles.TryGetValue(p, out var tile))
                        yield return tile;
                }
            }
        }
    }
}