#region

using Microsoft.Xna.Framework;
using TheShacklingOfSimon.Entities.Enemies;
using TheShacklingOfSimon.Entities.Enemies.EnemyTypes;
using TheShacklingOfSimon.Entities.Players;
using TheShacklingOfSimon.Rooms_and_Tiles.Tiles.TileConstructor;
using TheShacklingOfSimon.Sprites.Products;

#endregion

namespace TheShacklingOfSimon.Rooms_and_Tiles.Tiles.Obstacles
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
            if (enemy is FlyingEnemy) return;
            enemy?.TakeDamage(1);
        }
    }
}