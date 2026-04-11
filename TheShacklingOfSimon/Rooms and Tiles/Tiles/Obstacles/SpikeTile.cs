#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.LevelHandler.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.LevelHandler.Tiles.Obstacles
{
    public sealed class SpikeTile : Tile
    {
        protected override TileCollisionFlags CollisionFlags => TileCollisionFlags.None;

        public SpikeTile(ISprite sprite, Vector2 position) : base(sprite, position) { }

        public override void OnCollision(IPlayer player)
        {
            player?.TakeDamage(1);
        }

        public override void OnCollision(IEnemy enemy)
        {
            enemy?.TakeDamage(1);
        }
    }
}