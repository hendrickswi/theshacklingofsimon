#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Tiles.Obstacles
{
    // Solid obstacle that can be destroyed by bombs
    public sealed class RockTile : Tile, IBombableTile
    {
        protected override TileCollisionFlags CollisionFlags =>
            TileCollisionFlags.BlocksGround |
            TileCollisionFlags.BlocksFly |
            TileCollisionFlags.BlocksProjectiles;

        public RockTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        public void OnExplode()
        {
            Discontinue();
        }

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