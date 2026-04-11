#region

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TheShacklingOfSimon.LevelHandler.Rooms.RoomClass;
using TheShacklingOfSimon.LevelHandler.Tiles.Border;
using TheShacklingOfSimon.LevelHandler.Tiles.Obstacles;
using TheShacklingOfSimon.Sprites.Factory;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor
{
    // Centralizes mapping from TileType -> concrete tile + sprite.
    public sealed class TileFactory
    {
        private readonly Dictionary<TileType, Func<Vector2, ITile>> builders;

        public TileFactory(SpriteFactory spriteFactory)
        {
            if (spriteFactory == null) throw new ArgumentNullException(nameof(spriteFactory));

            builders = new Dictionary<TileType, Func<Vector2, ITile>>
            {
                { TileType.Rock,  pos => new RockTile(spriteFactory.CreateStaticSprite("images/Rocks"), pos) },
                { TileType.Hole,  pos => new HoleTile(spriteFactory.CreateStaticSprite("images/Hole"), pos) },
                { TileType.Spike, pos => new SpikeTile(spriteFactory.CreateStaticSprite("images/Spikes"), pos) },
                { TileType.Wall,  pos => new WallTile(spriteFactory.CreateStaticSprite("images/Rocks"), pos) },
                { TileType.Fire,  pos => new FireTile(spriteFactory.CreateAnimatedSprite("images/Fire", 0.15f), pos) }
            };
        }

        // TileMap is required so GridToWorld uses the room origin (centering).
        public ITile Create(TileType type, TileMap tileMap, Point gridPos)
        {
            if (tileMap == null) throw new ArgumentNullException(nameof(tileMap));

            if (!builders.TryGetValue(type, out var build))
                throw new ArgumentException($"Unknown tile type '{type}'", nameof(type));

            Vector2 worldPos = tileMap.GridToWorld(gridPos); // origin-aware
            return build(worldPos);
        }
    }
}