using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TheShacklingOfSimon.Level_Handler.Rooms.Room_Class;
using TheShacklingOfSimon.Level_Handler.Tiles.Obstacles;
using TheShacklingOfSimon.Sprites.Factory;

namespace TheShacklingOfSimon.Level_Handler.Tiles.Tile_Constructor
{
    // Centralizes mapping from TileType -> concrete tile + sprite.
    // Keeps TileMap focused on storage/querying, not construction.
    public sealed class TileFactory
    {
        private readonly Dictionary<TileType, Func<Vector2, ITile>> builders;

        public TileFactory(SpriteFactory spriteFactory)
        {
            builders = new Dictionary<TileType, Func<Vector2, ITile>>
            {
                { TileType.Rock,  pos => new RockTile(spriteFactory.CreateStaticSprite("images/Rocks"), pos) },
                { TileType.Hole,  pos => new HoleTile(spriteFactory.CreateStaticSprite("images/Hole"), pos) },

                // Adjust these sprite keys if needed:
                { TileType.Spike, pos => new SpikeTile(spriteFactory.CreateStaticSprite("images/Spikes"), pos) },
                { TileType.Fire,  pos => new FireTile(spriteFactory.CreateStaticSprite("images/Fire"), pos) }
            };
        }

        public ITile Create(TileType type, Point gridPos)
        {
            if (!builders.TryGetValue(type, out var build))
                throw new ArgumentException($"Unknown tile type '{type}'", nameof(type));

            var worldPos = new Vector2(
                gridPos.X * RoomConstants.TileSize,
                gridPos.Y * RoomConstants.TileSize);

            return build(worldPos);
        }
    }
}