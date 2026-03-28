using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Products;

namespace TheShacklingOfSimon.LevelHandler.Tiles.Obstacles
{
    // Pit tile: blocks ground entities, flying entities can pass over it
    public sealed class HoleTile : Tile
    {
        protected override TileCollisionFlags CollisionFlags => TileCollisionFlags.BlocksGround;

        public HoleTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        public override void OnCollision(IPlayer player)
        {
            if (player == null || !IsActive) return;

            ResolveEntityCollision(player);
        }

        public override void OnCollision(IEnemy enemy)
        {
            if (enemy == null || !IsActive) return;

            ResolveEntityCollision(enemy);
        }
    }
}